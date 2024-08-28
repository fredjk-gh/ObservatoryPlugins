using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records
{
    internal class SurfacePressureRecord : BodyRecord
    {
        public SurfacePressureRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, "Surface Pressure")
        { }

        public override bool Enabled => Settings.EnableSurfacePressureRecord;

        public override string ValueFormat { get => "{0:n3}"; }
        public override string Units { get => "atm"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
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

            var results = CheckMax(pressureAtms, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan));
            results.AddRange(CheckMin(pressureAtms, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan)));

            return results;
        }
    }
}
