using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using com.github.fredjk_gh.PluginCommon.Data;
using Observatory.Framework.Files.Journal;


namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Body
{
    internal class OrbitalPeriodRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
        : BodyRecord(settings, recordKind, data, "Orbital Period")
    {
        public override bool Enabled => Settings.EnableOrbitalPeriodRecord;

        public override string ValueFormat { get => "{0:n}"; }
        public override string Units { get => "d"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled
                || scan.OrbitalPeriod <= 0.0  // Triton has a negative orbital period.
                || string.IsNullOrEmpty(scan.StarType) && (string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan)))
                return [];

            // convert seconds -> days
            var periodDays = Conversions.SecondsToDays(scan.OrbitalPeriod);
            var results = CheckMax(periodDays, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan));
            results.AddRange(CheckMin(periodDays, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan)));

            return results;
        }
    }
}
