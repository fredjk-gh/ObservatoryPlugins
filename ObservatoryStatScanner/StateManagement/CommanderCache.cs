using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryStatScanner.StateManagement
{
    internal class CommanderCache
    {
        private string _currentSystem = "";
        private Dictionary<int, Scan> _scans = new();

        public string FID { get; set; }
        public string Name { get; set; }
        public string CurrentSystem
        {
            get => _currentSystem;
            set
            {
                _currentSystem = value;

                _scans.Clear();
                SystemBodyCount = 0;
                NavBeaconScanned = false;
            }
        }
        public bool IsOdyssey { get; set; }
        public bool ReadAllSinceFirstSeen { get; set; }

        [JsonIgnore]
        public LoadGame LastLoadGame { get; set; }
        [JsonIgnore]
        public Statistics LastStatistics { get; set; }

        [JsonIgnore]
        public Dictionary<int, Scan> Scans { get => _scans; }
        [JsonIgnore]
        public int SystemBodyCount { get; internal set; }
        [JsonIgnore]
        public bool NavBeaconScanned { get; internal set; }
    }
}
