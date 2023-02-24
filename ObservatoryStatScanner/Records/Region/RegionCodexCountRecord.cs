using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class RegionCodexCountRecord : RegionRecord
    {
        public RegionCodexCountRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, data.Variable)
        { }

        public override bool Enabled => Settings.EnableRegionCodexCountRecords;
        public override string ValueFormat { get => "{0}"; }
        public override string Units { get => "entries"; }

        public override List<StatScannerGrid> CheckCodexEntry(CodexEntry codexEntry)
        {

            if (!Enabled || !codexEntry.IsNewEntry
                || !Constants.RegionNamesByJournalId.ContainsKey(codexEntry.Region)
                || !Constants.CodexCategoriesByJournalId.ContainsKey(codexEntry.Category)) return new();

            var regionName = Constants.RegionNamesByJournalId[codexEntry.Region];
            var categoryName = Constants.CodexCategoriesByJournalId[codexEntry.Category];
            if (regionName != EDAstroObjectName || categoryName != Data.Variable) return new();

            // This is a new entry.
            var newValue = (Data.HasMax? Data.MaxValue + 1 : 1);

            return CheckMax(newValue, codexEntry.Timestamp, codexEntry.Name_Localised);
        }
    }
}
