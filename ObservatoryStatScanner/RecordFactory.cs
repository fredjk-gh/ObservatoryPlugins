using ObservatoryStatScanner.Records;
using System.Diagnostics;

namespace ObservatoryStatScanner
{
    internal class RecordFactory
    {
        // CSV data-based factory.
        public static IRecord CreateRecord(string[] csvFields, StatScannerSettings settings, RecordKind recordKind)
        {
            bool supportedVariable = false;

            switch (csvFields[Constants.CSV_Variable])
            {
                // Stars
                case Constants.V_SOLAR_MASSES:
                case Constants.V_SOLAR_RADIUS:

                // Planets
                case Constants.V_EARTH_MASSES:
                case Constants.V_PLANETARY_RADIUS:
                case Constants.V_SURFACE_GRAVITY:
                case Constants.V_SURFACE_PRESSURE:

                // Both Planets & Stars
                case Constants.V_SURFACE_TEMPERATURE:
                case Constants.V_ECCENTRICITY:
                case Constants.V_ORBITAL_PERIOD:
                case Constants.V_ROTATIONAL_PERIOD:

                // Rings
                case Constants.V_RING_OUTER_RADIUS:
                case Constants.V_RING_WIDTH:
                case Constants.V_RING_MASS:
                case Constants.V_RING_DENSITY:
                    supportedVariable = true;
                    break;
            }

            if (supportedVariable)
            {
                // Don't parse stuff if we don't need to!
                CSVData recordData = new(csvFields);
                if (!recordData.IsValid) return null;
                // Skip records with a zero min/max. They are pointless and a waste to process.
                if (recordData.MaxValue == 0.0 && recordData.MinValue == 0) return null;

                return CreateRecord(recordData, settings, recordKind);
            }
            return null;
        }

        // General factory; handles all kinds of records.
        public static IRecord CreateRecord(IRecordData recordData, StatScannerSettings settings, RecordKind recordKind = RecordKind.Personal)
        {
            if (!settings.EnablePersonalBests && recordKind == RecordKind.Personal) return null;

            Type typeToCreate = null;

            switch (recordData.Variable)
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
                case Constants.V_BODY_BIO_COUNT:
                    typeToCreate = typeof(PlanetaryOdysseyBiosRecord);
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
                case Constants.V_ROTATIONAL_PERIOD:
                    typeToCreate = typeof(RotationalPeriodRecord);
                    break;


                // Rings
                case Constants.V_RING_OUTER_RADIUS:
                    typeToCreate = typeof(RingOuterRadiusRecord);
                    break;
                case Constants.V_RING_WIDTH:
                    typeToCreate = typeof(RingWidthRecord);
                    break;
                case Constants.V_RING_MASS:
                    typeToCreate = typeof(RingMassRecord);
                    break;
                case Constants.V_RING_DENSITY:
                    typeToCreate = typeof(RingDensityRecord);
                    break;

                // Aggregate
                //
                // System-wide
                case Constants.V_SYS_BIO_COUNT:
                    typeToCreate = typeof(SystemOdysseyBiosRecord);
                    break;
                case Constants.V_SYS_BODY_COUNT:
                    typeToCreate = typeof(SystemBodyCountRecord);
                    break;
                case Constants.V_SYS_UNDISCOVERED_COUNT:
                    typeToCreate = typeof(UndiscoveredSystemTally);
                    break;

                // Regional
                case Constants.V_VISITED_REGIONS_COUNT:
                    typeToCreate = typeof(RegionsVisitedTally);
                    break;
                case Constants.V_CODEX_CATEGORY_BIO_GEO:
                case Constants.V_CODEX_CATEGORY_XENO:
                case Constants.V_CODEX_CATEGORY_ASTRO:
                    typeToCreate = typeof(RegionCodexCount);
                    break;
            }

            if (typeToCreate != null)
            {
                if (!recordData.IsValid) return null;

                object[] args = new object[] { settings, recordKind, recordData };
                return (IRecord)Activator.CreateInstance(typeToCreate, args);
            }
            return null;
        }
    }
}
