using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class PlanetaryRadiusRecord : BodyRecord
    {
        public PlanetaryRadiusRecord(StatScannerSettings settings, string journalObjectName, string eDAstroObjectName, long minCount, double minValue, string minBody, long maxCount, double maxValue, string maxBody)
            : base(settings, RecordTable.Planets, "Planetary Radius (km)", Constants.V_PLANETARY_RADIUS, journalObjectName, eDAstroObjectName, minCount, minValue, minBody, maxCount, maxValue, maxBody)
        { }
        public override string ValueFormat { get => "{0:0} km"; }

        public override bool Enabled => Settings.EnablePlanetaryRadiusRecord;

        public override List<StatScannerGrid> CheckScan(Scan scan)
        {
            if (!Enabled || string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan))
                return new();

            // Convert m -> km
            var radiusKm = scan.Radius / Constants.CONV_M_TO_KM_DIVISOR;
            var results = CheckMax(radiusKm, scan.Timestamp, scan.BodyName);
            results.AddRange(CheckMin(radiusKm, scan.Timestamp, scan.BodyName));

            return results;
        }
    }
}
