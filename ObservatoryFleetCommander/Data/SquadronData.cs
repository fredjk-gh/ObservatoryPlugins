using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.ObservatoryFleetCommander.Data
{
    public class SquadronData
    {
        private HashSet<string> _knownMemberCommanderNames = [];

        // Only to be used for JSON Deserialization.
        public SquadronData() { }

        public SquadronData(ulong id, string name)
        {
            ID = id;
            Name = name;
        }

        public ulong ID { get; set; } // Turns out this was not always set and is thus not a good key.
        public string Name { get; set; }
        public string Tag { get; set; } // Not known through squadron Startup; learned via Squadron CarrierStats.Name or Callsign.
        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrEmpty(Tag)) return Tag;
                if (!string.IsNullOrEmpty(Name)) return Name;
                return $"{ID}";
            }
        }
        public HashSet<string> MemberCommanderNames
        {
            get => _knownMemberCommanderNames;
            internal set => _knownMemberCommanderNames = value; // only for deserialization
        }
        public ulong CarrierId
        {
            get => Carrier?.CarrierId ?? 0;
        }

        [JsonIgnore]
        public bool HasCarrier
        {
            get => Carrier is not null;
        }

        public CarrierData Carrier { get; set; }
    }
}
