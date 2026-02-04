using com.github.fredjk_gh.PluginCommon.Data.Id64;
using Observatory.Framework.Files.ParameterTypes;

namespace com.github.fredjk_gh.ObservatoryHelm.Data
{
    internal class SuppressionSpec(
        string name,
        string description,
        Func<StarPosition, bool> Calc)
    {
        public static SuppressionSpec OType = new(
            "O-type Stars",
            "Math.Abs(x) < 1500 || Math.Abs(z) < 1500",
            (p) =>
        {
            return Math.Abs(p.x) < 1500 || Math.Abs(p.z) < 1500;
        });
        public static SuppressionSpec BType = new(
            "B-type Stars",
            "Math.Abs(x) < 500 || Math.Abs(z) < 500",
            (p) =>
        {
            return Math.Abs(p.x) < 500 || Math.Abs(p.z) < 500;
        });
        public static SuppressionSpec Carbon = new(
            "Carbon Stars",
            "Math.Abs(x) < 1000 || Math.Abs(y) < 1000 || Math.Abs(z) < 1000",
            (p) =>
        {
            return Math.Abs(p.x) < 1000 || Math.Abs(p.y) < 1000 || Math.Abs(p.z) < 1000;
        });
        public static SuppressionSpec WolfRayet = new(
            "Wolf-Rayet Stars",
            "Math.Abs(x) < 1500 || Math.Abs(z) < 2500",
            (p) =>
        {
            return Math.Abs(p.x) < 1500 || Math.Abs(p.z) < 2500;
        });
        public static SuppressionSpec NS = new(
            "Neutron Stars",
            @"Full suppression: (Math.Abs(x) < 500 || Math.Abs(z) < 500)
Partial suppression: (Math.Abs(x) < 1000 || Math.Abs(y) < 1000 || Math.Abs(z) < 1000)",
            (p) =>
        {
            return (Math.Abs(p.x) < 500 || Math.Abs(p.z) < 500) // Full suppression
                || (Math.Abs(p.x) < 1000 || Math.Abs(p.y) < 1000 || Math.Abs(p.z) < 1000); // Partial
        });
        public static SuppressionSpec BH = new(
            "Black Holes",
            @"Full suppression: (Math.Abs(x) < 500 || Math.Abs(z) < 500)
Partial suppression: (Math.Abs(x) < 1000 || Math.Abs(y) < 1000 || Math.Abs(z) < 1000)",
            (p) =>
        {
            return (Math.Abs(p.x) < 500 || Math.Abs(p.z) < 500) // Full suppression
                || (Math.Abs(p.x) < 1000 || Math.Abs(p.y) < 1000 || Math.Abs(p.z) < 1000); // Partial
        });
        public static SuppressionSpec WD = new(
            "White Dwarf Stars",
            "Math.Abs(x) < 1000 || Math.Abs(y) < 1000 || Math.Abs(z) < 1000",
            (p) =>
        {
            return Math.Abs(p.x) < 1000 || Math.Abs(p.y) < 1000 || Math.Abs(p.z) < 1000;
        });

        public static SuppressionSpec Helium = new(
            "Helium suppression zone",
            @"Distance( (0,  0, 27500), p) < 7000 || Distance( (-3500, 0, 24000), p) < 6000",
            (p) =>
        {
            return (Id64CoordHelper.Distance(new() { x = 0, y = 0, z = 27500 }, p) < 7000
                    || Id64CoordHelper.Distance(new() { x = -3500, y = 0, z = 24000 }, p) < 6000);
        });

        public static SuppressionSpec Bubble = new(
            "The Bubble: core systems centered around Sol",
            @"Math.Abs(x) < BubbleRadius && Math.Abs(y) < BubbleRadius && Math.Abs(z) < BubbleRadius
... where BubbleRadius is configured in settings; default 700 Ly.",
            (p) =>
            {
                var radius = HelmContext.Instance.Settings.BubbleRadiusLy;
                return Math.Abs(p.x) < radius && Math.Abs(p.y) < radius && Math.Abs(p.z) < radius;
            });

        public static List<SuppressionSpec> StarType =
        [
            OType,
            BType,
            Carbon,
            WolfRayet,
            NS,
            BH,
            WD,
        ];

        public bool Process(StarPosition pos, out string detail)
        {
            if (Calc(pos))
            {
                detail = name;
                return true;
            }

            detail = string.Empty;
            return false;
        }

        public string Name { get => name; }
        public string Description { get => description; }
    }
}
