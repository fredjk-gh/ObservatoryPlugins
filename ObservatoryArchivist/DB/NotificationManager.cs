using com.github.fredjk_gh.PluginCommon.Exceptions;
using com.github.fredjk_gh.PluginCommon.Utilities;
using LiteDB;
using System.Diagnostics;

namespace com.github.fredjk_gh.ObservatoryArchivist.DB
{
    internal class NotificationManager
    {
        private const string DB_NAME = "Archivist_Notifications";
        private const string NOTIFICATION_DB_FILENAME = DB_NAME + ".db";
        /// <summary>
        /// Error logger; string param is an arbitrary context hint.
        /// </summary>
        private readonly Action<Exception, string> ErrorLogger;

        private ILiteDatabase NotificationDB;
        private ILiteCollection<NotificationInfo> NotificationsTable;
        private readonly string _dbPath;
#if DEBUG
        private ConnectionMode _connectionMode = ConnectionMode.Shared;
#else
        private ConnectionMode _connectionMode = ConnectionMode.Direct;
#endif
        private readonly Dictionary<ulong, List<NotificationInfo>> _deferredInserts = [];

        public NotificationManager(string pluginDataPath, Action<Exception, string> errorLogger)
        {
            ErrorLogger = errorLogger;
            _dbPath = $"{pluginDataPath}{NOTIFICATION_DB_FILENAME}";

            Connect(_connectionMode);
            BatchModeProcessing = false;
        }

        public bool Connected { get => NotificationDB != null; }

        public bool BatchModeProcessing { get; set; }

        public ConnectionMode CurrentConnectionMode { get => _connectionMode; }

        public void Connect(ConnectionMode newConnectionMode)
        {
            if (Connected)
            {
                if (newConnectionMode == CurrentConnectionMode) return; // nothing to do -- already connected in the desired mode.

                NotificationsTable = null;
                NotificationDB.Dispose();
                NotificationDB = null;
                _connectionMode = newConnectionMode;
            }

            bool hasOpened = false;
            try
            {
                NotificationDB = new LiteDatabase($"Filename={_dbPath};Connection={CurrentConnectionMode.ToString().ToLower()}");
                NotificationDB.Pragma("UTC_DATE", true); // required to ensure dates read back from DB are still in UTC (to ensure old/dupe filtering works)

                NotificationsTable = NotificationDB.GetCollection<NotificationInfo>("notificationInfo");
                hasOpened = true;

                // No opportunity for a unique index here. Craft your queries carefully.
                // Detail, extended detail columns have NO index, those should be filtered client-side.
                NotificationsTable.EnsureIndex("cmdr_id64", "{cmdr:$.Commander, id64:$.SystemId64}", false);
                NotificationsTable.EnsureIndex("cmdr_sysname", "{cmdr:$.Commander, id64:$.SystemName}", false);
                NotificationsTable.EnsureIndex(n => n.SystemId64, false);
                NotificationsTable.EnsureIndex(n => n.SystemName, false);
                NotificationsTable.EnsureIndex(n => n.Commander, false);
                NotificationsTable.EnsureIndex(n => n.Title, false);
                NotificationsTable.EnsureIndex(n => n.Sender, false);
            }
            catch (Exception ex)
            {
                ErrorLogger(ex, $"While connecting to {DB_NAME} database (hasOpened? {hasOpened}). If 'hasOpened' is true, this could be a corrupt database or bad index and it may be deleted automatically. Please run restart Observatory and run Read-all!");
                Debug.WriteLine($"Failed to connect to DB {DB_NAME} database (hasOpened? {hasOpened})! {ex.Message}");

                // Make sure we appear NOT connected later on.
                NotificationsTable = null;
                NotificationDB?.Dispose();
                NotificationDB = null;

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
            UpsertBatch([.. _deferredInserts.Values.SelectMany(l => l)]);
            _deferredInserts.Clear();
        }

        public void ClearAll()
        {
            NotificationsTable.DeleteAll();
        }

        public long CountAll()
        {
            if (!Connected) throw new DBNotConnectedException(DB_NAME);

            return NotificationsTable.LongCount();
        }

        public long CountSystemsWithNotifications(string forCommander = "")
        {
            if (!Connected) throw new DBNotConnectedException(DB_NAME);

            if (!string.IsNullOrEmpty(forCommander))
                return NotificationsTable
                    .Find(n => n.Commander == forCommander)
                    .Select(n => n.SystemName)
                    .Distinct()
                    .LongCount();
            else
                return NotificationsTable
                    .Query()
                    .Select(n => new { n.Commander, n.SystemName, }).ToEnumerable()
                    .Distinct()
                    .LongCount();
        }

        public List<NotificationInfo> GetNotifications(ulong id64, string commander)
        {
            if (!Connected) throw new DBNotConnectedException(DB_NAME);

            // Handle batch mode: return from _deferredInserts.
            if (BatchModeProcessing && _deferredInserts.TryGetValue(id64, out List<NotificationInfo> deferredNotifs))
            {
                return deferredNotifs;
            }
            return [.. NotificationsTable.Find(si => si.SystemId64 == id64 && si.Commander == commander)];
        }

        public List<NotificationInfo> GetNotifications(string sysName, string commander)
        {
            if (!Connected) throw new DBNotConnectedException(DB_NAME);

            return [.. NotificationsTable.Find(si => si.SystemName == sysName && si.Commander == commander)];
        }

        public Dictionary<string, List<NotificationInfo>> FindNotifications(string sysName, string cmdrName = "")
        {
            if (!Connected) throw new DBNotConnectedException(DB_NAME);

            if (!string.IsNullOrWhiteSpace(cmdrName))
                return NotificationsTable
                    .Find(n => n.SystemName == sysName && n.Commander == cmdrName)
                    .GroupBy(n => n.Commander)
                    .ToDictionary(g => g.Key, g => g.ToList());
            else
                return NotificationsTable
                    .Find(n => n.SystemName == sysName)
                    .GroupBy(n => n.Commander)
                    .ToDictionary(g => g.Key, g => g.ToList());
        }

        public Dictionary<string, List<NotificationInfo>> FindNotifications(ulong id64, string cmdrName = "")
        {
            if (!Connected) throw new DBNotConnectedException(DB_NAME);

            if (!string.IsNullOrWhiteSpace(cmdrName))
                return NotificationsTable
                    .Find(n => n.SystemId64 == id64 && n.Commander == cmdrName)
                    .GroupBy(n => n.Commander)
                    .ToDictionary(g => g.Key, g => g.ToList());

            else
                return NotificationsTable
                    .Find(n => n.SystemId64 == id64)
                    .GroupBy(n => n.Commander)
                    .ToDictionary(g => g.Key, g => g.ToList());

        }

        public void UpsertNotifications(List<NotificationInfo> notifications)
        {
            // Don't save an empty set.
            if (notifications is null || notifications.Count == 0) return;

            if (BatchModeProcessing)
            {
                // Just stash insert/update it for later.
                try
                {
                    foreach (var n in notifications)
                    {
                        if (!_deferredInserts.ContainsKey(n.SystemId64))
                            _deferredInserts.Add(n.SystemId64, []);

                        _deferredInserts[n.SystemId64].Add(n);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Fail(ex.ToString());
                }
            }
            else
            {
                UpsertInternal(notifications);
            }
        }

        public void UpsertBatch(List<NotificationInfo> list)
        {
            NotificationsTable.Upsert(list);
        }

        protected void UpsertInternal(List<NotificationInfo> notifications)
        {
            // Assume that things are already de-duped and if this is a modification of an existing item, it will have
            // an _id set and will get updated, otherwise upserted.
            NotificationsTable.Upsert(notifications);
        }
    }
}
