using com.github.fredjk_gh.ObservatoryStatScanner.DB;
using com.github.fredjk_gh.ObservatoryStatScanner.Records.Data;
using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using com.github.fredjk_gh.PluginCommon.PluginInterop;
using com.github.fredjk_gh.PluginCommon.Utilities;
using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using System.Diagnostics;
using System.Text;
using TextFieldParserCore;

namespace com.github.fredjk_gh.ObservatoryStatScanner.StateManagement
{
    internal class StatScannerContext : ICoreContext<StatScanner>
    {
        private readonly IObservatoryCore _c;
        private readonly StatScanner _w;
        private readonly MessageDispatcher _d;
        private readonly DebugLogger _l;

        private readonly StateCache _stateCache;
        private StatScannerSettings _settings;

        /// <summary>
        /// Error logger; string param is an arbitrary context hint.
        /// </summary>
        private readonly Action<Exception, string> _errorLogger;
        private readonly string _storagePath;

        private readonly Dictionary<string, RecordBook> _recordBooks = [];
        private readonly Dictionary<string, PersonalBestManager> _managers = [];
        private bool _lastSeenIsOdyssey = false;

        public StatScannerContext(StateCache cached, IObservatoryCore core, StatScannerSettings settings, StatScanner worker)
        {
            _c = core;
            _w = worker;
            _d = new(core, worker, PluginTracker.PluginType.fredjk_StatScanner);
            _l = new(core, worker);
            _stateCache = cached;
            _settings = settings;
            _errorLogger = core.GetPluginErrorLogger(worker);
            _storagePath = core.PluginStorageFolder;

            OpenExistingDBs();
        }

        public IObservatoryCore Core { get => _c; }
        public StatScanner Worker { get => _w; }
        public MessageDispatcher Dispatcher { get => _d; }
        public DebugLogger Dlogger { get => _l; }
        internal StateCache Cacheable { get => _stateCache; }
        public Action<Exception, string> ErrorLogger { get => _errorLogger; }

        public bool IsCommanderFidKnown
        {
            get => !string.IsNullOrWhiteSpace(_stateCache.LastSeenCommanderFID) && _stateCache.IsCommanderKnown(_stateCache.LastSeenCommanderFID);
        }

        public List<string> KnownCommanderFids
        {
            get => [.. _stateCache.KnownCommanders.Keys];
        }

        public bool NeedsReadAll
        {
            get => _stateCache.ReadAllRequired || _stateCache.KnownCommanders.Count == 0;
        }

        public StatScannerSettings Settings
        {
            get { return _settings; }
            set { _settings = MaybeFixUnsetSettings(value); }
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
            get => _stateCache.CurrentCommander?.Scans ?? [];
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
            bool IsReadAll = Core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch);
            if (!_stateCache.IsCommanderKnown(fid))
            {
                _stateCache.AddCommander(fid, _lastSeenIsOdyssey, IsReadAll);
            }
            if (!_managers.TryGetValue(fid, out PersonalBestManager pbMgr))
            {
                pbMgr = new(_storagePath, _errorLogger, fid)
                {
                    BatchProcessingMode = IsReadAll
                };
                pbMgr.Connect(ConnectionMode.Direct);
                _managers.Add(fid, pbMgr);
            }
            if (!_recordBooks.TryGetValue(fid, out RecordBook rb))
            {
                rb = new(pbMgr);
                _recordBooks.Add(fid, rb);

                if (!deferLoadRecords)
                    LoadRecords([fid]);
            }

            return rb;
        }

        public void ResetForReadAll()
        {
            foreach (var book in _recordBooks)
            {
                book.Value.ResetPersonalBests();
                var mgr = _managers[book.Key];
                mgr.BatchProcessingMode = true;
                mgr.Connect(ConnectionMode.Direct); // Better performance for ReadAll.
            }
            _stateCache.ResetBeforeReadAll();
        }

        public void ReadAllComplete()
        {
            foreach (var book in _recordBooks)
            {
                var mgr = _managers[book.Key];
                mgr.BatchProcessingMode = false;
                mgr.Flush();

                mgr.Connect(ConnectionMode.Shared);
            }
            _stateCache.ClearReadAllRequired();
        }

        public List<StatScannerGrid> GetPluginStateMessages()
        {
            List<StatScannerGrid> messages = [];

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
            LoadGalacticRecords(fids ?? [.. _stateCache.KnownCommanders.Keys], GalacticRecordsCsv, RecordKind.Galactic);
            LoadGalacticRecords(fids ?? [.. _stateCache.KnownCommanders.Keys], GalacticRecordsPGCsv, RecordKind.GalacticProcGen);
            LoadPersonalBestRecords(fids ?? [.. _stateCache.KnownCommanders.Keys]);
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

        private void OpenExistingDBs()
        {
            string dbFilePattern = string.Format(PersonalBestManager.PERSONAL_BEST_DB_FILENAME_TEMPLATE, "", "*");

            DirectoryInfo pluginDataDir = new(_storagePath);
            FileInfo[] dbs = pluginDataDir.GetFiles(dbFilePattern);

            foreach (FileInfo db in dbs)
            {
                string[] parts = db.Name.Replace(".db", "").Replace("-log", "").Split('_');

                if (parts.Length < 3) continue;
                string FID = parts[2];

                if (_stateCache.IsCommanderKnown(FID)) continue;

                // Ok so we have extra files for unknown commander. Let's clean them up. It's not 
                // hard to re-create by re-reading all.
                try
                {
                    db.Delete();
                }
                catch(Exception ex)
                {
                    Debug.WriteLine($"Unable to clean up stray DB file: {db.FullName}: {ex.Message}");
                }
            }

            LoadRecords([.. _stateCache.KnownCommanders.Keys]);
        }


        private void ForceRefreshGalacticRecords()
        {
            if (FetchFreshGalacticRecords(Constants.EDASTRO_GALACTIC_RECORDS_CSV_URL, GalacticRecordsCsv)
                || FetchFreshGalacticRecords(Constants.EDASTRO_GALACTIC_RECORDS_PG_CSV_URL, GalacticRecordsPGCsv))
            {
                ReloadGalacticRecords();
            }
        }

        internal void MaybeUpdateGalacticRecords()
        {
            try
            {
                // If galactic records files are missing OR > 7 days, fetch a new one and freshen.
                if (!File.Exists(GalacticRecordsCsv) || File.GetLastWriteTimeUtc(GalacticRecordsCsv) < DateTime.UtcNow.Subtract(TimeSpan.FromDays(7.0)))
                {
                    FetchFreshGalacticRecords(Constants.EDASTRO_GALACTIC_RECORDS_CSV_URL, GalacticRecordsCsv);
                }
                if (!File.Exists(GalacticRecordsPGCsv) || File.GetLastWriteTimeUtc(GalacticRecordsPGCsv) < DateTime.UtcNow.Subtract(TimeSpan.FromDays(7.0)))
                {
                    FetchFreshGalacticRecords(Constants.EDASTRO_GALACTIC_RECORDS_PG_CSV_URL, GalacticRecordsPGCsv);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger(ex, "Fetching updated records files from edastro.com");
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
            while (!requestTask.IsCompleted)
            {
                requestTask.Wait(500);
            }

            if (requestTask.IsFaulted)
            {
                ErrorLogger(requestTask.Exception, $"Error while refreshing {localCSVFilename}; using copy stale records for now...");
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
                using Stream csvFile = File.Create(localCSVFilename);
                response.Content.ReadAsStream().CopyTo(csvFile);
            }
            catch (Exception ex)
            {
                ErrorLogger(ex, $"While writing updated {localCSVFilename}");
                return false;
            }

            return true;
        }

        private StatScannerSettings MaybeFixUnsetSettings(StatScannerSettings newSettings)
        {
            newSettings.ForceUpdateGalacticRecords = ForceRefreshGalacticRecords;
            newSettings.OpenStatScannerWiki = OpenWikiUrl;

            // Maybe don't need this anymore?
            StatScannerSettings defaults = new();
            if (newSettings.HighCardinalityTieSuppression == 0) newSettings.HighCardinalityTieSuppression = defaults.HighCardinalityTieSuppression;
            newSettings.ProcGenHandling ??= defaults.ProcGenHandling;

            return newSettings;
        }

        private void OpenWikiUrl()
        {
            Misc.OpenUrl(Constants.STATSCANNER_WIKI_URL);
        }
    }
}
