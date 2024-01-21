using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryArchivist
{
    internal class VisitedSystem
    {
        public VisitedSystem()
        {
            PreambleJournalEntries = new();
            SystemJournalEntries = new();
        }

        public long _id { get; set; }
        public string Commander {  get; set; }
        public string SystemName { get; set; }
        public UInt64 SystemId64 { get; set; }
        public DateTime FirstVisitDateTime { get; set; }
        public int VisitCount { get; set; }
        public List<string> PreambleJournalEntries { get; set; }

        public List<string> SystemJournalEntries { get; set; }
    }
}
