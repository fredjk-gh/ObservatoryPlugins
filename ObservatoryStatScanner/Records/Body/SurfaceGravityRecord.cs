using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using com.github.fredjk_gh.PluginCommon.Data;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Body
{
    internal class SurfaceGravityRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
        : BodyRecord(settings, recordKind, data, "Surface Gravity")
    {
        public override bool Enabled => Settings.EnableSurfaceGravityRecord;

        public override string ValueFormat { get => "{0:0.00##}"; }
        public override string Units { get => "g"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled || string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan))
                return [];

            // Convert m/s^2 -> g's
            var gravityG = Conversions.Mpers2ToG(scan.SurfaceGravity);
            var results = CheckMax(gravityG, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan));
            results.AddRange(CheckMin(gravityG, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan)));

            return results;
        }
    }
}
