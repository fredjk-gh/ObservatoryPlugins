using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class PlanetaryRadiusRecord : BodyRecord
    {
        public PlanetaryRadiusRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, "Planetary Radius")
        { }

        public override bool Enabled => Settings.EnablePlanetaryRadiusRecord;

        public override string ValueFormat { get => "{0:0}"; }
        public override string Units { get => "km"; }

        public override List<Result> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled || string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan))
                return new();

            // Convert m -> km
            var radiusKm = scan.Radius / Constants.CONV_M_TO_KM_DIVISOR;
            var results = CheckMax(radiusKm, scan.Timestamp, scan.BodyName, IsUndiscovered(scan));
            results.AddRange(CheckMin(radiusKm, scan.Timestamp, scan.BodyName, IsUndiscovered(scan)));

            return results;
        }
    }
}
