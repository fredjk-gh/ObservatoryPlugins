using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Body
{
    internal class SurfaceTemperatureRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
        : BodyRecord(settings, recordKind, data, "Surface Temperature")
    {
        public override bool Enabled => Settings.EnableSurfaceTemperatureRecord;

        public override string ValueFormat { get => "{0:0.0}"; }
        public override string Units { get => "K"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled || scan.SurfaceTemperature == 0.0 ||
                string.IsNullOrEmpty(scan.StarType) && (string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan)))
                return [];

            // Currently this record has integer granularity on edastro. Thus, we may detect potential new records
            // for values beyond the record by amounts that round to the integer record. Orvidius is working on it,
            // but it will be a while to back-fill and be realized in the galactic records file.
            var results = CheckMax(scan.SurfaceTemperature, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan));
            results.AddRange(CheckMin(scan.SurfaceTemperature, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan)));

            return results;
        }
    }
}
