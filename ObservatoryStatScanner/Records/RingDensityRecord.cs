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
        public RingDensityRecord(StatScannerSettings settings, RecordKind recordKind, CSVData data)
            : base(settings, recordKind, data, "Ring Density")
        {
            format = "{0:0.0000}";
        }

        public override bool Enabled => Settings.EnableRingDensityRecord;

        public override List<StatScannerGrid> CheckScan(Scan scan)
        {
            List<StatScannerGrid> results = new();

            if (!Enabled || scan.Rings?.Count == 0)
                return results;

            // Check all rings of the specified type for this record:
            foreach (var ring in scan.Rings.Where(r => r.RingClass == JournalObjectName && r.Name.Contains("Ring")))
            {
                var density = ring.MassMT / ((Math.PI * Math.Pow(ring.OuterRad, 2.0)) - Math.Pow(Math.PI * ring.InnerRad, 2.0));
                results.AddRange(CheckMax(density, scan.Timestamp, ring.Name, IsUndiscovered(scan)));
            }
            return results;
        }
    }
}
