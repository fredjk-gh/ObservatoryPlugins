using com.github.fredjk_gh.ObservatoryStatScanner.DB;
using com.github.fredjk_gh.ObservatoryStatScanner.Records;
using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TextFieldParserCore;

namespace com.github.fredjk_gh.ObservatoryStatScanner
{
    internal class StatScannerState
    {
        private string _storagePath;
        private Action<Exception, string> _errorLogger;
        private StatScannerSettings _settings;
        private Dictionary<string, RecordBook> _recordBooks = new();
        private Dictionary<string, PersonalBestManager> _managers = new();
        private HashSet<string> _knownCmdrFIDs = new();
        private bool newCmdrWithNoExistingDb = false;

        public StatScannerState(StatScannerSettings settings, string storagePath, Action<Exception, string> errorLogger)
        {
            IsOdyssey = false;
            _settings = settings;
            _storagePath = storagePath;
            _errorLogger = errorLogger;

            FindExistingDBs();
        }

        public bool Initialized
        {
            get { return !string.IsNullOrEmpty(_storagePath) && !string.IsNullOrEmpty(CurrentSystem) && LastStats != null && LastLoadGame != null; }
        }

        public bool NeedsReadAll
        {
            get
            {
                return !Initialized || newCmdrWithNoExistingDb;
            }
        }

        public StatScannerSettings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        public bool IsOdyssey { get; set; }
        public string CurrentSystem { get; set; }
        public Statistics LastStats { get; set; }
        public LoadGame LastLoadGame { get; set; }

        public string GalacticRecordsCsv { get => _storagePath + Constants.LOCAL_GALACTIC_RECORDS_FILE; }
        public string GalacticRecordsPGCsv { get => _storagePath + Constants.LOCAL_GALACTIC_PROCGEN_RECORDS_FILE; }

        public HashSet<string> KnownCommanderFIDs {
            get
            {
                return _knownCmdrFIDs;
            }
        }

        public string CommanderFID
        {
            get
            {
                if (!Initialized) throw new InvalidOperationException($"{this.GetType().Name} is not initialized");
                return LastLoadGame.FID;
            }
        }

        public string CommanderName
        {
            get
            {
                if (!Initialized) throw new InvalidOperationException($"{this.GetType().Name} is not initialized");
                return LastLoadGame.Commander;
            }

        }

        public RecordBook GetRecordBook()
        {
            if (!Initialized) throw new InvalidOperationException($"{this.GetType().Name} is not initialized");
            return GetRecordBookForFID(CommanderFID);
        }

        public void ResetForNewFile()
        {
            LastLoadGame = null;
            LastStats = null;
            CurrentSystem = null;
        }

        public void ResetForReadAll()
        {
            ResetForNewFile();
            newCmdrWithNoExistingDb = false;
            foreach (var book in _recordBooks.Values)
            {
                book.ResetPersonalBests();
            }
        }

        public void ReloadGalacticRecords()
        {
            ResetForNewFile();
            foreach (string fid in _knownCmdrFIDs)
            {
                _managers[fid].Clear();
                _recordBooks[fid] = new(_managers[fid]);
            }
            LoadRecords();
        }

        #region Record Loading
        private void LoadRecords(HashSet<string> fids = null)
        {
            // TODO: Check that nothing is already loaded? Otherwise, this is just a reload...
            LoadGalacticRecords(fids ?? KnownCommanderFIDs, GalacticRecordsCsv, RecordKind.Galactic);
            LoadGalacticRecords(fids ?? KnownCommanderFIDs, GalacticRecordsPGCsv, RecordKind.GalacticProcGen);
            LoadPersonalBestRecords(fids ?? KnownCommanderFIDs);
        }

        private void LoadGalacticRecords(HashSet<string> fids, string csvLocalFile, RecordKind recordKind, bool retry = false)
        {
            if (fids.Count == 0) return;

            try
            {
                if (!File.Exists(csvLocalFile)) return;
                int recordCount = 0;
                int pbRecordCount = 0;
                bool shouldInitPersonalBest = (recordKind == RecordKind.Galactic); // To avoid dupes

                // Open the file, parse it.
                using (var csvParser = new TextFieldParser(csvLocalFile, Encoding.UTF8))
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
                            foreach (var fid in fids)
                            {
                                RecordBook recordBook = GetRecordBookForFID(fid);

                                record = RecordFactory.CreateRecord(fields, Settings, recordKind);
                                if (record == null) continue;

                                recordCount++;
                                recordBook.AddRecord(record);
                                if (Settings.DevMode) Debug.WriteLine($"Tracking {recordKind} record: {record.Table}, {record.EDAstroObjectName}, {record.VariableName}");

                                if (shouldInitPersonalBest)
                                {
                                    PersonalBestData pbData = new(fields);
                                    record = RecordFactory.CreateRecord(pbData, Settings);
                                    if (record != null)
                                    {
                                        pbRecordCount++;
                                        recordBook.AddRecord(record);
                                        if (Settings.DevMode) Debug.WriteLine($"Tracking {RecordKind.Personal} record: {record.Table}, {record.EDAstroObjectName}, {record.VariableName}");
                                    }
                                }
                            }
                        }
                        catch (RecordsCSVParseException ex)
                        {
                            _errorLogger(ex, $"Error while parsing record: {String.Join(", ", fields)}");
                        }
                    } // while
                } // using
                Debug.WriteLine($"Created a total of {recordCount} {recordKind} and {pbRecordCount} {RecordKind.Personal} records in {fids.Count} record books.");
            }
            catch (RecordsCSVFormatChangedException ex)
            {
                if (retry)
                {
                    _errorLogger(ex, $"While parsing previously good {csvLocalFile} file; Giving up!");
                    throw;
                }
                _errorLogger(ex, $"While parsing {csvLocalFile} file; Attempting to use previous known good file");

                // Revert to previous good file and try again.
                File.Copy(csvLocalFile + Constants.GOOD_BACKUP_EXT, csvLocalFile, /* overwrite */ true);
                LoadGalacticRecords(fids, csvLocalFile, recordKind, true);
            }
        }

        private void LoadPersonalBestRecords(HashSet<string> fids)
        {
            int pbRecordCount = 0;

            foreach (var pbData in Constants.GeneratePersonalBestRecords())
            {
                foreach (string fid in fids)
                {
                    var recordBook = GetRecordBookForFID(fid);

                    var record = RecordFactory.CreateRecord(pbData.Clone(), Settings);
                    if (record == null) continue;
                    pbRecordCount++;
                    recordBook.AddRecord(record);
                    if (Settings.DevMode) Debug.WriteLine($"Tracking {RecordKind.Personal} record: {record.Table}, {record.EDAstroObjectName}, {record.VariableName}");
                }
            }

            Debug.WriteLine($"Created a total of {pbRecordCount} {RecordKind.Personal} records in {fids.Count} record books.");
        }

        #endregion

        private RecordBook GetRecordBookForFID(string fid, bool deferLoadRecords = false)
        {
            if (!_managers.ContainsKey(fid))
            {
                KnownCommanderFIDs.Add(fid);
                _managers.Add(fid, new PersonalBestManager(_storagePath, _errorLogger, fid));
            }
            if (!_recordBooks.ContainsKey(fid))
            {
                _recordBooks.Add(fid, new(_managers[fid]));

                if (!deferLoadRecords)
                    LoadRecords(new() { fid });
            }

            return _recordBooks[fid];
        }

        private void FindExistingDBs()
        {
            string dbFilePattern = string.Format(PersonalBestManager.PERSONAL_BEST_DB_FILENAME_TEMPLATE, "", "*");

            DirectoryInfo pluginDataDir = new DirectoryInfo(_storagePath);
            FileInfo[] dbs = pluginDataDir.GetFiles(dbFilePattern);
            
            foreach(FileInfo db in dbs)
            {
                string[] parts = db.Name.Replace(".db", "").Replace("-log", "").Split('_');

                if (parts.Length < 3) continue;
                string FID = parts[2];

                if (KnownCommanderFIDs.Contains(FID)) continue;

                KnownCommanderFIDs.Add(FID);

                // Just initialize 
                GetRecordBookForFID(FID, true);
            }

            LoadRecords(KnownCommanderFIDs);
        }

    }
}
