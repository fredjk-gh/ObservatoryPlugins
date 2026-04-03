using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using com.github.fredjk_gh.PluginCommon.Data;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryStatScanner.Records.Body
{
    internal class SMARecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
        : BodyRecord(settings, recordKind, data, "Semi-Major Axis")
    {
        public override bool Enabled => Settings.EnableSemiMajorAxisRecord;
        public override string Units { get => "AU"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled || (string.IsNullOrEmpty(scan.PlanetClass) && string.IsNullOrEmpty(scan.StarType))
                || IsNonProcGenOrTerraformedELW(scan) || scan.SemiMajorAxis == 0)
                return [];

            var sma_AU = Conversions.MetersToAu(scan.SemiMajorAxis);
            var results = CheckMax(sma_AU, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan));
            results.AddRange(CheckMin(sma_AU, scan.TimestampDateTime, scan.BodyName, scan.BodyID, IsUndiscovered(scan)));

            return results;
        }
    }
}
