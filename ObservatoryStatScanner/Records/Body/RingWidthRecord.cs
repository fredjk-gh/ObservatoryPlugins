﻿using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records
{
    internal class RingWidthRecord : BodyRecord
    {
        public RingWidthRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, "Ring Width")
        { }
        
        public override bool Enabled => Settings.EnableRingWidthRecord;

        public override string ValueFormat { get => "{0:n0}"; }
        public override string Units { get => "km"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            List<Result> results = new();

            if (!Enabled || scan.Rings?.Count == 0)
                return results;

            // Check all rings of the specified type for this record:
            foreach (var ring in scan.Rings.Where(r => r.RingClass == JournalObjectName && r.Name.Contains("Ring")))
            {
                var width = (ring.OuterRad - ring.InnerRad) / Constants.CONV_M_TO_KM_DIVISOR;
                results.AddRange(CheckMax(width, scan.TimestampDateTime, ring.Name, scan.BodyID, IsUndiscovered(scan)));
            }
            return results;
        }
    }
}
