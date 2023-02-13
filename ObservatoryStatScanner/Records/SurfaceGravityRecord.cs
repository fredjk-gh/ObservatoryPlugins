using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class SurfaceGravityRecord : BodyRecord
    {
        public SurfaceGravityRecord(StatScannerSettings settings, string journalObjectName, string eDAstroObjectName, long minCount, double minValue, string minBody, long maxCount, double maxValue, string maxBody)
            : base(settings, RecordTable.Planets, "Surface Gravity (g)", Constants.V_SURFACE_GRAVITY, journalObjectName, eDAstroObjectName, minCount, minValue, minBody, maxCount, maxValue, maxBody)
        { }
        public override string ValueFormat { get => "{0:0.##} g"; }
        public override bool Enabled => Settings.EnableSurfaceGravityRecord;

        public override List<StatScannerGrid> CheckScan(Scan scan)
        {
            if (!Enabled || string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan))
                return new();

            // Convert m/s^2 -> g's
            var gravityG = scan.SurfaceGravity / Constants.CONV_MperS2_TO_G_DIVISOR;
            var results = CheckMax(gravityG, scan.Timestamp, scan.BodyName);
            results.AddRange(CheckMin(gravityG, scan.Timestamp, scan.BodyName));

            return results;
        }
    }
}
