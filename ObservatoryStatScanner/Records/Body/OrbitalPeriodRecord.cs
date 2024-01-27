using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records
{
    internal class OrbitalPeriodRecord : BodyRecord
    {
        public OrbitalPeriodRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, "Orbital Period")
        { }

        public override bool Enabled => Settings.EnableOrbitalPeriodRecord;

        public override string ValueFormat { get => "{0:n}"; }
        public override string Units { get => "d"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled
                || scan.OrbitalPeriod <= 0.0  // Triton has a negative orbital period.
                || (string.IsNullOrEmpty(scan.StarType) && (string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan))))
                return new();

            // convert seconds -> days
            var periodDays = scan.OrbitalPeriod / Constants.CONV_S_TO_DAYS_DIVISOR;
            var results = CheckMax(periodDays, scan.Timestamp, scan.BodyName, scan.BodyID, IsUndiscovered(scan));
            results.AddRange(CheckMin(periodDays, scan.Timestamp, scan.BodyName, scan.BodyID, IsUndiscovered(scan)));

            return results;
        }
    }
}
