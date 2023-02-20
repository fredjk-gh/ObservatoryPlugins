using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class SurfaceTemperatureRecord : BodyRecord
    {
        public SurfaceTemperatureRecord(StatScannerSettings settings, RecordKind recordKind, CSVData data)
            : base(settings, recordKind, data, "Surface Temperature (K)")
        {
            format = "{0:0.0} K";
        }
        public override bool Enabled => Settings.EnableSurfaceTemperatureRecord;

        public override List<StatScannerGrid> CheckScan(Scan scan)
        {
            if (!Enabled || scan.SurfaceTemperature == 0.0 ||
                (string.IsNullOrEmpty(scan.StarType) && (string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan))))
                return new();

            var results = CheckMax(scan.SurfaceTemperature, scan.Timestamp, scan.BodyName, IsUndiscovered(scan));
            results.AddRange(CheckMin(scan.SurfaceTemperature, scan.Timestamp, scan.BodyName, IsUndiscovered(scan)));

            return results;
        }
    }
}
