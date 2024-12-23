using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryArchivist
{
    internal class ArchivistData
    {
        private string _currentCommander = "";
        private Dictionary<string, ArchivistCommanderData> _knownCommanders = new();
        private VisitedSystem _lastSearchResult = null;
        private List<string> _recentSystems = new();

        public Dictionary<string, ArchivistCommanderData> KnownCommanders
        {
            get => _knownCommanders;
            set => _knownCommanders = value; // Only for deserialization.
        }

        [JsonIgnore]
        public FileHeader LastFileHeader { get; set; }

        [JsonIgnore]
        public VisitedSystem LastSearchResult { get; set; }

        public string CurrentCommander {
            get => _currentCommander;
            set {
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

            if (!_knownCommanders.ContainsKey(commander)) return null;

            return _knownCommanders[commander];
        }

        public void ResetForReadAll()
        {
            _knownCommanders.Clear();
            _lastSearchResult = null;
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
    }
}
