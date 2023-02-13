using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class SurfacePressureRecord : BodyRecord
    {
        public SurfacePressureRecord(StatScannerSettings settings, string journalObjectName, string eDAstroObjectName, long minCount, double minValue, string minBody, long maxCount, double maxValue, string maxBody)
            : base(settings, RecordTable.Planets, "Surface Pressure (Atm)", Constants.V_SURFACE_PRESSURE, journalObjectName, eDAstroObjectName, minCount, minValue, minBody, maxCount, maxValue, maxBody)
        { }
        public override string ValueFormat { get => "{0:0.##} atm"; }
        public override bool Enabled => Settings.EnableSurfacePressureRecord;

        public override List<StatScannerGrid> CheckScan(Scan scan)
        {
            if (!Enabled || string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan))
                return new();

            // Convert Pa -> atms
            var pressureAtms = scan.SurfacePressure / Constants.CONV_PA_TO_ATM_DIVISOR;
            var results = CheckMax(pressureAtms, scan.Timestamp, scan.BodyName);
            results.AddRange(CheckMin(pressureAtms, scan.Timestamp, scan.BodyName));

            return results;
        }
    }
}
