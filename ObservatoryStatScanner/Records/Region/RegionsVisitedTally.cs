using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs;
using Observatory.Framework.Files.Journal;
using System.Diagnostics;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Region
{
    internal class RegionsVisitedTally(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
        : RegionRecord(settings, recordKind, data, "Visited")
    {
        private const char SEPARATOR = '|';

        public override bool Enabled => Settings.EnableVisitedRegionRecords;
        public override string ValueFormat { get => "{0}"; }
        public override string Units { get => "region"; }

        public override List<Result> CheckCodexEntry(CodexEntry codexEntry)
        {
            var regionName = FDevIDs.RegionById[codexEntry.Region].Name;
            // We check via string.Contains here as it's presumably cheaper to do on every codex entry vs. splitting the string into substrings and creating a hashset.
            if (!Enabled || !string.IsNullOrEmpty(Data.ExtraData) && Data.ExtraData.Contains(regionName)) return [];

            HashSet<string> visitedRegions = [];
            if (!string.IsNullOrEmpty(Data.ExtraData)) visitedRegions = [.. Data.ExtraData.Split(SEPARATOR)];
            // Consistency check on the MaxValue vs. visitedRegions set.
            if (Data.HasMax && Data.MaxValue != visitedRegions.Count) Debug.WriteLine($"RegionsVisitedRecord has MaxValue ${MaxValue}, but {visitedRegions.Count} items in ExtraData: {Data.ExtraData}!");

            // This is a new region.
            var newValue = Data.HasMax ? Data.MaxValue + 1 : 1;
            visitedRegions.Add(regionName);

            return CheckMax(NotificationClass.Tally, newValue, codexEntry.TimestampDateTime, regionName, Constants.UI_FIRST_VISIT, string.Join(SEPARATOR, visitedRegions));
        }
    }
}
