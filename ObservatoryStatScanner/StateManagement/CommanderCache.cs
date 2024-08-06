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

        public string FID { get; set; }
        public string Name { get; set; }
        public string CurrentSystem { get; set; }
        public bool IsOdyssey { get; set; }
        public bool ReadAllSinceFirstSeen { get; set; }

        [JsonIgnore]
        public LoadGame LastLoadGame { get; set; }
        [JsonIgnore]
        public Statistics LastStatistics { get; set; }
    }
}
