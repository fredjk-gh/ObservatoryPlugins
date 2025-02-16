﻿using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records
{
    internal class SurfaceGravityRecord : BodyRecord
    {
        public SurfaceGravityRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, "Surface Gravity")
        { }

        public override bool Enabled => Settings.EnableSurfaceGravityRecord;

        public override string ValueFormat { get => "{0:0.00##}"; }
        public override string Units { get => "g"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled || string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan))
                return new();

            // Convert m/s^2 -> g's
            var gravityG = scan.SurfaceGravity / Constants.CONV_MperS2_TO_G_DIVISOR;
            var results = CheckMax(gravityG, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan));
            results.AddRange(CheckMin(gravityG, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan)));

            return results;
        }
    }
}
