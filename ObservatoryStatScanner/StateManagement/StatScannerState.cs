using com.github.fredjk_gh.ObservatoryStatScanner.DB;
using com.github.fredjk_gh.ObservatoryStatScanner.Records;
using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TextFieldParserCore;

namespace com.github.fredjk_gh.ObservatoryStatScanner.StateManagement
{
    internal class StatScannerState
    {
        private StateCache _stateCache;
        private IObservatoryCore _core;
        private StatScannerSettings _settings;
        private IObservatoryWorker _worker;
        /// <summary>
        /// Error logger; string param is an arbitrary context hint.
        /// </summary>
        private Action<Exception, string> _errorLogger;
        private string _storagePath;

        private Dictionary<string, RecordBook> _recordBooks = new();
        private Dictionary<string, PersonalBestManager> _managers = new();
        private bool _lastSeenIsOdyssey = false;

        public StatScannerState(StateCache cached, IObservatoryCore core, StatScannerSettings settings, IObservatoryWorker worker)
        {
            _stateCache = cached;
            _core = core;
            _settings = settings;
            _worker = worker;
            _errorLogger = core.GetPluginErrorLogger(worker);
            _storagePath = core.PluginStorageFolder;

            FindExistingDBs();
        }

        internal StateCache Cacheable { get => _stateCache; }

        public IObservatoryCore Core { get => _core; }

        public Action<Exception, string> ErrorLogger { get => _errorLogger; }

        public bool IsCommanderFidKnown
        {
            get => !string.IsNullOrWhiteSpace(_stateCache.LastSeenCommanderFID) && _stateCache.IsCommanderKnown(_stateCache.LastSeenCommanderFID);
        }

        public List<string> KnownCommanderFids
        {
            get => _stateCache.KnownCommanders.Keys.ToList();
        }

        public bool NeedsReadAll
        {
            get => _stateCache.ReadAllRequired || _stateCache.KnownCommanders.Count == 0;
        }

        public StatScannerSettings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        public bool IsOdyssey {
            get
            {
                if (_stateCache.KnownCommanders.Count == 0 || !_stateCache.IsCommanderKnown(_stateCache.LastSeenCommanderFID))
                    return _lastSeenIsOdyssey;
                return _stateCache.KnownCommanders[_stateCache.LastSeenCommanderFID].IsOdyssey;
            }
            set
            {
                _lastSeenIsOdyssey = value;
                if (_stateCache.KnownCommanders.Count == 0 || !_stateCache.IsCommanderKnown(_stateCache.LastSeenCommanderFID))
                {
                    return;
                }
                _stateCache.UpdateIsOdyssey(value);
            }
        }

        public string CurrentSystem
        {
            get
            {
                if (_stateCache.KnownCommanders.Count == 0 || !_stateCache.IsCommanderKnown(_stateCache.LastSeenCommanderFID))
                    return "(Unknown commander / location)";
                return _stateCache.KnownCommanders[_stateCache.LastSeenCommanderFID].CurrentSystem;
            }
            set
            {
                if (_stateCache.KnownCommanders.Count == 0 || !_stateCache.IsCommanderKnown(_stateCache.LastSeenCommanderFID))
                    return;
                _stateCache.UpdateCommanderLocation(value);
            }
        }

        public void AddScan(Scan scan)
        {
            if (_stateCache.CurrentCommander == null) return;
            _stateCache.CurrentCommander.Scans[scan.BodyID] = scan;
        }

        public Dictionary<int, Scan> Scans
        {
            get => _stateCache.CurrentCommander?.Scans ?? new();
        }
        public int SystemBodyCount
        {
            get => _stateCache.CurrentCommander?.SystemBodyCount ?? 0;
            internal set
            {
                if (_stateCache.CurrentCommander == null) return;
                _stateCache.CurrentCommander.SystemBodyCount = value;
            }
        }

        public bool NavBeaconScanned 
        { 
            get => _stateCache.CurrentCommander?.NavBeaconScanned ?? false;
            internal set
            {
                if (_stateCache.CurrentCommander == null) return;
                _stateCache.CurrentCommander.NavBeaconScanned = value;
            }
        }

        public LoadGame LastLoadGame
        {
            get => _stateCache.CurrentCommander?.LastLoadGame;
            set => _stateCache.UpdateCommanderInfo(value, _lastSeenIsOdyssey, (Core.CurrentLogMonitorState & LogMonitorState.Batch) != 0);
        }

        public Statistics? LastStats
        {
            get => _stateCache.CurrentCommander?.LastStatistics;
            set => _stateCache.UpdateCommanderStats(value);
        }

        public string GalacticRecordsCsv { get => _storagePath + Constants.LOCAL_GALACTIC_RECORDS_FILE; }
        public string GalacticRecordsPGCsv { get => _storagePath + Constants.LOCAL_GALACTIC_PROCGEN_RECORDS_FILE; }

        public string CommanderFID
        {
            get => _stateCache.LastSeenCommanderFID;
        }

        public string CommanderName
        {
            get => _stateCache.LastSeenCommanderName;
        }

        public RecordBook GetRecordBook()
        {
            if (!IsCommanderFidKnown)
                throw new InvalidOperationException($"{GetType().Name} is not initialized");
            return GetRecordBookForFID(CommanderFID);
        }

        public RecordBook GetRecordBookForFID(string fid, bool deferLoadRecords = false)
        {
            if (!_stateCache.IsCommanderKnown(fid))
            {
                _stateCache.AddCommander(fid, _lastSeenIsOdyssey, (Core.CurrentLogMonitorState & LogMonitorState.Batch) != 0);
            }
            if (!_managers.ContainsKey(fid))
            {
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

        public void ResetForReadAll()
        {
            foreach (var book in _recordBooks)
            {
                book.Value.ResetPersonalBests();
                _managers[book.Key].BatchProcessingMode = true;
            }
            _stateCache.ResetBeforeReadAll();
        }

        public void ReadAllComplete()
        {
            foreach (var book in _recordBooks)
            {
                _managers[book.Key].BatchProcessingMode = false;
                _managers[book.Key].Flush();

            }
            _stateCache.ClearReadAllRequired();
        }

        public List<StatScannerGrid> GetPluginStateMessages()
        {
            List<StatScannerGrid> messages = new();

            if (Cacheable.ReadAllRequired)
            {
                messages.Add(
                    new StatScannerGrid
                    {
                        ObjectClass = "Plugin message",
                        Variable = $"Please run 'Read All' to initialize database(s)",
                        Details = Cacheable.ReadAllReason,
                    });
            }

            return messages;
        }

        public void ReloadGalacticRecords()
        {
            foreach (string fid in _stateCache.KnownCommanders.Keys)
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
            LoadGalacticRecords(fids ?? _stateCache.KnownCommanders.Keys.ToHashSet(), GalacticRecordsCsv, RecordKind.Galactic);
            LoadGalacticRecords(fids ?? _stateCache.KnownCommanders.Keys.ToHashSet(), GalacticRecordsPGCsv, RecordKind.GalacticProcGen);
            LoadPersonalBestRecords(fids ?? _stateCache.KnownCommanders.Keys.ToHashSet());
        }

        private void LoadGalacticRecords(HashSet<string> fids, string csvLocalFile, RecordKind recordKind, bool retry = false)
        {
            if (fids.Count == 0) return;

            try
            {
                if (!File.Exists(csvLocalFile)) return;
                int recordCount = 0;
                int pbRecordCount = 0;
                bool shouldInitPersonalBest = recordKind == RecordKind.Galactic; // To avoid dupes

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
                        if (fields[0].Equals("type", StringComparison.OrdinalIgnoreCase)) // Header row.
                        {
                            // Check that fields in the header row are as expected. If these 
                            for (int i = 0; i < fields.Length; i++)
                            {
                                if (!fields[i].Equals(Constants.ExpectedFields[i], StringComparison.OrdinalIgnoreCase))
                                {
                                    throw new RecordsCSVFormatChangedException(string.Format("Galactic Records CSV Format has changed! Field {0} of galactic records file is expected to be {1}, but is {2}", i, Constants.ExpectedFields[i], fields[i]));
                                }
                            }

                            continue;
                        }

                        // Filter a bunch of stuff we don't plan on using.
                        if (fields[0].Contains(" (as ", StringComparison.OrdinalIgnoreCase)) continue; // Not handled
                        if (fields[0].Contains(" (any)", StringComparison.OrdinalIgnoreCase)) continue; // Not handled.
                        if (fields[0].Contains(" (landable)", StringComparison.OrdinalIgnoreCase)) continue; // Not handled.

                        // Hey, a potentially usable record. See if we have a handler for it and if so, add it to the book of records we're tracking.
                        IRecord record;
                        try
                        {
                            foreach (var fid in fids)
                            {
                                // Don't load records for this potentially new record book. This is literally what we're doing here.
                                RecordBook recordBook = GetRecordBookForFID(fid, true /* deferLoadRecords */ );

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
                            _errorLogger(ex, $"Error while parsing record: {string.Join(", ", fields)}");
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
                    // Don't load records for this potentially new record book. This is literally what we're doing here.
                    var recordBook = GetRecordBookForFID(fid, true /* deferLoadRecords */);

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

        private void FindExistingDBs()
        {
            string dbFilePattern = string.Format(PersonalBestManager.PERSONAL_BEST_DB_FILENAME_TEMPLATE, "", "*");

            DirectoryInfo pluginDataDir = new DirectoryInfo(_storagePath);
            FileInfo[] dbs = pluginDataDir.GetFiles(dbFilePattern);

            foreach (FileInfo db in dbs)
            {
                string[] parts = db.Name.Replace(".db", "").Replace("-log", "").Split('_');

                if (parts.Length < 3) continue;
                string FID = parts[2];

                if (_stateCache.IsCommanderKnown(FID)) continue;

                _stateCache.AddCommander(FID, _lastSeenIsOdyssey, true); // Assume existing DBs means previous read-all is done?

                // Just initialize 
                GetRecordBookForFID(FID, true);
            }

            LoadRecords(_stateCache.KnownCommanders.Keys.ToHashSet());
        }
    }
}
