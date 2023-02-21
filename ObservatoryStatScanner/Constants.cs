using ObservatoryStatScanner.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ObservatoryStatScanner
{
    internal class Constants
    {
        // Galactic Records source-of-truth -- combined and procgen only versions.
        public const string EDASTRO_GALACTIC_RECORDS_CSV_URL = "https://edastro.com/mapcharts/files/galactic-records.csv";
        public const string EDASTRO_GALACTIC_RECORDS_PG_CSV_URL = "https://edastro.com/mapcharts/files/galactic-records-procgen.csv";

        // Local filenames:
        public const string LOCAL_GALACTIC_RECORDS_FILE = "galactic_records.csv";
        public const string LOCAL_GALACTIC_PROCGEN_RECORDS_FILE = "galactic_records_procgen.csv";
        public const string GOOD_BACKUP_EXT = ".good";

        // CSV Files are quoted CSV format with columns:
        public static readonly string[] ExpectedFields = new string[] {
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
            };

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

        // Conversion constants (divisor or factor).
        public const double CONV_MperS2_TO_G_DIVISOR = 9.81;
        public const double CONV_M_TO_SOLAR_RAD_DIVISOR = 696340000.0;
        public const double CONV_M_TO_KM_DIVISOR = 1000.0;
        public const double CONV_M_TO_LS_DIVISOR = 299792458.0;
        public const double CONV_S_TO_DAYS_DIVISOR = 86400.0;
        public const double CONV_PA_TO_ATM_DIVISOR = 101325.0;

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

        public const string V_BODY_BIO_COUNT = "bodyBioCount";
        public const string V_SYS_BIO_COUNT = "sysBioCount";
        public const string V_SYS_BODY_COUNT = "sysBodyCount";

        // Some of the object class names from the list below. These are used
        // in multiple places so are pulled into constants to avoid duplication/drift.
        // These particular ones were problematic in certain records because of bad
        // data and were used to filter out this bad data.
        public const string EDASTRO_STAR_Y_DWARF = "Y (Brown dwarf) Star";
        public const string EDASTRO_METAL_RICH_BODY = "Metal-rich body";
        public const string EDASTRO_STAR_BLACK_HOLE = "Black Hole";

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

        // Key: EDSM body type => Value: Journal body type.
        public static Dictionary<string, string> JournalTypeMap = new()
        {
            // RecordType: planets
            { "Ammonia world", "Ammonia world" },
            { "Class I gas giant", "Sudarsky class I gas giant" },
            { "Class II gas giant", "Sudarsky class II gas giant" },
            { "Class III gas giant", "Sudarsky class III gas giant" },
            { "Class IV gas giant", "Sudarsky class IV gas giant" },
            { "Class V gas giant", "Sudarsky class V gas giant" },
            { "Earth-like world ProcGen", SCAN_EARTHLIKE },
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

        public const string PROCGEN_NAME_RE = @"\s+[A-Z][A-Z]-[A-Z]\s+[a-z]\d+(-\d+)?";
        public static readonly Regex RE = new Regex(PROCGEN_NAME_RE);

        // Pseudo EDAstro/Journal Object type names for aggregate personal bests.
        public const string OBJECT_TYPE_SYSTEM = "System";
        public const string OBJECT_TYPE_ODYSSEY_PLANET = "Odyssey Landable Planet";

        // Aggregate personal best data definitions (since these can't be derived from an EDAstro equivelent).
        public static readonly PersonalBestData PB_SystemOdysseyBiosData =
            new PersonalBestData(RecordTable.Systems, OBJECT_TYPE_SYSTEM, V_SYS_BIO_COUNT);
        public static readonly PersonalBestData PB_SystemBodyCountData =
            new PersonalBestData(RecordTable.Systems, OBJECT_TYPE_SYSTEM, V_SYS_BODY_COUNT);
        public static readonly PersonalBestData PB_BodyBiosData =
            new PersonalBestData(RecordTable.Planets, OBJECT_TYPE_ODYSSEY_PLANET, V_BODY_BIO_COUNT);

        // An iterable list of the above data objects.
        public static readonly List<PersonalBestData> PB_DataObjects = new()
        {
            { PB_SystemOdysseyBiosData },
            { PB_SystemBodyCountData },
            { PB_BodyBiosData },
        };

        public static readonly List<Tuple<RecordTable, string>> PB_RecordTypesForFssScans = new()
        {
            { Tuple.Create(RecordTable.Planets, OBJECT_TYPE_ODYSSEY_PLANET)  },
            { Tuple.Create(RecordTable.Systems, OBJECT_TYPE_SYSTEM) },
        };

        // Some frequently used UI strings. Some (indicated by UI_FS_ prefix) are format strings!
        public const string UI_POTENTIAL_NEW_RECORD = "Potential new record";
        public const string UI_NEW_PERSONAL_BEST = "New personal best";
        public const string UI_FS_TIED_RECORD_COUNT = "Tied record (with ~{0} others)";
        public const string UI_FS_NEAR_RECORD_COUNT = "Near-record (within {0}%)";
        public const string UI_RECORD_HOLDER_VISITED = "Visited existing record";
        public const string UI_FIRST_DISCOVERY = "1st Discovery";
        public const string UI_ALREADY_DISCOVERED = "Discovered";
    }
}
