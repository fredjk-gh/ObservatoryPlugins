using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using com.github.fredjk_gh.PluginCommon.Data;
using com.github.fredjk_gh.PluginCommon.Data.Journals;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Body
{
    internal class RingOuterRadiusRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
        : BodyRecord(settings, recordKind, data, "Ring Outer Radius")
    {
        public override bool Enabled => Settings.EnableRingOuterRadiusRecord;

        public override string ValueFormat { get => "{0:N0}"; }
        public override string Units { get => "km"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            List<Result> results = [];

            if (!Enabled || scan.Rings?.Count == 0)
                return results;

            // Check all rings of the specified type for this record:
            foreach (var ring in scan.Rings.Where(r => r.RingClass == JournalObjectName && JournalConstants.IsRing(r.Name)))
            {
                var outerRadKm = Conversions.MetersToKm(ring.OuterRad);
                results.AddRange(CheckMax(outerRadKm, scan.TimestampDateTime, ring.Name, scan.BodyID, IsUndiscovered(scan)));
            }
            return results;
        }
    }
}
