using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Body
{
    internal class SolarMassesRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
        : BodyRecord(settings, recordKind, data, "Mass")
    {
        public override bool Enabled => Settings.EnableSolarMassesRecord;

        public override string ValueFormat { get => "{0:0.00##}"; }
        public override string Units { get => "SM"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled || string.IsNullOrEmpty(scan.StarType) && scan.StellarMass <= 0.0)
                return [];

            var results = CheckMax(scan.StellarMass, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan));
            results.AddRange(CheckMin(scan.StellarMass, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan)));

            return results;
        }
    }
}
