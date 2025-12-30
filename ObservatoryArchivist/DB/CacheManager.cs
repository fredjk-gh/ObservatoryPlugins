using com.github.fredjk_gh.PluginCommon.Data.Id64;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace com.github.fredjk_gh.ObservatoryArchivist.DB
{
    internal class CacheManager
    {
        private const string CACHE_DB_NAME = "Cache";
        private const string CACHE_DB_FILENAME = "Archivist_Cache.db";
        /// <summary>
        /// Error logger; string param is an arbitrary context hint.
        /// </summary>
        private Action<Exception, string> ErrorLogger;

        private ILiteDatabase ArchivistCacheDB;
        private ILiteCollection<SystemInfo> CachedSystemsCol;
        private string dbPath;
#if DEBUG
        private ConnectionMode connectionMode = ConnectionMode.Shared;
#else
        private ConnectionMode connectionMode = ConnectionMode.Direct;
#endif
        private HashSet<ulong> _knownSystems = new();
        private Dictionary<ulong, SystemInfo> _deferredInserts = new();

        public CacheManager(string pluginDataPath, Action<Exception, string> errorLogger)
        {
            ErrorLogger = errorLogger;
            dbPath = $"{pluginDataPath}{CACHE_DB_FILENAME}";

            Connect(connectionMode);
            BatchModeProcessing = false;

            _knownSystems = GetAllKnownId64s();
        }

        public bool Connected { get => ArchivistCacheDB != null; }

        public bool BatchModeProcessing { get; set; }

        public ConnectionMode CurrentConnectionMode { get => connectionMode; }

        public void Connect(ConnectionMode newConnectionMode)
        {
            if (Connected)
            {
                if (newConnectionMode != CurrentConnectionMode)
                {
                    CachedSystemsCol = null;
                    ArchivistCacheDB.Dispose();
                    ArchivistCacheDB = null;
                }
                else
                {
                    return; // nothing to do -- already connected in the desired mode.
                }
            }

            ArchivistCacheDB = new LiteDatabase($"Filename={dbPath};Connection={(CurrentConnectionMode == ConnectionMode.Direct ? "direct" : "shared")}");
            ArchivistCacheDB.Pragma("UTC_DATE", true); // required to ensure dates read back from DB are still in UTC (to ensure old/dupe filtering works)

            CachedSystemsCol = ArchivistCacheDB.GetCollection<SystemInfo>("systemInfo");
            CachedSystemsCol.EnsureIndex(sys => sys.Id64, true);
            CachedSystemsCol.EnsureIndex(sys => sys.ProcGenName, true);
            CachedSystemsCol.EnsureIndex(sys => sys.CommonName);
        }

        public void FinishReadAll()
        {
            // Flush anything in _deferredInserts
            UpsertBatch(_deferredInserts.Values.ToList());
            _deferredInserts.Clear();
        }

        public long CountCachedSystems()
        {
            if (!Connected) throw new DBNotConnectedException(CACHE_DB_NAME);

            return CachedSystemsCol.LongCount();
        }

        public SystemInfo GetSystem(ulong id64)
        {
            if (!Connected) throw new DBNotConnectedException(CACHE_DB_NAME);

            // Handle batch mode: return from _deferredInserts.
            if (BatchModeProcessing && _deferredInserts.ContainsKey(id64))
            {
                return _deferredInserts[id64];
            }
            return CachedSystemsCol.FindOne(si => si.Id64 == id64);
        }

        public SystemInfo GetSystem(string name)
        {
            if (!Connected) throw new DBNotConnectedException(CACHE_DB_NAME);

            // No optimization for this case -- This should be avoided during batch.

            return CachedSystemsCol.FindOne(si => si.ProcGenName == name || si.CommonName == name);
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

            List<SystemInfo> filtered = list.Where(si => !_knownSystems.Contains(si.Id64)).ToList();
            Debug.WriteLine($"Upserting {filtered.Count} items...");
            int upsertResult = CachedSystemsCol.Upsert(filtered);
            Debug.WriteLine($"Upsert returned {upsertResult}.");
            _knownSystems = _knownSystems.Union(filtered.Select(si => si.Id64)).ToHashSet();
        }

        protected void UpsertInternal(SystemInfo system, Id64Details id64Details, bool forceUpdate = false)
        {
            SystemInfo existing = null;
            if (_knownSystems.Contains(system.Id64) && !forceUpdate) return; // avoid pointless updates (particularly if this was part
            if (forceUpdate)
                existing = CachedSystemsCol.FindOne(si => si.Id64 == system.Id64);

            if (existing != null)
            {
                if (id64Details != null && id64Details.SectorName != null && existing.ProcGenName != id64Details.ProcGenSystemName)
                {
                    // This should be rare because it should have been stamped in when originally written.
                    // About the only cases this *could* happen are in newly discovered sectors which also have overrides.
                    Debug.WriteLine($"Archivist[Cache]: Existing record unexpectedly did NOT have correct procGen name; fixing! Was {existing.ProcGenName}, corrected to {id64Details.ProcGenSystemName}.");
                    existing.ProcGenName = id64Details.ProcGenSystemName;
                }

                if (existing.x != system.x || existing.y != system.y || existing.z != system.z)
                {
                    Debug.WriteLine("Archivist[Cache]: System Coords do not match on update!");
                }

                CachedSystemsCol.Upsert(existing);
            }
            else
            {
                CachedSystemsCol.Upsert(system);
                _knownSystems.Add(system.Id64);
            }
        }

        protected HashSet<ulong> GetAllKnownId64s()
        {
            return CachedSystemsCol.FindAll().Select(s => s.Id64).ToHashSet();
        }
    }
}
