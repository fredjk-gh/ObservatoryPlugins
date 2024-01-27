using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records
{
    internal class RotationalPeriodRecord : BodyRecord
    {
        public RotationalPeriodRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, "Rotational Period")
        { }

        public override bool Enabled => Settings.EnableRotationalPeriodRecord;

        public override string ValueFormat { get => (MaxValue < 1.0 ? "{0:0.00000###}" : "{0:0.##}"); }
        public override string Units { get => "d"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled
                || scan.RotationPeriod <= 0.0
                || (string.IsNullOrEmpty(scan.StarType) && (string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan))))
                return new();

            // convert seconds -> days
            var periodDays = scan.RotationPeriod / Constants.CONV_S_TO_DAYS_DIVISOR;
            var results = CheckMax(periodDays, scan.Timestamp, scan.BodyName, scan.BodyID, IsUndiscovered(scan));
            results.AddRange(CheckMin(periodDays, scan.Timestamp, scan.BodyName, scan.BodyID, IsUndiscovered(scan)));

            return results;
        }
    }
}
