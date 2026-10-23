using System.Text.RegularExpressions;

namespace com.github.fredjk_gh.PluginCommon.Data.Journals
{
    public static partial class JournalConstants
    {
        public const string SIGNAL_BIO = "$SAA_SignalType_Biological;";
        public const string SIGNAL_GEO = "$SAA_SignalType_Geological;";
        public const string SIGNAL_PLANETARY_MINING = "$PlanetaryMiningLocation_Name;";

        public const string MATERIAL_CONTENT_LOW = "$AsteroidMaterialContent_Low;";
        public const string MATERIAL_CONTENT_MED = "$AsteroidMaterialContent_Medium;";
        public const string MATERIAL_CONTENT_HIGH = "$AsteroidMaterialContent_High;";

        public const string V_CODEX_CATEGORY_BIO_GEO = "regionCodexBiologicalGeological";
        public const string V_CODEX_CATEGORY_XENO = "regionCodexXenological";
        public const string V_CODEX_CATEGORY_ASTRO = "regionCodexAstronomicalBodies";

        public static readonly Dictionary<string, string> CodexCategoriesByJournalId = new()
        {
            { "$Codex_Category_Biology;", V_CODEX_CATEGORY_BIO_GEO },
            { "$Codex_Category_Civilisations;", V_CODEX_CATEGORY_XENO },
            { "$Codex_Category_StellarBodies;", V_CODEX_CATEGORY_ASTRO },
        };

        public const string JOURNAL_ELW = "Earthlike body";
        public const string JOURNAL_AW = "Ammonia world";
        public const string JOURNAL_WW = "Water world";

        public static readonly IList<string> HighValueNonTerraformablePlanetClasses = [
            JOURNAL_ELW,
            JOURNAL_AW,
            JOURNAL_WW,
        ];

        public static readonly Dictionary<string, string> JournalTypeAbbreviation = new()
        {
            // Planets
            { JOURNAL_AW, "AW" },
            { "Sudarsky class I gas giant", "S-I" },
            { "Sudarsky class II gas giant", "S-II" },
            { "Sudarsky class III gas giant", "S-III" },
            { "Sudarsky class IV gas giant", "S-IV" },
            { "Sudarsky class V gas giant", "S-V" },
            { JOURNAL_ELW, "ELW" },
            { "Gas giant with ammonia based life", "GAL" },
            { "Gas giant with water based life", "GWL" },
            { "Helium gas giant", "HG" },
            { "Helium rich gas giant", "HRG" },
            { "High metal content body", "HMC" },
            { "Icy body", "IB" },
            { "Metal rich body", "MR" },
            { "Rocky ice body", "RIB" },
            { "Rocky body", "RB" },
            { "Water giant", "WG" },
            { JOURNAL_WW, "WW" },
            // Stars
            { "A_BlueWhiteSuperGiant", "A SG" },
            { "A", "A" },
            { "B_BlueWhiteSuperGiant", "B SG" }, // Undocumented, but present in my journals.
            { "B", "B" },
            { "H", "BH" },
            { "SupermassiveBlackHole", "Super BH" },
            { "C", "C" },
            { "CJ", "CJ" },
            { "CN", "﻿CN" },
            { "F_WhiteSuperGiant", "F SG" },
            { "F", "F" },
            { "G_WhiteSuperGiant", "G SG" }, // Undocumented, but Orvidius has seen it!
            { "G", "G" },
            { "AeBe", "AeBe" },
            { "K_OrangeGiant", "K G" },
            { "K", "K" },
            { "L", "L" },
            { "M", "M" },
            { "M_RedGiant", "M G" },
            { "M_RedSuperGiant", "M SG" },
            { "MS", "MS" },
            { "N", "N" },
            { "O", "O" },
            { "S", "S" },
            { "T", "T" },
            { "TTS", "TTS" },
            { "D", "D" },
            { "DA", "DA" },
            { "DAB", "DAB" },
            { "DAV", "DAV" },
            { "DAZ", "DAZ" },
            { "DB", "DB" },
            { "DBV", "DBV" },
            { "DBZ", "DBZ" },
            { "DC", "DC" },
            { "DCV", "DCV" },
            { "DQ", "DQ" },
            { "WC", "WC" },
            { "WN", "WN" },
            { "WNC", "WNC" },
            { "WO", "WO" },
            { "W", "W" },
            { "Y", "Y" },
            // Rings
            { "eRingClass_Icy", "Icy" },
            { "eRingClass_MetalRich", "Metal Rich" },
            { "eRingClass_Metalic", "Metallic" },
            { "eRingClass_Rocky", "Rocky" },
        };

        public static readonly HashSet<string> Scoopables = [
            "O", "B", "A", "F", "G", "K", "M",
            "A_BlueWhiteSuperGiant", "B_BlueWhiteSuperGiant", "F_WhiteSuperGiant",
            "G_WhiteSuperGiant", "K_OrangeGiant", "M_RedSuperGiant", "M_RedGiant" ];

        public static bool IsRing(string bodyName)
        {
            return bodyName?.Contains(" Ring") ?? false;
        }

        public static bool IsBelt(string bodyName)
        {
            return bodyName?.Contains(" Belt") ?? false;
        }

        public static bool IsTerraformed(string terraformState)
        {
            return "terraformed".Equals(terraformState, StringComparison.OrdinalIgnoreCase);
        }

        public const string PROCGEN_NAME_RE_STRING = @"\s+[A-Z][A-Z]-[A-Z]\s+[a-z]\d+(-\d+)?";
        public static readonly Regex PROCGEN_NAME_RE = ProcGenNameRegex();

        [GeneratedRegex(PROCGEN_NAME_RE_STRING)]
        public static partial Regex ProcGenNameRegex();
    }
}
