using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryProspectorBasic
{
    internal class SynthRecipes
    {
        public enum SynthLevel
        {
            Basic,
            Standard,
            Premium,
        }

        public readonly static Dictionary<SynthLevel, string> SynthLevelSymbols = new()
        {
            { SynthLevel.Basic, "B" },
            { SynthLevel.Standard, "S" },
            { SynthLevel.Premium, "P" },
        };

        public readonly static Dictionary<SynthLevel, HashSet<string>> FSDBoost = new()
        {
            { SynthLevel.Basic, new () { { "carbon" }, { "vanadium" }, { "germanium" }, } },
            { SynthLevel.Standard, new () { { "carbon" }, { "vanadium" }, { "germanium" }, { "cadmium" }, { "niobium" }, } },
            { SynthLevel.Premium, new () { { "carbon" }, { "germanium" }, { "arsenic" }, { "niobium" }, { "yttrium" }, { "polonium" }, } },
        };

        public readonly static Dictionary<SynthLevel, HashSet<string>> AFMURefill = new()
        {
            { SynthLevel.Basic, new () { { "nickel" }, { "zinc" }, { "chromium" }, { "vanadium" }, } },
            { SynthLevel.Standard, new () { { "Tin" }, { "manganese" }, { "vanadium" }, { "molybdenum" }, { "zirconium" }, } },
            { SynthLevel.Premium, new () { { "zinc" }, { "chromium" }, { "vanadium" }, { "zirconium" }, { "tellurium" }, { "ruthenium" }, } },
        };

        public readonly static Dictionary<SynthLevel, HashSet<string>> SRVRefuel = new()
        {
            { SynthLevel.Basic, new () { { "sulphur" }, { "phosphorus" }, } },
            { SynthLevel.Standard, new () { { "sulphur" }, { "phosphorus" }, { "arsenic" }, { "mercury" }, } },
            { SynthLevel.Premium, new () { { "sulphur" }, { "arsenic" }, { "mercury" }, { "technetium" }, } },
        };

        public readonly static Dictionary<SynthLevel, HashSet<string>> SRVRepair = new()
        {
            { SynthLevel.Basic, new () { { "iron" }, { "nickel" }, } },
            { SynthLevel.Standard, new () { { "nickel" }, { "manganese" }, { "vanadium" }, { "molybdenum" }, } },
            { SynthLevel.Premium, new () { { "zinc" }, { "chromium" }, { "vanadium" }, { "tungsten" }, { "tellurium" }, } },
        };
    }
}
