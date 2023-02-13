using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using ObservatoryStatScanner.Records;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TextFieldParserCore;
using static ObservatoryStatScanner.Records.BodyRecord;

namespace ObservatoryStatScanner
{
    public class StatScanner : IObservatoryWorker
    {
        private StatScannerSettings settings = new()
        {
            // EnablePersonalBests = true,
            MaxNearRecordThreshold = 0,
            MinNearRecordThreshold = 0,
            HighCardinalityTieSuppression = 50000,
            EnableEarthMassesRecord = true,
            EnablePlanetaryRadiusRecord = true,
            EnableSurfaceGravityRecord = true,
            EnableSurfacePressureRecord = true,
            EnableSurfaceTemperatureRecord = true,
            EnableOrbitalEccentricityRecord = true,
            EnableOrbitalPeriodRecord = true,
            EnableSolarMassesRecord = true,
            EnableSolarRadiusRecord = true,
        };

        private IObservatoryCore Core;
        /// <summary>
        /// Error logger; string param is an arbitrary context hint.
        /// </summary>
        private Action<Exception, string> ErrorLogger;
        private PluginUI pluginUI;
        ObservableCollection<object> gridCollection = new();
        private string galacticRecordsCSV;
        private string galacticRecordsCSVPreviousGood;

        private RecordBook recordBook = new();
        private bool IsOdyssey = false;
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

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            switch (journal)
            {
                case FileHeader fileHeader:
                    IsOdyssey = fileHeader.Odyssey;
                    break;
                case Scan scan:
                    OnScan(scan);
                    break;
                case FSSDiscoveryScan honk:
                    break;
                case FSSBodySignals bodySignals:
                    if (IsOdyssey)
                    {
                        // Check.
                    }
                    break;
            }
            // Intitial Limitations
            // - Only handle top-level records -- no distinction between bodies as moons vs. planets vs. main stars.
            // - Only handle records based directly on scan data, not derived ones (such as area) where my calcs may be inconsistent with EDAstro.
        }

        private void OnScan(Scan scan)
        {
            // Determine type of object from scan (stars, planets, rings)
            // Look up records for the specific variant of the object (type + variant).
            List<StatScannerGrid> results = new();
            if (!string.IsNullOrEmpty(scan.StarType))
            {
                results.AddRange(CheckScanForRecords(scan, recordBook.GetRecords(RecordTable.Stars, scan.StarType)));
            }
            if (!string.IsNullOrEmpty(scan.PlanetClass) && scan.PlanetClass != "Barycentre")
            {
                results.AddRange(CheckScanForRecords(scan, recordBook.GetRecords(RecordTable.Planets, scan.PlanetClass)));
            }
            if (scan.Rings != null)
            {
                // Process proper rings (NOT belts) by RingClass!
                foreach (var rc in scan.Rings.Where(r => r.Name.Contains("Ring")).Select(r => r.RingClass).Distinct())
                {
                    results.AddRange(CheckScanForRecords(scan, recordBook.GetRecords(RecordTable.Rings, rc)));
                }
            }

            foreach (var r in results)
            {
                Core.AddGridItem(this, r);
                // TODO: Fire notification.
            }
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

        public void Load(IObservatoryCore observatoryCore)
        {
            gridCollection = new();
            StatScannerGrid uiObject = new();

            gridCollection.Add(uiObject);
            pluginUI = new PluginUI(gridCollection);

            Core = observatoryCore;
            galacticRecordsCSV = Core.PluginStorageFolder + "galactic_records.csv";
            galacticRecordsCSVPreviousGood = galacticRecordsCSV + ".good";
            ErrorLogger = Core.GetPluginErrorLogger(this);
            MaybeUpdateGalacticRecords();

            try
            {
                LoadGalacticRecords(galacticRecordsCSV);
            }
            catch (RecordsCSVFormatChangedException ex)
            {
                ErrorLogger(ex, "While parsing galactic records CSV file; Attempting to use previous known good file");

                // Revert to previous good file and try again.
                File.Copy(galacticRecordsCSVPreviousGood, galacticRecordsCSV, true);
                LoadGalacticRecords(galacticRecordsCSV);
            }

            settings.ForceUpdateGalacticRecords = ForceRefreshGalacticRecords;
        }

        private void ForceRefreshGalacticRecords()
        {
            if (FetchFreshGalacticRecords())
            {
                ResetGalacticRecords();
                LoadGalacticRecords(galacticRecordsCSV);
            }
        }

        private void ResetGalacticRecords ()
        {
            recordBook = new RecordBook();
            // TODO: Reset any other state needed after a galactic record refresh.
        }

        private void MaybeUpdateGalacticRecords()
        {
            // If galactic records file is missing OR > 7 days, fetch a new one and freshen.
            if (!File.Exists(galacticRecordsCSV) || File.GetLastWriteTimeUtc(galacticRecordsCSV) < DateTime.UtcNow.Subtract(TimeSpan.FromDays(7.0)))
            {
                FetchFreshGalacticRecords();
            }
        }

        private bool FetchFreshGalacticRecords()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(Constants.EDASTRO_GALACTIC_RECORDS_CSV_URL),
            };
            var requestTask = Core.HttpClient.SendAsync(request);
            requestTask.Wait(1000);

            if (requestTask.IsFaulted)
            {
                ErrorLogger(requestTask.Exception, "While fetching fresh galactic records; using stale records for now...");
                return false;
            }

            var response = requestTask.Result;
            try
            {
                response.EnsureSuccessStatusCode(); // Maybe try EnsureSuccessStatusCode instead?
            }
            catch (HttpRequestException ex)
            {
                ErrorLogger(ex, "While downloading updated galactic records; using stale records (if available)");
                return false;
            }

            try
            {
                if (File.Exists(galacticRecordsCSV))
                {
                    // Preserve old one, just in case.
                    File.Copy(galacticRecordsCSV, galacticRecordsCSVPreviousGood, true);
                }
                // Write result to file.
                using (Stream csvFile = File.Create(galacticRecordsCSV))
                {
                    response.Content.ReadAsStream().CopyTo(csvFile);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger(ex, "While writing local galactic records cache file");
                return false;
            }

            return true;
        }

        private void LoadGalacticRecords(string csvFile)
        {
            // File is quoted CSV format with columns:
            string[] ExpectedFields = new string[] {
                "Type",
                "Variable",
                "Max Count",
                "Max Value",
                "Max Body",
                "Min Count",
                "Min Value",
                "Min Body",
                "Average",
                "Standard Deviation",
                "Count",
                "Table"
            };

            // Open the file, parse it.
            using (var csvParser = new TextFieldParser(csvFile, System.Text.Encoding.UTF8))
            {
                csvParser.TextFieldType = FieldType.Delimited;
                csvParser.SetDelimiters(",");

                while (!csvParser.EndOfData)
                {
                    var fields = csvParser.ReadFields();
                    if (fields.Length != ExpectedFields.Length)
                    {
                        // Format has changed!
                        throw new RecordsCSVFormatChangedException("Galactic Records Format has changed; Reverting to previous good file, if available. Please check for an updated plugin!");
                    }
                    if (fields[0].Equals("type", StringComparison.InvariantCultureIgnoreCase)) // Header row.
                    {
                        // Check that fields in the header row are as expected. If these 
                        for (int i = 0; i < fields.Length; i++)
                        {
                            if (!fields[i].Equals(ExpectedFields[i], StringComparison.InvariantCultureIgnoreCase))
                            {
                                throw new RecordsCSVFormatChangedException(String.Format("Galactic Records CSV Format has changed! Field {0} of galactic records file is expected to be {1}, but is {2}", i, ExpectedFields[i], fields[i]));
                            }
                        }

                        continue;
                    }
                    
                    // Filter a bunch of stuff we don't plan on useing.
                    if (fields[0].Contains(" (as ", StringComparison.InvariantCultureIgnoreCase)) continue; // Not handled
                    if (fields[0].Contains(" (any)", StringComparison.InvariantCultureIgnoreCase)) continue; // Not handled.
                    if (fields[0].Contains(" (landable)", StringComparison.InvariantCultureIgnoreCase)) continue; // Not handled.

                    // Hey, a potentially usable record. See if we have a handler for it and if so, add it to the book of records we're tracking.
                    var record = RecordFactory.CreateRecord(fields, settings);
                    if (record != null)
                    {
                        recordBook.AddRecord(record);
                        if (settings.DevMode) Debug.WriteLine("Tracking record: {0}, {1}, {2}", record.Table, record.EDAstroObjectName, record.VariableName);
                    }
                }
            }
        }
    }

    public class StatScannerGrid
    {
        public string Timestamp { get; set; }
        public string Body { get; set; }
        public string ObjectClass { get; set; }
        public string Variable { get; set; } // From DisplayName
        public string Function { get; set; }
        public string ObservedValue { get; set; }
        public string RecordValue { get; set; }
        public string RecordHolder { get; set; }
        public string Details { get; set; }
    }
}
