using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records
{
    internal class RegionCodexCount : RegionRecord
    {
        static readonly Dictionary<string, string> CodexCategoriesDisplayNames = new()
        {
            { Constants.V_CODEX_CATEGORY_ASTRO, "Codex: Astronomical Bodies" },
            { Constants.V_CODEX_CATEGORY_BIO_GEO, "Codex: Biological and Geological" },
            { Constants.V_CODEX_CATEGORY_XENO, "Codex: Xenological" },
        };

        public RegionCodexCount(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, CodexCategoriesDisplayNames[data.Variable])
        {
            _disallowedStates = new() { LogMonitorState.PreRead };
        }

        public override bool Enabled => Settings.EnableRegionCodexCountRecords;
        public override string ValueFormat { get => "{0}"; }
        public override string Units { get => "entries"; }

        public override List<Result> CheckCodexEntry(CodexEntry codexEntry)
        {

            if (!Enabled || !codexEntry.IsNewEntry
                || !Constants.RegionNamesByJournalId.ContainsKey(codexEntry.Region)
                || !Constants.CodexCategoriesByJournalId.ContainsKey(codexEntry.Category)) return new();

            var regionName = Constants.RegionNamesByJournalId[codexEntry.Region];
            var categoryName = Constants.CodexCategoriesByJournalId[codexEntry.Category];
            if (regionName != EDAstroObjectName || categoryName != Data.Variable) return new();

            // This is a new entry.
            var newValue = (Data.HasMax? Data.MaxValue + 1 : 1);

            return CheckMax(NotificationClass.NewCodex, newValue, codexEntry.TimestampDateTime, codexEntry.Name_Localised, Constants.UI_CODEX_CONFIRMATION);
        }
    }
}
