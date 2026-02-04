using com.github.fredjk_gh.PluginCommon.Exceptions;
using com.github.fredjk_gh.PluginCommon.Utilities;
using LiteDB;
using System.Diagnostics;

namespace com.github.fredjk_gh.ObservatoryArchivist.DB
{
    internal class JournalManager
    {
        private const string DB_NAME = "Archivist_Main";
        private const string ARCHIVE_DB_FILENAME = DB_NAME + ".db";
        /// <summary>
        /// Error logger; string param is an arbitrary context hint.
        /// </summary>
        private readonly Action<Exception, string> ErrorLogger;

        private ILiteDatabase ArchivistMainDB;
        private ILiteCollection<VisitedSystem> VisitedSystemsTable;
        private ILiteCollection<VisitedSystem> AugmentedSystemsTable;
        private readonly string _dbPath;
#if DEBUG
        private ConnectionMode connectionMode = ConnectionMode.Shared;
#else
        private ConnectionMode connectionMode = ConnectionMode.Direct;
#endif

        private readonly Dictionary<Tuple<string, string>, VisitedSystem> _deferredChanges = [];


        public JournalManager(string pluginDataPath, Action<Exception, string> errorLogger)
        {
            ErrorLogger = errorLogger;
            _dbPath = $"{pluginDataPath}{ARCHIVE_DB_FILENAME}";

            Connect(connectionMode);
            BatchModeProcessing = false;
        }

        public void Connect(ConnectionMode newConnectionMode)
        {
            if (Connected)
            {
                if (newConnectionMode == CurrentConnectionMode) return; // nothing to do -- already connected in the desired mode.

                VisitedSystemsTable = null;
                AugmentedSystemsTable = null;
                ArchivistMainDB.Dispose();
                ArchivistMainDB = null;
                connectionMode = newConnectionMode;
            }

            bool hasOpened = false;
            string status = "...opening DB... ";
            try
            {
                ArchivistMainDB = new LiteDatabase($"Filename={_dbPath};Connection={CurrentConnectionMode.ToString().ToLower()}");
                ArchivistMainDB.Pragma("UTC_DATE", true); // required to ensure dates read back from DB are still in UTC (to ensure old/dupe filtering works)

                VisitedSystemsTable = ArchivistMainDB.GetCollection<VisitedSystem>("visitedSystems");
                AugmentedSystemsTable = ArchivistMainDB.GetCollection<VisitedSystem>("augmentedSystems");
                hasOpened = true;

                status = "...setting up VisitedSystemsTable...";
                // https://github.com/mbdavid/LiteDB/issues/739 - fake multi-column unique key.
                VisitedSystemsTable.EnsureIndex("unique_cmdr_sysname", "{cmdr:$.Commander, sysname:$.SystemName}", true);
                VisitedSystemsTable.EnsureIndex(sys => sys.SystemId64);
                VisitedSystemsTable.EnsureIndex(sys => sys.SystemName);
                VisitedSystemsTable.EnsureIndex(sys => sys.FirstVisitDateTime);
                VisitedSystemsTable.EnsureIndex(sys => sys.LastVisitDateTime);
                VisitedSystemsTable.EnsureIndex(sys => sys.Commander);

                status = "...setting up AugmentedSystemsTable...";
                // https://github.com/mbdavid/LiteDB/issues/739 - fake multi-column unique key.
                AugmentedSystemsTable.EnsureIndex("unique_cmdr_sysname_a", "{cmdr:$.Commander, sysname:$.SystemName}", true);
                AugmentedSystemsTable.EnsureIndex(sys => sys.SystemId64);
                AugmentedSystemsTable.EnsureIndex(sys => sys.SystemName);
                AugmentedSystemsTable.EnsureIndex(sys => sys.FirstVisitDateTime);
                AugmentedSystemsTable.EnsureIndex(sys => sys.LastVisitDateTime);
                AugmentedSystemsTable.EnsureIndex(sys => sys.Commander);
            }
            catch (Exception ex)
            {
                ErrorLogger(ex, $"While connecting to {DB_NAME} database {status}(hasOpened? {hasOpened}). If 'hasOpened' is true, this could be a corrupt database and it may be deleted automatically. Please run restart Observatory and run Read-all!");
                Debug.WriteLine($"Failed to connect to {DB_NAME} database {status}(hasOpened? {hasOpened})! {ex.Message}");

                // Make sure we appear NOT connected later on.
                VisitedSystemsTable = null;
                AugmentedSystemsTable = null;
                ArchivistMainDB?.Dispose();
                ArchivistMainDB = null;

                if (hasOpened)
                {
                    // Failed creating indexes -- likely the unique ones. Torch the file and request a read-all.
                    // Consider just copying this out to a graveyard for analysis?
                    Debug.WriteLine($"Deleting {_dbPath}... We believe it is corrupted.");
                    File.Delete(_dbPath);
                    throw new DBCorruptedException(DB_NAME, ex);
                }
                else throw new DBNotConnectedException(DB_NAME, ex);
            }
        }

        public bool Connected { get => ArchivistMainDB is not null; }

        public bool BatchModeProcessing { get; set; }

        public ConnectionMode CurrentConnectionMode { get => connectionMode; }

        public int CountVisitedSystems(string commanderName = "")
        {
            if (!Connected) throw new DBNotConnectedException(DB_NAME);

            if (string.IsNullOrEmpty(commanderName))
            {
                return VisitedSystemsTable.Count();
            }
            else
            {
                return VisitedSystemsTable.Count(sys => sys.Commander == commanderName);
            }
        }

        public List<BsonDocument> GetVisitedSystemSummary()
        {
            var result = VisitedSystemsTable.Query().GroupBy("Commander")
                .Select("{ Cmdr:@key, SystemCount:COUNT(*)  }")
                .ToList();
            result.AddRange(
                AugmentedSystemsTable.Query()
                    .Select("{ Cmdr:'Augmented', SystemCount:COUNT(*) }").ToList());
            return result;
        }

        // Primarily for pre-loading
        public VisitedSystem GetVisitedSystem(string systemName, string commanderName)
        {
            if (!Connected) throw new DBNotConnectedException(DB_NAME);

            //var bsonKeyDoc = new BsonDocument() { ["cmdr"] = commanderName, ["sysname"] = systemName };
            //var bsonExpr = Query.EQ("unique_cmdr_sysname", bsonKeyDoc);
            //return VisitedSystemsCol.FindOne(bsonExpr);
            if (BatchModeProcessing)
            {
                var key = new Tuple<string, string>(commanderName, systemName);
                if (_deferredChanges.TryGetValue(key, out VisitedSystem system))
                    return system;
                return null;
            }
            else
                return VisitedSystemsTable.FindOne(sys => sys.SystemName == systemName && sys.Commander == commanderName);
        }

        public VisitedSystem GetVisitedSystem(ulong id64, string commanderName)
        {
            if (!Connected) throw new DBNotConnectedException(DB_NAME);

            Debug.Assert(!BatchModeProcessing);

            //var bsonKeyDoc = new BsonDocument() { ["cmdr"] = commanderName, ["sysname"] = systemName };
            //var bsonExpr = Query.EQ("unique_cmdr_sysname", bsonKeyDoc);
            //return VisitedSystemsCol.FindOne(bsonExpr);

            return VisitedSystemsTable.FindOne(sys => sys.SystemId64 == id64 && sys.Commander == commanderName);
        }

        public List<VisitedSystem> GetVisitedSystems(string systemName, string commanderName = "")
        {
            if (!Connected) throw new DBNotConnectedException(DB_NAME);
            if (BatchModeProcessing) return []; // Shouldn't be possible, but the database is likely to be empty.

            if (!string.IsNullOrEmpty(commanderName))
                return [.. VisitedSystemsTable
                    .Find(sys => sys.SystemName == systemName && sys.Commander == commanderName)
                    .OrderByDescending(sys => sys.SystemJournalEntries.Count)];
            else
                return [.. VisitedSystemsTable
                    .Find(sys => sys.SystemName == systemName)
                    .OrderByDescending(sys => sys.SystemJournalEntries.Count)];
        }

        public List<VisitedSystem> GetVisitedSystems(ulong id64, string commanderName = "")
        {
            if (!Connected) throw new DBNotConnectedException(DB_NAME);
            if (BatchModeProcessing) return []; // Shouldn't be possible, but the database is likely to be empty.

            if (!string.IsNullOrEmpty(commanderName))
                return [.. VisitedSystemsTable
                    .Find(sys => sys.SystemId64 == id64 && sys.Commander == commanderName)
                    .OrderByDescending(sys => sys.SystemJournalEntries.Count)];
            else
                return [.. VisitedSystemsTable
                    .Find(sys => sys.SystemId64 == id64)
                    .OrderByDescending(sys => sys.SystemJournalEntries.Count)];
        }

        // For auto-complete droplist.
        public List<string> FindVisitedSystemNames(string substring, string commanderName = "")
        {
            if (!string.IsNullOrEmpty(commanderName))
                return [.. VisitedSystemsTable
                    .Find(sys => sys.SystemName.Contains(substring) && sys.Commander == commanderName)
                    .OrderByDescending(sys => sys.FirstVisitDateTime)
                    .Select(sys => sys.SystemName)
                    .Distinct()
                    .Take(20)];
            else
                return [.. VisitedSystemsTable
                    .Find(sys => sys.SystemName.Contains(substring))
                    .OrderByDescending(sys => sys.FirstVisitDateTime)
                    .Select(sys => sys.SystemName)
                    .Distinct()
                    .Take(20)];
        }

        public List<string> GetRecentVisitedsystems()
        {
            return [.. VisitedSystemsTable
                .FindAll()
                .OrderByDescending(sys => sys.LastVisitDateTime)
                .ThenByDescending(sys => sys.FirstVisitDateTime)
                .Select(sys => sys.SystemName)
                .Distinct()
                .Take(25)];
        }

        public void UpsertVisitedSystem(VisitedSystem system)
        {
            if (system.PreambleJournalEntries.Count == 0 || system.SystemJournalEntries.Count == 0)
                return;

            VisitedSystemsTable.Upsert(system);
        }

        public void ClearAll()
        {
            VisitedSystemsTable.DeleteAll();
        }

        public void UpsertVisitedSystemData(VisitedSystem visited)
        {
            if (visited == null) return;

            if (BatchModeProcessing)
            {
                if (visited.SystemJournalEntries.Count > 0 || visited.PreambleJournalEntries.Count > 0)
                    _deferredChanges[new(visited.Commander, visited.SystemName)] = visited;
            }
            else
            {
                UpsertVisitedSystem(visited);
            }
        }

        public void FinishReadAll()
        {
            if (_deferredChanges.Count == 0) return;

            VisitedSystemsTable.Upsert(_deferredChanges.Values);
            _deferredChanges.Clear();
        }

        public void UpsertAugmentedSystem(VisitedSystem system)
        {
            if (system.PreambleJournalEntries.Count == 0 || system.SystemJournalEntries.Count == 0)
                return;

            var existing = GetExactMatchAugmentedSystem(system.SystemId64);

            if (existing != null)
                AugmentedSystemsTable.Delete(existing._id);

            AugmentedSystemsTable.Upsert(system);
        }

        public VisitedSystem? GetExactMatchAugmentedSystem(ulong id64)
        {
            if (!Connected) throw new DBNotConnectedException(DB_NAME);
            if (BatchModeProcessing) return new(); // Shouldn't be possible, but the database is likely to be empty.

            // We don't discriminate between commanders here.
            return AugmentedSystemsTable
                .Find(sys => sys.SystemId64 == id64)
                .FirstOrDefault();
        }
    }
}
