namespace com.github.fredjk_gh.PluginCommon.Data.Journals
{
    public class SRVs
    {
        private const string SRV_SCARAB_ID = "testbuggy";
        private const string SRV_SCORIPION_ID = "combat_multicrew_srv_01";
        private const string SRV_RHINO_ID = "mev_rhino";
        private const string SRV_NOMAD_ID = "lander01";

        private static Dictionary<string, Details> _SRVDetailsByJournalId = new()
        {
            { SRV_SCARAB_ID, new(Type.Scarab, SRV_SCARAB_ID, "Scarab", 4) },
            { SRV_SCORIPION_ID, new(Type.Scorpion, SRV_SCORIPION_ID, "Scorpion", 2) },
            { SRV_RHINO_ID, new(Type.Rhino, SRV_RHINO_ID, "Rhino", 72) },
            { SRV_NOMAD_ID, new(Type.Nomad, SRV_NOMAD_ID, "Nomad", 0) },
        };

        public enum Type
        {
            Scarab,
            Scorpion,
            Rhino,
            Nomad,
        }

        public static Details GetSRVDetails(string journalId)
        {
            if (string.IsNullOrWhiteSpace(journalId))
                return _SRVDetailsByJournalId[SRV_SCARAB_ID]; // this was the only option prior to the addition of SRVType.
            if (_SRVDetailsByJournalId.TryGetValue(journalId, out Details details))
            {
                return details;
            }
            return null;
        }

        public static Details GetSRVDetails(Type type)
        {
            return _SRVDetailsByJournalId.Values.Where(d => d.Type == type).FirstOrDefault();
        }

        public class Details
        {
            public Details(Type type, string journalId, string name, int maxCargo)
            {
                Type = type;
                JournalId = journalId;
                Name = name;
                MaxCargo = maxCargo;
            }

            public Type Type { get; }
            public string JournalId { get; }
            public string Name { get; }
            public int MaxCargo { get; }
        }
    }
}
