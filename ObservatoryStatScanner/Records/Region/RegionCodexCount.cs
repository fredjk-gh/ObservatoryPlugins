using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs;
using Observatory.Framework;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Region
{
    internal class RegionCodexCount : RegionRecord
    {
        static readonly Dictionary<string, string> CodexCategoriesDisplayNames = new()
        {
            { FDevIDs.V_CODEX_CATEGORY_ASTRO, "Codex: Astronomical Bodies" },
            { FDevIDs.V_CODEX_CATEGORY_BIO_GEO, "Codex: Biological and Geological" },
            { FDevIDs.V_CODEX_CATEGORY_XENO, "Codex: Xenological" },
        };

        public RegionCodexCount(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, CodexCategoriesDisplayNames[data.Variable])
        {
            _disallowedStates = [LogMonitorState.PreRead];
        }

        public override bool Enabled => Settings.EnableRegionCodexCountRecords;
        public override string ValueFormat { get => "{0}"; }
        public override string Units { get => "entries"; }

        public override List<Result> CheckCodexEntry(CodexEntry codexEntry)
        {
            if (!Enabled || !codexEntry.IsNewEntry
                || !FDevIDs.RegionById.TryGetValue(codexEntry.Region, out PluginCommon.Data.Journals.FDevIDs.Region region)
                || !FDevIDs.CodexCategoriesByJournalId.TryGetValue(codexEntry.Category, out string categoryName)) return [];

            var regionName = region.Name;
            if (regionName != EDAstroObjectName || categoryName != Data.Variable) return [];

            // This is a new entry.
            var newValue = Data.HasMax ? Data.MaxValue + 1 : 1;

            return CheckMax(NotificationClass.NewCodex, newValue, codexEntry.TimestampDateTime, codexEntry.Name_Localised, Constants.UI_CODEX_CONFIRMATION);
        }
    }
}
