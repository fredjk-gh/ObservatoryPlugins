using Accessibility;
using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using com.github.fredjk_gh.ObservatoryStatScanner.StateManagement;
using com.github.fredjk_gh.PluginCommon.AutoUpdates;
using com.github.fredjk_gh.PluginCommon.Data;
using com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs;
using com.github.fredjk_gh.PluginCommon.PluginInterop.Messages;
using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;

namespace com.github.fredjk_gh.ObservatoryStatScanner
{
    public class StatScanner : IObservatoryWorker
    {
        private readonly static Guid PLUGIN_GUID = new("398750b9-ffab-4d28-959b-3fc5648853eb");
        private const string STATE_CACHE_FILENAME = "stateCache.json";
        private readonly static AboutLink GH_LINK = new("github", "https://github.com/fredjk-gh/ObservatoryPlugins");
        private readonly static AboutLink GH_RELEASE_NOTES_LINK = new("github release notes", "https://github.com/fredjk-gh/ObservatoryPlugins/wiki/Plugin:-Stat-Scanner");
        private readonly static AboutLink DOC_LINK = new("Documentation", "https://observatory.xjph.net/usage/plugins/thirdparty/fredjk-gh/statscanner");
        private readonly static AboutLink EDASTRO_RECORDS_LINK = new("edastro.com Records", "https://edastro.com/records/");
        private readonly static AboutInfo ABOUT_INFO = new()
        {
            FullName = "Stat Scanner",
            ShortName = "Stat Scanner",
            Description = "Stat Scanner is your own personal record book of personal bests but can also compares your discoveries against one of two sets of galactic records provided by edastro.com.",
            AuthorName = "fredjk-gh",
            Links =
            [
                GH_LINK,
                GH_RELEASE_NOTES_LINK,
                DOC_LINK,
                EDASTRO_RECORDS_LINK,
            ]
        };

        private StatScannerSettings _settings = new();
        private PluginUI pluginUI;
        private bool HasHeaderRows = false;
        private StatScannerContext _c = null;
        private readonly List<Result> _batchResults = [];
        ObservableCollection<object> gridCollection = [];


        public static Guid Guid => PLUGIN_GUID;
        public AboutInfo AboutInfo => ABOUT_INFO;
        public string Version => typeof(StatScanner).Assembly.GetName().Version.ToString();
        public PluginUI PluginUI => pluginUI;

        public object Settings
        {
            get => _c?.Settings ?? _settings;
            set
            {
                if (_c is not null)
                {
                    // The context setter also fixes settings.
                    _settings = (_c.Settings = (StatScannerSettings)value);
                }
                else
                    _settings = (StatScannerSettings)value;
            }
        }

        public void Load(IObservatoryCore observatoryCore)
        {
            StateCache cached = DeserializeCachedState(observatoryCore);
            cached.CheckForNewAssemblyVersion();
            _c = new(cached, observatoryCore, _settings, this);
            _settings = _c.Settings;

            gridCollection.Add(new StatScannerGrid());
            pluginUI = new PluginUI(gridCollection);

            _c.MaybeUpdateGalacticRecords();

            if (_c.Cacheable.KnownCommanders.Count == 0)
            {
                _c.Cacheable.SetReadAllRequired("No known commanders.");
                _c.Core.AddGridItems(this, _c.GetPluginStateMessages());
            }

            SerializeState(true);
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
            if (_c is null) return;
            ShowPersonalBestSummary();

            var readyMsg = GenericPluginReadyMessage.New();
            _c.Core.SendPluginMessage(this, readyMsg.ToPluginMessage());
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args)
        {
            // * -> ReadAll
            if ((args.NewState & LogMonitorState.Batch) != 0)
            {
                _c.ResetForReadAll();
                _c.Core.ClearGrid(this, new StatScannerGrid());
                _batchResults.Clear();
                HasHeaderRows = false;
            }
            // ReadAll -> *
            else if ((args.PreviousState & LogMonitorState.Batch) != 0 && (args.NewState & LogMonitorState.Batch) == 0)
            {
                if (!_c.Settings.EnableGridOutputAfterReadAll)
                    _batchResults.Clear(); // Dump what we have accumulated until now.

                if ((args.NewState & LogMonitorState.BatchCancelled) != 0)
                {
                    _c.Cacheable.SetReadAllRequired("Read-all was cancelled; incomplete data.");
                }
                else
                {
                    _c.ReadAllComplete();
                }

                ShowPersonalBestSummary();
                _c.Core.AddGridItems(this, _batchResults);
                //AddResultsToGrid_(_batchResults, false);
                _batchResults.Clear();

                SerializeState(true);
            }
        }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            switch (journal)
            {
                case FileHeader fileHeader:
                    _c.IsOdyssey = fileHeader.Odyssey;
                    break;
                case LoadGame loadGame:
                    var previousCommanderName = _c.CommanderName;
                    _c.LastLoadGame = loadGame;
                    if (previousCommanderName != _c.CommanderName)
                    {
                        Debug.WriteLine($"[{loadGame.Timestamp}] Switching commanders: {_c.CommanderName}");
                        AddResultsToGrid(
                        [
                            new(NotificationClass.None,
                                new StatScannerGrid
                                {
                                    ObjectClass = "Plugin message",
                                    Variable = "Commander",
                                    Function = "Changed to",
                                    BodyOrItem = _c.CommanderName,
                                },
                                CoalescingIDs.HEADER_COALESCING_ID)
                        ]);

                    }
                    break;
                case Statistics statistics:
                    // TODO: do more with this?
                    _c.LastStats = statistics;
                    if (!_c.Core.IsLogMonitorBatchReading)
                    {
                        AddResultsToGrid(MaybeAddStats(_c.CommanderName, statistics));
                    }
                    break;
                case FSDJump fsdJump:
                    // Track this here because it's not present in older Scans.
                    _c.CurrentSystem = fsdJump.StarSystem;
                    break;
                case Location location:
                    _c.CurrentSystem = location.StarSystem;
                    break;
                case DiscoveryScan discoveryScan:
                    _c.SystemBodyCount = discoveryScan.Bodies;
                    break;
                case NavBeaconScan navBeaconScan:
                    _c.SystemBodyCount = navBeaconScan.NumBodies;
                    _c.NavBeaconScanned = true;
                    break;
                case Scan scan:
                    OnScan(scan);

                    if (_c.NavBeaconScanned && _c.Scans.Count == _c.SystemBodyCount)
                    {
                        // Nav beacon was used; generate an AllBodies Found event.
                        OnFssAllBodiesFound(new()
                        {
                            Count = _c.Scans.Count,
                            SystemName = _c.CurrentSystem,
                            Event = "FSSAllBodiesFound",
                            SystemAddress = scan.SystemAddress,
                        });
                    }
                    break;
                case FSSBodySignals bodySignals:
                    OnFssBodySignals(bodySignals);
                    break;
                case FSSAllBodiesFound fssAllBodies:
                    OnFssAllBodiesFound(fssAllBodies);
                    break;
                case CodexEntry codexEntry:
                    OnCodexEntry(codexEntry);
                    break;

            }
        }

        // Requires core explicitly because _c is not initialized yet when this is first called.
        private StateCache DeserializeCachedState(IObservatoryCore core)
        {
            string dataCacheFile = $"{core.PluginStorageFolder}{STATE_CACHE_FILENAME}";
            if (!File.Exists(dataCacheFile)) return new();

            StateCache stateCache = new();
            try
            {
                string jsonString = File.ReadAllText(dataCacheFile);
                stateCache = JsonSerializer.Deserialize<StateCache>(jsonString)!;
            }
            catch (Exception ex)
            {
                _c.Core.GetPluginErrorLogger(this)(ex, "Deserializing state cache");
            }
            return stateCache;
        }

        public void SerializeState(bool forceWrite = false)
        {
            if (((_c.Core.CurrentLogMonitorState & LogMonitorState.Batch) != 0 && !forceWrite) || !_c.Cacheable.IsDirty) return;

                string dataCacheFile = $"{_c.Core.PluginStorageFolder}{STATE_CACHE_FILENAME}";
            string jsonString = JsonSerializer.Serialize(_c.Cacheable, JsonHelper.PRETTY_PRINT_OPTIONS);
            File.WriteAllText(dataCacheFile, jsonString);
        }

        private void MaybeAddHeaderRows()
        {
            if (_c.Core.IsLogMonitorBatchReading || HasHeaderRows) return;

            if (!_c.IsCommanderFidKnown || _c.NeedsReadAll || _c.Cacheable.KnownCommanders.Count == 0)
            {
                _c.Core.AddGridItems(this, _c.GetPluginStateMessages());
            }

            var devModeWarning = (_c.Settings.DevMode ? "!!DEV mode!!" : "");
            var handlingMode = (StatScannerSettings.ProcGenHandlingMode)_c.Settings.ProcGenHandlingOptions[_c.Settings.ProcGenHandling];
            var gridItems = new List<Result>
            {
                new (NotificationClass.None,
                    new StatScannerGrid
                    {
                        ObjectClass = "Plugin info",
                        Variable = $"{AboutInfo.ShortName} version",
                        ObservedValue = $"v{Version}",
                    },
                    CoalescingIDs.HEADER_COALESCING_ID),
                new (NotificationClass.None,
                    new StatScannerGrid
                    {
                        ObjectClass = "Plugin settings",
                        Variable = "Tracking First Discoveries only?",
                        ObservedValue = $"{_c.Settings.FirstDiscoveriesOnly}",
                    },
                    CoalescingIDs.HEADER_COALESCING_ID),
            };
            foreach (var Cmdr in _c.Cacheable.KnownCommanders)
            {
                var recordBook = _c.GetRecordBookForFID(Cmdr.Key);

                gridItems.Add(new(
                    NotificationClass.None,
                    new StatScannerGrid
                    {
                        ObjectClass = "Plugin stats",
                        Variable = "Tracked Record(s)",
                        Function = Function.Count.ToString(),
                        BodyOrItem = "Total",
                        ObservedValue = $"{recordBook.Count}",
                        RecordHolder = Cmdr.Value.Name,
                        RecordValue = devModeWarning,
                    },
                    CoalescingIDs.HEADER_COALESCING_ID));

                foreach (RecordKind kind in Enum.GetValues(typeof(RecordKind)))
                {
                    var count = recordBook.CountByKind(kind);
                    var details = "";
                    switch (kind)
                    {
                        case RecordKind.Personal:
                            details = (count == 0 ? (!_c.Settings.EnablePersonalBests ? "Disabled via settings" : "Not yet implemented") : "");
                            break;
                        case RecordKind.GalacticProcGen:
                            details = (handlingMode == StatScannerSettings.ProcGenHandlingMode.ProcGenIgnore ? "Ignored via settings" : "");
                            break;
                        case RecordKind.Galactic:
                            details = (handlingMode == StatScannerSettings.ProcGenHandlingMode.ProcGenOnly ? "Ignored except for identifying known record-holders" : "");
                            break;
                    }

                    gridItems.Add(
                        new(NotificationClass.None,
                            new StatScannerGrid
                            {
                                ObjectClass = "Plugin stats",
                                Variable = $"Tracked Record(s)",
                                Function = Function.Count.ToString(),
                                BodyOrItem = $"{kind}",
                                ObservedValue = $"{count}",
                                RecordValue = devModeWarning,
                                RecordHolder = Cmdr.Value.Name,
                                Details = details,
                            },
                            CoalescingIDs.SUMMARY_COALESCING_ID));
                }
                gridItems.AddRange(MaybeAddStats(Cmdr.Value.Name, Cmdr.Value.LastStatistics));

            }

            AddResultsToGrid(gridItems);
            HasHeaderRows = true;
        }

        private List<Result> MaybeAddStats(string cmdrName, Statistics stats)
        {
            var gridItems = new List<Result>();
            if (stats != null)
            {
                gridItems.Add(
                    new(
                        NotificationClass.None,
                        new StatScannerGrid
                        {
                            Timestamp = stats.Timestamp,
                            ObjectClass = "Game stats",
                            Variable = "Time played",
                            BodyOrItem = cmdrName,
                            Function = Function.Count.ToString(),
                            ObservedValue = $"{(stats.Exploration.TimePlayed / Conversions.CONV_S_TO_HOURS_DIVISOR):N0}",
                            Units = "hr",
                        },
                        CoalescingIDs.STATS_COALESCING_ID));
            }
            return gridItems;
        }

        private void ShowPersonalBestSummary()
        {
            MaybeAddHeaderRows();

            if (!_c.IsCommanderFidKnown || _c.NeedsReadAll) return;

            List<RecordTable> tableOrder  =
            [
                RecordTable.Regions,
                RecordTable.Systems,
                RecordTable.Stars,
                // RecordTable.Belts,
                RecordTable.Planets,
                RecordTable.Rings,
                RecordTable.Codex,
            ];

            var gridItems = new List<Result>();
#if DEBUG
            foreach (var fid in _c.KnownCommanderFids)
            {
                var recordBook = _c.GetRecordBookForFID(fid);
                foreach (var rt in tableOrder)
                {
                    foreach (var best in recordBook.GetPersonalBests(rt))
                    {
                        gridItems.AddRange(best.Summary());
                    }
                }
                gridItems.Add(new(
                    NotificationClass.None,
                    new(),
                    CoalescingIDs.HEADER_COALESCING_ID));
            }
#else
            var recordBook = _c.GetRecordBook();
            foreach ( var rt in tableOrder )
            {
                foreach (var best in recordBook.GetPersonalBests(rt))
                {
                    gridItems.AddRange(best.Summary());
                }
            }
#endif
            AddResultsToGrid(gridItems);
        }

        private void OnScan(Scan scan)
        {
            _c.AddScan(scan);
            // Determine type of object from scan (stars, planets, rings, systems)
            // Look up records for the specific variant of the object (type + variant).
            List<Result> results = [];
            var recordBook = _c.GetRecordBook();

            if (!String.IsNullOrEmpty(scan.StarType))
            {
                results.AddRange(CheckScanForRecords(scan, recordBook.GetRecords(RecordTable.Stars, scan.StarType)));
            }
            if (!String.IsNullOrEmpty(scan.PlanetClass) && !scan.PlanetClass.Equals("Barycentre", StringComparison.OrdinalIgnoreCase))
            {
                results.AddRange(CheckScanForRecords(scan, recordBook.GetRecords(RecordTable.Planets, scan.PlanetClass)));
                results.AddRange(CheckScanForRecords(scan, recordBook.GetRecords(RecordTable.Planets, Constants.OBJECT_TYPE_ODYSSEY_PLANET)));
            }
            if (scan.Rings != null)
            {
                HashSet<string> ringClassesAlreadyChecked = [];
                // Process proper rings (NOT belts) by RingClass!
                foreach (var r in scan.Rings.Where(r => r.Name.Contains("Ring")))
                {
                    if (!ringClassesAlreadyChecked.Contains(r.RingClass))
                    {
                        results.AddRange(CheckScanForRecords(scan, recordBook.GetRecords(RecordTable.Rings, r.RingClass)));
                        ringClassesAlreadyChecked.Add(r.RingClass);
                    }
                }
            }
            results.AddRange(CheckScanForRecords(scan, recordBook.GetRecords(RecordTable.Systems, Constants.OBJECT_TYPE_SYSTEM)));

            AddResultsToGrid(results, /* notify */ true);
        }

        private List<Result> CheckScanForRecords(Scan scan, List<IRecord> records)
        {
            var readMode = _c.Core.CurrentLogMonitorState;
            List<Result> results = [];
            foreach (var record in records)
            {
                try
                {
                    if (!record.DisallowedLogMonitorStates.Any(s => (readMode & s) != 0))
                        results.AddRange(record.CheckScan(scan, _c.CurrentSystem));
                }
                catch (Exception ex)
                {
                    throw new PluginException(AboutInfo.ShortName, $"Failure while processing record {record.DisplayName} while processing scan: {scan.Json}", ex);
                }
            }
            return results;
        }

        private void OnFssBodySignals(FSSBodySignals bodySignals)
        {
            var readMode = _c.Core.CurrentLogMonitorState;
            List<Result> results = [];
            foreach (var t in Constants.PB_RecordTypesForFssScans)
            {
                foreach (var record in _c.GetRecordBook().GetRecords(t.Item1, t.Item2))
                {
                    if (!record.DisallowedLogMonitorStates.Any(s => (readMode & s) != 0))
                        results.AddRange(record.CheckFSSBodySignals(bodySignals, _c.IsOdyssey));
                }
            }
            AddResultsToGrid(results, /* notify */ true);
        }

        private void OnFssAllBodiesFound(FSSAllBodiesFound fssAllBodies)
        {
            var readMode = _c.Core.CurrentLogMonitorState;
            List<Result> results = [];
            foreach (var t in Constants.PB_RecordTypesForFssScans)
            {
                foreach (var record in _c.GetRecordBook().GetRecords(t.Item1, t.Item2))
                {
                    if (!record.DisallowedLogMonitorStates.Any(s => (readMode & s) != 0))
                        results.AddRange(record.CheckFSSAllBodiesFound(fssAllBodies, _c.Scans));
                }
            }
            AddResultsToGrid(results, /* notify */ true);
        }

        private void OnCodexEntry(CodexEntry codexEntry)
        {
            if (!FDevIDs.RegionById.TryGetValue(codexEntry.Region, out PluginCommon.Data.Journals.FDevIDs.Region region)
                || !codexEntry.IsNewEntry) return;

            var recordBook = _c.GetRecordBook();
            var readMode = _c.Core.CurrentLogMonitorState;
            List<Result> results = [];

            foreach (var record in recordBook.GetRecords(RecordTable.Regions, Constants.OBJECT_TYPE_REGION))
            {
                if (!record.DisallowedLogMonitorStates.Any(s => (readMode & s) != 0))
                    results.AddRange(record.CheckCodexEntry(codexEntry));
            }
            foreach (var record in recordBook.GetRecords(RecordTable.Regions, region.Name))
            {
                if (!record.DisallowedLogMonitorStates.Any(s => (readMode & s) != 0))
                    results.AddRange(record.CheckCodexEntry(codexEntry));
            }
            AddResultsToGrid(results, /* notify */ true);
        }

        private void AddResultsToGrid(List<Result> results, bool maybeNotify = false)
        {
            if (results.Count == 0) return;
            if ((_c.Core.CurrentLogMonitorState & LogMonitorState.Batch) != 0)
            {
                _batchResults.AddRange(results);
                return;
            }
            else
                _c.Core.AddGridItems(this, results.Select(r => r.ResultItem));

            if (!maybeNotify || !_c.Settings.NotifySilentFallback)
                return;

            // Squash high-volumes of personal best notifications for a single body into 1 to avoid spamming notifications.
            var grouped = results.GroupBy(r => r.CoalescingID).ToDictionary(g => g.Key, g => g.ToList());
            foreach (var e in grouped)
            {
                var resultItems = e.Value;
                var pbs = e.Value.Where(r => r.ResultItem.Details.Contains("personal best"));
                if (_c.Settings.MaxPersonalBestsPerBody > 0 && pbs.Count() > _c.Settings.MaxPersonalBestsPerBody)
                {
                    var r = pbs.First();
                    var title = "";
                    if (r.ResultItem.ObjectClass == Constants.OBJECT_TYPE_SYSTEM || r.ResultItem.ObjectClass == Constants.OBJECT_TYPE_REGION)
                        title = r.ResultItem.ObjectClass;
                    else
                        title = SharedLogic.GetBodyDisplayName(SharedLogic.GetBodyShortName(r.ResultItem.BodyOrItem, _c.CurrentSystem));

                    _c.Core.SendNotification(new()
                    {
                        Title = title,
                        Detail = $"{pbs.Count()} personal bests: {r.ResultItem.ObjectClass}",
                        Rendering = GetNotificationRendering(r),
                        ExtendedDetails = string.Join("; ", pbs.Select(pb => $"{pb.ResultItem.Variable}, {pb.ResultItem.Function}")),
                        Sender = AboutInfo.ShortName,
                        CoalescingId = r.CoalescingID,
                    });

                    // Continue processing the non-personal bests.
                    resultItems = [.. e.Value.Where(r => !r.ResultItem.Details.Contains("personal best"))];
                }

                foreach (var r in resultItems)
                {
                    string firstDiscoveryStatus = "";
                    if (!string.IsNullOrEmpty(r.ResultItem.DiscoveryStatus) && r.ResultItem.DiscoveryStatus != "-")
                    {
                        firstDiscoveryStatus = $" ({r.ResultItem.DiscoveryStatus})";
                    }
                    string title = "";
                    if (r.ResultItem.ObjectClass == Constants.OBJECT_TYPE_SYSTEM || r.ResultItem.ObjectClass == Constants.OBJECT_TYPE_REGION)
                        title = r.ResultItem.ObjectClass;
                    else
                        title = SharedLogic.GetBodyDisplayName(SharedLogic.GetBodyShortName(r.ResultItem.BodyOrItem, _c.CurrentSystem));

                    _c.Core.SendNotification(new()
                    {
                        Title = title,
                        Detail = $"{r.ResultItem.Details}: {r.ResultItem.ObjectClass}; {r.ResultItem.Variable}; {r.ResultItem.Function}",
                        Rendering = GetNotificationRendering(r),
                        ExtendedDetails = $"{r.ResultItem.ObservedValue} {r.ResultItem.Units} @ {r.ResultItem.BodyOrItem}{firstDiscoveryStatus}. Previous value: {r.ResultItem.RecordValue} {r.ResultItem.Units}",
                        Sender = AboutInfo.ShortName,
                        CoalescingId = r.CoalescingID,
                    });
                }
            }
        }

        private NotificationRendering GetNotificationRendering(Result result)
        {
            var rendering = NotificationRendering.PluginNotifier;

            switch(result.NotificationClass)
            {
                case NotificationClass.PossibleNewGalacticRecord:
                    if (_c.Settings.NotifyPossibleNewGalacticRecords) rendering = NotificationRendering.All;
                    break;
                case NotificationClass.MatchedGalacticRecord:
                    if (_c.Settings.NotifyMatchedGalacticRecords) rendering = NotificationRendering.All;
                    break;
                case NotificationClass.VisitedGalacticRecord:
                    if (_c.Settings.NotifyVisitedGalacticRecords) rendering = NotificationRendering.All;
                    break;
                case NotificationClass.NearGalacticRecord:
                    if (_c.Settings.NotifyNearGalacticRecords) rendering = NotificationRendering.All;
                    break;
                case NotificationClass.PersonalBest:
                    if (_c.Settings.NotifyNewPersonalBests) rendering = NotificationRendering.All;
                    break;
                case NotificationClass.NewCodex:
                    if (_c.Settings.NotifyNewCodexEntries) rendering = NotificationRendering.All;
                    break;
                case NotificationClass.Tally:
                    if (_c.Settings.NotifyTallies) rendering = NotificationRendering.All;
                    break;
            }
            return rendering;
        }
    }
}
