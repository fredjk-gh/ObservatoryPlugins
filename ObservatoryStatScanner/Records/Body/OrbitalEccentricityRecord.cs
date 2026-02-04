using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Body
{
    internal class OrbitalEccentricityRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
        : BodyRecord(settings, recordKind, data, "Orbital Eccentricity")
    {
        public override bool Enabled => Settings.EnableOrbitalEccentricityRecord;

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled
                || scan.Eccentricity == 0.0
                || string.IsNullOrEmpty(scan.StarType) && (string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan)))
                return [];

            return CheckMax(scan.Eccentricity, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan));
        }
    }
}
