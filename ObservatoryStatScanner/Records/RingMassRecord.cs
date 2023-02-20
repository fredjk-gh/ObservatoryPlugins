﻿using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class RingMassRecord : BodyRecord
    {
        public RingMassRecord(StatScannerSettings settings, RecordKind recordKind, CSVData data)
            : base(settings, recordKind, data, "Ring mass")
        {
            format = "{0:N0} Mt";
        }
        public override bool Enabled => Settings.EnableRingMassRecord;

        public override List<StatScannerGrid> CheckScan(Scan scan)
        {
            List<StatScannerGrid> results = new();

            if (!Enabled || scan.Rings?.Count == 0)
                return results;

            // Check all rings of the specified type for this record:
            foreach (var ring in scan.Rings.Where(r => r.RingClass == JournalObjectName && r.Name.Contains("Ring")))
            {
                results.AddRange(CheckMax(ring.MassMT, scan.Timestamp, ring.Name, IsUndiscovered(scan)));
            }
            return results;
        }
    }
}
