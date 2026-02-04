using com.github.fredjk_gh.PluginCommon.Data.Id64;
using com.github.fredjk_gh.PluginCommon.Exceptions;
using com.github.fredjk_gh.PluginCommon.Utilities;
using LiteDB;
using System.Diagnostics;

namespace com.github.fredjk_gh.ObservatoryArchivist.DB
{
    internal class PositionCacheManager
    {
        private const string DB_NAME = "Archivist_PosCache";
        private const string POSITION_CACHE_DB_FILENAME = DB_NAME + ".db";
        /// <summary>
        /// Error logger; string param is an arbitrary context hint.
        /// </summary>
        private readonly Action<Exception, string> ErrorLogger;

        private ILiteDatabase ArchivistPositionCacheDB;
        private ILiteCollection<SystemInfo> PositionCacheTable;
        private readonly string _dbPath;
#if DEBUG
        private ConnectionMode _connectionMode = ConnectionMode.Shared;
#else
        private ConnectionMode _connectionMode = ConnectionMode.Direct;
#endif
        private HashSet<ulong> _knownSystems = [];
        private readonly Dictionary<ulong, SystemInfo> _deferredInserts = [];

        public PositionCacheManager(string pluginDataPath, Action<Exception, string> errorLogger)
        {
            ErrorLogger = errorLogger;
            _dbPath = $"{pluginDataPath}{POSITION_CACHE_DB_FILENAME}";

            Connect(_connectionMode);
            BatchModeProcessing = false;

            _knownSystems = GetAllKnownId64s();
        }

        public bool Connected { get => ArchivistPositionCacheDB != null; }

        public bool BatchModeProcessing { get; set; }

        public ConnectionMode CurrentConnectionMode { get => _connectionMode; }

        public void Connect(ConnectionMode newConnectionMode)
        {
            if (Connected)
            {
                if (newConnectionMode == CurrentConnectionMode) return; // nothing to do -- already connected in the desired mode.

                PositionCacheTable = null;
                ArchivistPositionCacheDB.Dispose();
                ArchivistPositionCacheDB = null;
                _connectionMode = newConnectionMode;
            }

            bool hasOpened = false;
            try
            {
                ArchivistPositionCacheDB = new LiteDatabase($"Filename={_dbPath};Connection={CurrentConnectionMode.ToString().ToLower()}");
                ArchivistPositionCacheDB.Pragma("UTC_DATE", true); // required to ensure dates read back from DB are still in UTC (to ensure old/dupe filtering works)
                hasOpened = true;

                PositionCacheTable = ArchivistPositionCacheDB.GetCollection<SystemInfo>("systemInfo");
                PositionCacheTable.EnsureIndex(sys => sys.Id64, true);
                PositionCacheTable.EnsureIndex(sys => sys.ProcGenName, true);
                PositionCacheTable.EnsureIndex(sys => sys.CommonName);
            }
            catch (Exception ex)
            {
                ErrorLogger(ex, $"While connecting to {DB_NAME} database (hasOpened? {hasOpened}). If 'hasOpened' is true, this could be a corrupt database or bad index and it may be deleted automatically. Please run restart Observatory and run Read-all!");
                Debug.WriteLine($"Failed to connect to {DB_NAME} database (hasOpened? {hasOpened})! {ex.Message}");

                // Make sure we appear NOT connected later on.
                PositionCacheTable = null;
                ArchivistPositionCacheDB?.Dispose();
                ArchivistPositionCacheDB = null;

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

        public void FinishReadAll()
        {
            // Flush anything in _deferredInserts
            UpsertBatch([.. _deferredInserts.Values]);
            _deferredInserts.Clear();
        }

        public long CountCachedSystems()
        {
            if (!Connected) throw new DBNotConnectedException(DB_NAME);

            return PositionCacheTable.LongCount();
        }

        public SystemInfo GetSystem(ulong id64)
        {
            if (!Connected) throw new DBNotConnectedException(DB_NAME);

            // Handle batch mode: return from _deferredInserts.
            if (BatchModeProcessing && _deferredInserts.TryGetValue(id64, out SystemInfo deferredSystem))
            {
                return deferredSystem;
            }
            return PositionCacheTable.FindOne(si => si.Id64 == id64);
        }

        public SystemInfo GetSystem(string name)
        {
            if (!Connected) throw new DBNotConnectedException(DB_NAME);

            // No optimization for this case -- This should be avoided during batch.

            return PositionCacheTable.FindOne(si => si.ProcGenName == name || si.CommonName == name);
        }

        public void UpsertSystem(SystemInfo system, bool forceUpdate = false)
        {
            // Only update what we aren't already aware of. This data doesn't change often and updates are costly.
            if ((_knownSystems.Contains(system.Id64) && !forceUpdate) || _deferredInserts.ContainsKey(system.Id64)) return;

            // Ensure SystemName is always the procgen name.
            Id64Details details = Id64Details.FromId64(system.Id64);
            string procGenName = details.ProcGenSystemName;

            // Check that SectorName is set to ensure we're not replacing with an incomplete/incorrect procgen name.
            if (details.SectorName != null && system.ProcGenName != procGenName)
            {
                system.ProcGenName = procGenName;
            }

            if (BatchModeProcessing)
            {
                // Just stash it (we've already checked it's not there).
                _deferredInserts[system.Id64] = system;
            }
            else
            {
                UpsertInternal(system, details, forceUpdate);
            }
        }

        public void UpsertBatch(List<SystemInfo> list)
        {
            Debug.Assert(!BatchModeProcessing, "Attempt to batch insert while in batch mode!");

            List<SystemInfo> filtered = [.. list.Where(si => !_knownSystems.Contains(si.Id64))];
            if (filtered.Count == 0) return;

            Debug.WriteLine($"Archivist[{DB_NAME}]: Upserting {filtered.Count} items...");
            int upsertResult = PositionCacheTable.Upsert(filtered);
            Debug.WriteLine($"Archivist[{DB_NAME}]: Upsert returned {upsertResult}.");
            _knownSystems = [.. _knownSystems.Union(filtered.Select(si => si.Id64))];
        }

        protected void UpsertInternal(SystemInfo system, Id64Details id64Details, bool forceUpdate = false)
        {
            SystemInfo existing = null;
            if (_knownSystems.Contains(system.Id64) && !forceUpdate) return; // avoid pointless updates
            if (forceUpdate)
                existing = PositionCacheTable.FindOne(si => si.Id64 == system.Id64);

            if (existing != null)
            {
                if (id64Details != null && id64Details.SectorName != null && existing.ProcGenName != id64Details.ProcGenSystemName)
                {
                    // This should be rare because it should have been stamped in when originally written.
                    // About the only cases this *could* happen are in newly discovered sectors which also have overrides.
                    Debug.WriteLine($"Archivist[{DB_NAME}]: Existing record unexpectedly did NOT have correct procGen name; fixing! Was {existing.ProcGenName}, corrected to {id64Details.ProcGenSystemName}.");
                    existing.ProcGenName = id64Details.ProcGenSystemName;
                }

                if (existing.x != system.x || existing.y != system.y || existing.z != system.z)
                {
                    Debug.WriteLine($"Archivist[{DB_NAME}]: System Coords do not match on update!");
                }

                PositionCacheTable.Upsert(existing);
            }
            else
            {
                PositionCacheTable.Upsert(system);
                _knownSystems.Add(system.Id64);
            }
        }

        protected HashSet<ulong> GetAllKnownId64s()
        {
            return [.. PositionCacheTable.FindAll().Select(s => s.Id64)];
        }
    }
}
