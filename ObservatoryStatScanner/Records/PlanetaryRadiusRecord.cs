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
        public PlanetaryRadiusRecord(StatScannerSettings settings, RecordKind recordKind, CSVData data)
            : base(settings, recordKind, data, "Planetary Radius (km)")
        {
            format = "{0:0} km";
        }

        public override bool Enabled => Settings.EnablePlanetaryRadiusRecord;

        public override List<StatScannerGrid> CheckScan(Scan scan)
        {
            if (!Enabled || string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan))
                return new();

            // Convert m -> km
            var radiusKm = scan.Radius / Constants.CONV_M_TO_KM_DIVISOR;
            var results = CheckMax(radiusKm, scan.Timestamp, scan.BodyName, IsUndiscovered(scan));
            results.AddRange(CheckMin(radiusKm, scan.Timestamp, scan.BodyName, IsUndiscovered(scan)));

            return results;
        }
    }
}
