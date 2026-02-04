using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Body
{
    internal class EarthMassesRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
        : BodyRecord(settings, recordKind, data, "Mass")
    {
        public override bool Enabled => Settings.EnableEarthMassesRecord;
        public override string ValueFormat { get => "{0:N40.00##}"; }
        public override string Units { get => "EM"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled || scan.PlanetClass?.Length > 0 || IsNonProcGenOrTerraformedELW(scan))
                return [];

            var results = CheckMax(scan.MassEM, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan));
            results.AddRange(CheckMin(scan.MassEM, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan)));

            return results;
        }
    }
}
