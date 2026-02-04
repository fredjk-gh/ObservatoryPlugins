using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using com.github.fredjk_gh.PluginCommon.Data;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Body
{
    internal class SurfacePressureRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
        : BodyRecord(settings, recordKind, data, "Surface Pressure")
    {
        public override bool Enabled => Settings.EnableSurfacePressureRecord;

        public override string ValueFormat { get => "{0:n3}"; }
        public override string Units { get => "atm"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled || string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan))
                return [];

            // Convert Pa -> atms
            var pressureAtms = Conversions.PaToAtm(scan.SurfacePressure);
            // 0 atmo landables are fairly common. As are AWs and WWs. Most GGs have either no atmo or are 0.
            // So ignore zero alltogether for the moment except for ELWs (of which there are only a few).
            if (pressureAtms == 0.0 && scan.PlanetClass != Constants.SCAN_EARTHLIKE) {
                // 0.0 atmosphere bodies are relatively common.
                return [];
            }

            var results = CheckMax(pressureAtms, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan));
            results.AddRange(CheckMin(pressureAtms, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan)));

            return results;
        }
    }
}
