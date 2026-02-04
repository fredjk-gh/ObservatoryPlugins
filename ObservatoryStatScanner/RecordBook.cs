using com.github.fredjk_gh.ObservatoryStatScanner.DB;
using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;

namespace com.github.fredjk_gh.ObservatoryStatScanner
{
    internal class RecordBook
    {
        // Root key: RecordTable (one of: stars, planets, rings, systems)
        // Leaf key: Journal-based object name (eg. "M_RedSuperGiant" or "Ammonia world" or "eRingClass_Metalic")
        // Values: List of IRecords which apply to the object type.
        private readonly Dictionary<RecordTable, Dictionary<string, List<IRecord>>> RecordsByTable = [];

        private readonly PersonalBestManager _manager;

        public RecordBook(PersonalBestManager manager)
        {
            _manager = manager;

            RecordsByTable.Add(RecordTable.Stars, []);
            RecordsByTable.Add(RecordTable.Planets, []);
            RecordsByTable.Add(RecordTable.Rings, []);
            RecordsByTable.Add(RecordTable.Systems, []);
            RecordsByTable.Add(RecordTable.Regions, []);
            RecordsByTable.Add(RecordTable.Codex, []);
        }

        public void AddRecord(IRecord record)
        {
            var recordsForTable = RecordsByTable[record.Table];

            if (!recordsForTable.ContainsKey(record.JournalObjectName))
                recordsForTable.Add(record.JournalObjectName, []);

            var recordsForJournalObject = recordsForTable[record.JournalObjectName];

            recordsForJournalObject.Add(record);

            if (record.RecordKind == RecordKind.Personal)
            {
                record.MaybeInitForPersonalBest(_manager);
            }
        }

        public List<IRecord> GetRecords(RecordTable table, string journalObjectName)
        {
            if (RecordsByTable[table].TryGetValue(journalObjectName, out List<IRecord> records))
                return records;
            return [];
        }

        public List<IRecord> GetPersonalBests()
        {
            List<IRecord> personalBests = [];
            foreach (var rt in RecordsByTable.Keys)
            {
                foreach (var objName in RecordsByTable[rt].Keys)
                {
                    foreach (var r in RecordsByTable[rt][objName])
                    {
                        if (r.RecordKind == RecordKind.Personal) personalBests.Add(r);
                    }
                }
            }
            return personalBests;
        }

        public List<IRecord> GetPersonalBests(RecordTable rt)
        {
            List<IRecord> personalBests = [];
            foreach (var objName in RecordsByTable[rt].Keys)
            {
                foreach (var r in RecordsByTable[rt][objName])
                {
                    if (r.RecordKind == RecordKind.Personal) personalBests.Add(r);
                }
            }
            return personalBests;
        }

        public void ResetPersonalBests()
        {
            foreach (var rt in RecordsByTable.Keys)
            {
                foreach (var objName in RecordsByTable[rt].Keys)
                {
                    foreach (var r in RecordsByTable[rt][objName])
                    {
                        if (r.RecordKind == RecordKind.Personal) r.Reset();
                    }
                }
            }
            _manager.Clear();
        }

        public int Count
        {
            get { return RecordsByTable.Keys.Sum(table => CountByTable(table)); }
        }

        public int CountByTable(RecordTable table)
        {
            return RecordsByTable[table].Values.Sum(list => list.Count);
        }

        public int CountByKind(RecordKind kind)
        {
            return RecordsByTable.Values
                .Sum<Dictionary<string, List<IRecord>>>(leafDict => leafDict.Values.Sum<List<IRecord>>(list => list.Count<IRecord>(r => r.RecordKind == kind)));
        }
    }
}
