using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryArchivist
{
    internal class CurrentSystemInfo
    {
        private VisitedSystem _internalData = null;
        private DateTime? _latestDateTime = null;
        private EntryDeduper _entryDeduper = new();

        public CurrentSystemInfo(FileHeaderInfo fileHeaderInfo, string systemName, UInt64 systemId64, DateTime firstVisit)
        {
            _internalData = new();
            _internalData.Commander = fileHeaderInfo.Commander;
            _internalData.PreambleJournalEntries = fileHeaderInfo.PreambleJournalEntries;
            _internalData.SystemName = systemName;
            _internalData.SystemId64 = systemId64;
            _internalData.FirstVisitDateTime = firstVisit;
            _internalData.VisitCount = 1;
        }

        public CurrentSystemInfo(VisitedSystem dataFromDb)
        {
            _internalData = dataFromDb;
            _entryDeduper = new(dataFromDb.SystemJournalEntries);
        }

        public string Commander { get => _internalData.Commander; }
        public List<string> PreambleJournalEntries { get => _internalData.PreambleJournalEntries; }
        public string SystemName { get => _internalData.SystemName; }
        public UInt64 SystemId64 { get => _internalData.SystemId64; }
        public DateTime VisitedDateTime { get => _internalData.FirstVisitDateTime; }
        public DateTime? LatestDateTime {
            get
            {
                if (SystemJournalEntries.Count == 0) return null;
                if (_latestDateTime == null)
                {
                    _latestDateTime = ExtractTimestamp(SystemJournalEntries[SystemJournalEntries.Count - 1]);
                }
                return _latestDateTime;
            }
        }

        public int VisitCount
        {
            get => _internalData.VisitCount;
            set => _internalData.VisitCount = value;
        }
        public List<string> SystemJournalEntries { get => _internalData.SystemJournalEntries; }

        public void AddSystemJournalJson(string jsonStr, DateTime? timestamp = null)
        {
            DateTime? latestTimestamp = timestamp ?? ExtractTimestamp(jsonStr);
            if (latestTimestamp != null && LatestDateTime != null && latestTimestamp <= LatestDateTime)
            {
                return; // Old or duplicate record.
            }

            if (_entryDeduper.IsThisADuplicate(jsonStr)) return;

            _internalData.SystemJournalEntries.Add(jsonStr);
            _latestDateTime = latestTimestamp;
        }

        private DateTime? ExtractTimestamp(string jsonStr)
        {
            DateTime? entryDateTime = null;
            using (var json = JsonDocument.Parse(jsonStr))
            {
                var entry = json.RootElement;
                entryDateTime = entry.GetProperty("timestamp").GetDateTime();
            }
            return entryDateTime;
        }

        public VisitedSystem ToSystemInfo()
        {
            return _internalData;
        }
    }
}
