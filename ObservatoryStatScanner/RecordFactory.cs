using ObservatoryStatScanner.Records;
using System.Diagnostics;

namespace ObservatoryStatScanner
{
    internal class RecordFactory
    {

        // Field indexes
        static readonly int CSV_Type = 0;
        static readonly int CSV_Variable = 1;
        static readonly int CSV_MaxCount = 2;
        static readonly int CSV_MaxValue = 3;
        static readonly int CSV_MaxBody = 4;
        static readonly int CSV_MinCount = 5;
        static readonly int CSV_MinValue = 6;
        static readonly int CSV_MinBody = 7;
        //static readonly int CSV_Average = 8;
        //static readonly int CSV_StandardDeviation = 9;
        //static readonly int CSV_Count = 10;
        static readonly int CSV_Table = 11;

        public static IRecord CreateRecord(string[] csvFields, StatScannerSettings settings)
        {
            RecordTable table = RecordTableFromString(csvFields[CSV_Table]);
            if (table == RecordTable.Unknown) return null;

            string type = csvFields[CSV_Type];
            if (!Constants.JournalTypeMap.ContainsKey(type)) return null;
            string journalType = Constants.JournalTypeMap[type];
            string variable = csvFields[CSV_Variable];

            Type typeToCreate = null;
            bool useExplicitTable = false;

            // TODO: Flesh this out.
            switch (variable)
            {
                // Stars
                case Constants.V_SOLAR_MASSES:
                    typeToCreate = typeof(SolarMassesRecord);
                    break;
                case Constants.V_SOLAR_RADIUS:
                    typeToCreate = typeof(SolarRadiusRecord);
                    break;

                // Planets
                case Constants.V_EARTH_MASSES:
                    typeToCreate = typeof(EarthMassesRecord);
                    break;
                case Constants.V_PLANETARY_RADIUS:
                    typeToCreate = typeof(PlanetaryRadiusRecord);
                    break;
                case Constants.V_SURFACE_GRAVITY:
                    typeToCreate = typeof(SurfaceGravityRecord);
                    break;
                case Constants.V_SURFACE_PRESSURE:
                    typeToCreate = typeof(SurfacePressureRecord);
                    break;

                // Both Planets & Stars (these require useExplicitTable = true)
                case Constants.V_SURFACE_TEMPERATURE:
                    typeToCreate = typeof(SurfaceTemperatureRecord);
                    useExplicitTable = true;
                    break;
                case Constants.V_ECCENTRICITY:
                    typeToCreate = typeof(OrbitalEccentricityRecord);
                    useExplicitTable = true;
                    break;
                case Constants.V_ORBITAL_PERIOD:
                    typeToCreate = typeof(OrbitalPeriodRecord);
                    useExplicitTable = true;
                    break;


                // Rings


                // System-wide / Aggregate
            }

            if (typeToCreate != null)
            {
                object[] args;
                if (useExplicitTable)
                {
                    args = new object[] {
                        settings, table, journalType, type,
                        Int64.Parse(csvFields[CSV_MinCount]), Double.Parse(csvFields[CSV_MinValue]), csvFields[CSV_MinBody],
                        Int64.Parse(csvFields[CSV_MaxCount]), Double.Parse(csvFields[CSV_MaxValue]), csvFields[CSV_MaxBody]
                    };
                }
                else
                {
                    args = new object[] {
                        settings, journalType, type,
                        Int64.Parse(csvFields[CSV_MinCount]), Double.Parse(csvFields[CSV_MinValue]), csvFields[CSV_MinBody],
                        Int64.Parse(csvFields[CSV_MaxCount]), Double.Parse(csvFields[CSV_MaxValue]), csvFields[CSV_MaxBody]
                    };
                }
                return (IRecord)Activator.CreateInstance(typeToCreate,args);
            }
            return null;
        }

        private static RecordTable RecordTableFromString(string str)
        {
            switch (str)
            {
                case "planets":
                    return RecordTable.Planets;
                case "rings":
                    return RecordTable.Rings;
                case "stars":
                    return RecordTable.Stars;
                default:
                    Debug.WriteLine("Unknown table value found in galactic records csv file: " + str);
                    break;
            }
            return RecordTable.Unknown;
        }
    }
}
