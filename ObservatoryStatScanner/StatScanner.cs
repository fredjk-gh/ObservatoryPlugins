using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using com.github.fredjk_gh.ObservatoryStatScanner.DB;
using com.github.fredjk_gh.ObservatoryStatScanner.Records;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TextFieldParserCore;

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
        private string galacticRecordsCSV;
        private string galacticRecordsPGCSV;

        private RecordBook recordBook;
        private PersonalBestManager manager;

        private bool IsOdyssey = false;
        private bool HasHeaderRows = false;
        private string CurrentSystem = "";
        private Statistics lastStats = null;
        private LoadGame lastLoadGame = null;

        public string Name => "Observatory Stat Scanner";
        public string ShortName => "Stat Scanner";
        public string Version => typeof(StatScanner).Assembly.GetName().Version.ToString();

        public PluginUI PluginUI => pluginUI;

        public object Settings
        {
            get => settings;
            set => settings = (StatScannerSettings)value;
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args)
        {
            // * -> ReadAll
            if (args.NewState.HasFlag(LogMonitorState.Batch))
            {
                recordBook.ResetPersonalBests();
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
                case FSDJump fsdJump:
                    // Track this here because it's not present in older Scans.
                    CurrentSystem = fsdJump.StarSystem;
                    break;
                case FileHeader fileHeader:
                    IsOdyssey = fileHeader.Odyssey;
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
                case LoadGame loadGame:
                    lastLoadGame = loadGame;
                    break;
                case Statistics statistics:
                    // TODO: do more with this?
                    lastStats = statistics;
                    if (!Core.IsLogMonitorBatchReading)
                    {
                        AddResultsToGrid(MaybeAddStats());
                    }
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
            var storagePath = Core.PluginStorageFolder;
            galacticRecordsCSV = storagePath + Constants.LOCAL_GALACTIC_RECORDS_FILE;
            galacticRecordsPGCSV = storagePath + Constants.LOCAL_GALACTIC_PROCGEN_RECORDS_FILE;

            ErrorLogger = Core.GetPluginErrorLogger(this);

            MaybeUpdateGalacticRecords();

            manager = new PersonalBestManager(storagePath, ErrorLogger);
            recordBook = new(manager);

            LoadRecords();
        }

        private void MaybeFixUnsetSettings()
        {
            settings.ForceUpdateGalacticRecords = ForceRefreshGalacticRecords;

            if (settings.HighCardinalityTieSuppression == 0) settings.HighCardinalityTieSuppression = StatScannerSettings.DEFAULT.HighCardinalityTieSuppression;
            if (settings.ProcGenHandling == null) settings.ProcGenHandling = StatScannerSettings.DEFAULT_PROCGEN_HANDLING;
        }

        private void MaybeAddHeaderRows()
        {
            if (Core.IsLogMonitorBatchReading || HasHeaderRows) return;

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
            if (lastStats != null && lastLoadGame != null)
            {
                gridItems.Add(
                    new(
                        NotificationClass.None,
                        new StatScannerGrid
                        {
                            Timestamp = lastStats.Timestamp,
                            ObjectClass = "Game stats",
                            Variable = "Time played",
                            BodyOrItem = lastLoadGame.Commander,
                            Function = Function.Count.ToString(),
                            ObservedValue = $"{(lastStats.Exploration.TimePlayed / Constants.CONV_S_TO_HOURS_DIVISOR):N0}",
                            Units = "hr",
                        }));
            }
            return gridItems;
        }

        private void ShowPersonalBestSummary()
        {
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
            if (FetchFreshGalacticRecords(Constants.EDASTRO_GALACTIC_RECORDS_CSV_URL, galacticRecordsCSV)
                || FetchFreshGalacticRecords(Constants.EDASTRO_GALACTIC_RECORDS_PG_CSV_URL, galacticRecordsPGCSV))
            {
                ResetGalacticRecords();
                LoadRecords();
            }
        }

        private void ResetGalacticRecords()
        {
            manager.Clear();
            recordBook = new RecordBook(manager);
        }

        private void MaybeUpdateGalacticRecords()
        {
            // If galactic records files are missing OR > 7 days, fetch a new one and freshen.
            if (!File.Exists(galacticRecordsCSV) || File.GetLastWriteTimeUtc(galacticRecordsCSV) < DateTime.UtcNow.Subtract(TimeSpan.FromDays(7.0)))
            {
                FetchFreshGalacticRecords(Constants.EDASTRO_GALACTIC_RECORDS_CSV_URL, galacticRecordsCSV);
            }
            if (!File.Exists(galacticRecordsPGCSV) || File.GetLastWriteTimeUtc(galacticRecordsPGCSV) < DateTime.UtcNow.Subtract(TimeSpan.FromDays(7.0)))
            {
                FetchFreshGalacticRecords(Constants.EDASTRO_GALACTIC_RECORDS_PG_CSV_URL, galacticRecordsPGCSV);
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

        private void LoadRecords()
        {
            LoadGalacticRecords(galacticRecordsCSV, RecordKind.Galactic);
            LoadGalacticRecords(galacticRecordsPGCSV, RecordKind.GalacticProcGen);
            LoadPersonalBestRecords();
        }

        private void LoadGalacticRecords(string csvLocalFile, RecordKind recordKind, bool retry = false)
        {
            try
            {
                if (!File.Exists(csvLocalFile)) return;
                int recordCount = 0;
                int pbRecordCount = 0;
                bool shouldInitPersonalBest = (recordKind == RecordKind.Galactic); // To avoid dupes

                // Open the file, parse it.
                using (var csvParser = new TextFieldParser(csvLocalFile, System.Text.Encoding.UTF8))
                {
                    csvParser.TextFieldType = FieldType.Delimited;
                    csvParser.SetDelimiters(",");

                    while (!csvParser.EndOfData)
                    {
                        var fields = csvParser.ReadFields();
                        if (fields.Length != Constants.ExpectedFields.Length)
                        {
                            // Format has changed!
                            throw new RecordsCSVFormatChangedException("Galactic Records Format has changed; Reverting to previous good file, if available. Please check for an updated plugin!");
                        }
                        if (fields[0].Equals("type", StringComparison.InvariantCultureIgnoreCase)) // Header row.
                        {
                            // Check that fields in the header row are as expected. If these 
                            for (int i = 0; i < fields.Length; i++)
                            {
                                if (!fields[i].Equals(Constants.ExpectedFields[i], StringComparison.InvariantCultureIgnoreCase))
                                {
                                    throw new RecordsCSVFormatChangedException(String.Format("Galactic Records CSV Format has changed! Field {0} of galactic records file is expected to be {1}, but is {2}", i, Constants.ExpectedFields[i], fields[i]));
                                }
                            }

                            continue;
                        }

                        // Filter a bunch of stuff we don't plan on using.
                        if (fields[0].Contains(" (as ", StringComparison.InvariantCultureIgnoreCase)) continue; // Not handled
                        if (fields[0].Contains(" (any)", StringComparison.InvariantCultureIgnoreCase)) continue; // Not handled.
                        if (fields[0].Contains(" (landable)", StringComparison.InvariantCultureIgnoreCase)) continue; // Not handled.

                        // Hey, a potentially usable record. See if we have a handler for it and if so, add it to the book of records we're tracking.
                        IRecord record;
                        try
                        {
                            record = RecordFactory.CreateRecord(fields, settings, recordKind);
                            if (record != null)
                            {
                                recordCount++;
                                recordBook.AddRecord(record);
                                if (settings.DevMode) Debug.WriteLine($"Tracking {recordKind} record: {record.Table}, {record.EDAstroObjectName}, {record.VariableName}");

                                if (shouldInitPersonalBest)
                                {
                                    PersonalBestData pbData = new(fields);
                                    record = RecordFactory.CreateRecord(pbData, settings);
                                    if (record != null)
                                    {
                                        pbRecordCount++;
                                        recordBook.AddRecord(record);
                                        if (settings.DevMode) Debug.WriteLine($"Tracking {RecordKind.Personal} record: {record.Table}, {record.EDAstroObjectName}, {record.VariableName}");
                                    }
                                }
                            }
                        }
                        catch (RecordsCSVParseException ex)
                        {
                            ErrorLogger(ex, $"Error while parsing record: {String.Join(", ", fields)}");
                        }
                    }
                }
                Debug.WriteLine($"Created a total of {recordCount} {recordKind} and {pbRecordCount} {RecordKind.Personal} records; {recordBook.Count} are now in recordBook");
            }
            catch (RecordsCSVFormatChangedException ex)
            {
                if (retry)
                {
                    ErrorLogger(ex, $"While parsing previously good {csvLocalFile} file; Giving up!");
                    throw ex;
                }
                ErrorLogger(ex, $"While parsing {csvLocalFile} file; Attempting to use previous known good file");

                // Revert to previous good file and try again.
                File.Copy(csvLocalFile + Constants.GOOD_BACKUP_EXT, csvLocalFile, /* overwrite */ true);
                LoadGalacticRecords(csvLocalFile, recordKind, true);
            }
        }

        private void LoadPersonalBestRecords()
        {
            int pbRecordCount = 0;

            foreach (var pbData in Constants.GeneratePersonalBestRecords())
            {
                var record = RecordFactory.CreateRecord(pbData, settings);
                if (record == null) continue;
                pbRecordCount++;
                recordBook.AddRecord(record);
                if (settings.DevMode) Debug.WriteLine($"Tracking {RecordKind.Personal} record: {record.Table}, {record.EDAstroObjectName}, {record.VariableName}");
            }

            Debug.WriteLine($"Created a total of {pbRecordCount} {RecordKind.Personal} records; {recordBook.Count} are now in recordBook");
        }

        private void OnScan(Scan scan)
        {
            // Determine type of object from scan (stars, planets, rings, systems)
            // Look up records for the specific variant of the object (type + variant).
            List<Result> results = new();

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
                if (!record.DisallowedLogMonitorStates.Any(s => readMode.HasFlag(s)))
                    results.AddRange(record.CheckScan(scan, CurrentSystem));
            }
            return results;
        }

        private void OnFssBodySignals(FSSBodySignals bodySignals)
        {
            var readMode = Core.CurrentLogMonitorState;
            List<Result> results = new();
            foreach (var t in Constants.PB_RecordTypesForFssScans)
            {
                foreach (var record in recordBook.GetRecords(t.Item1, t.Item2))
                {
                    if (!record.DisallowedLogMonitorStates.Any(s => readMode.HasFlag(s)))
                        results.AddRange(record.CheckFSSBodySignals(bodySignals, IsOdyssey));
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
                foreach (var record in recordBook.GetRecords(t.Item1, t.Item2))
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
            return string.IsNullOrEmpty(baseName) ? bodyName.Replace(CurrentSystem, "").Trim() : bodyName.Replace(baseName, "").Trim();
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
