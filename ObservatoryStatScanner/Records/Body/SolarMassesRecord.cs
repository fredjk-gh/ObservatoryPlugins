using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class SolarMassesRecord : BodyRecord
    {
        public SolarMassesRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, "Mass")
        { }

        public override bool Enabled => Settings.EnableSolarMassesRecord;

        public override string ValueFormat { get => "{0:0.00##}"; }
        public override string Units { get => "SM"; }

        public override List<StatScannerGrid> CheckScan(Scan scan)
        {
            if (!Enabled || string.IsNullOrEmpty(scan.StarType) && scan.StellarMass <= 0.0)
                return new();

            var results = CheckMax(scan.StellarMass, scan.Timestamp, scan.BodyName, IsUndiscovered(scan));
            results.AddRange(CheckMin(scan.StellarMass, scan.Timestamp, scan.BodyName, IsUndiscovered(scan)));

            return results;
        }
    }
}
