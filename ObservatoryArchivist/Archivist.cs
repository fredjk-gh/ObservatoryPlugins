using com.github.fredjk_gh.ObservatoryArchivist.DB;
using com.github.fredjk_gh.ObservatoryArchivist.UI;
using com.github.fredjk_gh.PluginCommon.AutoUpdates;
using com.github.fredjk_gh.PluginCommon.Data;
using com.github.fredjk_gh.PluginCommon.Data.EdGIS;
using com.github.fredjk_gh.PluginCommon.Data.Id64;
using com.github.fredjk_gh.PluginCommon.PluginInterop.Messages;
using com.github.fredjk_gh.PluginCommon.PluginInterop.Messages.Data;
using com.github.fredjk_gh.PluginCommon.Utilities;
using Observatory.Framework;
using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;
using Observatory.Framework.Interfaces;
using System.Diagnostics;
using System.Text;
using static com.github.fredjk_gh.PluginCommon.PluginInterop.PluginTracker;

namespace com.github.fredjk_gh.ObservatoryArchivist
{
    public class Archivist : IObservatoryNotifier, IObservatoryWorker
    {
        private static readonly Guid PLUGIN_GUID = new("0bec76f9-772b-4b0b-80fd-4809d23d394e");
        private static readonly AboutLink GH_LINK = new("github", "https://github.com/fredjk-gh/ObservatoryPlugins");
        private static readonly AboutLink GH_RELEASE_NOTES_LINK = new("github release notes", "https://github.com/fredjk-gh/ObservatoryPlugins/wiki/Plugin:-Archivist");
        private static readonly AboutLink DOC_LINK = new("Documentation", "https://observatory.xjph.net/usage/plugins/thirdparty/fredjk-gh/archivist");
        private static readonly AboutInfo ABOUT_INFO = new()
        {
            FullName = "Archivist",
            ShortName = "Archivist",
            Description = @"The Archivist plugin captures exploration related journals and notifications and stores them in a database for later re-use.

It also provides a cache of system positions (and other info) for improved distance calculations and recall.

May use data from Spansh, EDGIS and/or EDAstro.",
            AuthorName = "fredjk-gh",
            Links =
            [
                GH_LINK,
                GH_RELEASE_NOTES_LINK,
                DOC_LINK,
            ]
        };

        private PluginUI pluginUI;
        private ArchivistPanel _archivistPanel;
        private ArchivistSettings settings = new();
        private ArchivistContext _c;
        private ulong? _lastSharedSystemDataId64 = null;

        public static Guid Guid => PLUGIN_GUID;
        public AboutInfo AboutInfo => ABOUT_INFO;
        public string Version => typeof(Archivist).Assembly.GetName().Version.ToString();
        public PluginUI PluginUI => pluginUI;
        public bool OverrideAcceptNotificationsDuringBatch { get => true; }

        public object Settings
        {
            get => settings;
            set => settings = (ArchivistSettings)value;
        }

        public void Load(IObservatoryCore observatoryCore)
        {
            var errorLogger = observatoryCore.GetPluginErrorLogger(this);
            _c = new ArchivistContext(observatoryCore, this)
            {
                Settings = settings,
                ErrorLogger = errorLogger,
                Journals = new(observatoryCore.PluginStorageFolder, errorLogger),
                Notifications = new(observatoryCore.PluginStorageFolder, errorLogger),
                PositionCache = new(observatoryCore.PluginStorageFolder, errorLogger),
            };

            _c.Dispatcher.RegisterHandler<ArchivistPositionCacheSingleLookup>(HandleSinglePositionRequest);
            _c.Dispatcher.RegisterHandler<ArchivistPositionCacheSingle>(HandlePositionCacheAddSingle);
            _c.Dispatcher.RegisterHandler<ArchivistPositionCacheBatchLookup>(HandleBatchPositionRequest);
            _c.Dispatcher.RegisterHandler<ArchivistPositionCacheBatch>(HandlePositionCacheAddBatch);

            _c.DeserializeState();

            _archivistPanel = new ArchivistPanel(_c);
            pluginUI = new PluginUI(PluginUI.UIType.Panel, _archivistPanel);

            MaybeFixNewSettings();
        }

        private void MaybeFixNewSettings()
        {
            if (settings.JsonViewerFontSize < 5 || settings.JsonViewerFontSize > 24)
            {
                settings.JsonViewerFontSize = (int)_archivistPanel.Font.Size;
            }
        }

        public PluginUpdateInfo CheckForPluginUpdate()
        {
            if (_c is null) return new();

            AutoUpdateHelper.Init(_c.Core);
            return AutoUpdateHelper.CheckForPluginUpdate(
                this, GH_RELEASE_NOTES_LINK.Url, _c.Settings.EnableAutoUpdates, _c.Settings.EnableBetaUpdates);
        }

        public void ObservatoryReady()
        {
            _c.Core.ExecuteOnUIThread(() =>
            {
                _c.UI.Draw();
                _c.DisplaySummary();
            });

            _c.Dispatcher.SendMessage(GenericPluginReadyMessage.New());
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args)
        {
            // * -> ReadAll
            if ((args.NewState & LogMonitorState.Batch) != 0)
            {
                _c.Data.ResetForReadAll();

                // Re-connect in Direct mode for performance.
                _c.Connect(ConnectionMode.Direct);
                _c.ClearForReadAll();

                _c.BatchModeProcessing = true;
                _c.LastSystemId64DataShared = 0;
                _c.UI.SetMessage("Read All started");
            }
            // ReadAll -> *
            else if ((args.PreviousState & LogMonitorState.Batch) != 0 && (args.NewState & LogMonitorState.Batch) == 0)
            {
                _c.FlushIfDirty(/* force= */ true);
                _c.BatchModeProcessing = false; // Do before attempting to flush deferred data.
                _c.FinishReadAll();
                _c.Connect(ConnectionMode.Shared);

                // ReadAll -> Cancelled
                if ((args.NewState & LogMonitorState.BatchCancelled) != 0)
                {
                    _c.Core.ExecuteOnUIThread(() =>
                    {
                        _c.UI.Draw();
                        _c.DisplaySummary("Read All Cancelled; data is incomplete.");
                    });
                }
                else
                {
                    _c.Core.ExecuteOnUIThread(() =>
                    {
                        _c.UI.Draw();
                        _c.DisplaySummary("Read All completed");
                    });
                    // Only really helpful if the latest system was previously visited.
                    MaybeShareSystemDataWithAugmentation(_c.Data.ForCommander().CurrentSystem, /*allowAugmentation*/ false);
                }
                _c.SerializeState();
            }
            else if (args.PreviousState.HasFlag(LogMonitorState.PreRead) && !args.NewState.HasFlag(LogMonitorState.PreRead))
            {
                // TODO: Maybe send this after a short delay?
                MaybeShareSystemDataWithAugmentation(_c.Data.ForCommander().CurrentSystem, /*allowAugmentation*/ false);
            }
        }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            // needed to prevent spanshData journals from being collected as real journals.
            if (_c.IsResending) return;

            switch (journal)
            {
                case FileHeader fileHeader:
                    // This happens before we know who the Commander it. Cache it until needed (LoadGame).
                    _c.Data.LastFileHeader = fileHeader;
                    break;
                case LoadGame loadGame:
                    bool isNewCommander = false;
                    bool isDifferentCommander = _c.Data.CurrentCommander != loadGame.Commander;
                    if (!_c.Data.KnownCommanders.ContainsKey(loadGame.Commander))
                    {
                        isNewCommander = true;
                    }

                    _c.Data.CurrentCommander = loadGame.Commander;
                    // Keep only latest value to avoid relogging from clogging up the works.
                    _c.Data.ForCommander().FileHeaderInfo.FileHeader = _c.Data.LastFileHeader;
                    _c.Data.ForCommander().FileHeaderInfo.LoadGame = loadGame;

                    if (isNewCommander || isDifferentCommander)
                    {
                        if (!_c.IsReadAll)
                        {
                            _c.Core.ExecuteOnUIThread(() =>
                            {
                                _c.UI.Draw($"Switched to Cmdr {loadGame.Commander}.{(isNewCommander ? " o7!" : "")}");
                            });
                        }
                        _c.SerializeState();
                    }
                    break;
                case Statistics statistics:
                    // Keep only latest value to avoid relogging from clogging up the works.
                    _c.Data.ForCommander().FileHeaderInfo.Statistics = statistics;
                    break;
                case FSDJump fsdJump:
                    ProcessNewLocation(
                        fsdJump.StarSystem, 
                        fsdJump.SystemAddress,
                        fsdJump.TimestampDateTime, 
                        fsdJump.Json,
                        SharedLogic.IsWellKnownCoreSys(fsdJump.StarPos, _c.Data.SystemHasNavBeacon(fsdJump.SystemAddress), fsdJump.Powers));
                    CacheSystemPosition(fsdJump.SystemAddress, fsdJump.StarSystem, fsdJump.StarPos);
                    break;
                case Location location:
                    ProcessNewLocation(
                        location.StarSystem,
                        location.SystemAddress,
                        location.TimestampDateTime,
                        location.Json,
                        SharedLogic.IsWellKnownCoreSys(location.StarPos, _c.Data.SystemHasNavBeacon(location.SystemAddress), location.Powers));
                    CacheSystemPosition(location.SystemAddress, location.StarSystem, location.StarPos);
                    break;
                case NavRouteFile navRoute:
                    CacheSystemPositionsFromRoute(navRoute);
                    break;
                case DiscoveryScan discoveryScan:
                case FSSDiscoveryScan fssDiscoveryScan:
                case FSSBodySignals fssBodySignals:
                case FSSAllBodiesFound fssAllBodiesFound:
                case Scan scan:
                case ScanBaryCentre scanBaryCentre:
                case ScanOrganic scanOrganic:
                case NavBeaconScan navBeaconScan:
                case SAASignalsFound saaSignalsFound:
                case SAAScanComplete saaScanComplete:
                case CodexEntry codexEntry:
                    bool captured = false;
                    if (_c.Data.ForCommander()?.CurrentSystem != null && (_c.Core.CurrentLogMonitorState & LogMonitorState.PreRead) == 0)
                    {
                        _c.Data.ForCommander().CurrentSystem.AddSystemJournalJson(journal.Json, journal.TimestampDateTime);
                        _c.FlushIfDirty();
                    }

                    if (!_c.IsReadAll && captured)
                    {
                        _c.Core.ExecuteOnUIThread(() =>
                        {
                            if (!_c.Core.IsLogMonitorBatchReading)
                                _c.UI.SetMessage($"Captured journal event of type: {journal.Event}. {_c.Data.ForCommander().CurrentSystem.SystemJournalEntries.Count} entries captured so far...");
                        });
                    }
                    break;
                case FSSSignalDiscovered fssSignals:
                    if (fssSignals.SignalType == "NavBeacon")
                    {
                        _c.Data.TrackSystemNavBeacon(fssSignals.SystemAddress);

                        if (!_c.Core.IsLogMonitorBatchReading && _c.Data.ForCommander()?.CurrentSystem?.SystemId64 == fssSignals.SystemAddress)
                            // Share system data if we haven't already with un-conditional auto-augmentation because we have a nav beacon.
                            MaybeShareSystemDataWithAugmentation(_c.Data.ForCommander()?.CurrentSystem, /*doAutoAugmentation=*/ true);
                    }

                    break;
                case Shutdown shutdown:
                    _c.FlushIfDirty();
                    break;
            }
        }

        public byte[] ExportContent(string delimiter, ref string filetype)
        {
            if (_c.Search.CollectedJournals != null)
            {
                byte[] content = Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, _c.Search.CollectedJournals.SystemJournalEntries));
                filetype = $"{_c.Search.CollectedJournals.Commander}-{_c.Search.CollectedJournals.SystemName}.json";
                return content;
            }

            return null;
        }

        private void ProcessNewLocation(
            string newSystemName, ulong newSystemAddress, DateTime timestamp, string json, bool isCoreSystem)
        {
            var sys = _c.Data.ForCommander()?.CurrentSystem;
            if (!_c.Core.CurrentLogMonitorState.HasFlag(LogMonitorState.PreRead)
                && sys is not null
                && newSystemName != sys.SystemName)
            {
                _c.FlushIfDirty(/* force= */ true);
            }

            // We may get multiple events for the same system (ie. FSDJump, then Location after game restart).
            bool isDifferentSystem = sys is null
                || newSystemName != sys.SystemName;

            if (!isDifferentSystem) return;
            _lastSharedSystemDataId64 = null;

            // Initialize the current system during pre-read in case we find/do more stuff or haven't flushed it yet.
            // Duplicate entries should be filtered out by AddSystemJournalJson.
            //if (newSystemName == "Subra" || newSystemName == "Tiangchi")
            //    Debug.Fail("Debug loading code here");
            _c.Data.ForCommander().CurrentSystem =
                LoadOrInitVisitedSystemInfo(_c.Data.ForCommander().FileHeaderInfo, newSystemName, newSystemAddress, timestamp);
            _c.Data.MaybeAddToRecentSystems(newSystemName);
            _c.SerializeState();

            sys = _c.Data.ForCommander()?.CurrentSystem;
            // Don't add extraneous location events -- so only add the very first location change triggering event.
            bool isFirstVisit = false;
            if (sys.SystemJournalEntries.Count == 0)
            {
                isFirstVisit = true;
                _lastSharedSystemDataId64 = sys.SystemId64; // Suppress further share attempts
                sys.AddSystemJournalJson(json, timestamp);
            }

            // If we had existing data for this new system, share it. Skip if during read/all or pre-read -- this
            // is done explicitly after exiting these modes, above.
            // However, if that existing data is what we just added, above, don't share it. We need at last 3 
            // journals: FSD Jump, honk, primary star scan to initiate a share OR it's a known core system.
            if (!_c.Core.IsLogMonitorBatchReading && (!isFirstVisit || isCoreSystem))
                MaybeShareSystemDataWithAugmentation(sys, isCoreSystem);

            if (!_c.IsReadAll)
            {
                _c.Core.ExecuteOnUIThread(() =>
                {
                    _c.UI.PopulateCurrentSystemAndCommander();
                    _c.UI.PopulateRecentSystems();
                    _c.UI.SetMessage($"New system detected: {newSystemName}");
                });
            }
        }

        private CurrentSystemInfo LoadOrInitVisitedSystemInfo(FileHeaderInfo lastFileHeaderInfo, string starSystem, ulong systemAddress, DateTime timestampDateTime)
        {
            CurrentSystemInfo systemInfo;
            VisitedSystem systemData = _c.Journals.GetVisitedSystem(starSystem, lastFileHeaderInfo.Commander);
            // MUST use systemaddress here to ensure we de-dupe during read-all.
            List<NotificationInfo> notifs = _c.Notifications.GetNotifications(systemAddress, lastFileHeaderInfo.Commander);

            if (systemData != null)
            {
                systemInfo = new(systemData, notifs);
                systemInfo.VisitCount++;
                systemInfo.LastVisitedDateTime = timestampDateTime;
            }
            else
            {
                systemInfo = new(lastFileHeaderInfo, starSystem, systemAddress, timestampDateTime);
            }
            return systemInfo;
        }

        #region Journal Sharing

        enum DataShared
        {
            All,
            Partial,
            Nothing,
            AlreadyShared,
        }

        enum DataOrigin
        {
            PlayerJournals,
            SpanshCached,
            SpanshFetched,
        }

        private void MaybeShareSystemDataWithAugmentation(CurrentSystemInfo sysInfo, bool doAutoAugmentation)
        {
            if (sysInfo == null || _c.Core.IsLogMonitorBatchReading) return;
            if (_lastSharedSystemDataId64.HasValue && _lastSharedSystemDataId64.Value == sysInfo.SystemId64) return; // Already shared.

            var collectedShareResult = MaybeShareSystemData(sysInfo);
            if (collectedShareResult != DataShared.Nothing) return;

            // see if we have previous data from Spansh...
            VisitedSystem augmented = _c.Journals.GetExactMatchAugmentedSystem(sysInfo.SystemId64);
            CurrentSystemInfo augmentedSysinfo = null;
            if (augmented != null)
            {
                augmentedSysinfo = new(augmented, sysInfo.ToNotificationInfo());
                if (MaybeShareSystemData(augmentedSysinfo, DataOrigin.SpanshCached) != DataShared.Nothing) return;
            }

            // If we got here, we either didn't have previous spansh data OR it's incomplete.
            //
            // doAutoAugmentation indicates this is a "well known" or "core" system and we should
            // auto-fetch system data from Spansh, cache it and share that data with plugins.
            // Of course, subject to a setting.
            if (_c.Settings.AutoFetchWellKnownSystemsFromSpansh && doAutoAugmentation)
            {
                // Do this on a separate task.
                CancellationTokenSource cts = new(5000);
                Task.Run(() =>
                {
                    var augmentTask = SpanshAugmenter.FetchAndCacheFromSpansh(_c, sysInfo.ToVisitedSystem(), cts.Token);
                    
                    // It is safe to block here.
                    VisitedSystem spanshData = augmentTask.Result;

                    if (spanshData == null) return;
                    CurrentSystemInfo freshSysInfo = new(spanshData, sysInfo.ToNotificationInfo());

                    // Last attempt.
                    MaybeShareSystemData(freshSysInfo, DataOrigin.SpanshFetched);
                }, cts.Token);
                return;
            }

            // if we've gotten here, we've still not shared anything. Share any partial results we've got.
            MaybeShareSystemData(sysInfo, DataOrigin.PlayerJournals, /*forceShareIncomplete=*/ true);
        }

        // If the partial collected data is sent, then honks will be rehydrated (useful!) and continued scans will be consistent.
        // But don't share partial generated data.
        private DataShared MaybeShareSystemData(
            CurrentSystemInfo systemInfo,
            DataOrigin dataOrigin = DataOrigin.PlayerJournals,
            bool forceShareIncomplete = false)
        {
            // Send existing system data via inter-plugin message bus, in case anyone can use it.
            if (_c.IsReadAll
                || !settings.ShareSystemData
                || systemInfo == null)
                return DataShared.Nothing;
            if (_c.LastSystemId64DataShared == systemInfo.SystemId64) return DataShared.AlreadyShared;

            List<JournalBase> preamble = ArchivistData.ToJournalObj(_c.Core, systemInfo.PreambleJournalEntries);
            List<JournalBase> systemJournals = ArchivistData.ToJournalObj(_c.Core, systemInfo.SystemJournalEntries);

            var isCompleteScan = ArchivistData.IsSystemScanComplete(systemJournals);
            if (!isCompleteScan && !forceShareIncomplete)
            {
                _c.UI.SetMessage($"Stored data for {systemInfo.SystemName} is incomplete (source: {Misc.SplitCamelCase(dataOrigin.ToString())})!");
                if (dataOrigin == DataOrigin.PlayerJournals) return DataShared.Nothing;
            }

            ArchivistJournalsMessage msg = ArchivistJournalsMessage.New(
                systemInfo.SystemName,
                systemInfo.SystemId64,
                preamble,
                systemJournals,
                dataOrigin != DataOrigin.PlayerJournals,
                systemInfo.Commander,
                systemInfo.VisitCount);

            _c.LastSystemId64DataShared = systemInfo.SystemId64;
            _c.Dispatcher.SendMessage(msg);
            _c.Core.ExecuteOnUIThread(() =>
            {
                _c.UI.SetMessage($"Shared {systemInfo.SystemJournalEntries.Count} events for {systemInfo.SystemName} from {Misc.SplitCamelCase(dataOrigin.ToString())}.");
            });

            // No boolean response from this. Either we have it or we don't.
            MaybeAlsoShareNotificationData(systemInfo);
            _lastSharedSystemDataId64 = systemInfo.SystemId64;
            return !isCompleteScan ? DataShared.Partial :  DataShared.All;
        }

        private void MaybeAlsoShareNotificationData(CurrentSystemInfo systemInfo)
        {
            if (systemInfo.Notifications.Count == 0) return;
            if (!_c.Tracker.IsActive(PluginType.fredjk_Aggregator)) return; // Nobody listening.


            ArchivistNotificationsMessage msg = ArchivistNotificationsMessage.New(
                systemInfo.SystemName,
                systemInfo.SystemId64,
                systemInfo.Commander,
                systemInfo.GetNotificationArgs());

            _c.Dispatcher.SendMessage(msg, PluginType.fredjk_Aggregator);
            _c.Core.ExecuteOnUIThread(() =>
            {
                _c.UI.SetMessage($"Shared {systemInfo.Notifications.Count} cached notifications for {systemInfo.SystemName} from a previous visit.");
            });
        }

        #endregion

        #region Position Cache - Journal event handlers
        private void CacheSystemPosition(ulong id64, string systemName, StarPosition position)
        {
            SystemInfo info = new(id64, systemName, position);

            _c.PositionCache.UpsertSystem(info);
        }

        private void CacheSystemPositionsFromRoute(NavRouteFile route)
        {
            List<SystemInfo> systems = [];

            foreach (var r in route.Route)
            {
                SystemInfo i = new(r.SystemAddress, r.StarSystem, r.StarPos);
                systems.Add(i);
            }

            _c.PositionCache.UpsertBatch(systems);
        }
        #endregion

        #region Plugin MessageBus handling

        public void HandlePluginMessage(MessageSender sender, PluginMessage messageArgs)
        {
            _c.Dispatcher.MaybeHandlePluginMessage(sender, messageArgs, out _);
        }

        private void HandleSinglePositionRequest(ArchivistPositionCacheSingleLookup singleReq)
        {
            if (_c.IsReadAll) return;

            List<ArchivistPositionCacheRequestItem> reqs =
            [
                new()
                {
                    SystemName = singleReq.SystemName,
                    SystemId64 = singleReq.SystemId64,
                },
            ];

            List<ArchivistPositionCacheItem> results = ProcessAddressCacheRequest(reqs, singleReq.ExternalFallback);
            if (results.Count > 0)
            {
                var result = results[0];
                var response = singleReq.ReplyWith(result.SystemName, result.SystemId64, result.X, result.Y, result.Z);
                _c.Dispatcher.SendResponse(singleReq, response);
            }
        }

        private void HandleBatchPositionRequest(ArchivistPositionCacheBatchLookup posReq)
        {
            if (_c.IsReadAll) return;

            List<ArchivistPositionCacheItem> results = ProcessAddressCacheRequest(posReq.RequestItems, posReq.ExternalFallback);

            if (results.Count > 0)
            {
                var response = posReq.ReplyWith(results);
                _c.Dispatcher.SendResponse(posReq, response);
            }
        }

        class TaskState(int d, string sysName, ulong? sysId64)
        {
            internal int delayMs = d;
            internal string systemName = sysName;
            internal ulong? systemId64 = sysId64;
        }

        private List<ArchivistPositionCacheItem> ProcessAddressCacheRequest(List<ArchivistPositionCacheRequestItem> posReqs, bool extFallback)
        {
            List<ArchivistPositionCacheItem> results = [];
            List<SystemInfo> newItems = [];
            List<Task<SystemInfo>> externalReqs = [];
            int extReqDelayMs = 0;

            foreach (var req in posReqs)
            {
                if ((string.IsNullOrWhiteSpace(req.SystemName) && req.SystemId64 == 0) || _c.IsReadAll)
                {
                    continue; // Empty request. Can't do anything, so ignore.
                }

                // Look up in the DB. If not found and requested, fallback to
                // https://edgis.elitedangereuse.fr/coords?q=<system name or id64> -- and cache the response.

                UInt64? id64 = null;
                SystemInfo result = null;
                if (!string.IsNullOrEmpty(req.SystemName))
                {
                    result = _c.PositionCache.GetSystem(req.SystemName);
                }
                else if (req.SystemId64 >= 0)
                {
                    id64 = req.SystemId64;
                    result = _c.PositionCache.GetSystem(req.SystemId64);
                }

                TaskState taskState = new(extReqDelayMs, req.SystemName, id64);

                if (result is null && extFallback)
                {
                    CancellationTokenSource cts = new(5000);
                    Task<SystemInfo> extReq = Task.Factory.StartNew(state =>
                    {
                        TaskState myState = (TaskState)state;
                        Debug.WriteLine($"[Task] Fetching coordinates for {myState.systemName} | {myState.systemId64} after a delay of {myState.delayMs} ms...");

                        Task.Delay(myState.delayMs); // Ratelimit
                        var coordsResponse = EdGISHelper.LookupCoords(_c.Core.HttpClient, cts.Token, myState.systemName, id64);

                        Id64Details details = Id64Details.FromId64(coordsResponse.SystemId64);
                        result = new()
                        {
                            CommonName = coordsResponse.SystemName,
                            ProcGenName = details.ProcGenSystemName,
                            Id64 = coordsResponse.SystemId64,
                            x = coordsResponse.Coords.X,
                            y = coordsResponse.Coords.Y,
                            z = coordsResponse.Coords.Z,
                        };
                        Debug.WriteLine($"[Task] Fetching coordinates for {myState.systemName} | {myState.systemId64} completed");
                        return result;

                    }, taskState, cts.Token);

                    externalReqs.Add(extReq);
                    extReqDelayMs += 100;
                }
                else if (result is not null)
                {
                    results.Add(new()
                    {
                        SystemName = result.CommonName,
                        SystemId64 = result.Id64,
                        X = result.x,
                        Y = result.y,
                        Z = result.z
                    });
                }
            }

            if (externalReqs.Count > 0)
            {
                Task.WhenAll(externalReqs).Wait();

                foreach (var externalReq in externalReqs)
                {
                    try
                    {
                        var sysInfo = externalReq.Result;
                        newItems.Add(sysInfo);
                        results.Add(new()
                        {
                            SystemName = sysInfo.CommonName,
                            SystemId64 = sysInfo.Id64,
                            X = sysInfo.x,
                            Y = sysInfo.y,
                            Z = sysInfo.z
                        });
                    }
                    catch (Exception e)
                    {
                        TaskState state = externalReq.AsyncState as TaskState;
                        _c.UI.SetMessage($"ProcessAddressCacheRequest: Async EdGIS fetch failed for {state?.systemName ?? "<unknown>"}: {e.Message}");
                    }
                }
            }

            if (newItems.Count > 0)
            {
                _c.PositionCache.UpsertBatch(newItems);
            }
            return results;
        }

        private void HandlePositionCacheAddSingle(ArchivistPositionCacheSingle addReq)
        {
            HandlePositionCacheAdd([addReq.Position]);
        }

        private void HandlePositionCacheAddBatch(ArchivistPositionCacheBatch addReq)
        {
            HandlePositionCacheAdd(addReq.Items);
        }

        private void HandlePositionCacheAdd(List<ArchivistPositionCacheItem> itemsToAdd)
        {
            List<SystemInfo> validatedItems = [];

            foreach (var item in itemsToAdd)
            {
                StarPosition posToCache = item.ToStarPosition();
                Id64Coords est = Id64CoordHelper.EstimatedCoords(item.SystemId64);
                if (!est.ValidateStarPosition(posToCache))
                {
                    continue; // Doesn't pass id64 validation.
                }
                validatedItems.Add(new(item.SystemId64, item.SystemName, posToCache));
            }

            _c.PositionCache.UpsertBatch(validatedItems);
        }
        #endregion

        #region Notification Handling
        public void OnNotificationEvent(NotificationArgs args)
        {
            // Since the worker part does not reflect state changes, we should not collect notifications 
            // during resend to avoid mixing data from differnt systems.
            //
            // TODO: Implement some sort of filter.
            // Treat an  NULL coalescing ID as "default"; ie. 1001.
            if (_c.IsResending
                || !args.CoalescingId.HasValue || args.CoalescingId < -1 || args.CoalescingId >= 1001)
                return;

            // De-duping happens internally.
            if (args.Sender == "Commander" && args.Title == "Carrier Status Update")
                Debug.Fail("Why not filtered?");

            _c.Data.ForCommander().CurrentSystem.AddNotificationArg(args);
        }
        #endregion
    }
}
