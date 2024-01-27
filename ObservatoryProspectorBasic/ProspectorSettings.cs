using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryProspectorBasic
{
    class ProspectorSettings
    {
        private Dictionary<Commodities, Boolean> ProspectingFor = new();
        internal bool getFor(Commodities commodity)
        {
            return ProspectingFor.GetValueOrDefault(commodity, false);
        }
        internal void setFor(Commodities c, bool value)
        {
            ProspectingFor[c] = value;
        }

        [SettingDisplayName("Minimum Commodity Percent (Laser mining)")]
        [SettingNumericBounds(0.0, 66.0)]
        public int MinimumPercent { get; set; }

        [SettingIgnore]
        public RingType? MentionableRings
        {
            get
            {
                List<RingType> allDesiredRingTypes = ProspectingFor.Where(kvp => kvp.Value)
                    .Select(kvp => kvp.Key.Details().RingType)
                    .ToList();
                RingType? desiredRingTypes = null;
                foreach (RingType rt in allDesiredRingTypes)
                {
                    desiredRingTypes = desiredRingTypes.HasValue ? desiredRingTypes | rt : rt;
                }
                return desiredRingTypes;
            }
        }

        [SettingDisplayName("Highlight ring types which may contain commodites selected below")]
        public bool MentionPotentiallyMineableRings { get; set; }

        [SettingDisplayName("Show Prospector notifications")]
        public bool ShowProspectorNotifications { get; set; }

        [SettingDisplayName("Show Cargo notification")]
        public bool ShowCargoNotification { get; set; }

        public List<Commodities> DesirableCommonditiesByRingType(RingType ringType)
        {
            return ProspectingFor.Where(kvp => kvp.Value && kvp.Key.Details().RingType.HasFlag(ringType))
                .Select(kvp => kvp.Key)
                .ToList();
        }

        // Raw mats
        [SettingDisplayName("Prospect high material content")]
        public bool ProspectHighMaterialContent { get; set; }


        // Minerals:
        [SettingDisplayName("Prospect Alexandrite")]
        public bool ProspectAlexandrite
        {
            get { return getFor(Commodities.Alexandrite); }
            set { setFor(Commodities.Alexandrite, value); }
        }
        [SettingDisplayName("Prospect Bauxite")]
        public bool ProspectBauxite
        {
            get { return getFor(Commodities.Bauxite); }
            set { setFor(Commodities.Bauxite, value); }
        }
        [SettingDisplayName("Prospect Benitoite")]
        public bool ProspectBenitoite
        {
            get { return getFor(Commodities.Benitoite); }
            set { setFor(Commodities.Benitoite, value); }
        }
        [SettingDisplayName("Prospect Bertrandite")]
        public bool ProspectBertrandite
        {
            get { return getFor(Commodities.Bertrandite); }
            set { setFor(Commodities.Bertrandite, value); }
        }
        [SettingDisplayName("Prospect Bromellite")]
        public bool ProspectBromellite
        {
            get { return getFor(Commodities.Bromellite); }
            set { setFor(Commodities.Bromellite, value); }
        }
        [SettingDisplayName("Prospect Coltan")]
        public bool ProspectColtan
        {
            get { return getFor(Commodities.Coltan); }
            set { setFor(Commodities.Coltan, value); }
        }
        [SettingDisplayName("Prospect Gallite")]
        public bool ProspectGallite
        {
            get { return getFor(Commodities.Gallite); }
            set { setFor(Commodities.Gallite, value); }
        }
        [SettingDisplayName("Prospect Grandidierite")]
        public bool ProspectGrandidierite
        {
            get { return getFor(Commodities.Grandidierite); }
            set { setFor(Commodities.Grandidierite, value); }
        }
        [SettingDisplayName("Prospect Indite")]
        public bool ProspectIndite
        {
            get { return getFor(Commodities.Indite); }
            set { setFor(Commodities.Indite, value); }
        }
        [SettingDisplayName("Prospect Lepidolite")]
        public bool ProspectLepidolite
        {
            get { return getFor(Commodities.Lepidolite); }
            set { setFor(Commodities.Lepidolite, value); }
        }
        [SettingDisplayName("Prospect Lithium ydroxide")]
        public bool ProspectLithiumHydroxide
        {
            get { return getFor(Commodities.LithiumHydroxide); }
            set { setFor(Commodities.LithiumHydroxide, value); }
        }
        [SettingDisplayName("Prospect Low Temperature Diamonds")]
        public bool ProspectLowTemperatureDiamond
        {
            get { return getFor(Commodities.LowTemperatureDiamond); }
            set { setFor(Commodities.LowTemperatureDiamond, value); }
        }
        [SettingDisplayName("Prospect Methane Clathrate")]
        public bool ProspectMethaneClathrate
        {
            get { return getFor(Commodities.MethaneClathrate); }
            set { setFor(Commodities.MethaneClathrate, value); }
        }
        [SettingDisplayName("Prospect Methanol Monohydrate Crystals")]
        public bool ProspectMethanolMonohydrateCrystals
        {
            get { return getFor(Commodities.MethanolMonohydrateCrystals); }
            set { setFor(Commodities.MethanolMonohydrateCrystals, value); }
        }
        [SettingDisplayName("Prospect Monazite")]
        public bool ProspectMonazite
        {
            get { return getFor(Commodities.Monazite); }
            set { setFor(Commodities.Monazite, value); }
        }
        [SettingDisplayName("Prospect Musgravite")]
        public bool ProspectMusgravite
        {
            get { return getFor(Commodities.Musgravite); }
            set { setFor(Commodities.Musgravite, value); }
        }
        [SettingDisplayName("Prospect Void Opal")]
        public bool ProspectOpal
        {
            get { return getFor(Commodities.Opal); }
            set { setFor(Commodities.Opal, value); }
        }
        [SettingDisplayName("Prospect Painite")]
        public bool ProspectPainite
        {
            get { return getFor(Commodities.Painite); }
            set { setFor(Commodities.Painite, value); }
        }
        [SettingDisplayName("Prospect Rhodplumsite")]
        public bool ProspectRhodplumsite
        {
            get { return getFor(Commodities.Rhodplumsite); }
            set { setFor(Commodities.Rhodplumsite, value); }
        }
        [SettingDisplayName("Prospect Rutile")]
        public bool ProspectRutile
        {
            get { return getFor(Commodities.Rutile); }
            set { setFor(Commodities.Rutile, value); }
        }
        [SettingDisplayName("Prospect Serendibite")]
        public bool ProspectSerendibite
        {
            get { return getFor(Commodities.Serendibite); }
            set { setFor(Commodities.Serendibite, value); }
        }
        [SettingDisplayName("Prospect Uraninite")]
        public bool ProspectUraninite
        {
            get { return getFor(Commodities.Uraninite); }
            set { setFor(Commodities.Uraninite, value); }
        }

        // Metals:
        [SettingDisplayName("Prospect Cobalt")]
        public bool ProspectCobalt
        {
            get { return getFor(Commodities.Cobalt); }
            set { setFor(Commodities.Cobalt, value); }
        }
        [SettingDisplayName("Prospect Gold")]
        public bool ProspectGold
        {
            get { return getFor(Commodities.Gold); }
            set { setFor(Commodities.Gold, value); }
        }
        [SettingDisplayName("Prospect Osmium")]
        public bool ProspectOsmium
        {
            get { return getFor(Commodities.Osmium); }
            set { setFor(Commodities.Osmium, value); }
        }
        [SettingDisplayName("Prospect Palladium")]
        public bool ProspectPalladium
        {
            get { return getFor(Commodities.Palladium); }
            set { setFor(Commodities.Palladium, value); }
        }
        [SettingDisplayName("Prospect Platinum (Laser)")]
        public bool ProspectPlatinum
        {
            get { return getFor(Commodities.Platinum); }
            set { setFor(Commodities.Platinum, value); }
        }
        [SettingDisplayName("Prospect Praseodymium")]
        public bool ProspectPraseodymium
        {
            get { return getFor(Commodities.Praseodymium); }
            set { setFor(Commodities.Praseodymium, value); }
        }
        [SettingDisplayName("Prospect Samarium")]
        public bool ProspectSamarium
        {
            get { return getFor(Commodities.Samarium); }
            set { setFor(Commodities.Samarium, value); }
        }
        [SettingDisplayName("Prospect Silver")]
        public bool ProspectSilver
        {
            get { return getFor(Commodities.Silver); }
            set { setFor(Commodities.Silver, value); }
        }
        [SettingDisplayName("Prospect Thorium")]

        // Chemicals:
        public bool ProspectThorium
        {
            get { return getFor(Commodities.Thorium); }
            set { setFor(Commodities.Thorium, value); }
        }
        [SettingDisplayName("Prospect Hydrogen Peroxide")]
        public bool ProspectHydrogenPeroxide
        {
            get { return getFor(Commodities.HydrogenPeroxide); }
            set { setFor(Commodities.HydrogenPeroxide, value); }
        }
        [SettingDisplayName("Prospect Liquid Oxygen")]
        public bool ProspectLiquidOxygen
        {
            get { return getFor(Commodities.LiquidOxygen); }
            set { setFor(Commodities.LiquidOxygen, value); }
        }
        [SettingDisplayName("Prospect Tritium")]
        public bool ProspectTritium
        {
            get { return getFor(Commodities.Tritium); }
            set { setFor(Commodities.Tritium, value); }
        }
        [SettingDisplayName("Prospect Water")]
        public bool ProspectWater
        {
            get { return getFor(Commodities.Water); }
            set { setFor(Commodities.Water, value); }
        }

        // Surface material prospecting
        [SettingDisplayName("Prospect Mats for FSD Boost")]
        public bool MatsFSDBoost { get; set; }

        [SettingDisplayName("Prospect Mats for AFMU Refill")]
        public bool MatsAFMURefill { get; set; }

        [SettingDisplayName("Prospect Mats for SRV Refuel")]
        public bool MatsSRVRefuel { get; set; }

        [SettingDisplayName("Prospect Mats for SRV Repair")]
        public bool MatsSRVRepair { get; set; }
    }

    public class CommodityDetails
    {
        internal CommodityDetails(RingType ringType, MiningMethod method)
        {
            MiningMethod = method;
            RingType = ringType;
        }

        public MiningMethod MiningMethod { get; }
        public RingType RingType { get; }
    }

    public class RingTypeString
    {
        internal RingTypeString(string display, string match = "")
        {
            Display = display;
            Match = string.IsNullOrEmpty(match) ? display : match;
        }
        public string Display { get; }
        public string Match { get; }
    }
    public static class Extensions
    {
        private static Dictionary<Commodities, CommodityDetails> commodityDetails = new()
        {
            // Minerals
            { Commodities.Alexandrite, new CommodityDetails(RingType.MetalRich | RingType.Rocky | RingType.Icy, MiningMethod.Core) },
            { Commodities.Bauxite, new CommodityDetails(RingType.Rocky, MiningMethod.Laser) },
            { Commodities.Benitoite, new CommodityDetails(RingType.MetalRich | RingType.Rocky, MiningMethod.Core) },
            { Commodities.Bertrandite, new CommodityDetails(RingType.MetalRich | RingType.Metallic, MiningMethod.Laser) },
            { Commodities.Bromellite, new CommodityDetails(RingType.Icy, MiningMethod.Core | MiningMethod.Laser | MiningMethod.SubSurface) },
            { Commodities.Coltan, new CommodityDetails(RingType.MetalRich | RingType.Rocky, MiningMethod.Laser) },
            { Commodities.Gallite, new CommodityDetails(RingType.MetalRich | RingType.Metallic | RingType.Rocky, MiningMethod.Laser) },
            { Commodities.Grandidierite, new CommodityDetails(RingType.Metallic | RingType.Icy, MiningMethod.Core) },
            { Commodities.Indite, new CommodityDetails(RingType.Metallic | RingType.MetalRich | RingType.Rocky, MiningMethod.Laser) },
            { Commodities.Lepidolite, new CommodityDetails(RingType.MetalRich | RingType.Rocky, MiningMethod.Laser) },
            { Commodities.LithiumHydroxide, new CommodityDetails(RingType.Icy, MiningMethod.Laser) },
            { Commodities.LowTemperatureDiamond, new CommodityDetails(RingType.Icy, MiningMethod.Core | MiningMethod.Laser | MiningMethod.SubSurface) },
            { Commodities.MethaneClathrate, new CommodityDetails(RingType.Icy, MiningMethod.Laser) },
            { Commodities.MethanolMonohydrateCrystals, new CommodityDetails(RingType.Icy, MiningMethod.Laser) },
            { Commodities.Monazite, new CommodityDetails(RingType.MetalRich | RingType.Rocky, MiningMethod.Core) },
            { Commodities.Musgravite, new CommodityDetails(RingType.Rocky, MiningMethod.Core) },
            { Commodities.Painite, new CommodityDetails(RingType.Metallic, MiningMethod.Core | MiningMethod.Laser | MiningMethod.SubSurface) },
            { Commodities.Rhodplumsite, new CommodityDetails(RingType.MetalRich, MiningMethod.Core) },
            { Commodities.Rutile, new CommodityDetails(RingType.Rocky, MiningMethod.Laser) },
            { Commodities.Serendibite, new CommodityDetails(RingType.MetalRich | RingType.Rocky, MiningMethod.Core) },
            { Commodities.Uraninite, new CommodityDetails(RingType.MetalRich | RingType.Rocky, MiningMethod.Laser) },
            { Commodities.Opal, new CommodityDetails(RingType.Icy, MiningMethod.Core) },

            // Metals
            { Commodities.Cobalt, new CommodityDetails(RingType.Rocky, MiningMethod.Laser) },
            { Commodities.Gold, new CommodityDetails(RingType.Metallic | RingType.MetalRich, MiningMethod.Laser) },
            { Commodities.Osmium, new CommodityDetails(RingType.Metallic | RingType.MetalRich, MiningMethod.Laser) },
            { Commodities.Palladium, new CommodityDetails(RingType.Metallic, MiningMethod.Laser) },
            { Commodities.Platinum, new CommodityDetails(RingType.Metallic, MiningMethod.Core | MiningMethod.Laser | MiningMethod.SubSurface) },
            { Commodities.Praseodymium, new CommodityDetails(RingType.Metallic | RingType.MetalRich, MiningMethod.Laser) },
            { Commodities.Samarium, new CommodityDetails(RingType.Metallic | RingType.MetalRich, MiningMethod.Laser) },
            { Commodities.Silver, new CommodityDetails(RingType.Metallic | RingType.MetalRich, MiningMethod.Laser) },
            { Commodities.Thorium, new CommodityDetails(RingType.Metallic, MiningMethod.Laser) },
 
            // Chemicals
            { Commodities.HydrogenPeroxide, new CommodityDetails(RingType.Icy, MiningMethod.Laser) },
            { Commodities.LiquidOxygen, new CommodityDetails(RingType.Icy, MiningMethod.Laser | MiningMethod.SubSurface) },
            { Commodities.Tritium, new CommodityDetails(RingType.Icy, MiningMethod.Laser | MiningMethod.SubSurface) },
            { Commodities.Water, new CommodityDetails(RingType.Icy, MiningMethod.Laser | MiningMethod.SubSurface) },
        };

        public static CommodityDetails Details(this Commodities commodities)
        {
            return commodityDetails[commodities];
        }

        private static Dictionary<RingType, RingTypeString> ringTypeStrings = new()
        {
            { RingType.Rocky, new RingTypeString("Rocky") },
            { RingType.Metallic, new RingTypeString("Metallic", "Metalic") }, // To match Frontier's spelling
            { RingType.MetalRich, new RingTypeString("Metal Rich", "MetalRich") },
            { RingType.Icy, new RingTypeString("Icy") },
        };
        public static string MatchString(this RingType rt)
        {
            return ringTypeStrings[rt].Match;
        }

        public static string DisplayString(this RingType rt)
        {
            return ringTypeStrings[rt].Display;
        }
    }

    [Flags]
    public enum MiningMethod
    {
        Laser = 1,
        SubSurface = 2,
        Core = 4,
    }

    [Flags]
    public enum RingType
    {
        Rocky = 1,
        MetalRich = 2,
        Metallic = 4,  
        Icy = 8,
    }

    public enum Commodities
    {
        // Minerals:
        Alexandrite,
        Bauxite,
        Benitoite,
        Bertrandite,
        Bromellite,
        Coltan,
        Gallite,
        Grandidierite,
        Indite,
        Lepidolite,
        LithiumHydroxide,
        LowTemperatureDiamond,
        MethaneClathrate,
        MethanolMonohydrateCrystals,
        Monazite,
        Musgravite,
        Opal,
        Painite,
        Rhodplumsite,
        Rutile,
        Serendibite,
        Uraninite,

        // Metals:
        Cobalt,
        Gold,
        Osmium,
        Palladium,
        Platinum,
        Praseodymium,
        Samarium,
        Silver,
        Thorium,

        // Chemicals:
        HydrogenPeroxide,
        LiquidOxygen,
        Tritium,
        Water,
    }
}
