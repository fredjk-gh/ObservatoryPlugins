using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using com.github.fredjk_gh.PluginCommon.Data;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Body
{
    internal class RotationalPeriodRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
        : BodyRecord(settings, recordKind, data, "Rotational Period")
    {
        public override bool Enabled => Settings.EnableRotationalPeriodRecord;

        // Neutrons and BHs have very short rotational periods. Conditional format doesn't help in all cases.
        public override string ValueFormat { get => MaxValue < 1.0 ? "{0:0.00000###}" : "{0:0.##}"; }
        public override string Units { get => "d"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled
                || scan.RotationPeriod <= 0.0
                || string.IsNullOrEmpty(scan.StarType) && (string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan)))
                return [];

            // convert seconds -> days
            var periodDays = Conversions.SecondsToDays(scan.RotationPeriod);
            var results = CheckMax(periodDays, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan));
            results.AddRange(CheckMin(periodDays, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan)));

            return results;
        }
    }
}
