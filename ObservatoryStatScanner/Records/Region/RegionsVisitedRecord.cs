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
        private readonly HashSet<string> visitedRegions;

        public RegionsVisitedRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, "Visited regions")
        {

            if (string.IsNullOrEmpty(data.ExtraData)) {
                data.ExtraData = "";
                visitedRegions = new();
            }
            else
            {
                visitedRegions = data.ExtraData.Split(SEPARATOR).ToHashSet();
            }

            if (data.MaxValue != visitedRegions.Count)
            {
                Debug.WriteLine($"RegionsVisitedRecord has MaxValue ${MaxValue}, but {visitedRegions.Count} items in ExtraData: {data.ExtraData}!");
            }
        }

        public override bool Enabled => Settings.EnableVisitedRegionRecords;
        public override string ValueFormat { get => "{0}"; }
        public override string Units { get => "visited"; }

        public override List<StatScannerGrid> CheckCodexEntry(CodexEntry codexEntry)
        {
            var regionName = Constants.RegionNamesByJournalId[codexEntry.Region];
            if (!Enabled || visitedRegions.Contains(regionName)) return new();

            // This is a new region.
            var newValue = (Data.HasMax ? Data.MaxValue + 1 : 1);
            visitedRegions.Add(regionName);

            var extraData = string.Join(SEPARATOR, visitedRegions);
            return CheckMax(newValue, codexEntry.Timestamp, regionName, "First visit", extraData);
        }

        public override void Reset()
        {
            base.Reset();

            visitedRegions.Clear();
        }
    }
}
