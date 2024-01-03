using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records
{
    internal class OrbitalEccentricityRecord : BodyRecord
    {
        public OrbitalEccentricityRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, "Orbital Eccentricity")
        { }

        public override bool Enabled => Settings.EnableOrbitalEccentricityRecord;

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled
                || scan.Eccentricity == 0.0
                || (string.IsNullOrEmpty(scan.StarType) && (string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan))))
                return new();

            return CheckMax(scan.Eccentricity, scan.Timestamp, scan.BodyName, IsUndiscovered(scan));
        }
    }
}
