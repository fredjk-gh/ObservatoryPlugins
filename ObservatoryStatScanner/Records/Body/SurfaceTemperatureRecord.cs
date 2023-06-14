using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class SurfaceTemperatureRecord : BodyRecord
    {
        public SurfaceTemperatureRecord(StatScannerSettings settings, RecordKind recordKind, IRecordData data)
            : base(settings, recordKind, data, "Surface Temperature")
        { }
        
        public override bool Enabled => Settings.EnableSurfaceTemperatureRecord;

        public override string ValueFormat { get => "{0:0.0}"; }
        public override string Units { get => "K"; }

        public override List<StatScannerGrid> CheckScan(Scan scan, string currentSystem)
        {
            if (!Enabled || scan.SurfaceTemperature == 0.0 ||
                (string.IsNullOrEmpty(scan.StarType) && (string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan))))
                return new();

            // Currently this record has integer granularity on edastro. Thus, we may detect potential new records
            // for values beyond the record by amounts that round to the integer record. Orvidius is working on it,
            // but it will be a while to back-fill and be realized in the galactic records file.
            var results = CheckMax(scan.SurfaceTemperature, scan.Timestamp, scan.BodyName, IsUndiscovered(scan));
            results.AddRange(CheckMin(scan.SurfaceTemperature, scan.Timestamp, scan.BodyName, IsUndiscovered(scan)));

            return results;
        }
    }
}
