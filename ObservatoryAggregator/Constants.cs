using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryAggregator
{
    internal class Constants
    {
        public const string PLUGIN_SHORT_NAME = "Aggregator";
        public const string DETAIL_SEP = " | ";
        public const string PRIMARY_STAR = "Primary star";

        public const string JOURNAL_ELW = "Earthlike body";
        public const string JOURNAL_AW = "Ammonia world";
        public const string JOURNAL_WW = "Water world";

        public static readonly IList<string> HighValueNonTerraformablePlanetClasses = new string[] {
            JOURNAL_ELW,
            JOURNAL_AW,
            JOURNAL_WW,
        };

        public static Dictionary<string, string> JournalTypeMap = new()
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
    }
}
