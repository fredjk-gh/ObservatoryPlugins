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

        public Dictionary<string, ArchivistCommanderData> KnownCommanders
        {
            get => _knownCommanders;
            set => _knownCommanders = value; // Only for deserialization.
        }

        [JsonIgnore]
        public FileHeader LastFileHeader { get; set; }

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
        }
    }
}
