
namespace com.github.fredjk_gh.ObservatoryProspectorBasic
{
    internal class MaterialData
    {
        public enum Grade
        {
            VeryCommon,
            Common,
            Standard,
            Rare,
            VeryRare
        }

        public static readonly Dictionary<Grade, int> GradeCapacity = new()
        {
            { Grade.VeryCommon, 300 },
            { Grade.Common, 250 },
            { Grade.Standard, 200 },
            { Grade.Rare, 150 },
            { Grade.VeryRare, 100 },
        };

        public static readonly Dictionary<string, Grade> RawMaterialGrade = new()
        {
            { "carbon", Grade.VeryCommon },
            { "iron", Grade.VeryCommon },
            { "lead", Grade.VeryCommon },
            { "nickel", Grade.VeryCommon },
            { "phosphorus", Grade.VeryCommon },
            { "rhenium", Grade.VeryCommon },
            { "sulphur", Grade.VeryCommon },
            { "arsenic", Grade.Common },
            { "chromium", Grade.Common },
            { "germanium", Grade.Common },
            { "manganese", Grade.Common },
            { "vanadium", Grade.Common },
            { "zinc", Grade.Common },
            { "zirconium", Grade.Common },
            { "boron", Grade.Standard },
            { "cadmium", Grade.Standard },
            { "mercury", Grade.Standard },
            { "molybdenum", Grade.Standard },
            { "niobium", Grade.Standard },
            { "tin", Grade.Standard },
            { "tungsten", Grade.Standard },
            { "antimony", Grade.Rare },
            { "polonium", Grade.Rare },
            { "ruthenium", Grade.Rare },
            { "selenium", Grade.Rare },
            { "technetium", Grade.Rare },
            { "tellurium", Grade.Rare },
            { "yttrium", Grade.Rare },
        };

        public static int MaterialCapacity(string name)
        {
            return GradeCapacity[RawMaterialGrade[name.ToLower()]];
        }

        public static bool IsRawMat(string name)
        {
            return RawMaterialGrade.ContainsKey(name.ToLower());
        }
    }
}