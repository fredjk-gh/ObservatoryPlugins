﻿using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class SurfaceGravityRecord : BodyRecord
    {
        public SurfaceGravityRecord(StatScannerSettings settings, RecordKind recordKind, CSVData data)
            : base(settings, recordKind, data, "Surface Gravity (g)")
        {
            format = "{0:0.##} g";
        }
        public override bool Enabled => Settings.EnableSurfaceGravityRecord;

        public override List<StatScannerGrid> CheckScan(Scan scan)
        {
            if (!Enabled || string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan))
                return new();

            // Convert m/s^2 -> g's
            var gravityG = scan.SurfaceGravity / Constants.CONV_MperS2_TO_G_DIVISOR;
            var results = CheckMax(gravityG, scan.Timestamp, scan.BodyName, IsUndiscovered(scan));
            results.AddRange(CheckMin(gravityG, scan.Timestamp, scan.BodyName, IsUndiscovered(scan)));

            return results;
        }
    }
}