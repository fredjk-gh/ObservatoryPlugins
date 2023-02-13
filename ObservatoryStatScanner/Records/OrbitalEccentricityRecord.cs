using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class OrbitalEccentricityRecord : BodyRecord
    {
        public OrbitalEccentricityRecord(StatScannerSettings settings, RecordTable table, string journalObjectName, string eDAstroObjectName, long minCount, double minValue, string minBody, long maxCount, double maxValue, string maxBody)
            : base(settings, table, "Orbital Eccentricity", Constants.V_ECCENTRICITY, journalObjectName, eDAstroObjectName, minCount, minValue, minBody, maxCount, maxValue, maxBody)
        { }

        public override bool Enabled => Settings.EnableOrbitalEccentricityRecord;

        public override List<StatScannerGrid> CheckScan(Scan scan)
        {
            if (!Enabled
                || scan.Eccentricity ==- 0.0
                || (string.IsNullOrEmpty(scan.StarType) && (string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan))))
                return new();

            var results = CheckMax(scan.Eccentricity, scan.Timestamp, scan.BodyName);
            results.AddRange(CheckMin(scan.Eccentricity, scan.Timestamp, scan.BodyName));

            return results;
        }
    }
}
