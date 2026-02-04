using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using com.github.fredjk_gh.PluginCommon.Data;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Body
{
    internal class SolarRadiusRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
        : BodyRecord(settings, recordKind, data, "Radius")
    {
        public override bool Enabled => Settings.EnableSolarRadiusRecord;

        // Black holes are *really* small in terms of solar radius. So add extra digits to the display format.
        public override string ValueFormat { get => EDAstroObjectName == Constants.EDASTRO_STAR_BLACK_HOLE ? "{0:n8}" : "{0:n4}"; }
        public override string Units { get => "SR"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled || scan.Radius < 1.0 || string.IsNullOrEmpty(scan.StarType))
                return [];

            // Convert m -> SR
            var radiusSR = Conversions.MetersToSr(scan.Radius);
            var results = CheckMax(radiusSR, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan));
            results.AddRange(CheckMin(radiusSR, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan)));

            return results;
        }
    }
}
