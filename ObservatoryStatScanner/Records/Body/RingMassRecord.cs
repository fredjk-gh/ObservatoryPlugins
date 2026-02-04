using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Body
{
    internal class RingMassRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
        : BodyRecord(settings, recordKind, data, "Ring Mass")
    {
        public override bool Enabled => Settings.EnableRingMassRecord;

        public override string ValueFormat { get => "{0:n0}"; }
        public override string Units { get => "MT"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            List<Result> results = [];

            if (!Enabled || scan.Rings?.Count == 0)
                return results;

            // Check all rings of the specified type for this record:
            foreach (var ring in scan.Rings.Where(r => r.RingClass == JournalObjectName && r.Name.Contains("Ring")))
            {
                results.AddRange(CheckMax(ring.MassMT, scan.TimestampDateTime, ring.Name, scan.BodyID, IsUndiscovered(scan)));
            }
            return results;
        }
    }
}
