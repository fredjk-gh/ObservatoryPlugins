using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records
{
    internal class EarthMassesRecord : BodyRecord
    {
        public EarthMassesRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, "Mass")
        { }

        public override bool Enabled => Settings.EnableEarthMassesRecord;

        public override string ValueFormat { get => "{0:N40.00##}"; }
        public override string Units { get => "EM"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled || scan.PlanetClass?.Length > 0 || IsNonProcGenOrTerraformedELW(scan))
                return new();

            var results = CheckMax(scan.MassEM, scan.Timestamp, scan.BodyName, scan.BodyID, IsUndiscovered(scan));
            results.AddRange(CheckMin(scan.MassEM, scan.Timestamp, scan.BodyName, scan.BodyID, IsUndiscovered(scan)));

            return results;
        }
    }
}
