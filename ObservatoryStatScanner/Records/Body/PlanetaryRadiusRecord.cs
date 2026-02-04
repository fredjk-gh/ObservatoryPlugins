using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using com.github.fredjk_gh.PluginCommon.Data;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Body
{
    internal class PlanetaryRadiusRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
        : BodyRecord(settings, recordKind, data, "Planetary Radius")
    {
        public override bool Enabled => Settings.EnablePlanetaryRadiusRecord;

        public override string ValueFormat { get => "{0:n0}"; }
        public override string Units { get => "km"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled || string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan))
                return [];

            // Convert m -> km
            var radiusKm = Conversions.MetersToKm(scan.Radius);
            var results = CheckMax(radiusKm, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan));
            results.AddRange(CheckMin(radiusKm, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan)));

            return results;
        }
    }
}
