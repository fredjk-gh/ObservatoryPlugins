﻿using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records
{
    internal class SolarMassesRecord : BodyRecord
    {
        public SolarMassesRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, "Mass")
        { }

        public override bool Enabled => Settings.EnableSolarMassesRecord;

        public override string ValueFormat { get => "{0:0.00##}"; }
        public override string Units { get => "SM"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled || string.IsNullOrEmpty(scan.StarType) && scan.StellarMass <= 0.0)
                return new();

            var results = CheckMax(scan.StellarMass, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan));
            results.AddRange(CheckMin(scan.StellarMass, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan)));

            return results;
        }
    }
}
