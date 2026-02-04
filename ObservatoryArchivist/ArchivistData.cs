using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.ObservatoryArchivist
{
    internal class ArchivistData
    {
        private readonly HashSet<ulong> _navBeaconSystemId64s = [];
        private string _currentCommander = "";
        private Dictionary<string, ArchivistCommanderData> _knownCommanders = [];
        private List<string> _recentSystems = [];

        public Dictionary<string, ArchivistCommanderData> KnownCommanders
        {
            get => _knownCommanders;
            set => _knownCommanders = value; // Only for deserialization.
        }

        [JsonIgnore]
        public FileHeader LastFileHeader { get; set; }

        public string CurrentCommander
        {
            get => _currentCommander;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) return;

                // Initialize new Cmdr entry if needed.
                if (!_knownCommanders.ContainsKey(value))
                    _knownCommanders.Add(value, new()
                    {
                        CommanderName = value,
                    });
                _currentCommander = value;
            }
        }

        public ArchivistCommanderData ForCommander(string cmdrName = "")
        {
            var commander = string.IsNullOrWhiteSpace(cmdrName) ? CurrentCommander : cmdrName;

            if (!_knownCommanders.TryGetValue(commander, out ArchivistCommanderData cmdrData)) return null;

            return cmdrData;
        }

        public void ResetForReadAll()
        {
            _knownCommanders.Clear();
            _recentSystems.Clear();
        }

        public List<string> RecentSystems
        {
            get => _recentSystems;
            internal set => _recentSystems = value;
        }

        public void MaybeAddToRecentSystems(string? systemName)
        {
            if (!string.IsNullOrWhiteSpace(systemName))
            {
                // De-dupe by removing the item and re-inserting it at the top of the list.
                _recentSystems.Remove(systemName);
                _recentSystems.Insert(0, systemName);
                if (_recentSystems.Count > 25)
                {
                    _recentSystems.RemoveRange(25, _recentSystems.Count - 25);
                }
            }
        }

        public static bool IsSystemScanComplete(List<JournalBase> systemJournals)
        {
            // Santy check the System Journal Entries:
            // - Do we have have either a NavBeacon or discovery scan with body count? If not, bail.
            // - Do we have a scan for the correct # all of the bodies? If not, bail.
            int numBodies = -1;
            HashSet<int> uniqueBodies = [];
            foreach (JournalBase j in systemJournals)
            {
                switch (j)
                {
                    // The number of bodies returned here appears to be sometimes incorrect! See Subra and Soyota
                    //case NavBeaconScan beaconScan:
                    //    numBodies = beaconScan.NumBodies;
                    //    break;
                    case FSSDiscoveryScan honk:
                        numBodies = honk.BodyCount;
                        break;
                    case Scan scan:
                        if (!string.IsNullOrEmpty(scan.PlanetClass) || !string.IsNullOrEmpty(scan.StarType))
                        {
                            uniqueBodies.Add(scan.BodyID);
                        }
                        break;
                }
            }

            if (numBodies <= 0) return false;
            if (uniqueBodies.Count < numBodies) return false;

            // Ok, seems complete enough...
            return true;
        }

        public void TrackSystemNavBeacon(ulong systemAddr)
        {
            _navBeaconSystemId64s.Add(systemAddr);
        }

        public bool SystemHasNavBeacon(ulong systemAddr)
        {
            return _navBeaconSystemId64s.Contains(systemAddr);
        }


        public static List<JournalBase> ToJournalObj(IObservatoryCore core, List<string> journalEntries)
        {
            List<JournalBase> journalObj = [];

            foreach (string journalEntry in journalEntries)
            {
                var args = core.DeserializeEvent(journalEntry);
                if (args.journalEvent is JournalBase entry)
                {
                    journalObj.Add(entry);
                }
            }

            return journalObj;
        }

    }
}
