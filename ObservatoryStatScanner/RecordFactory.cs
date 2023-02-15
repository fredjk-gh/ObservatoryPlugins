using ObservatoryStatScanner.Records;
using System.Diagnostics;

namespace ObservatoryStatScanner
{
    internal class RecordFactory
    {

        public static IRecord CreateRecord(string[] csvFields, StatScannerSettings settings, RecordKind recordKind)
        {
            CSVData data = new(csvFields);
            if (!data.IsValid) return null;

            Type typeToCreate = null;

            switch (data.Variable)
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

                // Both Planets & Stars
                case Constants.V_SURFACE_TEMPERATURE:
                    typeToCreate = typeof(SurfaceTemperatureRecord);
                    break;
                case Constants.V_ECCENTRICITY:
                    typeToCreate = typeof(OrbitalEccentricityRecord);
                    break;
                case Constants.V_ORBITAL_PERIOD:
                    typeToCreate = typeof(OrbitalPeriodRecord);
                    break;


                // Rings


                // System-wide / Aggregate
            }

            if (typeToCreate != null)
            {
                object[] args = new object[] { settings, recordKind, data };
                return (IRecord)Activator.CreateInstance(typeToCreate,args);
            }
            return null;
        }
    }
}
