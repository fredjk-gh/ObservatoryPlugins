using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class OrbitalPeriodRecord : BodyRecord
    {
        public OrbitalPeriodRecord(StatScannerSettings settings, RecordKind recordKind, CSVData data)
            : base(settings, recordKind, data, "Orbital Period (d)")
        {
            format = "{0:0.##} d";
        }
        public override bool Enabled => Settings.EnableOrbitalPeriodRecord;

        public override List<StatScannerGrid> CheckScan(Scan scan)
        {
            if (!Enabled
                || scan.OrbitalPeriod == 0.0
                || (string.IsNullOrEmpty(scan.StarType) && (string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan))))
                return new();

            // convert seconds -> days
            var periodDays = scan.OrbitalPeriod / Constants.CONV_S_TO_DAYS_DIVISOR;
            var results = CheckMax(periodDays, scan.Timestamp, scan.BodyName, IsUndiscovered(scan));
            results.AddRange(CheckMin(periodDays, scan.Timestamp, scan.BodyName, IsUndiscovered(scan)));

            return results;
        }
    }
}
