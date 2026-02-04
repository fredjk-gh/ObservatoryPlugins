using com.github.fredjk_gh.ObservatoryStatScanner.Records.Data;
using com.github.fredjk_gh.ObservatoryStatScanner.Records.Interfaces_BaseClasses;
using com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs;
using System.Text.RegularExpressions;

namespace com.github.fredjk_gh.ObservatoryStatScanner
{
    internal partial class Constants
    {
        public const string STATSCANNER_WIKI_URL = "https://github.com/fredjk-gh/ObservatoryPlugins/wiki/Plugin:-Stat-Scanner";

        // Galactic Records source-of-truth -- combined and procgen only versions.
        public const string EDASTRO_GALACTIC_RECORDS_CSV_URL = "https://edastro.com/mapcharts/files/galactic-records.csv";
        public const string EDASTRO_GALACTIC_RECORDS_PG_CSV_URL = "https://edastro.com/mapcharts/files/galactic-records-procgen.csv";

        // Local filenames:
        public const string LOCAL_GALACTIC_RECORDS_FILE = "galactic_records.csv";
        public const string LOCAL_GALACTIC_PROCGEN_RECORDS_FILE = "galactic_records_procgen.csv";
        public const string GOOD_BACKUP_EXT = ".good";

        // CSV Files are quoted CSV format with columns:
        public static readonly string[] ExpectedFields =
            [
                "Type",
                "Variable",
                "Max Count",
                "Max Value",
                "Max Body",
                "Min Count",
                "Min Value",
                "Min Body",
                "Average",
                "Standard Deviation",
                "Count",
                "Table"
            ];

        // Field indexes
        public const int CSV_Type = 0;
        public const int CSV_Variable = 1;
        public const int CSV_MaxCount = 2;
        public const int CSV_MaxValue = 3;
        public const int CSV_MaxBody = 4;
        public const int CSV_MinCount = 5;
        public const int CSV_MinValue = 6;
        public const int CSV_MinBody = 7;
        public const int CSV_Average = 8;
        public const int CSV_StandardDeviation = 9;
        public const int CSV_Count = 10;
        public const int CSV_Table = 11;

        // Orvidius has indicated they use 12 digit precision. Matching
        // that here for more stable comparisons (particularly when applying
        // "near record" thresholds).
        public const int EDASTRO_PRECISION = 12;

        // Variable names from the edastro CSV.
        public const string V_SOLAR_MASSES = "solarMasses";
        public const string V_SOLAR_RADIUS = "solarRadius";

        public const string V_EARTH_MASSES = "earthMasses";
        public const string V_PLANETARY_RADIUS = "radius";
        public const string V_SURFACE_GRAVITY = "gravity";
        public const string V_SURFACE_PRESSURE = "surfacePressure";

        public const string V_SURFACE_TEMPERATURE = "surfaceTemperature";
        public const string V_ECCENTRICITY = "orbitalEccentricity";
        public const string V_ORBITAL_PERIOD = "orbitalPeriod";
        public const string V_ROTATIONAL_PERIOD = "rotationalPeriod";

        public const string V_RING_OUTER_RADIUS = "outerRadius";
        public const string V_RING_WIDTH = "width";
        public const string V_RING_MASS = "mass";
        public const string V_RING_DENSITY = "density";

        // Personal best variable names
        public const string V_BODY_BIO_COUNT = "bodyBioCount";
        public const string V_BODY_TYPE_COUNT = "bodyTypeCount";
        public const string V_BODY_ELW_SIMILARITY = "earthSimilarityScore";
        public const string V_BODY_MARS_SIMILARITY = "marsSimilarityScore";
        public const string V_SYS_BIO_COUNT = "sysBioCount";
        public const string V_SYS_BODY_COUNT = "sysBodyCount";
        public const string V_SYS_UNDISCOVERED_COUNT = "sysUndiscoveredCount";

        public const string V_VISITED_REGIONS_COUNT = "regionsVisitedCount";

        // Some of the object class names from the list below. These are used
        // in multiple places so are pulled into constants to avoid duplication/drift.
        // These particular ones were problematic in certain records because of bad
        // data and were used to filter out this bad data.
        public const string EDASTRO_STAR_Y_DWARF = "Y (Brown dwarf) Star";
        public const string EDASTRO_METAL_RICH_BODY = "Metal-rich body";
        public const string EDASTRO_STAR_BLACK_HOLE = "Black Hole";
        public const string EDASTRO_STAR_SM_BLACK_HOLE = "Supermassive BH";
        public const string EDASTRO_ELW_PROCGEN = "Earth-like world ProcGen";

        // Values from the Scan which are used in multiple places.
        public const string SCAN_EARTHLIKE = "Earthlike body";
        public const string SCAN_ICY_BODY = "Icy body";
        public const string SCAN_HIGH_METAL_CONTENT_BODY = "High metal content body";
        public const string SCAN_METAL_RICH_BODY = "Metal rich body";
        public const string SCAN_ROCKY_ICE_BODY = "Rocky ice body";
        public const string SCAN_ROCKY_BODY = "Rocky body";
        public const string SCAN_BARYCENTRE = "Barycentre";

        public const string SCAN_TYPE_NAV_BEACON = "NavBeaconDetail";

        public const string FSS_BODY_SIGNAL_BIOLOGICAL = "$SAA_SignalType_Biological;";

        // Key: EDAstro body type => Value: Journal body type.
        public static Dictionary<string, string> JournalTypeMap = new()
        {
            // RecordType: planets
            { "Ammonia world", "Ammonia world" },
            { "Class I gas giant", "Sudarsky class I gas giant" },
            { "Class II gas giant", "Sudarsky class II gas giant" },
            { "Class III gas giant", "Sudarsky class III gas giant" },
            { "Class IV gas giant", "Sudarsky class IV gas giant" },
            { "Class V gas giant", "Sudarsky class V gas giant" },
            { EDASTRO_ELW_PROCGEN, SCAN_EARTHLIKE },
            { "Gas giant with ammonia-based life", "Gas giant with ammonia based life" },
            { "Gas giant with water-based life", "Gas giant with water based life" },
            { "Helium gas giant", "Helium gas giant" },
            { "Helium-rich gas giant", "Helium rich gas giant" },
            { "High metal content world", SCAN_HIGH_METAL_CONTENT_BODY },
            { "Icy body", SCAN_ICY_BODY },
            { EDASTRO_METAL_RICH_BODY, SCAN_METAL_RICH_BODY },
            { "Rocky Ice world", SCAN_ROCKY_ICE_BODY },
            { "Rocky body", SCAN_ROCKY_BODY },
            { "Water giant", "Water giant" },
            { "Water world", "Water world" },
            // RecordType: stars
            { "A (Blue-White super giant) Star", "A_BlueWhiteSuperGiant" },
            { "A (Blue-White) Star", "A" },
            { "B (Blue-White super giant) Star", "B_BlueWhiteSuperGiant" }, // Undocumented, but present in my journals.
            { "B (Blue-White) Star", "B" },
            { EDASTRO_STAR_BLACK_HOLE, "H" },
            { EDASTRO_STAR_SM_BLACK_HOLE, "SupermassiveBlackHole" },
            { "C Star", "C" },
            { "CJ Star", "CJ" },
            { "CN Star", "﻿CN" },
            { "F (White super giant) Star", "F_WhiteSuperGiant" },
            { "F (White) Star", "F" },
            { "G (White-Yellow super giant) Star", "G_WhiteSuperGiant" }, // Undocumented, but Orvidius has seen it!
            { "G (White-Yellow) Star", "G" },
            { "Herbig Ae/Be Star", "AeBe" },
            { "K (Yellow-Orange giant) Star", "K_OrangeGiant" },
            { "K (Yellow-Orange) Star", "K" },
            { "L (Brown dwarf) Star", "L" },
            { "M (Red dwarf) Star", "M" },
            { "M (Red giant) Star", "M_RedGiant" },
            { "M (Red super giant) Star", "M_RedSuperGiant" },
            { "MS-type Star", "MS" },
            { "Neutron Star", "N" },
            { "O (Blue-White) Star", "O" },
            { "S-type Star", "S" },
            { "T (Brown dwarf) Star", "T" },
            { "T Tauri Star", "TTS" },
            { "White Dwarf (D) Star", "D" },
            { "White Dwarf (DA) Star", "DA" },
            { "White Dwarf (DAB) Star", "DAB" },
            { "White Dwarf (DAV) Star", "DAV" },
            { "White Dwarf (DAZ) Star", "DAZ" },
            { "White Dwarf (DB) Star", "DB" },
            { "White Dwarf (DBV) Star", "DBV" },
            { "White Dwarf (DBZ) Star", "DBZ" },
            { "White Dwarf (DC) Star", "DC" },
            { "White Dwarf (DCV) Star", "DCV" },
            { "White Dwarf (DQ) Star", "DQ" },
            { "Wolf-Rayet C Star", "WC" },
            { "Wolf-Rayet N Star", "WN" },
            { "Wolf-Rayet NC Star", "WNC" },
            { "Wolf-Rayet O Star", "WO" },
            { "Wolf-Rayet Star", "W" },
            { EDASTRO_STAR_Y_DWARF, "Y" },
            // RecordType: rings
            { "Icy Ring", "eRingClass_Icy" },
            { "Metal Rich Ring", "eRingClass_MetalRich" },
            { "Metallic Ring", "eRingClass_Metalic" },
            { "Rocky Ring", "eRingClass_Rocky" },
        };

        public const string REGION_DEFAULT = "Inner Orion Spur";

        // Pseudo EDAstro/Journal Object type names for aggregate personal bests.
        public const string OBJECT_TYPE_REGION = "Galactic Region";
        public const string OBJECT_TYPE_SYSTEM = "System";
        public const string OBJECT_TYPE_ODYSSEY_PLANET = "Odyssey Landable Planet";

        // Aggregate personal best data definitions (since these can't be derived from an EDAstro equivelent).
        public static readonly PersonalBestData PB_RegionsVisited =
            new(RecordTable.Regions, OBJECT_TYPE_REGION, V_VISITED_REGIONS_COUNT);

        public static readonly PersonalBestData PB_SystemOdysseyBiosData =
            new(RecordTable.Systems, OBJECT_TYPE_SYSTEM, V_SYS_BIO_COUNT);
        public static readonly PersonalBestData PB_SystemBodyCountData =
            new(RecordTable.Systems, OBJECT_TYPE_SYSTEM, V_SYS_BODY_COUNT);
        public static readonly PersonalBestData PB_UndiscoveredSystemTallyData =
            new(RecordTable.Systems, OBJECT_TYPE_SYSTEM, V_SYS_UNDISCOVERED_COUNT);

        public static readonly PersonalBestData PB_BodyBiosData =
            new(RecordTable.Planets, OBJECT_TYPE_ODYSSEY_PLANET, V_BODY_BIO_COUNT);
        public static readonly PersonalBestData PB_EarthSimilarityScoreData =
            new(RecordTable.Planets, EDASTRO_ELW_PROCGEN, V_BODY_ELW_SIMILARITY, SCAN_EARTHLIKE);
        public static readonly PersonalBestData PB_MarsSimilarityScoreData =
            new(RecordTable.Planets, EDASTRO_ELW_PROCGEN, V_BODY_MARS_SIMILARITY, SCAN_EARTHLIKE);

        public static List<PersonalBestData> GeneratePersonalBestRecords()
        {
            // A couple aggregate records:
            List<PersonalBestData> records = new()
            {
                { PB_SystemOdysseyBiosData },
                { PB_SystemBodyCountData },
                { PB_UndiscoveredSystemTallyData },
                { PB_BodyBiosData },
                { PB_EarthSimilarityScoreData },
                { PB_MarsSimilarityScoreData },
                { PB_RegionsVisited },
            };

            // Generate Codex Category per region records:
            foreach (var r in FDevIDs.RegionById.Values)
            {
                foreach (var codexCategory in FDevIDs.CodexCategoriesByJournalId.Values)
                {
                    records.Add(new(RecordTable.Codex, r.Name, codexCategory));
                }
            }

            // Generate BodyTypeTally records for each body type.
            foreach (var bodyType in JournalTypeMap)
            {
                if (bodyType.Key.Contains("Ring")) continue;
                var rt = RecordTable.Planets;
                if (bodyType.Key.Contains("Star") || bodyType.Key == EDASTRO_STAR_BLACK_HOLE || bodyType.Key == EDASTRO_STAR_SM_BLACK_HOLE)
                    rt = RecordTable.Stars;
                records.Add(new(rt, bodyType.Key, V_BODY_TYPE_COUNT, bodyType.Value));
            }

            return records;
        }

        public static readonly List<Tuple<RecordTable, string>> PB_RecordTypesForFssScans = new()
        {
            { Tuple.Create(RecordTable.Planets, SCAN_EARTHLIKE) },
            { Tuple.Create(RecordTable.Planets, OBJECT_TYPE_ODYSSEY_PLANET)  },
            { Tuple.Create(RecordTable.Systems, OBJECT_TYPE_SYSTEM) },
        };

        // Some frequently used UI strings. Some (indicated by UI_FS_ prefix) are format strings!
        public const string UI_POTENTIAL_NEW_RECORD = "Potential new record";
        public const string UI_NEW_PERSONAL_BEST = "New personal best";
        public const string UI_CURRENT_PERSONAL_BEST = "Current personal best";
        public const string UI_FS_TIED_RECORD_COUNT = "Tied record (with ~{0} others)";
        public const string UI_FS_NEAR_RECORD_COUNT = "Near-record (within {0}%)";
        public const string UI_RECORD_HOLDER_VISITED = "Visited existing record";
        public const string UI_CODEX_CONFIRMATION = "Codex Confirmation";
        public const string UI_FIRST_DISCOVERY = "1st Discovery";
        public const string UI_FIRST_VISIT = "First visit";
        public const string UI_ALREADY_DISCOVERED = "Discovered";
        public const string UI_DISCOVERY_STATE_ANY = "Any";
    }
}
