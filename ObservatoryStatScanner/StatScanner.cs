using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using com.github.fredjk_gh.ObservatoryStatScanner.Records;
using System.Collections.ObjectModel;
using System.Diagnostics;
using com.github.fredjk_gh.ObservatoryStatScanner.StateManagement;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace com.github.fredjk_gh.ObservatoryStatScanner
{
    public class StatScanner : IObservatoryWorker
    {
        private StatScannerSettings settings = StatScannerSettings.DEFAULT;

        private const string STATE_CACHE_FILENAME = "stateCache.json";

        private PluginUI pluginUI;
        ObservableCollection<object> gridCollection = new();
        private bool HasHeaderRows = false;
        private StatScannerState _state = null;
        private List<Result> _batchResults = new List<Result>();

        private AboutInfo _aboutInfo = new()
        {
            FullName = "Stat Scanner",
            ShortName = "Stat Scanner",
            Description = "Stat Scanner is your own personal record book of personal bests but can also compares your discoveries against one of two sets of galactic records provided by edastro.com.",
            AuthorName = "fredjk-gh",
            Links = new()
            {
                new AboutLink("github", "https://github.com/fredjk-gh/ObservatoryPlugins"),
                new AboutLink("github release notes", "https://github.com/fredjk-gh/ObservatoryPlugins/wiki/Plugin:-Stat-Scanner"),
                new AboutLink("Documentation", "https://observatory.xjph.net/usage/plugins/thirdparty/fredjk-gh/statscanner"),
                new AboutLink("edastro.com Records", "https://edastro.com/records/")
            }
        };

        public AboutInfo AboutInfo => _aboutInfo;
        public string Version => typeof(StatScanner).Assembly.GetName().Version.ToString();
        public PluginUI PluginUI => pluginUI;

        public object Settings
        {
            get => settings;
            set
            {
                settings = (StatScannerSettings)value;
                if (_state != null) _state.Settings = settings;
            }
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args)
        {
            // * -> ReadAll
            if ((args.NewState & LogMonitorState.Batch) != 0)
            {
                _state.ResetForReadAll();
                _state.Core.ClearGrid(this, new StatScannerGrid());
                _batchResults.Clear();
                HasHeaderRows = false;
            }
            // ReadAll -> *
            else if ((args.PreviousState & LogMonitorState.Batch) != 0 && (args.NewState & LogMonitorState.Batch) == 0)
            {
                if (!_state.Settings.EnableGridOutputAfterReadAll) 
                    _batchResults.Clear(); // Dump what we have accumulated until now.

                if ((args.NewState & LogMonitorState.BatchCancelled) != 0)
                {
                    _state.Cacheable.SetReadAllRequired("Read-all was cancelled; incomplete data.");
                }
                else
                {
                    _state.ReadAllComplete();
                }

                ShowPersonalBestSummary();
                AddResultsToGrid_(_batchResults, false);
                _batchResults.Clear();

                SerializeState(true);
            }
        }

        public void ObservatoryReady()
        {
            ShowPersonalBestSummary();
        }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            switch (journal)
            {
                case FileHeader fileHeader:
                    _state.IsOdyssey = fileHeader.Odyssey;
                    break;
                case LoadGame loadGame:
                    var previousCommanderName = _state.CommanderName;
                    _state.LastLoadGame = loadGame;
                    if (previousCommanderName != _state.CommanderName)
                    {
                        Debug.WriteLine($"Switching commanders: {_state.CommanderName}");
                        AddResultsToGrid(new()
                        {
                            new(NotificationClass.None,
                                new StatScannerGrid
                                {
                                    ObjectClass = "Plugin message",
                                    Variable = "Commander",
                                    Function = "Changed to",
                                    BodyOrItem = _state.CommanderName,
                                },
                                Constants.HEADER_COALESCING_ID)
                        });

                    }
                    break;
                case Statistics statistics:
                    // TODO: do more with this?
                    _state.LastStats = statistics;
                    if (!_state.Core.IsLogMonitorBatchReading)
                    {
                        AddResultsToGrid(MaybeAddStats(_state.CommanderName, statistics));
                    }
                    break;
                case FSDJump fsdJump:
                    // Track this here because it's not present in older Scans.
                    _state.CurrentSystem = fsdJump.StarSystem;
                    break;
                case Location location:
                    _state.CurrentSystem = location.StarSystem;
                    break;
                case DiscoveryScan discoveryScan:
                    _state.SystemBodyCount = discoveryScan.Bodies;
                    break;
                case NavBeaconScan navBeaconScan:
                    _state.SystemBodyCount = navBeaconScan.NumBodies;
                    _state.NavBeaconScanned = true;
                    break;
                case Scan scan:
                    OnScan(scan);

                    if (_state.NavBeaconScanned && _state.Scans.Count == _state.SystemBodyCount)
                    {
                        // Nav beacon was used; generate an AllBodies Found event.
                        OnFssAllBodiesFound(new()
                        {
                            Count = _state.Scans.Count,
                            SystemName = _state.CurrentSystem,
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

        public void Load(IObservatoryCore observatoryCore)
        {
            MaybeFixUnsetSettings();

            StateCache cached = DeserializeCachedState(observatoryCore);
            cached.CheckForNewAssemblyVersion();
            _state = new(cached, observatoryCore, settings, this);

            gridCollection = new();
            StatScannerGrid uiObject = new();
            gridCollection.Add(uiObject);
            pluginUI = new PluginUI(gridCollection);

            MaybeUpdateGalacticRecords();

            if (_state.Cacheable.KnownCommanders.Count == 0)
            {
                _state.Cacheable.SetReadAllRequired("No known commanders.");
                _state.Core.AddGridItems(this, _state.GetPluginStateMessages());
            }

            SerializeState(true);
        }

        // Requires core explicitly because _state is not initialized yet when this is first called.
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
                _state.Core.GetPluginErrorLogger(this)(ex, "Deserializing state cache");
            }
            return stateCache;
        }

        public void SerializeState(bool forceWrite = false)
        {
            if (((_state.Core.CurrentLogMonitorState & LogMonitorState.Batch) != 0 && !forceWrite) || !_state.Cacheable.IsDirty) return;

                string dataCacheFile = $"{_state.Core.PluginStorageFolder}{STATE_CACHE_FILENAME}";
            string jsonString = JsonSerializer.Serialize(_state.Cacheable,
                new JsonSerializerOptions() { AllowTrailingCommas = true, WriteIndented = true });
            File.WriteAllText(dataCacheFile, jsonString);
        }

        private void MaybeFixUnsetSettings()
        {
            settings.ForceUpdateGalacticRecords = ForceRefreshGalacticRecords;
            settings.OpenStatScannerWiki = OpenWikiUrl;

            if (settings.HighCardinalityTieSuppression == 0) settings.HighCardinalityTieSuppression = StatScannerSettings.DEFAULT.HighCardinalityTieSuppression;
            if (settings.ProcGenHandling == null) settings.ProcGenHandling = StatScannerSettings.DEFAULT_PROCGEN_HANDLING;
        }

        private void MaybeAddHeaderRows()
        {
            if (_state.Core.IsLogMonitorBatchReading || HasHeaderRows) return;

            if (!_state.IsCommanderFidKnown || _state.NeedsReadAll || _state.Cacheable.KnownCommanders.Count == 0)
            {
                _state.Core.AddGridItems(this, _state.GetPluginStateMessages());
            }

            var devModeWarning = (settings.DevMode ? "!!DEV mode!!" : "");
            var handlingMode = (StatScannerSettings.ProcGenHandlingMode)settings.ProcGenHandlingOptions[settings.ProcGenHandling];
            var gridItems = new List<Result>
            {
                new (NotificationClass.None,
                    new StatScannerGrid
                    {
                        ObjectClass = "Plugin info",
                        Variable = $"{AboutInfo.ShortName} version",
                        ObservedValue = $"v{Version}",
                    },
                    Constants.HEADER_COALESCING_ID),
                new (NotificationClass.None,
                    new StatScannerGrid
                    {
                        ObjectClass = "Plugin settings",
                        Variable = "Tracking First Discoveries only?",
                        ObservedValue = $"{settings.FirstDiscoveriesOnly}",
                    },
                    Constants.HEADER_COALESCING_ID),
            };
            foreach (var Cmdr in _state.Cacheable.KnownCommanders)
            {
                var recordBook = _state.GetRecordBookForFID(Cmdr.Key);

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
                    Constants.HEADER_COALESCING_ID));

                foreach (RecordKind kind in Enum.GetValues(typeof(RecordKind)))
                {
                    var count = recordBook.CountByKind(kind);
                    var details = "";
                    switch (kind)
                    {
                        case RecordKind.Personal:
                            details = (count == 0 ? (!settings.EnablePersonalBests ? "Disabled via settings" : "Not yet implemented") : "");
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
                            Constants.SUMMARY_COALESCING_ID));
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
                            ObservedValue = $"{(stats.Exploration.TimePlayed / Constants.CONV_S_TO_HOURS_DIVISOR):N0}",
                            Units = "hr",
                        },
                        Constants.STATS_COALESCING_ID));
            }
            return gridItems;
        }

        private void ShowPersonalBestSummary()
        {
            MaybeAddHeaderRows();

            if (!_state.IsCommanderFidKnown || _state.NeedsReadAll) return;

            List<RecordTable> tableOrder  = new()
            {
                RecordTable.Regions,
                RecordTable.Systems,
                RecordTable.Stars,
                // RecordTable.Belts,
                RecordTable.Planets,
                RecordTable.Rings,
                RecordTable.Codex,
            };

            var gridItems = new List<Result>();
#if DEBUG
            foreach (var fid in _state.KnownCommanderFids)
            {
                var recordBook = _state.GetRecordBookForFID(fid);
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
                    Constants.HEADER_COALESCING_ID));
            }
#else
            var recordBook = _state.GetRecordBook();
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

        private void ForceRefreshGalacticRecords()
        {
            if (FetchFreshGalacticRecords(Constants.EDASTRO_GALACTIC_RECORDS_CSV_URL, _state.GalacticRecordsCsv)
                || FetchFreshGalacticRecords(Constants.EDASTRO_GALACTIC_RECORDS_PG_CSV_URL, _state.GalacticRecordsPGCsv))
            {
                _state.ReloadGalacticRecords();
            }
        }

        private void MaybeUpdateGalacticRecords()
        {
            try
            {
                // If galactic records files are missing OR > 7 days, fetch a new one and freshen.
                if (!File.Exists(_state.GalacticRecordsCsv) || File.GetLastWriteTimeUtc(_state.GalacticRecordsCsv) < DateTime.UtcNow.Subtract(TimeSpan.FromDays(7.0)))
                {
                    FetchFreshGalacticRecords(Constants.EDASTRO_GALACTIC_RECORDS_CSV_URL, _state.GalacticRecordsCsv);
                }
                if (!File.Exists(_state.GalacticRecordsPGCsv) || File.GetLastWriteTimeUtc(_state.GalacticRecordsPGCsv) < DateTime.UtcNow.Subtract(TimeSpan.FromDays(7.0)))
                {
                    FetchFreshGalacticRecords(Constants.EDASTRO_GALACTIC_RECORDS_PG_CSV_URL, _state.GalacticRecordsPGCsv);
                }
            }
            catch (Exception ex)
            {
                _state.ErrorLogger(ex, "Fetching updated records files from edastro.com");
            }
        }

        private bool FetchFreshGalacticRecords(string csvUrl, string localCSVFilename)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(csvUrl),
            };
            var requestTask = _state.Core.HttpClient.SendAsync(request);
            while (!requestTask.IsCompleted)
            {
                requestTask.Wait(500);
            }

            if (requestTask.IsFaulted)
            {
                _state.ErrorLogger(requestTask.Exception, $"Error while refreshing {localCSVFilename}; using copy stale records for now...");
                return false;
            }
            else if (requestTask.IsCanceled)
            {
                Debug.WriteLine($"Request to update {localCSVFilename} was cancelled; using copy stale records for now...");
                return false;
            }

            var response = requestTask.Result;
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                _state.ErrorLogger(ex, $"Error while downloading updated {localCSVFilename}; using stale records (if available)");
                return false;
            }

            try
            {
                if (File.Exists(localCSVFilename))
                {
                    // Preserve old one, just in case.
                    File.Copy(localCSVFilename, localCSVFilename + Constants.GOOD_BACKUP_EXT, true);
                }
                // Write result to file.
                using (Stream csvFile = File.Create(localCSVFilename))
                {
                    response.Content.ReadAsStream().CopyTo(csvFile);
                }
            }
            catch (Exception ex)
            {
                _state.ErrorLogger(ex, $"While writing updated {localCSVFilename}");
                return false;
            }

            return true;
        }

        private void OnScan(Scan scan)
        {
            _state.AddScan(scan);
            // Determine type of object from scan (stars, planets, rings, systems)
            // Look up records for the specific variant of the object (type + variant).
            List<Result> results = new();
            var recordBook = _state.GetRecordBook();

            if (!String.IsNullOrEmpty(scan.StarType))
            {
                results.AddRange(CheckScanForRecords(scan, recordBook.GetRecords(RecordTable.Stars, scan.StarType)));
            }
            if (!String.IsNullOrEmpty(scan.PlanetClass) && !scan.PlanetClass.Equals("Barycentre", StringComparison.InvariantCultureIgnoreCase))
            {
                results.AddRange(CheckScanForRecords(scan, recordBook.GetRecords(RecordTable.Planets, scan.PlanetClass)));
                results.AddRange(CheckScanForRecords(scan, recordBook.GetRecords(RecordTable.Planets, Constants.OBJECT_TYPE_ODYSSEY_PLANET)));
            }
            if (scan.Rings != null)
            {
                HashSet<string> ringClassesAlreadyChecked = new();
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
            var readMode = _state.Core.CurrentLogMonitorState;
            List<Result> results = new();
            foreach (var record in records)
            {
                try
                {
                    if (!record.DisallowedLogMonitorStates.Any(s => (readMode & s) != 0))
                        results.AddRange(record.CheckScan(scan, _state.CurrentSystem));
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
            var readMode = _state.Core.CurrentLogMonitorState;
            List<Result> results = new();
            foreach (var t in Constants.PB_RecordTypesForFssScans)
            {
                foreach (var record in _state.GetRecordBook().GetRecords(t.Item1, t.Item2))
                {
                    if (!record.DisallowedLogMonitorStates.Any(s => (readMode & s) != 0))
                        results.AddRange(record.CheckFSSBodySignals(bodySignals, _state.IsOdyssey));
                }
            }
            AddResultsToGrid(results, /* notify */ true);
        }

        private void OnFssAllBodiesFound(FSSAllBodiesFound fssAllBodies)
        {
            var readMode = _state.Core.CurrentLogMonitorState;
            List<Result> results = new();
            foreach (var t in Constants.PB_RecordTypesForFssScans)
            {
                foreach (var record in _state.GetRecordBook().GetRecords(t.Item1, t.Item2))
                {
                    if (!record.DisallowedLogMonitorStates.Any(s => (readMode & s) != 0))
                        results.AddRange(record.CheckFSSAllBodiesFound(fssAllBodies, _state.Scans));
                }
            }
            AddResultsToGrid(results, /* notify */ true);
        }

        private void OnCodexEntry(CodexEntry codexEntry)
        {
            if (!Constants.RegionNamesByJournalId.ContainsKey(codexEntry.Region)
                || !codexEntry.IsNewEntry) return;

            var recordBook = _state.GetRecordBook();
            var readMode = _state.Core.CurrentLogMonitorState;
            string regionNameByJournalValue = Constants.RegionNamesByJournalId[codexEntry.Region];
            List<Result> results = new();

            foreach (var record in recordBook.GetRecords(RecordTable.Regions, Constants.OBJECT_TYPE_REGION))
            {
                if (!record.DisallowedLogMonitorStates.Any(s => (readMode & s) != 0))
                    results.AddRange(record.CheckCodexEntry(codexEntry));
            }
            foreach (var record in recordBook.GetRecords(RecordTable.Regions, regionNameByJournalValue))
            {
                if (!record.DisallowedLogMonitorStates.Any(s => (readMode & s) != 0))
                    results.AddRange(record.CheckCodexEntry(codexEntry));
            }
            AddResultsToGrid(results, /* notify */ true);
        }

        // TODO: Extract these to shared library or move into IObservatoryCore?
        private string GetShortBodyName(string bodyName, string baseName = "")
        {
            return string.IsNullOrEmpty(baseName) ?
                (string.IsNullOrEmpty(_state.CurrentSystem) ? bodyName : bodyName.Replace(_state.CurrentSystem, "").Trim())
                : bodyName.Replace(baseName, "").Trim();
        }

        private string GetBodyTitle(string bodyName)
        {
            if (bodyName.Length == 0)
            {
                return "Primary Star";
            }
            return $"Body {bodyName}";
        }

        private void AddResultsToGrid(List<Result> results, bool maybeNotify = false)
        {
            if (results.Count == 0) return;
            if ((_state.Core.CurrentLogMonitorState & LogMonitorState.Batch) != 0)
            {
                _batchResults.AddRange(results);
            }
            else
            {
                AddResultsToGrid_(results, maybeNotify);
            }
        }

        private void AddResultsToGrid_(List<Result> results, bool maybeNotify = false)
        {
            _state.Core.AddGridItems(this, results.Select(r => r.ResultItem));
            if (!maybeNotify || !settings.NotifySilentFallback)
                return;

            // Squash high-volumes of personal best notifications for a single body into 1 to avoid spamming notifications.
            var grouped = results.GroupBy(r => r.CoalescingID).ToDictionary(g => g.Key, g => g.ToList());
            foreach (var e in grouped)
            {
                var resultItems = e.Value;
                var pbs = e.Value.Where(r => r.ResultItem.Details.Contains("personal best"));
                if (settings.MaxPersonalBestsPerBody > 0 && pbs.Count() > settings.MaxPersonalBestsPerBody)
                {
                    var r = pbs.First();
                    var title = "";
                    if (r.ResultItem.ObjectClass == Constants.OBJECT_TYPE_SYSTEM || r.ResultItem.ObjectClass == Constants.OBJECT_TYPE_REGION)
                        title = r.ResultItem.ObjectClass;
                    else
                        title = GetBodyTitle(GetShortBodyName(r.ResultItem.BodyOrItem));

                    _state.Core.SendNotification(new()
                    {
                        Title = title,
                        Detail = $"{pbs.Count()} personal bests: {r.ResultItem.ObjectClass}",
                        Rendering = GetNotificationRendering(r),
                        ExtendedDetails = string.Join("; ", pbs.Select(pb => $"{pb.ResultItem.Variable}, {pb.ResultItem.Function}")),
                        Sender = AboutInfo.ShortName,
                        CoalescingId = r.CoalescingID,
                    });

                    // Continue processing the non-personal bests.
                    resultItems = e.Value.Where(r => !r.ResultItem.Details.Contains("personal best")).ToList();
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
                        title = GetBodyTitle(GetShortBodyName(r.ResultItem.BodyOrItem));

                    _state.Core.SendNotification(new()
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
                    if (settings.NotifyPossibleNewGalacticRecords) rendering = NotificationRendering.All;
                    break;
                case NotificationClass.MatchedGalacticRecord:
                    if (settings.NotifyMatchedGalacticRecords) rendering = NotificationRendering.All;
                    break;
                case NotificationClass.VisitedGalacticRecord:
                    if (settings.NotifyVisitedGalacticRecords) rendering = NotificationRendering.All;
                    break;
                case NotificationClass.NearGalacticRecord:
                    if (settings.NotifyNearGalacticRecords) rendering = NotificationRendering.All;
                    break;
                case NotificationClass.PersonalBest:
                    if (settings.NotifyNewPersonalBests) rendering = NotificationRendering.All;
                    break;
                case NotificationClass.NewCodex:
                    if (settings.NotifyNewCodexEntries) rendering = NotificationRendering.All;
                    break;
                case NotificationClass.Tally:
                    if (settings.NotifyTallies) rendering = NotificationRendering.All;
                    break;
            }
            return rendering;
        }

        private void OpenWikiUrl()
        {
            OpenUrl(Constants.STATSCANNER_WIKI_URL);
        }

        private void OpenUrl(string url)
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
}
