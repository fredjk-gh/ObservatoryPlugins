using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryArchivist
{
    internal class ArchivistCommanderData
    {
        private CurrentSystemInfo _currentSystemInfo = null;
        private string _currentSystemName = "";
        private List<string> _recentSystems = new();

        public ArchivistCommanderData()
        {
            FileHeaderInfo = new();
        }

        public string CommanderName {
            get => FileHeaderInfo.Commander;
            set =>  FileHeaderInfo.Commander = value;
        }

        public string CurrentSystemName
        {
            get
            {
                return CurrentSystem == null ? _currentSystemName : _currentSystemName = CurrentSystem.SystemName;
            }
            set
            {
                // Used only for deserialization.
                MaybeAddToRecentSystems(value);
                _currentSystemName = value;
            }
        }

        public FileHeaderInfo FileHeaderInfo { get; set; }

        [JsonIgnore]
        public CurrentSystemInfo CurrentSystem
        {
            get => _currentSystemInfo;
            set
            {
                MaybeAddToRecentSystems(value?.SystemName);
                _currentSystemInfo = value;
            }
        }

        [JsonIgnore]
        public List<string> RecentSystems {
            get => _recentSystems;
        }

        private void MaybeAddToRecentSystems(string? systemName)
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
