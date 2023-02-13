using ObservatoryStatScanner.Records;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner
{
    internal class RecordBook
    {
        // Root key: RecordTable (one of: stars, planets, rings)
        // Leaf key: Journal-based object name (eg. "M_RedSuperGiant" or "Ammonia world" or "eRingClass_Metalic")
        private Dictionary<RecordTable, Dictionary<string, List<IRecord>>> RecordsByTable = new();

        public RecordBook()
        {
            RecordsByTable.Add(RecordTable.Stars, new());
            RecordsByTable.Add(RecordTable.Planets, new());
            RecordsByTable.Add(RecordTable.Rings, new());
        }

        public void AddRecord(IRecord record)
        {
            var recordsForTable = RecordsByTable[record.Table];

            if (!recordsForTable.ContainsKey(record.JournalObjectName))
                recordsForTable.Add(record.JournalObjectName, new());

            var recordsForJournalObject = recordsForTable[record.JournalObjectName];

            recordsForJournalObject.Add(record);
        }

        public List<IRecord> GetRecords(RecordTable table, string journalObjectName)
        {
            if (RecordsByTable[table].ContainsKey(journalObjectName))
                return RecordsByTable[table][journalObjectName];
            return new();
        }
    }
}
