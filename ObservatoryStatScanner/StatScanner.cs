using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using com.github.fredjk_gh.ObservatoryStatScanner.Records;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace com.github.fredjk_gh.ObservatoryStatScanner
{
    public class StatScanner : IObservatoryWorker
    {
        private StatScannerSettings settings = StatScannerSettings.DEFAULT;

        private IObservatoryCore Core;
        /// <summary>
        /// Error logger; string param is an arbitrary context hint.
        /// </summary>
        private Action<Exception, string> ErrorLogger;
        private PluginUI pluginUI;
        ObservableCollection<object> gridCollection = new();
        private bool HasHeaderRows = false;
        private StatScannerState _state = null;
        private string currentCommander = "";

        public string Name => "Observatory Stat Scanner";
        public string ShortName => "Stat Scanner";
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
            if (args.NewState.HasFlag(LogMonitorState.Batch))
            {
                _state.ResetForReadAll();
                Core.ClearGrid(this, new StatScannerGrid());
                HasHeaderRows = false;
            }
            // ReadAll -> *
            else if (args.PreviousState.HasFlag(LogMonitorState.Batch) || args.PreviousState.HasFlag(LogMonitorState.PreRead))
            {
                ShowPersonalBestSummary();
            }
        }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            switch (journal)
            {
                case FileHeader fileHeader:
                    _state.ResetForNewFile();
                    _state.IsOdyssey = fileHeader.Odyssey;
                    break;
                case LoadGame loadGame:
                    _state.LastLoadGame = loadGame;
                    if (loadGame.Commander != currentCommander)
                    {
                        Debug.WriteLine($"Switching commanders: {loadGame.Commander}");
                        currentCommander = loadGame.Commander;
                        Core.AddGridItem(
                            this,
                            new StatScannerGrid
                            {
                                ObjectClass = "Plugin message",
                                Variable = "Commander",
                                Function = "Changed to",
                                BodyOrItem = currentCommander,
                            });
                    }
                    break;
                case Statistics statistics:
                    // TODO: do more with this?
                    _state.LastStats = statistics;
                    if (!Core.IsLogMonitorBatchReading)
                    {
                        AddResultsToGrid(MaybeAddStats());
                    }
                    break;
                case FSDJump fsdJump:
                    // Track this here because it's not present in older Scans.
                    _state.CurrentSystem = fsdJump.StarSystem;
                    break;
                case Location location:
                    _state.CurrentSystem = location.StarSystem;
                    break;
                case Scan scan:
                    OnScan(scan);
                    break;
                case FSSBodySignals bodySignals:
                    OnFssBodySignals(bodySignals);
                    break;
                case FSSAllBodiesFound fssAllBodies:
                    // TODO: gather scans
                    OnFssAllBodiesFound(fssAllBodies, new List<Scan>());
                    break;
                case CodexEntry codexEntry:
                    OnCodexEntry(codexEntry);
                    break;

            }
        }

        public void Load(IObservatoryCore observatoryCore)
        {
            MaybeFixUnsetSettings();

            gridCollection = new();
            StatScannerGrid uiObject = new();
            gridCollection.Add(uiObject);
            pluginUI = new PluginUI(gridCollection);

            Core = observatoryCore;
            ErrorLogger = Core.GetPluginErrorLogger(this);

            _state = new(settings, Core.PluginStorageFolder, ErrorLogger);

            MaybeUpdateGalacticRecords();

            if (_state.KnownCommanderFIDs.Count == 0)
            {
                Core.AddGridItem(
                    this,
                    new StatScannerGrid
                    {
                        ObjectClass = "Plugin message",
                        Variable = $"Please run 'Read All' to initialize database(s)",
                    });
            }
        }

        private void MaybeFixUnsetSettings()
        {
            settings.ForceUpdateGalacticRecords = ForceRefreshGalacticRecords;

            if (settings.HighCardinalityTieSuppression == 0) settings.HighCardinalityTieSuppression = StatScannerSettings.DEFAULT.HighCardinalityTieSuppression;
            if (settings.ProcGenHandling == null) settings.ProcGenHandling = StatScannerSettings.DEFAULT_PROCGEN_HANDLING;
        }

        private void MaybeAddHeaderRows()
        {
            if (Core.IsLogMonitorBatchReading || HasHeaderRows || _state.KnownCommanderFIDs.Count == 0) return;

            if (!_state.Initialized || _state.NeedsReadAll)
            {
                Core.AddGridItem(
                    this,
                    new StatScannerGrid
                    {
                        ObjectClass = "Plugin message",
                        Variable = $"Please run 'Read All' to initialize database(s)",
                    });
                HasHeaderRows = true;
                return;
            }

            var recordBook = _state.GetRecordBook();
            var devModeWarning = (settings.DevMode ? "!!DEV mode!!" : "");
            var handlingMode = (StatScannerSettings.ProcGenHandlingMode)settings.ProcGenHandlingOptions[settings.ProcGenHandling];
            var gridItems = new List<Result>
            {
                new (NotificationClass.None,
                    new StatScannerGrid
                    {
                        ObjectClass = "Plugin info",
                        Variable = $"{this.ShortName} version",
                        ObservedValue = $"v{this.Version}",
                    }),
                new (NotificationClass.None,
                    new StatScannerGrid
                    {
                        ObjectClass = "Plugin settings",
                        Variable = "Tracking First Discoveries only?",
                        ObservedValue = $"{settings.FirstDiscoveriesOnly}",
                    }),
                new (NotificationClass.None,
                    new StatScannerGrid
                    {
                        ObjectClass = "Plugin stats",
                        Variable = "Tracked Record(s)",
                        Function = Function.Count.ToString(),
                        BodyOrItem = "Total",
                        ObservedValue = $"{recordBook.Count}",
                        RecordValue = devModeWarning,
                    }),
            };
            foreach (RecordKind kind in Enum.GetValues(typeof(RecordKind))) {
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
                    new (NotificationClass.None,
                        new StatScannerGrid
                        {
                            ObjectClass = "Plugin stats",
                            Variable = $"Tracked Record(s)",
                            Function = Function.Count.ToString(),
                            BodyOrItem = $"{kind}",
                            ObservedValue = $"{count}",
                            RecordValue = devModeWarning,
                            Details = details,
                        }));
            }
            gridItems.AddRange(MaybeAddStats());
            AddResultsToGrid(gridItems);
            HasHeaderRows = true;
        }

        private List<Result> MaybeAddStats()
        {
            var gridItems = new List<Result>();
            if (_state.Initialized)
            {
                gridItems.Add(
                    new(
                        NotificationClass.None,
                        new StatScannerGrid
                        {
                            Timestamp = _state.LastStats.Timestamp,
                            ObjectClass = "Game stats",
                            Variable = "Time played",
                            BodyOrItem = _state.CommanderName,
                            Function = Function.Count.ToString(),
                            ObservedValue = $"{(_state.LastStats.Exploration.TimePlayed / Constants.CONV_S_TO_HOURS_DIVISOR):N0}",
                            Units = "hr",
                        }));
            }
            return gridItems;
        }

        private void ShowPersonalBestSummary()
        {
            if (!_state.Initialized || _state.NeedsReadAll) return;

            var recordBook = _state.GetRecordBook();
            MaybeAddHeaderRows();
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
            foreach ( var rt in tableOrder )
            {
                foreach (var best in recordBook.GetPersonalBests(rt))
                {
                    gridItems.AddRange(best.Summary());
                }
            }
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

        private bool FetchFreshGalacticRecords(string csvUrl, string localCSVFilename)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(csvUrl),
            };
            var requestTask = Core.HttpClient.SendAsync(request);
            requestTask.Wait(1000);

            if (requestTask.IsFaulted)
            {
                ErrorLogger(requestTask.Exception, $"Error while refreshing {localCSVFilename}; using copy stale records for now...");
                return false;
            }

            var response = requestTask.Result;
            try
            {
                response.EnsureSuccessStatusCode(); // Maybe try EnsureSuccessStatusCode instead?
            }
            catch (HttpRequestException ex)
            {
                ErrorLogger(ex, $"Error while downloading updated {localCSVFilename}; using stale records (if available)");
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
                ErrorLogger(ex, $"While writing updated {localCSVFilename}");
                return false;
            }

            return true;
        }

        private void OnScan(Scan scan)
        {
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
            var readMode = Core.CurrentLogMonitorState;
            List<Result> results = new();
            foreach (var record in records)
            {
                try
                {
                    if (!record.DisallowedLogMonitorStates.Any(s => readMode.HasFlag(s)))
                        results.AddRange(record.CheckScan(scan, _state.CurrentSystem));
                }
                catch (Exception ex)
                {
                    // TODO: Replace "this.ShortName" with just "this" after next update
                    // ... or update PluginEventHandler's RecordError method(s) to include version for us
                    //     and just remove this try-catch.
                    throw new PluginException(this.ShortName, $"Failure while processing record {record.DisplayName} while processing scan: {scan.Json}", ex);
                }
            }
            return results;
        }

        private void OnFssBodySignals(FSSBodySignals bodySignals)
        {
            var readMode = Core.CurrentLogMonitorState;
            List<Result> results = new();
            foreach (var t in Constants.PB_RecordTypesForFssScans)
            {
                foreach (var record in _state.GetRecordBook().GetRecords(t.Item1, t.Item2))
                {
                    if (!record.DisallowedLogMonitorStates.Any(s => readMode.HasFlag(s)))
                        results.AddRange(record.CheckFSSBodySignals(bodySignals, _state.IsOdyssey));
                }
            }
            AddResultsToGrid(results, /* notify */ true);
        }

        private void OnFssAllBodiesFound(FSSAllBodiesFound fssAllBodies, List<Scan> scans)
        {
            var readMode = Core.CurrentLogMonitorState;
            List<Result> results = new();
            foreach (var t in Constants.PB_RecordTypesForFssScans)
            {
                foreach (var record in _state.GetRecordBook().GetRecords(t.Item1, t.Item2))
                {
                    if (!record.DisallowedLogMonitorStates.Any(s => readMode.HasFlag(s)))
                        results.AddRange(record.CheckFSSAllBodiesFound(fssAllBodies, scans));
                }
            }
            AddResultsToGrid(results, /* notify */ true);
        }

        private void OnCodexEntry(CodexEntry codexEntry)
        {
            if (!Constants.RegionNamesByJournalId.ContainsKey(codexEntry.Region)
                || !codexEntry.IsNewEntry) return;

            var recordBook = _state.GetRecordBook();
            var readMode = Core.CurrentLogMonitorState;
            string regionNameByJournalValue = Constants.RegionNamesByJournalId[codexEntry.Region];
            List<Result> results = new();

            foreach (var record in recordBook.GetRecords(RecordTable.Regions, Constants.OBJECT_TYPE_REGION))
            {
                if (!record.DisallowedLogMonitorStates.Any(s => readMode.HasFlag(s)))
                    results.AddRange(record.CheckCodexEntry(codexEntry));
            }
            foreach (var record in recordBook.GetRecords(RecordTable.Regions, regionNameByJournalValue))
            {
                if (!record.DisallowedLogMonitorStates.Any(s => readMode.HasFlag(s)))
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
            Core.AddGridItems(this, results.Select(r => r.ResultItem));
            if (!maybeNotify || !settings.NotifySilentFallback)
                return;

            foreach (var r in results)
            {
                string firstDiscoveryStatus = "";
                if (!string.IsNullOrEmpty(r.ResultItem.DiscoveryStatus) && r.ResultItem.DiscoveryStatus != "-")
                {
                    firstDiscoveryStatus = $" ({r.ResultItem.DiscoveryStatus})";
                }
                string title = "";
                if (r.ResultItem.ObjectClass == Constants.OBJECT_TYPE_SYSTEM || r.ResultItem.ObjectClass == Constants.OBJECT_TYPE_REGION)
                {
                    title = r.ResultItem.ObjectClass;
                }
                else
                {
                    title = GetBodyTitle(GetShortBodyName(r.ResultItem.BodyOrItem));
                }

                Core.SendNotification(new()
                {
                    Title = title,
                    Detail = $"{r.ResultItem.Details}: {r.ResultItem.ObjectClass}; {r.ResultItem.Variable}; {r.ResultItem.Function}",
                    Rendering = GetNotificationRendering(r),
#if EXTENDED_EVENT_ARGS
                    ExtendedDetails = $"{r.ResultItem.ObservedValue} {r.ResultItem.Units} @ {r.ResultItem.BodyOrItem}{firstDiscoveryStatus}. Previous value: {r.ResultItem.RecordValue} {r.ResultItem.Units}",
                    Sender = this,
#endif
                }); ;
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
    }
}
