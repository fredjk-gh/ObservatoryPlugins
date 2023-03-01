using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class RegionsVisitedRecord : RegionRecord
    {
        private const char SEPARATOR = '|';

        public RegionsVisitedRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, "Visited regions")
        { }

        public override bool Enabled => Settings.EnableVisitedRegionRecords;
        public override string ValueFormat { get => "{0}"; }
        public override string Units { get => "visited"; }

        public override List<StatScannerGrid> CheckCodexEntry(CodexEntry codexEntry)
        {
            var regionName = Constants.RegionNamesByJournalId[codexEntry.Region];
            // We check via string.Contains here as it's presumably cheaper to do on every codex entry vs. splitting the string into substrings and creating a hashset.
            if (!Enabled || (!string.IsNullOrEmpty(Data.ExtraData) && Data.ExtraData.Contains(regionName))) return new();

            HashSet<string> visitedRegions = new();
            if (!string.IsNullOrEmpty(Data.ExtraData)) visitedRegions = Data.ExtraData.Split(SEPARATOR).ToHashSet();
            // Consistency check on the MaxValue vs. visitedRegions set.
            if (Data.HasMax && Data.MaxValue != visitedRegions.Count) Debug.WriteLine($"RegionsVisitedRecord has MaxValue ${MaxValue}, but {visitedRegions.Count} items in ExtraData: {Data.ExtraData}!");

            // This is a new region.
            var newValue = (Data.HasMax ? Data.MaxValue + 1 : 1);
            visitedRegions.Add(regionName);

            return CheckMax(newValue, codexEntry.Timestamp, regionName, "First visit", string.Join(SEPARATOR, visitedRegions));
        }
    }
}
