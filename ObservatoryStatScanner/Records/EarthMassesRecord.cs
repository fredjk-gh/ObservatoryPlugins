﻿using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class EarthMassesRecord : BodyRecord
    {
        public EarthMassesRecord(StatScannerSettings settings, RecordKind recordKind, CSVData data)
            : base(settings, recordKind, data, "Mass (EM)")
        {
            format = "{0:N40.00##} EM";
        }

        public override bool Enabled => Settings.EnableEarthMassesRecord;

        public override List<StatScannerGrid> CheckScan(Scan scan)
        {
            if (!Enabled || scan.PlanetClass?.Length > 0 || IsNonProcGenOrTerraformedELW(scan))
                return new();

            var results = CheckMax(scan.MassEM, scan.Timestamp, scan.BodyName, IsUndiscovered(scan));
            results.AddRange(CheckMin(scan.MassEM, scan.Timestamp, scan.BodyName, IsUndiscovered(scan)));

            return results;
        }
    }
}
