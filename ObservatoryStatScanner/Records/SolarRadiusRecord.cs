﻿using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class SolarRadiusRecord : BodyRecord
    {
        public SolarRadiusRecord(StatScannerSettings settings, RecordKind recordKind, CSVData data)
            : base(settings, recordKind, data, "Radius")
        {
            // Black holes are *really* small in terms of solar radius. So add extra digits to the display format.
            format = (EDAstroObjectName == Constants.EDASTRO_STAR_BLACK_HOLE ? "{0:0.00000000}" : "{0:0.0000} SR");
        }
        public override bool Enabled => Settings.EnableSolarRadiusRecord;

        public override List<StatScannerGrid> CheckScan(Scan scan)
        {
            if (!Enabled || scan.Radius < 1.0 || string.IsNullOrEmpty(scan.StarType))
                return new();

            // Convert m -> SR
            var radiusSR = scan.Radius / Constants.CONV_M_TO_SOLAR_RAD_DIVISOR;
            var results = CheckMax(radiusSR, scan.Timestamp, scan.BodyName, IsUndiscovered(scan));

            // TODO: remove after Orvidius has finished regenerating the records with the fix!
            // This object + variable has a data loss issue (min is 0.0001 and there's many bodies with lower value in my journals). Exclude it
            if (!(Table == RecordTable.Stars && EDAstroObjectName == Constants.EDASTRO_STAR_BLACK_HOLE && VariableName == Constants.V_SOLAR_RADIUS))
            {
                results.AddRange(CheckMin(radiusSR, scan.Timestamp, scan.BodyName, IsUndiscovered(scan)));
            }

            return results;
        }

    }
}
