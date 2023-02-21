using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class SolarRadiusRecord : BodyRecord
    {
        public SolarRadiusRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, "Radius")
        { }

        public override bool Enabled => Settings.EnableSolarRadiusRecord;

        // Black holes are *really* small in terms of solar radius. So add extra digits to the display format.
        public override string ValueFormat { get => (EDAstroObjectName == Constants.EDASTRO_STAR_BLACK_HOLE ? "{0:0.00000000}" : "{0:0.0000} SR"); }

        public override List<StatScannerGrid> CheckScan(Scan scan)
        {
            if (!Enabled || scan.Radius < 1.0 || string.IsNullOrEmpty(scan.StarType))
                return new();

            // Convert m -> SR
            var radiusSR = scan.Radius / Constants.CONV_M_TO_SOLAR_RAD_DIVISOR;
            var results = CheckMax(radiusSR, scan.Timestamp, scan.BodyName, IsUndiscovered(scan));
            results.AddRange(CheckMin(radiusSR, scan.Timestamp, scan.BodyName, IsUndiscovered(scan)));

            return results;
        }

    }
}
