using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner.Records
{
    internal class SolarMassesRecord : BodyRecord
    {
        public SolarMassesRecord(StatScannerSettings settings, string journalObjectName, string eDAstroObjectName, long minCount, double minValue, string minBody, long maxCount, double maxValue, string maxBody)
            : base(settings, RecordTable.Stars, "Mass (SM)", Constants.V_SOLAR_MASSES, journalObjectName, eDAstroObjectName, minCount, minValue, minBody, maxCount, maxValue, maxBody)
        { }
        public override string ValueFormat { get => "{0:0.##00} SM"; }

        public override bool Enabled => Settings.EnableSolarMassesRecord;

        public override List<StatScannerGrid> CheckScan(Scan scan)
        {
            if (!Enabled || string.IsNullOrEmpty(scan.StarType))
                return new();

            var results = CheckMax(scan.StellarMass, scan.Timestamp, scan.BodyName);

            // TODO: remove after Orvidius has finished regenerating the records with the fix!
            // This object + variable has a data loss issue (min is 0.0001 and there's many bodies with lower value in my journals). Exclude it
            if (!(Table == RecordTable.Stars && EDAstroObjectName == Constants.EDASTRO_STAR_Y_DWARF && VariableName == Constants.V_SOLAR_MASSES))
            {
                results.AddRange(CheckMin(scan.StellarMass, scan.Timestamp, scan.BodyName));
            }

            return results;
        }
    }
}
