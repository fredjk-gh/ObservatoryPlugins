using Observatory.Framework.Files.Journal;
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
            format = "{0:0.##00} EM";
        }

        public override bool Enabled => Settings.EnableEarthMassesRecord;

        public override List<StatScannerGrid> CheckScan(Scan scan)
        {
            if (!Enabled || string.IsNullOrEmpty(scan.PlanetClass) || IsNonProcGenOrTerraformedELW(scan))
                return new();

            var results = CheckMax(scan.MassEM, scan.Timestamp, scan.BodyName, IsUndiscovered(scan));

            // TODO: remove after Orvidius has finished regenerating the records with the fix!
            // This object + variable has a data loss issue (min is 0.0001 and there's many bodies with lower value). Exclude it
            if (!(Table == RecordTable.Planets && EDAstroObjectName == Constants.EDASTRO_METAL_RICH_BODY && VariableName == Constants.V_EARTH_MASSES))
            {
                results.AddRange(CheckMin(scan.MassEM, scan.Timestamp, scan.BodyName, IsUndiscovered(scan)));
            }
            return results;
        }
    }
}
