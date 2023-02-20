﻿using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class RingOuterRadiusRecord : BodyRecord
    {
        public RingOuterRadiusRecord(StatScannerSettings settings, RecordKind recordKind, CSVData data)
            : base(settings, recordKind, data, "Ring Outer Radius")
        {
            format = "{0:N0} km";
        }

        public override bool Enabled => Settings.EnableRingOuterRadiusRecord;

        public override List<StatScannerGrid> CheckScan(Scan scan)
        {
            List<StatScannerGrid> results = new();

            if (!Enabled || scan.Rings?.Count == 0)
                return results;

            // Check all rings of the specified type for this record:
            foreach (var ring in scan.Rings.Where(r => r.RingClass == JournalObjectName && r.Name.Contains("Ring")))
            {
                var outerRadKm = ring.OuterRad / Constants.CONV_M_TO_KM_DIVISOR;
                results.AddRange(CheckMax(outerRadKm, scan.Timestamp, ring.Name, IsUndiscovered(scan)));
            }
            return results;
        }
    }
}
