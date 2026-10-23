using com.github.fredjk_gh.PluginCommon.Data.Prospecting;
using Observatory.Framework;
using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.ObservatoryProspectorBasic
{
    class ProspectorSettings
    {
        public ProspectorSettings()
        {
            // Defaults go here.
            ShowProspectorNotifications = true;
            ShowCargoNotification = true;
            LimpetReminderNotifications = true;
            LimpetNotificationMinMiningModules = 2;
            MinimumPercent = 10;
            MinimumDensity = 5.0;
            MentionPotentiallyMineableRings = true;
            MentionPotentiallyMineableBodies = false;
            ProspectingFor.Add(Commodities.Tritium);
            ProspectingFor.Add(Commodities.Platinum);
            EnableAutoUpdates = true;
        }

        [SettingIgnore]
        public HashSet<Commodities> ProspectingFor { get; init; } = [];

        // For use by properties, etc. 
        internal bool GetFor(Commodities commodity)
        {
            return ProspectingFor.Contains(commodity);
        }
        internal void SetFor(Commodities c, bool value)
        {
            if (value)
                ProspectingFor.Add(c);
            else
                ProspectingFor.Remove(c);
        }

        [SettingIgnore]
        [JsonIgnore]
        public RingType MentionableRings
        {
            get => ProspectingFor.Select(kvp => kvp.Details().RingType).Aggregate((c, v) => c |= v);
        }

        [SettingIgnore]
        [JsonIgnore]
        public BodyType MentionableBodies
        {
            get => ProspectingFor.Select(kvp => kvp.Details().BodyType).Aggregate((c, v) => c |= v);
        }

        [SettingNewGroup("Notifications")]
        [SettingDisplayName("Notify rings types with potential items of interest")]
        public bool MentionPotentiallyMineableRings { get; set; }

        [SettingDisplayName("Notify body types with potential items of interest")]
        public bool MentionPotentiallyMineableBodies { get; set; }

        [SettingDisplayName("Notify High material content asteroids")]
        public bool ProspectHighMaterialContent { get; set; }

        [SettingDisplayName("Show Cargo notification (sticky)")]
        public bool ShowCargoNotification { get; set; }

        [SettingDisplayName("Show Prospector notifications (long duration)")]
        public bool ShowProspectorNotifications { get; set; }

        [SettingDisplayName("Notify about limpets when mining modules equipped when undocking")]
        public bool LimpetReminderNotifications { get; set; }

        [SettingDisplayName("Minimum number of mining modules equipped for limpet notification")]
        [SettingNumericBounds(1, 15)]
        public int LimpetNotificationMinMiningModules{ get; set; }

        [SettingDisplayName("Minimum % content required to be 'good'")]
        [SettingNumericBounds(0.0, 66.0)]
        public double MinimumPercent { get; set; }

        [SettingDisplayName("Minimum ring density to trigger notification (MT/km^3)")]
        [SettingNumericBounds(0.0, 9.0)]
        public double MinimumDensity { get; set; }

        public List<Commodities> DesireableCommoditiesByRingType(RingType ringType)
        {
            return [.. ProspectingFor.Where(c => c.Details().RingType.HasFlag(ringType))];
        }

        public List<Commodities> DesireableCommoditiesByBodyType(BodyType bodyType)
        {
            return [.. ProspectingFor.Where(c => c.Details().BodyType.HasFlag(bodyType))];
        }

        [SettingNewGroup("Commodities of interest")]
        [SettingDisplayName("Select commodities to watch")]
        [JsonIgnore]
        public Action CommoditySelector { get; internal set; }

        #region Deprecated Settings -- Remove in a future release.
        // Minerals:
        [SettingDisplayName("Alexandrite")]
        [SettingIgnore]
        public bool ProspectAlexandrite
        {
            get { return GetFor(Commodities.Alexandrite); }
            set { SetFor(Commodities.Alexandrite, value); }
        }
        [SettingDisplayName("Bauxite")]
        [SettingIgnore]
        public bool ProspectBauxite
        {
            get { return GetFor(Commodities.Bauxite); }
            set { SetFor(Commodities.Bauxite, value); }
        }
        [SettingDisplayName("Benitoite")]
        [SettingIgnore]
        public bool ProspectBenitoite
        {
            get { return GetFor(Commodities.Benitoite); }
            set { SetFor(Commodities.Benitoite, value); }
        }
        [SettingDisplayName("Bertrandite")]
        [SettingIgnore]
        public bool ProspectBertrandite
        {
            get { return GetFor(Commodities.Bertrandite); }
            set { SetFor(Commodities.Bertrandite, value); }
        }
        [SettingDisplayName("Bromellite")]
        [SettingIgnore]
        public bool ProspectBromellite
        {
            get { return GetFor(Commodities.Bromellite); }
            set { SetFor(Commodities.Bromellite, value); }
        }
        [SettingDisplayName("Coltan")]
        [SettingIgnore]
        public bool ProspectColtan
        {
            get { return GetFor(Commodities.Coltan); }
            set { SetFor(Commodities.Coltan, value); }
        }
        [SettingDisplayName("Gallite")]
        [SettingIgnore]
        public bool ProspectGallite
        {
            get { return GetFor(Commodities.Gallite); }
            set { SetFor(Commodities.Gallite, value); }
        }
        [SettingDisplayName("Grandidierite")]
        [SettingIgnore]
        public bool ProspectGrandidierite
        {
            get { return GetFor(Commodities.Grandidierite); }
            set { SetFor(Commodities.Grandidierite, value); }
        }
        [SettingDisplayName("Indite")]
        [SettingIgnore]
        public bool ProspectIndite
        {
            get { return GetFor(Commodities.Indite); }
            set { SetFor(Commodities.Indite, value); }
        }
        [SettingDisplayName("Lepidolite")]
        [SettingIgnore]
        public bool ProspectLepidolite
        {
            get { return GetFor(Commodities.Lepidolite); }
            set { SetFor(Commodities.Lepidolite, value); }
        }
        [SettingDisplayName("Lithium Hydroxide")]
        [SettingIgnore]
        public bool ProspectLithiumHydroxide
        {
            get { return GetFor(Commodities.LithiumHydroxide); }
            set { SetFor(Commodities.LithiumHydroxide, value); }
        }
        [SettingDisplayName("Low Temperature Diamonds")]
        [SettingIgnore]
        public bool ProspectLowTemperatureDiamond
        {
            get { return GetFor(Commodities.LowTemperatureDiamond); }
            set { SetFor(Commodities.LowTemperatureDiamond, value); }
        }
        [SettingDisplayName("Methane Clathrate")]
        [SettingIgnore]
        public bool ProspectMethaneClathrate
        {
            get { return GetFor(Commodities.MethaneClathrate); }
            set { SetFor(Commodities.MethaneClathrate, value); }
        }
        [SettingDisplayName("Methanol Monohydrate Crystals")]
        [SettingIgnore]
        public bool ProspectMethanolMonohydrateCrystals
        {
            get { return GetFor(Commodities.MethanolMonohydrateCrystals); }
            set { SetFor(Commodities.MethanolMonohydrateCrystals, value); }
        }
        [SettingDisplayName("Monazite")]
        [SettingIgnore]
        public bool ProspectMonazite
        {
            get { return GetFor(Commodities.Monazite); }
            set { SetFor(Commodities.Monazite, value); }
        }
        [SettingDisplayName("Musgravite")]
        [SettingIgnore]
        public bool ProspectMusgravite
        {
            get { return GetFor(Commodities.Musgravite); }
            set { SetFor(Commodities.Musgravite, value); }
        }
        [SettingDisplayName("Void Opal")]
        [SettingIgnore]
        public bool ProspectOpal
        {
            get { return GetFor(Commodities.Opal); }
            set { SetFor(Commodities.Opal, value); }
        }
        [SettingDisplayName("Painite")]
        [SettingIgnore]
        public bool ProspectPainite
        {
            get { return GetFor(Commodities.Painite); }
            set { SetFor(Commodities.Painite, value); }
        }
        [SettingDisplayName("Rhodplumsite")]
        [SettingIgnore]
        public bool ProspectRhodplumsite
        {
            get { return GetFor(Commodities.Rhodplumsite); }
            set { SetFor(Commodities.Rhodplumsite, value); }
        }
        [SettingDisplayName("Rutile")]
        [SettingIgnore]
        public bool ProspectRutile
        {
            get { return GetFor(Commodities.Rutile); }
            set { SetFor(Commodities.Rutile, value); }
        }
        [SettingDisplayName("Serendibite")]
        [SettingIgnore]
        public bool ProspectSerendibite
        {
            get { return GetFor(Commodities.Serendibite); }
            set { SetFor(Commodities.Serendibite, value); }
        }
        [SettingDisplayName("Uraninite")]
        [SettingIgnore]
        public bool ProspectUraninite
        {
            get { return GetFor(Commodities.Uraninite); }
            set { SetFor(Commodities.Uraninite, value); }
        }

        // Metals:
        //[SettingNewGroup("Metals of interest")]
        [SettingDisplayName("Cobalt")]
        [SettingIgnore]
        public bool ProspectCobalt
        {
            get { return GetFor(Commodities.Cobalt); }
            set { SetFor(Commodities.Cobalt, value); }
        }
        [SettingDisplayName("Gold")]
        [SettingIgnore]
        public bool ProspectGold
        {
            get { return GetFor(Commodities.Gold); }
            set { SetFor(Commodities.Gold, value); }
        }
        [SettingDisplayName("Osmium")]
        [SettingIgnore]
        public bool ProspectOsmium
        {
            get { return GetFor(Commodities.Osmium); }
            set { SetFor(Commodities.Osmium, value); }
        }
        [SettingDisplayName("Palladium")]
        [SettingIgnore]
        public bool ProspectPalladium
        {
            get { return GetFor(Commodities.Palladium); }
            set { SetFor(Commodities.Palladium, value); }
        }
        [SettingDisplayName("Platinum (Laser)")]
        [SettingIgnore]
        public bool ProspectPlatinum
        {
            get { return GetFor(Commodities.Platinum); }
            set { SetFor(Commodities.Platinum, value); }
        }
        [SettingDisplayName("Praseodymium")]
        [SettingIgnore]
        public bool ProspectPraseodymium
        {
            get { return GetFor(Commodities.Praseodymium); }
            set { SetFor(Commodities.Praseodymium, value); }
        }
        [SettingDisplayName("Samarium")]
        [SettingIgnore]
        public bool ProspectSamarium
        {
            get { return GetFor(Commodities.Samarium); }
            set { SetFor(Commodities.Samarium, value); }
        }
        [SettingDisplayName("Silver")]
        [SettingIgnore]
        public bool ProspectSilver
        {
            get { return GetFor(Commodities.Silver); }
            set { SetFor(Commodities.Silver, value); }
        }

        // Chemicals:
        //[SettingNewGroup("Chemicals of interest")]
        [SettingDisplayName("Thorium")]
        [SettingIgnore]
        public bool ProspectThorium
        {
            get { return GetFor(Commodities.Thorium); }
            set { SetFor(Commodities.Thorium, value); }
        }
        [SettingDisplayName("Hydrogen Peroxide")]
        [SettingIgnore]
        public bool ProspectHydrogenPeroxide
        {
            get { return GetFor(Commodities.HydrogenPeroxide); }
            set { SetFor(Commodities.HydrogenPeroxide, value); }
        }
        [SettingDisplayName("Liquid Oxygen")]
        [SettingIgnore]
        public bool ProspectLiquidOxygen
        {
            get { return GetFor(Commodities.LiquidOxygen); }
            set { SetFor(Commodities.LiquidOxygen, value); }
        }
        [SettingDisplayName("Tritium")]
        [SettingIgnore]
        public bool ProspectTritium
        {
            get { return GetFor(Commodities.Tritium); }
            set { SetFor(Commodities.Tritium, value); }
        }
        [SettingDisplayName("Water")]
        [SettingIgnore]
        public bool ProspectWater
        {
            get { return GetFor(Commodities.Water); }
            set { SetFor(Commodities.Water, value); }
        }
        #endregion Deprecated settings

        // Raw mats
        [SettingNewGroup("Raw materials for synthesis (Experimental)")]
        // Surface material prospecting
        [SettingDisplayName("Mats for FSD Boost")]
        [SettingIgnore]
        public bool MatsFSDBoost { get; set; }

        [SettingDisplayName("Mats for AFMU Refill")]
        [SettingIgnore]
        public bool MatsAFMURefill { get; set; }

        [SettingDisplayName("Mats for SRV Refuel")]
        [SettingIgnore]
        public bool MatsSRVRefuel { get; set; }

        [SettingDisplayName("Mats for SRV Repair")]
        [SettingIgnore]
        public bool MatsSRVRepair { get; set; }


        [SettingNewGroup("Updates")]
        [SettingDisplayName("Enable automatic updates")]
        public bool EnableAutoUpdates { get; set; }

        [SettingDisplayName("Enable Beta versions (warning: things may break)")]
        public bool EnableBetaUpdates { get; set; }
    }
}
