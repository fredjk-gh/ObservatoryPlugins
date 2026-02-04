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
        public ProspectorSettings()
        {
            // Defaults go here.
            ShowProspectorNotifications = true;
            ShowCargoNotification = true;
            MinimumPercent = 10;
            MinimumDensity = 5.0;
            ProspectTritium = true;
            ProspectPlatinum = true;
            EnableAutoUpdates = true;
        }

        private readonly Dictionary<Commodities, Boolean> ProspectingFor = [];
        internal bool GetFor(Commodities commodity)
        {
            return ProspectingFor.GetValueOrDefault(commodity, false);
        }
        internal void SetFor(Commodities c, bool value)
        {
            ProspectingFor[c] = value;
        }

        [SettingIgnore]
        public RingType? MentionableRings
        {
            get
            {
                List<RingType> allDesiredRingTypes = [.. ProspectingFor.Where(kvp => kvp.Value).Select(kvp => kvp.Key.Details().RingType)];
                RingType? desiredRingTypes = null;
                foreach (RingType rt in allDesiredRingTypes)
                {
                    desiredRingTypes = desiredRingTypes.HasValue ? desiredRingTypes | rt : rt;
                }
                return desiredRingTypes;
            }
        }

        [SettingNewGroup("Notifications")]
        [SettingDisplayName("Notify rings types with potential items of interest")]
        public bool MentionPotentiallyMineableRings { get; set; }

        [SettingDisplayName("Notify High material content asteroids")]
        public bool ProspectHighMaterialContent { get; set; }

        [SettingDisplayName("Show Cargo notification (sticky)")]
        public bool ShowCargoNotification { get; set; }

        [SettingDisplayName("Show Prospector notifications (long duration)")]
        public bool ShowProspectorNotifications { get; set; }

        [SettingDisplayName("Minimum % content required to be 'good'")]
        [SettingNumericBounds(0.0, 66.0)]
        public double MinimumPercent { get; set; }

        [SettingDisplayName("Minimum ring density to trigger notification (MT/km^3)")]
        [SettingNumericBounds(0.0, 9.0)]
        public double MinimumDensity { get; set; }

        public List<Commodities> DesirableCommonditiesByRingType(RingType ringType)
        {
            return [.. ProspectingFor.Where(kvp => kvp.Value && kvp.Key.Details().RingType.HasFlag(ringType)).Select(kvp => kvp.Key)];
        }

        // Minerals:
        [SettingNewGroup("Minerals of interest")]
        [SettingDisplayName("Alexandrite")]
        public bool ProspectAlexandrite
        {
            get { return GetFor(Commodities.Alexandrite); }
            set { SetFor(Commodities.Alexandrite, value); }
        }
        [SettingDisplayName("Bauxite")]
        public bool ProspectBauxite
        {
            get { return GetFor(Commodities.Bauxite); }
            set { SetFor(Commodities.Bauxite, value); }
        }
        [SettingDisplayName("Benitoite")]
        public bool ProspectBenitoite
        {
            get { return GetFor(Commodities.Benitoite); }
            set { SetFor(Commodities.Benitoite, value); }
        }
        [SettingDisplayName("Bertrandite")]
        public bool ProspectBertrandite
        {
            get { return GetFor(Commodities.Bertrandite); }
            set { SetFor(Commodities.Bertrandite, value); }
        }
        [SettingDisplayName("Bromellite")]
        public bool ProspectBromellite
        {
            get { return GetFor(Commodities.Bromellite); }
            set { SetFor(Commodities.Bromellite, value); }
        }
        [SettingDisplayName("Coltan")]
        public bool ProspectColtan
        {
            get { return GetFor(Commodities.Coltan); }
            set { SetFor(Commodities.Coltan, value); }
        }
        [SettingDisplayName("Gallite")]
        public bool ProspectGallite
        {
            get { return GetFor(Commodities.Gallite); }
            set { SetFor(Commodities.Gallite, value); }
        }
        [SettingDisplayName("Grandidierite")]
        public bool ProspectGrandidierite
        {
            get { return GetFor(Commodities.Grandidierite); }
            set { SetFor(Commodities.Grandidierite, value); }
        }
        [SettingDisplayName("Indite")]
        public bool ProspectIndite
        {
            get { return GetFor(Commodities.Indite); }
            set { SetFor(Commodities.Indite, value); }
        }
        [SettingDisplayName("Lepidolite")]
        public bool ProspectLepidolite
        {
            get { return GetFor(Commodities.Lepidolite); }
            set { SetFor(Commodities.Lepidolite, value); }
        }
        [SettingDisplayName("Lithium Hydroxide")]
        public bool ProspectLithiumHydroxide
        {
            get { return GetFor(Commodities.LithiumHydroxide); }
            set { SetFor(Commodities.LithiumHydroxide, value); }
        }
        [SettingDisplayName("Low Temperature Diamonds")]
        public bool ProspectLowTemperatureDiamond
        {
            get { return GetFor(Commodities.LowTemperatureDiamond); }
            set { SetFor(Commodities.LowTemperatureDiamond, value); }
        }
        [SettingDisplayName("Methane Clathrate")]
        public bool ProspectMethaneClathrate
        {
            get { return GetFor(Commodities.MethaneClathrate); }
            set { SetFor(Commodities.MethaneClathrate, value); }
        }
        [SettingDisplayName("Methanol Monohydrate Crystals")]
        public bool ProspectMethanolMonohydrateCrystals
        {
            get { return GetFor(Commodities.MethanolMonohydrateCrystals); }
            set { SetFor(Commodities.MethanolMonohydrateCrystals, value); }
        }
        [SettingDisplayName("Monazite")]
        public bool ProspectMonazite
        {
            get { return GetFor(Commodities.Monazite); }
            set { SetFor(Commodities.Monazite, value); }
        }
        [SettingDisplayName("Musgravite")]
        public bool ProspectMusgravite
        {
            get { return GetFor(Commodities.Musgravite); }
            set { SetFor(Commodities.Musgravite, value); }
        }
        [SettingDisplayName("Void Opal")]
        public bool ProspectOpal
        {
            get { return GetFor(Commodities.Opal); }
            set { SetFor(Commodities.Opal, value); }
        }
        [SettingDisplayName("Painite")]
        public bool ProspectPainite
        {
            get { return GetFor(Commodities.Painite); }
            set { SetFor(Commodities.Painite, value); }
        }
        [SettingDisplayName("Rhodplumsite")]
        public bool ProspectRhodplumsite
        {
            get { return GetFor(Commodities.Rhodplumsite); }
            set { SetFor(Commodities.Rhodplumsite, value); }
        }
        [SettingDisplayName("Rutile")]
        public bool ProspectRutile
        {
            get { return GetFor(Commodities.Rutile); }
            set { SetFor(Commodities.Rutile, value); }
        }
        [SettingDisplayName("Serendibite")]
        public bool ProspectSerendibite
        {
            get { return GetFor(Commodities.Serendibite); }
            set { SetFor(Commodities.Serendibite, value); }
        }
        [SettingDisplayName("Uraninite")]
        public bool ProspectUraninite
        {
            get { return GetFor(Commodities.Uraninite); }
            set { SetFor(Commodities.Uraninite, value); }
        }

        // Metals:
        [SettingNewGroup("Metals of interest")]
        [SettingDisplayName("Cobalt")]
        public bool ProspectCobalt
        {
            get { return GetFor(Commodities.Cobalt); }
            set { SetFor(Commodities.Cobalt, value); }
        }
        [SettingDisplayName("Gold")]
        public bool ProspectGold
        {
            get { return GetFor(Commodities.Gold); }
            set { SetFor(Commodities.Gold, value); }
        }
        [SettingDisplayName("Osmium")]
        public bool ProspectOsmium
        {
            get { return GetFor(Commodities.Osmium); }
            set { SetFor(Commodities.Osmium, value); }
        }
        [SettingDisplayName("Palladium")]
        public bool ProspectPalladium
        {
            get { return GetFor(Commodities.Palladium); }
            set { SetFor(Commodities.Palladium, value); }
        }
        [SettingDisplayName("Platinum (Laser)")]
        public bool ProspectPlatinum
        {
            get { return GetFor(Commodities.Platinum); }
            set { SetFor(Commodities.Platinum, value); }
        }
        [SettingDisplayName("Praseodymium")]
        public bool ProspectPraseodymium
        {
            get { return GetFor(Commodities.Praseodymium); }
            set { SetFor(Commodities.Praseodymium, value); }
        }
        [SettingDisplayName("Samarium")]
        public bool ProspectSamarium
        {
            get { return GetFor(Commodities.Samarium); }
            set { SetFor(Commodities.Samarium, value); }
        }
        [SettingDisplayName("Silver")]
        public bool ProspectSilver
        {
            get { return GetFor(Commodities.Silver); }
            set { SetFor(Commodities.Silver, value); }
        }

        // Chemicals:
        [SettingNewGroup("Chemicals of interest")]
        [SettingDisplayName("Thorium")]
        public bool ProspectThorium
        {
            get { return GetFor(Commodities.Thorium); }
            set { SetFor(Commodities.Thorium, value); }
        }
        [SettingDisplayName("Hydrogen Peroxide")]
        public bool ProspectHydrogenPeroxide
        {
            get { return GetFor(Commodities.HydrogenPeroxide); }
            set { SetFor(Commodities.HydrogenPeroxide, value); }
        }
        [SettingDisplayName("Liquid Oxygen")]
        public bool ProspectLiquidOxygen
        {
            get { return GetFor(Commodities.LiquidOxygen); }
            set { SetFor(Commodities.LiquidOxygen, value); }
        }
        [SettingDisplayName("Tritium")]
        public bool ProspectTritium
        {
            get { return GetFor(Commodities.Tritium); }
            set { SetFor(Commodities.Tritium, value); }
        }
        [SettingDisplayName("Water")]
        public bool ProspectWater
        {
            get { return GetFor(Commodities.Water); }
            set { SetFor(Commodities.Water, value); }
        }

        // Raw mats
        [SettingNewGroup("Raw materials for synthesis (Experimental)")]
        // Surface material prospecting
        [SettingDisplayName("Mats for FSD Boost")]
        public bool MatsFSDBoost { get; set; }

        [SettingDisplayName("Mats for AFMU Refill")]
        public bool MatsAFMURefill { get; set; }

        [SettingDisplayName("Mats for SRV Refuel")]
        public bool MatsSRVRefuel { get; set; }

        [SettingDisplayName("Mats for SRV Repair")]
        public bool MatsSRVRepair { get; set; }


        [SettingNewGroup("Updates")]
        [SettingDisplayName("Enable automatic updates")]
        public bool EnableAutoUpdates { get; set; }

        [SettingDisplayName("Enable Beta versions (warning: things may break)")]
        public bool EnableBetaUpdates { get; set; }
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
        private readonly static Dictionary<Commodities, CommodityDetails> commodityDetails = new()
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

        private static readonly Dictionary<RingType, RingTypeString> ringTypeStrings = new()
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
