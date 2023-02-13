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
        public SurfaceTemperatureRecord(StatScannerSettings settings, RecordTable table, string journalObjectName, string eDAstroObjectName, long minCount, double minValue, string minBody, long maxCount, double maxValue, string maxBody)
            : base(settings, table, "Surface Temperature (K)", Constants.V_SURFACE_TEMPERATURE, journalObjectName, eDAstroObjectName, minCount, minValue, minBody, maxCount, maxValue, maxBody)
        { }
        public override string ValueFormat { get => "{0:0} K"; }
        public override bool Enabled => Settings.EnableSurfaceTemperatureRecord;

        public override List<StatScannerGrid> CheckScan(Scan scan)
        {
            if (!Enabled || scan.SurfaceTemperature == 0.0 ||
                (string.IsNullOrEmpty(scan.StarType) && (string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan))))
                return new();

            var results = CheckMax(scan.SurfaceTemperature, scan.Timestamp, scan.BodyName);
            results.AddRange(CheckMin(scan.SurfaceTemperature, scan.Timestamp, scan.BodyName));

            return results;
        }
    }
}
