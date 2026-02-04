using com.github.fredjk_gh.ObservatoryArchivist.DB;
using Observatory.Framework;
using System.Text.Json;

namespace com.github.fredjk_gh.ObservatoryArchivist
{
    internal class CurrentSystemInfo
    {
        private readonly VisitedSystem _internalData;
        private readonly List<NotificationInfo> _internalNotifications;
        private DateTime? _latestDateTime;
        private readonly EntryDeduper _entryDeduper;

        private bool _isDirty;
        private bool _areNotifcationsDirty;

        public CurrentSystemInfo(FileHeaderInfo fileHeaderInfo, string systemName, UInt64 systemId64, DateTime firstVisit)
        {
            _internalData = new()
            {
                Commander = fileHeaderInfo.Commander,
                PreambleJournalEntries = fileHeaderInfo.PreambleJournalEntries,
                SystemName = systemName,
                SystemId64 = systemId64,
                FirstVisitDateTime = firstVisit,
                VisitCount = 1,
                LastVisitDateTime = firstVisit
            };
            _internalNotifications = [];
            _entryDeduper = new();
            _isDirty = true;
            _areNotifcationsDirty = true;
        }

        public CurrentSystemInfo(VisitedSystem dataFromDb, List<NotificationInfo> notifsFromDB)
        {
            _internalData = dataFromDb;
            _internalNotifications = notifsFromDB ?? [];
            _entryDeduper = new(dataFromDb.SystemJournalEntries, notifsFromDB);
        }

        public bool IsSystemInfoDirty { get => _isDirty; }
        public bool AreNotificationDirty { get => _areNotifcationsDirty; }

        public string Commander { get => _internalData.Commander; }
        public string SystemName { get => _internalData.SystemName; }
        public UInt64 SystemId64 { get => _internalData.SystemId64; }
        public DateTime FirstVisitedDateTime { get => _internalData.FirstVisitDateTime; }
        public DateTime? LatestSystemJournalDateTime
        {
            get
            {
                if (SystemJournalEntries.Count == 0) return null;
                return _latestDateTime ??= ExtractTimestamp(SystemJournalEntries[^1]);
            }
        }

        public int VisitCount
        {
            get => _internalData.VisitCount;
            set
            {
                _internalData.VisitCount = value;
                _isDirty = true;
            }
        }

        public DateTime LastVisitedDateTime
        {
            get => _internalData.LastVisitDateTime;
            set
            {
                _internalData.LastVisitDateTime = value;
                _isDirty = true;
            }
        }

        public List<string> PreambleJournalEntries { get => _internalData.PreambleJournalEntries; }

        public List<string> SystemJournalEntries { get => _internalData.SystemJournalEntries; }

        public bool AddSystemJournalJson(string jsonStr, DateTime? timestamp = null)
        {
            DateTime? latestTimestamp = timestamp ?? ExtractTimestamp(jsonStr);

            if (_entryDeduper.IsThisADuplicate(jsonStr)) return false;

            _internalData.SystemJournalEntries.Add(jsonStr);
            _latestDateTime = latestTimestamp;
            _isDirty = true;
            return true;
        }

        public List<NotificationInfo> Notifications { get => _internalNotifications; }

        public List<NotificationArgs> GetNotificationArgs()
        {
            List<NotificationArgs> rehydrated = [.. Notifications.Select(n => n.ToNotificationArgs())];
            return rehydrated;
        }

        public void AddNotificationArg(NotificationArgs n)
        {
            var data = NotificationInfo.FromNotificationArgs(SystemId64, SystemName, Commander, n);
            if (_entryDeduper.IsThisADuplicate(data))
                return;
            _internalNotifications.Add(data);
            _areNotifcationsDirty = true;
        }

        private DateTime? ExtractTimestamp(string jsonStr)
        {
            DateTime? entryDateTime;
            using (var json = JsonDocument.Parse(jsonStr))
            {
                var entry = json.RootElement;
                entryDateTime = entry.GetProperty("timestamp").GetDateTime();
            }
            return entryDateTime;
        }

        internal VisitedSystem ToVisitedSystem(bool forFlush = false)
        {
            if (forFlush) _isDirty = false;
            return _internalData;
        }

        internal List<NotificationInfo> ToNotificationInfo(bool forFlush = false)
        {
            if (forFlush)
            {
                _areNotifcationsDirty = false;
                return [.. _internalNotifications]; // Make a copy to avoid weird concurrent modification issues
            }
            return _internalNotifications;
        }
    }
}
