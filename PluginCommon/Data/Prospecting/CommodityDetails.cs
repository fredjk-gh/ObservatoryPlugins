using System.Collections.ObjectModel;
using System.Text;

namespace com.github.fredjk_gh.PluginCommon.Data.Prospecting
{

    [Flags]
    public enum MiningMethod
    {
        Laser = 1,
        SubSurface = 2,
        Core = 4,
        Planetary = 8,
    }

    [Flags]
    public enum RingType
    {
        None = 0,
        Rocky = 1,
        MetalRich = 2,
        Metallic = 4,
        Icy = 8,
    }

    [Flags]
    public enum BodyType
    {
        None = 0,
        MetalRich = 1,
        HighMetalContent = 2,
        Rocky = 4,
        Icy = 8,
        RockyIcy = 16,
    }

    public enum Commodities
    {
        // Minerals:
        Alexandrite,
        Bastnasite,
        Bauxite,
        Benitoite,
        Bertrandite,
        Bromellite,
        Coltan,
        Deuterium,
        Diamond,
        Gallite,
        Grandidierite,
        Indite,
        Jadeite,
        Lepidolite,
        LithiumHydroxide,
        LowTemperatureDiamond,
        Magnesite,
        MethaneClathrate,
        MethanolMonohydrateCrystals,
        Monazite,
        Musgravite,
        Olivine,
        Opal,
        Painite,
        PericlaseDunite,
        QuartzPyroxenite,
        Rhodplumsite,
        Ruby,
        Rutile,
        Sapphire,
        Serendibite,
        Thortveitite,
        Uraninite,

        // Metals:
        Cobalt,
        Copper,
        Gold,
        Iridium,
        Lithium,
        Osmium,
        Palladium,
        Platinum,
        Praseodymium,
        Samarium,
        Silver,
        Tantalum,
        Thorium,
        Titanium,
        Uranium,

        // Chemicals:
        Helium,
        Helium3,
        HydrogenPeroxide,
        LiquidOxygen,
        Tritium,
        Water,
    }

    public enum CommodityType
    {
        Chemical,
        Metal,
        Mineral,
    }

    public class CommodityDetails
    {
        private readonly static ReadOnlyDictionary<Commodities, CommodityDetails> commodityDetails = new(new Dictionary<Commodities, CommodityDetails>()
        {
            // Minerals
            { Commodities.Alexandrite, CommodityDetails.Mineral(
                "Alexandrite",
                MiningMethod.Core | MiningMethod.Planetary,
                RingType.MetalRich | RingType.Rocky | RingType.Icy,
                BodyType.Rocky) },
            { Commodities.Bastnasite, CommodityDetails.Mineral("Bastnasite", MiningMethod.Planetary, BodyType.Rocky) },
            { Commodities.Bauxite, CommodityDetails.Mineral("Bauxite", MiningMethod.Laser, RingType.Rocky) },
            { Commodities.Benitoite, CommodityDetails.Mineral("Benitoite", MiningMethod.Core, RingType.MetalRich | RingType.Rocky) },
            { Commodities.Bertrandite, CommodityDetails.Mineral("Bertrandite", MiningMethod.Laser, RingType.MetalRich | RingType.Metallic) },
            { Commodities.Bromellite, CommodityDetails.Mineral("Bromellite", MiningMethod.Core | MiningMethod.Laser | MiningMethod.SubSurface, RingType.Icy) },
            { Commodities.Coltan, CommodityDetails.Mineral("Coltan", MiningMethod.Laser, RingType.MetalRich | RingType.Rocky) },
            { Commodities.Deuterium, CommodityDetails.Mineral("Deuterium", MiningMethod.Planetary, BodyType.Icy | BodyType.RockyIcy) },
            { Commodities.Diamond, CommodityDetails.Mineral("Diamond", MiningMethod.Planetary, BodyType.Rocky) },
            { Commodities.Gallite, CommodityDetails.Mineral("Gallite", MiningMethod.Laser, RingType.MetalRich | RingType.Metallic | RingType.Rocky) },
            { Commodities.Grandidierite, CommodityDetails.Mineral("Grandidierite", MiningMethod.Core | MiningMethod.Planetary, RingType.Metallic | RingType.Icy, BodyType.Rocky) },
            { Commodities.Indite, CommodityDetails.Mineral("Indite", MiningMethod.Laser, RingType.Metallic | RingType.MetalRich | RingType.Rocky) },
            { Commodities.Jadeite, CommodityDetails.Mineral("Jadeite", MiningMethod.Planetary, BodyType.Rocky) },
            { Commodities.Lepidolite, CommodityDetails.Mineral("Lepidolite", MiningMethod.Laser, RingType.MetalRich | RingType.Rocky) },
            { Commodities.LithiumHydroxide, CommodityDetails.Mineral("Lithium Hydroxide", MiningMethod.Laser, RingType.Icy) },
            { Commodities.LowTemperatureDiamond, CommodityDetails.Mineral(
                "Low Temperature Diamonds",
                MiningMethod.Core | MiningMethod.Laser | MiningMethod.SubSurface | MiningMethod.Planetary,
                RingType.Icy,
                BodyType.Icy | BodyType.Rocky) },
            { Commodities.Magnesite, CommodityDetails.Mineral("Magnesite", MiningMethod.Planetary, BodyType.Rocky) },
            { Commodities.MethaneClathrate, CommodityDetails.Mineral("Methane Clathrate", MiningMethod.Laser, RingType.Icy) },
            { Commodities.MethanolMonohydrateCrystals, CommodityDetails.Mineral(
                "Methanol Monohydrate Crystals",
                MiningMethod.Laser | MiningMethod.Planetary,
                RingType.Icy,
                BodyType.Icy | BodyType.RockyIcy) },
            { Commodities.Monazite, CommodityDetails.Mineral("Monazite", MiningMethod.Core | MiningMethod.Planetary, RingType.MetalRich | RingType.Rocky, BodyType.Rocky) },
            { Commodities.Musgravite, CommodityDetails.Mineral("Musgravite", MiningMethod.Core, RingType.Rocky) },
            { Commodities.Olivine, CommodityDetails.Mineral("Olivine", MiningMethod.Planetary, BodyType.Rocky | BodyType.RockyIcy) },
            { Commodities.Opal, CommodityDetails.Mineral("Void Opal", MiningMethod.Core, RingType.Icy) },
            { Commodities.Painite, CommodityDetails.Mineral("Painite", MiningMethod.Core | MiningMethod.Laser | MiningMethod.SubSurface, RingType.Metallic) },
            { Commodities.PericlaseDunite, CommodityDetails.Mineral("Periclase Dunite", MiningMethod.Planetary, BodyType.Rocky) },
            { Commodities.QuartzPyroxenite, CommodityDetails.Mineral("Quartz Pyroxenite ", MiningMethod.Planetary, BodyType.Rocky) },
            { Commodities.Rhodplumsite, CommodityDetails.Mineral(
                "Rhodplumsite",
                MiningMethod.Core | MiningMethod.Planetary,
                RingType.MetalRich,
                BodyType.MetalRich | BodyType.HighMetalContent) },
            { Commodities.Ruby, CommodityDetails.Mineral("Ruby", MiningMethod.Planetary, BodyType.Rocky) },
            { Commodities.Rutile, CommodityDetails.Mineral("Rutile", MiningMethod.Laser, RingType.Rocky) },
            { Commodities.Sapphire, CommodityDetails.Mineral("Sapphire", MiningMethod.Planetary, BodyType.Rocky | BodyType.MetalRich) },
            { Commodities.Serendibite, CommodityDetails.Mineral("Serendibite", MiningMethod.Core | MiningMethod.Planetary, RingType.MetalRich | RingType.Rocky, BodyType.Rocky) },
            { Commodities.Thortveitite, CommodityDetails.Mineral("Thortveitite", MiningMethod.Planetary, BodyType.Rocky) },
            { Commodities.Uraninite, CommodityDetails.Mineral("Uraninite", MiningMethod.Laser | MiningMethod.Planetary, RingType.MetalRich | RingType.Rocky, BodyType.Rocky) },

            // Metals
            { Commodities.Cobalt, CommodityDetails.Metal("Cobalt", MiningMethod.Laser, RingType.Rocky) },
            { Commodities.Copper, CommodityDetails.Metal(
                "Copper",
                MiningMethod.Planetary,
                BodyType.MetalRich | BodyType.Rocky | BodyType.RockyIcy | BodyType.HighMetalContent) },
            { Commodities.Gold, CommodityDetails.Metal(
                "Gold",
                MiningMethod.Laser | MiningMethod.Planetary,
                RingType.Metallic | RingType.MetalRich,
                BodyType.MetalRich | BodyType.Rocky | BodyType.HighMetalContent) },
            { Commodities.Iridium, CommodityDetails.Metal("Iridium", MiningMethod.Planetary, BodyType.MetalRich | BodyType.HighMetalContent) },
            { Commodities.Lithium, CommodityDetails.Metal("Lithium", MiningMethod.Planetary, BodyType.MetalRich | BodyType.HighMetalContent) },
            { Commodities.Osmium, CommodityDetails.Metal(
                "Osmium",
                MiningMethod.Laser | MiningMethod.Planetary,
                RingType.Metallic | RingType.MetalRich,
                BodyType.MetalRich | BodyType.HighMetalContent) },
            { Commodities.Palladium, CommodityDetails.Metal(
                "Palladium",
                MiningMethod.Laser | MiningMethod.Planetary,
                RingType.Metallic,
                BodyType.Rocky | BodyType.HighMetalContent | BodyType.RockyIcy) },
            { Commodities.Platinum, CommodityDetails.Metal(
                "Platinum",
                MiningMethod.Core | MiningMethod.Laser | MiningMethod.SubSurface | MiningMethod.Planetary,
                RingType.Metallic,
                BodyType.MetalRich | BodyType.HighMetalContent) },
            { Commodities.Praseodymium, CommodityDetails.Metal("Praseodymium", MiningMethod.Laser, RingType.Metallic | RingType.MetalRich) },
            { Commodities.Samarium, CommodityDetails.Metal(
                "Samarium",
                MiningMethod.Laser | MiningMethod.Planetary,
                RingType.Metallic | RingType.MetalRich,
                BodyType.MetalRich | BodyType.Rocky | BodyType.HighMetalContent) },
            { Commodities.Silver, CommodityDetails.Metal(
                "Silver",
                MiningMethod.Laser | MiningMethod.Planetary,
                RingType.Metallic | RingType.MetalRich,
                BodyType.MetalRich | BodyType.Rocky | BodyType.HighMetalContent) },
            { Commodities.Tantalum, CommodityDetails.Metal("Tantalum", MiningMethod.Planetary, BodyType.MetalRich | BodyType.HighMetalContent) },
            { Commodities.Thorium, CommodityDetails.Metal(
                "Thorium",
                MiningMethod.Laser | MiningMethod.Planetary,
                RingType.Metallic,
                BodyType.MetalRich | BodyType.Rocky | BodyType.HighMetalContent) },
            { Commodities.Titanium, CommodityDetails.Metal("Titanium", MiningMethod.Planetary, BodyType.MetalRich | BodyType.HighMetalContent) },
            { Commodities.Uranium, CommodityDetails.Metal("Uranium", MiningMethod.Planetary, BodyType.MetalRich | BodyType.HighMetalContent) },
 
            // Chemicals
            { Commodities.Helium, CommodityDetails.Chemical("Helium", MiningMethod.Planetary, BodyType.Rocky | BodyType.HighMetalContent | BodyType.RockyIcy) },
            { Commodities.Helium3, CommodityDetails.Chemical("Helium-3 ", MiningMethod.Planetary, BodyType.Icy) },
            { Commodities.HydrogenPeroxide, CommodityDetails.Chemical("Hydrogen Peroxide", MiningMethod.Laser, RingType.Icy) },
            { Commodities.LiquidOxygen, CommodityDetails.Chemical("Liquid oxygen", MiningMethod.Laser | MiningMethod.SubSurface, RingType.Icy) },
            { Commodities.Tritium, CommodityDetails.Chemical(
                "Tritium",
                MiningMethod.Laser | MiningMethod.SubSurface | MiningMethod.Planetary,
                RingType.Icy,
                BodyType.Icy) },
            { Commodities.Water, CommodityDetails.Chemical(
                "Water",
                MiningMethod.Laser | MiningMethod.SubSurface | MiningMethod.Planetary,
                RingType.Icy,
                BodyType.Icy | BodyType.RockyIcy) },
        });

        public static ReadOnlyDictionary<Commodities, CommodityDetails> ALL { get => commodityDetails; }

        internal static CommodityDetails Mineral(string displayName, MiningMethod miningMethod, RingType ringType)
        {
            return new CommodityDetails(displayName, CommodityType.Mineral, miningMethod, ringType, BodyType.None);
        }
        internal static CommodityDetails Mineral(string displayName, MiningMethod miningMethod, BodyType bodyType)
        {
            return new CommodityDetails(displayName, CommodityType.Mineral, miningMethod, RingType.None, bodyType);
        }
        internal static CommodityDetails Mineral(string displayName, MiningMethod miningMethod, RingType ringType, BodyType bodyType)
        {
            return new CommodityDetails(displayName, CommodityType.Mineral, miningMethod, ringType, bodyType);
        }

        internal static CommodityDetails Metal(string displayName, MiningMethod miningMethod, RingType ringType)
        {
            return new CommodityDetails(displayName, CommodityType.Metal, miningMethod, ringType, BodyType.None);
        }
        internal static CommodityDetails Metal(string displayName, MiningMethod miningMethod, BodyType bodyType)
        {
            return new CommodityDetails(displayName, CommodityType.Metal, miningMethod, RingType.None, bodyType);
        }
        internal static CommodityDetails Metal(string displayName, MiningMethod miningMethod, RingType ringType, BodyType bodyType)
        {
            return new CommodityDetails(displayName, CommodityType.Metal, miningMethod, ringType, bodyType);
        }

        internal static CommodityDetails Chemical(string displayName, MiningMethod miningMethod, RingType ringType)
        {
            return new CommodityDetails(displayName, CommodityType.Chemical, miningMethod, ringType, BodyType.None);
        }
        internal static CommodityDetails Chemical(string displayName, MiningMethod miningMethod, BodyType bodyType)
        {
            return new CommodityDetails(displayName, CommodityType.Chemical, miningMethod, RingType.None, bodyType);
        }
        internal static CommodityDetails Chemical(string displayName, MiningMethod miningMethod, RingType ringType, BodyType bodyType)
        {
            return new CommodityDetails(displayName, CommodityType.Chemical, miningMethod, ringType, bodyType);
        }

        internal CommodityDetails(string displayName, CommodityType commodityType, MiningMethod method, RingType ringType, BodyType bodyType)
        {
            DisplayName = displayName;
            CommodityType = commodityType;
            MiningMethod = method;
            RingType = ringType;
            BodyType = bodyType;
        }

        public string DisplayName { get; init; }
        public CommodityType CommodityType { get; init; }
        public MiningMethod MiningMethod { get; init; }
        public RingType RingType { get; init; }
        public BodyType BodyType { get; init; }
    }

    public class DisplayMatchString
    {
        internal DisplayMatchString(string display, string match = "")
        {
            Display = display;
            Match = string.IsNullOrEmpty(match) ? display : match;
        }
        public string Display { get; }
        public string Match { get; }
    }

    public static class Extensions
    {

        public static CommodityDetails Details(this Commodities commodities)
        {
            return CommodityDetails.ALL[commodities];
        }

        private static readonly Dictionary<RingType, DisplayMatchString> ringTypeStrings = new()
        {
            { RingType.None, new DisplayMatchString("None") },
            { RingType.Rocky, new DisplayMatchString("Rocky") },
            { RingType.Metallic, new DisplayMatchString("Metallic", "Metalic") }, // To match Frontier's spelling
            { RingType.MetalRich, new DisplayMatchString("Metal Rich", "MetalRich") },
            { RingType.Icy, new DisplayMatchString("Icy") },
        };

        // TODO: Handle multiple values for [Flags] enums?
        public static string MatchString(this RingType rt)
        {
            return ringTypeStrings[rt].Match;
        }

        public static string DisplayString(this RingType rt)
        {
            StringBuilder sb = new();
            
            foreach (RingType r in Enum.GetValues(typeof(RingType)))
            {
                if (!rt.HasFlag(r) || r == RingType.None) continue;

                if (sb.Length > 0) sb.Append(" | ");
                sb.Append(ringTypeStrings[r].Display);
            }

            return sb.ToString();
        }

        private static readonly Dictionary<BodyType, DisplayMatchString> bodyTypeStrings = new()
        {
            { BodyType.None, new DisplayMatchString("None") },
            { BodyType.MetalRich, new DisplayMatchString("Metal-rich", "Metal rich body") },
            { BodyType.HighMetalContent, new DisplayMatchString("High Metal Content", "High metal content body") },
            { BodyType.Rocky, new DisplayMatchString("Rocky", "Rocky body") },
            { BodyType.Icy, new DisplayMatchString("Icy", "Icy body") },
            { BodyType.RockyIcy, new DisplayMatchString("Rocky Ice", "Rocky ice body") },
        };

        // TODO: Handle multiple values for [Flags] enums?
        public static string MatchString(this BodyType bt)
        {
            return bodyTypeStrings[bt].Match;
        }

        public static string DisplayString(this BodyType bt)
        {
            StringBuilder sb = new();

            foreach (BodyType b in Enum.GetValues(typeof(BodyType)))
            {
                if (!bt.HasFlag(b) || b == BodyType.None) continue;

                if (sb.Length > 0) sb.Append(" | ");
                sb.Append(bodyTypeStrings[b].Display);
            }

            return sb.ToString();
        }


        public static string DisplayString(this MiningMethod mm)
        {
            return mm.ToString(); // TODO: Something more intelligent -- particularly considering this is a [Flags] enum.
        }

        public static string DisplayString(this CommodityType ct)
        {
            return ct.ToString(); // Good enough for government work since this isn't a [Flags] enum.
        }
    }
}
