using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using ObservatoryStatScanner.DB;
using ObservatoryStatScanner.Records;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TextFieldParserCore;
using static ObservatoryStatScanner.Records.BodyRecord;
using static ObservatoryStatScanner.StatScannerSettings;

namespace ObservatoryStatScanner
{
    public class StatScanner : IObservatoryWorker
    {
        private StatScannerSettings settings = new()
        {
            MaxNearRecordThreshold = 0,
            MinNearRecordThreshold = 0,
            HighCardinalityTieSuppression = 50000,
            FirstDiscoveriesOnly = false,
            ProcGenHandling = StatScannerSettings.DEFAULT_PROCGEN_HANDLING,
            EnableEarthMassesRecord = true,
            EnablePlanetaryRadiusRecord = true,
            EnableSurfaceGravityRecord = true,
            EnableSurfacePressureRecord = true,
            EnableSurfaceTemperatureRecord = true,
            EnableOrbitalEccentricityRecord = true,
            EnableOrbitalPeriodRecord = true,
            EnableRotationalPeriodRecord = true,
            EnableSolarMassesRecord = true,
            EnableSolarRadiusRecord = true,
            EnableRingOuterRadiusRecord = true,
            EnableRingWidthRecord = true,
            EnableRingMassRecord = true,
            EnableRingDensityRecord = true,
            EnableOdysseySurfaceBioRecord = true,
            EnableSystemBodyCountRecords = true,
            EnableRegionCodexCountRecords = false,
            EnableVisitedRegionRecords = true,
        };

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
        // private string CurrentSystem = "";
        // private List<Scan> SystemScans = new();


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
            if (args.NewState.HasFlag(LogMonitorState.Batch))
            {
                recordBook.ResetPersonalBests();
                Core.ClearGrid(this, new StatScannerGrid());
                HasHeaderRows = false;
            }
        }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            MaybeAddHeaderRows();

            switch (journal)
            {
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
            }

            // Intitial Limitations
            // - Only handle top-level records -- no distinction between bodies as moons vs. planets vs. main stars.
        }

        public void Load(IObservatoryCore observatoryCore)
        {
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

            manager = new PersonalBestManager(storagePath);
            recordBook = new(manager);

            LoadRecords();
            settings.ForceUpdateGalacticRecords = ForceRefreshGalacticRecords;
        }

        private void MaybeAddHeaderRows()
        {
            if (HasHeaderRows) return;
            HasHeaderRows = true;

            var devModeWarning = (settings.DevMode ? "!!DEV mode!!" : "");
            var gridItems = new List<StatScannerGrid>();
            gridItems.Add(new StatScannerGrid
            {
                ObjectClass = "Plugin stats",
                Variable = "Tracked Record(s): Total",
                Function = Function.Count.ToString(),
                ObservedValue = $"{recordBook.Count}",
                RecordValue = devModeWarning,
            });
            foreach (RecordKind kind in Enum.GetValues(typeof(RecordKind))) {
                var count = recordBook.CountByKind(kind);
                var details = "";
                switch (kind)
                {
                    case RecordKind.Personal:
                        details = (count == 0 ? "Not yet implemented" : "");
                        break;
                    case RecordKind.GalacticProcGen:
                        var handlingMode = (ProcGenHandlingMode)settings.ProcGenHandlingOptions[settings.ProcGenHandling ?? DEFAULT_PROCGEN_HANDLING];
                        details = (handlingMode == ProcGenHandlingMode.ProcGenIgnore ? "Ignored via settings" : "");
                        break;
                }

                gridItems.Add(new StatScannerGrid
                {
                    ObjectClass = "Plugin stats",
                    Variable = $"Tracked Record(s): {kind.ToString()}",
                    Function = Function.Count.ToString(),
                    ObservedValue = $"{count}",
                    RecordValue = devModeWarning,
                    Details = details,
                });
            }
            AddResultsToGrid(gridItems);
        }

        private void ShowPersonalBestSummary()
        {
            MaybeAddHeaderRows();
            var gridItems = new List<StatScannerGrid>();
            foreach (var best in recordBook.GetPersonalBests())
            {
                gridItems.AddRange(best.Summary());
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

        private void ResetGalacticRecords ()
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
            ShowPersonalBestSummary();
        }
        private void LoadGalacticRecords(string csvLocalFile, RecordKind recordKind, bool retry = false)
        {
            try
            {
                if (!File.Exists(csvLocalFile)) return;
                int recordCount = 0;
                int pbRecordCount = 0;
                bool shouldInitPersonalBest = (recordKind == RecordKind.Galactic);

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
                                    PersonalBestData pbData = new PersonalBestData(fields);
                                    record = RecordFactory.CreateRecord(pbData, settings);
                                    pbRecordCount++;
                                    recordBook.AddRecord(record);
                                    if (settings.DevMode) Debug.WriteLine($"Tracking {RecordKind.Personal} record: {record.Table}, {record.EDAstroObjectName}, {record.VariableName}");
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
            List<StatScannerGrid> results = new();

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

        private List<StatScannerGrid> CheckScanForRecords(Scan scan, List<IRecord> records)
        {
            List<StatScannerGrid> results = new();
            foreach (var record in records)
            {
                results.AddRange(record.CheckScan(scan));
            }
            return results;
        }

        private void OnFssBodySignals(FSSBodySignals bodySignals)
        {
            List<StatScannerGrid> results = new();
            foreach (var t in Constants.PB_RecordTypesForFssScans)
            {
                foreach (var record in recordBook.GetRecords(t.Item1, t.Item2))
                {
                    results.AddRange(record.CheckFSSBodySignals(bodySignals, IsOdyssey));
                }
            }
            AddResultsToGrid(results, /* notify */ true);
        }

        private void OnFssAllBodiesFound(FSSAllBodiesFound fssAllBodies, List<Scan> scans)
        {
            List<StatScannerGrid> results = new();
            foreach (var t in Constants.PB_RecordTypesForFssScans)
            {
                foreach (var record in recordBook.GetRecords(t.Item1, t.Item2))
                {
                    results.AddRange(record.CheckFSSAllBodiesFound(fssAllBodies, scans));
                }
            }
            AddResultsToGrid(results, /* notify */ true);
        }
        private void OnCodexEntry(CodexEntry codexEntry)
        {
            if (!Constants.RegionNamesByJournalId.ContainsKey(codexEntry.Region)
                || !codexEntry.IsNewEntry) return;

            string regionNameByJournalValue = Constants.RegionNamesByJournalId[codexEntry.Region];
            List<StatScannerGrid> results = new();

            foreach (var record in recordBook.GetRecords(RecordTable.Regions, Constants.OBJECT_TYPE_REGION))
            {
                results.AddRange(record.CheckCodexEntry(codexEntry));
            }
            foreach (var record in recordBook.GetRecords(RecordTable.Regions, regionNameByJournalValue))
            {
                results.AddRange(record.CheckCodexEntry(codexEntry));
            }
            AddResultsToGrid(results, /* notify */ true);
        }

        private void AddResultsToGrid(List<StatScannerGrid> results, bool notify = false)
        {
            //if (!notify)
            //{
            //    // REQUIRES LATEST OBSERVATORY CORE!!!
            //    Core.AddGridItems(this, results);
            //    return;
            //}

            foreach (var r in results)
            {
                Core.AddGridItem(this, r);
                // TODO: Fire notification
            }
        }

    }
}
