namespace com.github.fredjk_gh.ObservatoryArchivist.DB
{
    internal class VisitedSystem
    {
        public VisitedSystem()
        {
            PreambleJournalEntries = [];
            SystemJournalEntries = [];
        }

        public long _id { get; set; }
        public string Commander { get; set; }
        public string SystemName { get; set; }
        public UInt64 SystemId64 { get; set; }
        public DateTime FirstVisitDateTime { get; set; }
        public int VisitCount { get; set; }
        public DateTime LastVisitDateTime { get; set; }
        public List<string> PreambleJournalEntries { get; set; }
        public List<string> SystemJournalEntries { get; set; }
    }
}
