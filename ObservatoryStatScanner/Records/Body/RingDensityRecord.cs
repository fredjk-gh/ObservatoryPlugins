using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using com.github.fredjk_gh.PluginCommon.Data;
using com.github.fredjk_gh.PluginCommon.Data.Journals;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Body
{
    internal class RingDensityRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
        : BodyRecord(settings, recordKind, data, "Ring Density")
    {
        public override bool Enabled => Settings.EnableRingDensityRecord;

        public override string ValueFormat { get => "{0:n4}"; }
        public override string Units { get => "MT/km^3"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            List<Result> results = [];

            if (!Enabled || scan.Rings?.Count == 0)
                return results;

            // Check all rings of the specified type for this record:
            foreach (var ring in scan.Rings.Where(r => r.RingClass == JournalObjectName && JournalConstants.IsRing(r.Name)))
            {
                var densityMtPerKm3 = Conversions.RingDensity(ring.MassMT, ring.InnerRad, ring.OuterRad);

                results.AddRange(CheckMax(densityMtPerKm3, scan.TimestampDateTime, ring.Name, scan.BodyID, IsUndiscovered(scan)));
            }
            return results;
        }
    }
}
