using com.github.fredjk_gh.ObservatoryStatScanner.DB;
using com.github.fredjk_gh.ObservatoryStatScanner.Records;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryStatScanner
{
    internal class RecordBook
    {
        // Root key: RecordTable (one of: stars, planets, rings, systems)
        // Leaf key: Journal-based object name (eg. "M_RedSuperGiant" or "Ammonia world" or "eRingClass_Metalic")
        // Values: List of IRecords which apply to the object type.
        private Dictionary<Records.RecordTable, Dictionary<string, List<Records.IRecord>>> RecordsByTable = new();

        private PersonalBestManager manager;

        public RecordBook(PersonalBestManager manager)
        {
            this.manager = manager;

            RecordsByTable.Add(RecordTable.Stars, new());
            RecordsByTable.Add(RecordTable.Planets, new());
            RecordsByTable.Add(RecordTable.Rings, new());
            RecordsByTable.Add(RecordTable.Systems, new());
            RecordsByTable.Add(RecordTable.Regions, new());
            RecordsByTable.Add(RecordTable.Codex, new());
        }

        public void AddRecord(IRecord record)
        {
            var recordsForTable = RecordsByTable[record.Table];

            if (!recordsForTable.ContainsKey(record.JournalObjectName))
                recordsForTable.Add(record.JournalObjectName, new());

            var recordsForJournalObject = recordsForTable[record.JournalObjectName];

            recordsForJournalObject.Add(record);

            if (record.RecordKind == RecordKind.Personal)
            {
                record.MaybeInitForPersonalBest(manager);
            }
        }

        public List<IRecord> GetRecords(RecordTable table, string journalObjectName)
        {
            if (RecordsByTable[table].ContainsKey(journalObjectName))
                return RecordsByTable[table][journalObjectName];
            return new();
        }

        public List<IRecord> GetPersonalBests()
        {
            List<IRecord> personalBests = new();
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
            List<IRecord> personalBests = new();
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
            manager.Clear();
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
            return RecordsByTable.Values.Sum(leafDict => leafDict.Values.Sum(list => list.Where(r => r.RecordKind == kind).Count()));
        }
    }
}
