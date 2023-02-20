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
        public SurfacePressureRecord(StatScannerSettings settings, RecordKind recordKind, CSVData data)
            : base(settings, recordKind, data, "Surface Pressure (Atm)")
        {
            format = "{0:0.##} atm";
        }
        public override bool Enabled => Settings.EnableSurfacePressureRecord;

        public override List<StatScannerGrid> CheckScan(Scan scan)
        {
            if (!Enabled || string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan))
                return new();

            // Convert Pa -> atms
            var pressureAtms = scan.SurfacePressure / Constants.CONV_PA_TO_ATM_DIVISOR;
            // 0 atmo landables are fairly common. As are AWs and WWs. Most GGs have either no atmo or are 0.
            // So ignore zero alltogether for the moment except for ELWs (of which there are only a few).
            if (pressureAtms == 0.0 && scan.PlanetClass != Constants.SCAN_EARTHLIKE) {
                // 0.0 atmosphere bodies are relatively common.
                return new();
            }

            var results = CheckMax(pressureAtms, scan.Timestamp, scan.BodyName, IsUndiscovered(scan));
            results.AddRange(CheckMin(pressureAtms, scan.Timestamp, scan.BodyName, IsUndiscovered(scan)));

            return results;
        }
    }
}
