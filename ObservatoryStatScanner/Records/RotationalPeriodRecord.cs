using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class RotationalPeriodRecord : BodyRecord
    {
        public RotationalPeriodRecord(StatScannerSettings settings, RecordKind recordKind, CSVData data)
            : base(settings, recordKind, data, "Rotational Period (d)")
        {
            format = (MaxValue < 1.0 ? "{0:0.00000###" : "{0:0.##} d");
        }

        public override bool Enabled => Settings.EnableRotationalPeriodRecord;

        public override List<StatScannerGrid> CheckScan(Scan scan)
        {
            if (!Enabled
                || scan.RotationPeriod <= 0.0
                || (string.IsNullOrEmpty(scan.StarType) && (string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan))))
                return new();

            // convert seconds -> days
            var periodDays = scan.RotationPeriod / Constants.CONV_S_TO_DAYS_DIVISOR;
            var results = CheckMax(periodDays, scan.Timestamp, scan.BodyName, IsUndiscovered(scan));
            results.AddRange(CheckMin(periodDays, scan.Timestamp, scan.BodyName, IsUndiscovered(scan)));

            return results;
        }
    }
}
