using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class RingDensityRecord : BodyRecord
    {
        public RingDensityRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, "Ring Density")
        { }

        public override bool Enabled => Settings.EnableRingDensityRecord;

        public override string ValueFormat { get => "{0:0.000000####}"; }
        public override string Units { get => "Mt/km^3"; }

        public override List<StatScannerGrid> CheckScan(Scan scan, string currentSystem)
        {
            List<StatScannerGrid> results = new();

            if (!Enabled || scan.Rings?.Count == 0)
                return results;

            // Check all rings of the specified type for this record:
            foreach (var ring in scan.Rings.Where(r => r.RingClass == JournalObjectName && r.Name.Contains("Ring")))
            {
                var densityMtPerKm2 = ring.MassMT /
                    ((Math.PI * Math.Pow(ring.OuterRad / Constants.CONV_M_TO_KM_DIVISOR, 2.0)) - (Math.PI * Math.Pow(ring.InnerRad / Constants.CONV_M_TO_KM_DIVISOR, 2.0)));
                results.AddRange(CheckMax(densityMtPerKm2, scan.Timestamp, ring.Name, IsUndiscovered(scan)));
            }
            return results;
        }
    }
}
