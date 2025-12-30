using com.github.fredjk_gh.ObservatoryArchivist.DB;
using com.github.fredjk_gh.ObservatoryArchivist.UI;
using com.github.fredjk_gh.PluginCommon.AutoUpdates;
using com.github.fredjk_gh.PluginCommon.PluginInterop;
using com.github.fredjk_gh.PluginCommon.PluginInterop.Marshalers;
using com.github.fredjk_gh.PluginCommon.PluginInterop.Messages;
using Observatory.Framework;
using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;
using Observatory.Framework.Interfaces;
using System.Text;
using static com.github.fredjk_gh.PluginCommon.PluginInterop.PluginTracker;

namespace com.github.fredjk_gh.ObservatoryArchivist
{
    public class Archivist : IObservatoryWorker
    {
        private static Guid PLUGIN_GUID = new("0bec76f9-772b-4b0b-80fd-4809d23d394e");
        private static AboutLink GH_LINK = new("github", "https://github.com/fredjk-gh/ObservatoryPlugins");
        private static AboutLink GH_RELEASE_NOTES_LINK = new ("github release notes", "https://github.com/fredjk-gh/ObservatoryPlugins/wiki/Plugin:-Archivist");
        private static AboutLink DOC_LINK = new("Documentation", "https://observatory.xjph.net/usage/plugins/thirdparty/fredjk-gh/archivist");
        private static AboutInfo ABOUT_INFO = new()
        {
            FullName = "Archivist",
            ShortName = "Archivist",
            Description = "The Archivist plugin captures exploration related journals and stores them in a database for later re-use."
                    + Environment.NewLine + Environment.NewLine
                    + "It also provides a cache of system positions (and other info) for improved distance calculations and recall.",
            AuthorName = "fredjk-gh",
            Links = new()
            {
                GH_LINK,
                GH_RELEASE_NOTES_LINK,
                DOC_LINK,
            }
        };
        
        private PluginUI pluginUI;
        private ArchivistPanel _archivistPanel;
        private ArchivistSettings settings = new();
        private ArchivistContext _context;

        public static Guid Guid => PLUGIN_GUID;
        public AboutInfo AboutInfo => ABOUT_INFO;
        public string Version => typeof(Archivist).Assembly.GetName().Version.ToString();
        public PluginUI PluginUI => pluginUI;

        public object Settings
        {
            get => settings;
            set => settings = (ArchivistSettings)value;
        }

        public void Load(IObservatoryCore observatoryCore)
        {
            
            _context = new ArchivistContext()
            {
                Core = observatoryCore,
                PluginWorker = this,
                Settings = settings,
                ErrorLogger = observatoryCore.GetPluginErrorLogger(this),
                Archive = new(observatoryCore.PluginStorageFolder, observatoryCore.GetPluginErrorLogger(this)),
                Cache = new(observatoryCore.PluginStorageFolder, observatoryCore.GetPluginErrorLogger(this)),
            };
            _context.DeserializeState();

            _archivistPanel = new ArchivistPanel(_context);
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
            AutoUpdateHelper.Init(_context.Core);
            return AutoUpdateHelper.CheckForPluginUpdate(
                this, GH_RELEASE_NOTES_LINK.Url, _context.Settings.EnableAutoUpdates, _context.Settings.EnableBetaUpdates);
        }

        public void ObservatoryReady()
        {
            _context.Core.ExecuteOnUIThread(() =>
            {
                _context.UI.Draw();
                _context.DisplaySummary();
            });

            var readyMsg = GenericPluginReadyMessage.New();
            _context.Core.SendPluginMessage(this, readyMsg.ToPluginMessage());
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args)
        {
            // * -> ReadAll
            if ((args.NewState & LogMonitorState.Batch) != 0)
            {
                _context.Data.ResetForReadAll();

                // Re-connect in Direct mode for performance.
                _context.Archive.Connect(ConnectionMode.Direct);
                _context.Archive.ClearVisitedSystems();
                _context.Cache.Connect(ConnectionMode.Direct);
                // Don't ever clear the cache (or only do so manually). That would lose data we can't recover via re-reading all.

                _context.BatchModeProcessing = true;
                _context.LastSystemId64DataShared = 0;
                _context.UI.SetMessage( "Read All started");
            }
            // ReadAll -> *
            else if ((args.PreviousState & LogMonitorState.Batch) != 0 && (args.NewState & LogMonitorState.Batch) == 0)
            {
                _context.FlushIfDirty(/* force= */ true);
                _context.BatchModeProcessing = false;

                _context.Cache.FinishReadAll();
                _context.Cache.Connect(ConnectionMode.Shared);

                _context.Archive.FlushDeferredVisitedSystems();
                _context.Archive.Connect(ConnectionMode.Shared);

                // ReadAll -> Cancelled
                if ((args.NewState & LogMonitorState.BatchCancelled) != 0)
                {
                    _context.Core.ExecuteOnUIThread(() =>
                    {
                        _context.UI.Draw();
                        _context.DisplaySummary("Read All Cancelled; data is incomplete.");
                    });
                }
                else
                {
                    _context.Core.ExecuteOnUIThread(() =>
                    {
                        _context.UI.Draw();
                        _context.DisplaySummary("Read All completed");
                    });
                    MaybeShareSystemDataWithAugmentation(_context.Data.ForCommander().CurrentSystem);
                }
                _context.SerializeState();
            }
            else if (args.PreviousState.HasFlag(LogMonitorState.PreRead) && !args.NewState.HasFlag(LogMonitorState.PreRead))
            {
                MaybeShareSystemDataWithAugmentation(_context.Data.ForCommander().CurrentSystem);
            }
        }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            if (_context.IsResending) return;

            switch (journal)
            {
                case FileHeader fileHeader:
                    // This happens before we know who the Commander it. Cache it until needed (LoadGame).
                    _context.Data.LastFileHeader = fileHeader;
                    break;
                case LoadGame loadGame:
                    bool isNewCommander = false;
                    bool isDifferentCommander = _context.Data.CurrentCommander != loadGame.Commander;
                    if (!_context.Data.KnownCommanders.ContainsKey(loadGame.Commander))
                    {
                        isNewCommander = true;
                    }

                    _context.Data.CurrentCommander = loadGame.Commander;
                    // Keep only latest value to avoid relogging from clogging up the works.
                    _context.Data.ForCommander().FileHeaderInfo.FileHeader = _context.Data.LastFileHeader;
                    _context.Data.ForCommander().FileHeaderInfo.LoadGame = loadGame;

                    if (isNewCommander || isDifferentCommander)
                    {
                        if (!_context.IsReadAll)
                        {
                            _context.Core.ExecuteOnUIThread(() =>
                            {
                                _context.UI.Draw($"Switched to Cmdr {loadGame.Commander}.{(isNewCommander ? " o7!" : "")}");
                            });
                        }
                        _context.SerializeState();
                    }
                    break;
                case Statistics statistics:
                    // Keep only latest value to avoid relogging from clogging up the works.
                    _context.Data.ForCommander().FileHeaderInfo.Statistics = statistics;
                    break;
                case FSDJump fsdJump:
                    ProcessNewLocation(fsdJump.StarSystem, fsdJump.SystemAddress, fsdJump.TimestampDateTime, fsdJump.Json);
                    CacheSystemLocation(fsdJump.SystemAddress, fsdJump.StarSystem, fsdJump.StarPos);
                    break;
                case Location location:
                    ProcessNewLocation(location.StarSystem, location.SystemAddress, location.TimestampDateTime, location.Json);
                    CacheSystemLocation(location.SystemAddress, location.StarSystem, location.StarPos);
                    break;
                case NavRouteFile navRoute:
                    CacheRoute(navRoute);
                    break;
                case DiscoveryScan discoveryScan:
                case FSSDiscoveryScan fssDiscoveryScan:
                case FSSBodySignals fssBodySignals:
                // case FSSSignalDiscovered fssSignalDiscovered:
                case FSSAllBodiesFound fssAllBodiesFound:
                case Scan scan:
                case ScanBaryCentre scanBaryCentre:
                case ScanOrganic scanOrganic:
                case NavBeaconScan navBeaconScan:
                case SAASignalsFound saaSignalsFound:
                case SAAScanComplete saaScanComplete:
                case CodexEntry codexEntry:
                    if (_context.Data.ForCommander()?.CurrentSystem != null && (_context.Core.CurrentLogMonitorState & LogMonitorState.PreRead) == 0)
                    {
                        _context.Data.ForCommander().CurrentSystem.AddSystemJournalJson(journal.Json, journal.TimestampDateTime);
                        _context.FlushIfDirty();
                    }

                    if (!_context.IsReadAll)
                    {
                        _context.Core.ExecuteOnUIThread(() =>
                        {
                            _context.UI.PopulateLatestEntry();
                            if (!_context.Core.IsLogMonitorBatchReading)
                                _context.UI.SetMessage($"Captured journal event of type: {journal.Event}.{Environment.NewLine}{_context.Data.ForCommander().CurrentSystem.SystemJournalEntries.Count} entries captured so far...");
                        });
                    }
                    break;
                case Shutdown shutdown:
                    _context.FlushIfDirty();
                    break;
            }
        }

        public byte[] ExportContent(string delimiter, ref string filetype)
        {
            if (_context.Data.LastSearchResult != null)
            {
                byte[] content = Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, _context.Data.LastSearchResult.SystemJournalEntries));
                filetype = $"{_context.Data.LastSearchResult.Commander}-{_context.Data.LastSearchResult.SystemName}.json";
                return content;
            }

            return null;
        }

        private void ProcessNewLocation(string newSystemName, ulong newSystemAddress, DateTime timestamp, string json)
        {
            if (!_context.Core.CurrentLogMonitorState.HasFlag(LogMonitorState.PreRead)
                && _context.Data.ForCommander()?.CurrentSystem != null
                && newSystemName != _context.Data.ForCommander()?.CurrentSystem.SystemName)
            {
                _context.FlushIfDirty(/* force= */ true);
            }

            // We may get multiple events for the same system (ie. FSDJump, then Location after game restart).
            bool isDifferentSystem = _context.Data.ForCommander()?.CurrentSystem == null
                || newSystemName != _context.Data.ForCommander()?.CurrentSystem.SystemName;

            if (!isDifferentSystem) return;

            // Initialize the current system during pre-read in case we find/do more stuff or haven't flushed it yet.
            // Duplicate entries should be filtered out by AddSystemJournalJason.
            _context.Data.ForCommander().CurrentSystem = _context.Archive.LoadOrInitVisitedSystemInfo(
                _context.Data.ForCommander().FileHeaderInfo, newSystemName, newSystemAddress, timestamp);
            _context.Data.MaybeAddToRecentSystems(newSystemName);
            _context.SerializeState();

            // Don't add extraneous location events -- so only add the very first location change triggering event.
            if (_context.Data.ForCommander()?.CurrentSystem.SystemJournalEntries.Count == 0)
                _context.Data.ForCommander().CurrentSystem.AddSystemJournalJson(json, timestamp);
            else
            {
                // If we had existing data for this new system, share it.
                MaybeShareSystemDataWithAugmentation(_context.Data.ForCommander().CurrentSystem);
            }

            if (!_context.IsReadAll)
            {
                _context.Core.ExecuteOnUIThread(() =>
                {
                    _context.UI.PopulateCurrentSystem();
                    _context.UI.PopulateRecentSystems();
                    _context.UI.SetMessage($"New system detected.");
                });
            }
        }

        private void MaybeShareSystemDataWithAugmentation(CurrentSystemInfo sysInfo)
        {
            if (sysInfo == null) return;

            if (!MaybeShareSystemData(sysInfo))
            {
                // see if we have data from Spansh...
                VisitedSystem augmented = _context.Archive.GetExactMatchAugmentedSystem(sysInfo.SystemId64);
                if (augmented == null) return;
                CurrentSystemInfo sysinfo = new(augmented);
                MaybeShareSystemData(sysinfo, /*isGeneratedFromSpansh=*/ true);
            }
        }

        private bool MaybeShareSystemData(CurrentSystemInfo systemInfo, bool isGeneratedFromSpansh = false)
        {
            // Send existing system data via inter-plugin message bus, in case anyone can use it.
            if (_context.IsReadAll
                || !settings.ShareSystemData
                || systemInfo == null
                || _context.LastSystemId64DataShared == systemInfo.SystemId64) return false;

            List<JournalBase> preamble = ArchivistData.ToJournalObj(_context.Core, systemInfo.PreambleJournalEntries);
            List<JournalBase> systemJournals = ArchivistData.ToJournalObj(_context.Core, systemInfo.SystemJournalEntries);

            if (!ArchivistData.IsSystemScanComplete(preamble, systemJournals))
            {
                _context.UI.SetMessage($"Stored data for this system is incomplete (generated? {isGeneratedFromSpansh}); data was not shared.");
                return false;
            }

            ArchivistScansMessage msg = ArchivistScansMessage.New(
                systemInfo.SystemName,
                systemInfo.SystemId64,
                preamble,
                systemJournals,
                isGeneratedFromSpansh,
                systemInfo.Commander,
                systemInfo.VisitCount);

            _context.LastSystemId64DataShared = systemInfo.SystemId64;
            _context.Core.SendPluginMessage(this, msg.ToPluginMessage());
            _context.Core.ExecuteOnUIThread(() =>
            {
                _context.UI.SetMessage($"Shared {systemInfo.SystemJournalEntries.Count} events from {(isGeneratedFromSpansh ? "Spansh": "a previous visit")}.");
            });
            return true;
        }

        private void CacheSystemLocation(ulong id64, string systemName, StarPosition position)
        {
            SystemInfo info = new(id64, systemName, position);

            _context.Cache.UpsertSystem(info);
        }

        private void CacheRoute(NavRouteFile route)
        {
            List<SystemInfo> systems = new();

            foreach (var r in route.Route)
            {
                SystemInfo i = new(r.SystemAddress, r.StarSystem, r.StarPos);
                systems.Add(i);
            }

            _context.Cache.UpsertBatch(systems);
        }

        public void HandlePluginMessage(MessageSender sender, PluginMessage messageArgs)
        {
            PluginMessageWrapper w = new(sender, messageArgs);

            if (w.Type == "LegacyPluginMessage")
            {
                LegacyPluginMessageWrapper legacy = new(sender.ShortName, sender.Version, messageArgs.MessagePayload["message"]);

                LegacyPluginMessageWrapper unMarshaledLegacy;
                if (!PluginMessageUnmarshaler.TryUnmarshal(legacy, out unMarshaledLegacy)) return;

                switch (unMarshaledLegacy)
                {
                    case LegacyGenericPluginReadyMessage readyMsg:
                        HandleLegacyReadyMessage(readyMsg);
                        break;
                }
            }
            else
            {
                PluginMessageWrapper unMarshaled;
                if (!PluginMessageUnmarshaler.TryUnmarshal(w, out unMarshaled)) return;

                switch (unMarshaled)
                {
                    case GenericPluginReadyMessage readyMsg:
                        HandleReadyMessage(readyMsg);
                        break;
                }
            }
        }

        private void HandleReadyMessage(GenericPluginReadyMessage readyMsg)
        {
            var pluginType = PluginTracker.PluginTypeByGuid.GetValueOrDefault(readyMsg.Sender.Guid, PluginType.Unknown);
            switch (pluginType)
            {
                case PluginType.mattg_Evaluator:

                    break;
            }
        }

        private void HandleLegacyReadyMessage(LegacyGenericPluginReadyMessage readyMsg)
        {
            switch (readyMsg.SourceOrTargetName)
            {
                case "Evaluator":
                    break;
            }
        }

        // Potential messages to support:
        // 
        // Misc:
        // -> Ready {sender}
        //
        // Main Archive:
        // <- ArchivistScansRequest { id64, forReplay } // for current cmdr only?
        //   -- if forReplay is true, response is sent via replay.
        // -> DONE: ArchivistScansMessage { id64, list<string> journals, visitCount, Cmdr }
        //
        // Position cache:
        // <- ArchivistSystemCoordsRequest { id64 }
        // -> ArchivistSystemCoordsMessage { id64, x, y, z }
        // <- ReportSystemCoords { id64, x, y, z, SysName } // For data requested outside the game (ie. spansh routes, etc.)
        //
        // Notification cache:
        // -- Like aggregator, listen to notifications and cache them.
        // -- Like system journals, if previously visited, send.
        // 
        // Station cache?
        //
        // Bookmarks/notes:
        // <- SetSystemBookmark {id64, bool visited, text? }
        // <- ClearSystemBookmark {id64 }
        // -> SystemBookmark { id64, ?? }
    }
}