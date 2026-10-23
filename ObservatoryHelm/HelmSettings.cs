using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using com.github.fredjk_gh.PluginCommon.Data.Prospecting;
using Observatory.Framework;

[assembly: InternalsVisibleTo("ObservatoryPlugins.Tests")]
namespace com.github.fredjk_gh.ObservatoryHelm
{
    internal class HelmSettings
    {
        // Constant names for 
        public const string SETTING_FONTSIZEADJUSTMENT = "FontSizeAdjustment";
        public const string SETTING_ENABLEWRAPPEDLAYOUT = "EnableWrappedLayout";
        public const string SETTING_USEINGAMETIME = "UseInGameTime";

        private double _fontSizeAdjustment = 0.0;
        private bool _enableWrappedLayout = true;
        private bool _useInGameTime;

        public HelmSettings()
        {
            // Defaults go here.
            GravityAdvisoryThreshold = 1.5;
            MaxNearbyScoopableDistance = 3000;
            WarnIncompleteUndiscoveredSystemScan = false;
            NotifyJetConeBoost = false;
            BubbleRadiusLy = 700.0;
            CardOrdering = [];
            CollapsedCards = [];
            ShipPanelHidden = false;
            RoutePanelHidden = false;
            SystemPanelHidden = false;
            BodyPanelHidden = false;
            //StationPanelHidden = false;
            CargoPanelHidden = false;
            ProspectorPanelHidden = false;
            MessagesPanelHidden = false;
            EnableWrappedLayout = true;
            EnableAutoUpdates = true;
            UseInGameTime = false;
            // Migrated from Prospector
            LimpetReminderNotifications = true;
            LimpetNotificationMinMiningModules = 2;
            MinimumPercent = 10;
            MinimumDensity = 5.0;
            ProspectingFor.Add(Commodities.Tritium);
            ProspectingFor.Add(Commodities.Platinum);
            MentionPotentiallyMineableRings = true;
            MentionPotentiallyMineableBodies = false;
            MentionHighMaterialContent = false;
        }

        // TODO: Put notifiers on these properties and respond to them at run-time to avoid a restart?
        [SettingNewGroup("Enabled Cards - Requires restart")]
        [SettingDisplayName("Hide Ship Panel")]
        public bool ShipPanelHidden { get; set; }

        [SettingDisplayName("Hide Route Panel")]
        public bool RoutePanelHidden { get; set; }

        [SettingDisplayName("Hide System Panel")]
        public bool SystemPanelHidden { get; set; }

        [SettingDisplayName("Hide Body Panel")]
        public bool BodyPanelHidden { get; set; }

        //[SettingDisplayName("Hide Station Panel")]
        //public bool StationPanelHidden { get; set; }

        [SettingDisplayName("Hide Cargo Panel")]
        public bool CargoPanelHidden { get; set; }

        [SettingDisplayName("Hide Prospector Panel")]
        public bool ProspectorPanelHidden { get; set; }

        [SettingDisplayName("Hide Messages Panel")]
        public bool MessagesPanelHidden { get; set; }

        [SettingNewGroup("Display Options")]
        [SettingDisplayName("Font Size Adjustment")]
        [SettingNumericUseSlider]
        [SettingNumericBounds(-5.0, 15.0)]
        public double FontSizeAdjustment
        {
            get => _fontSizeAdjustment;
            set
            {
                _fontSizeAdjustment = Math.Clamp(value, -5.0, 15);
                OnPropertyChanged("FontSizeAdjustment");
            }
        }

        [SettingNewGroup("Notifications")]
        [SettingDisplayName("Notify if current undiscovered system is not fully scanned when starting jump")]
        public bool WarnIncompleteUndiscoveredSystemScan { get; set; }

        [SettingDisplayName("Notify when FSD is supercharged")]
        public bool NotifyJetConeBoost { get; set; }

        [SettingDisplayName("Enable high gravity advisory on approach to planet")]
        public bool EnableHighGravityAdvisory { get; set; }

        [SettingDisplayName("Enable card layout wrap-around")]
        public bool EnableWrappedLayout
        {
            get => _enableWrappedLayout;
            set
            {
                _enableWrappedLayout = value;
                OnPropertyChanged("EnableWrappedLayout");
            }
        }

        [SettingDisplayName("Use in-game time")]
        public bool UseInGameTime
        {
            get => _useInGameTime;
            set
            {
                _useInGameTime = value;
                OnPropertyChanged("UseInGameTime");
            }
        }

        [SettingNewGroup("Tuning")]

        [SettingDisplayName("Gravity advisory threshold")]
        [SettingNumericBounds(0.0, 10.0, 0.1)]
        public double GravityAdvisoryThreshold { get; set; }

        [SettingDisplayName("Bubble radius (Ly)")]
        [SettingNumericBounds(100.0, 1500.0, 100.0)]
        public double BubbleRadiusLy { get; set; }

        [SettingDisplayName("Maximum secondary scoopable star distance (Ls) when Neutron hopping")]
        [SettingNumericBounds(500, 50000, 500)]
        public int MaxNearbyScoopableDistance { get; set; }


        [SettingNewGroup("Prospecting / Mining")]

        [SettingDisplayName("Select commodities to watch")]
        [JsonIgnore]
        public Action CommoditySelector { get; internal set; }

        [SettingDisplayName("Notify ring types with potential items of interest")]
        public bool MentionPotentiallyMineableRings { get; set; }

        [SettingDisplayName("Notify body types with potential items of interest")]
        public bool MentionPotentiallyMineableBodies { get; set; }

        [SettingDisplayName("Notify High material content asteroids")]
        public bool MentionHighMaterialContent { get; set; }

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

        [SettingDisplayName("Notify if no limpets in cargo when mining modules equipped when undocking")]
        public bool LimpetReminderNotifications { get; set; }

        [SettingDisplayName("Minimum number of mining modules equipped for limpet notification")]
        [SettingNumericBounds(1, 15)]

        public int LimpetNotificationMinMiningModules { get; set; }

        [SettingDisplayName("Minimum % content required for an asteroid to be 'good'")]
        [SettingNumericBounds(0.0, 66.0)]
        public double MinimumPercent { get; set; }

        [SettingDisplayName("Minimum ring density to trigger notification (MT/km^3)")]
        [SettingNumericBounds(0.0, 9.0)]
        public double MinimumDensity { get; set; }

        [SettingIgnore]
        public HashSet<Commodities> ProspectingFor { get; init; } = [];

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

        public List<Commodities> DesireableCommoditiesByRingType(RingType ringType)
        {
            return [.. ProspectingFor.Where(c => c.Details().RingType.HasFlag(ringType))];
        }

        public List<Commodities> DesireableCommoditiesByBodyType(BodyType bodyType)
        {
            return [.. ProspectingFor.Where(c => c.Details().BodyType.HasFlag(bodyType))];
        }

        [SettingNewGroup("Updates")]
        [SettingDisplayName("Enable automatic updates")]
        public bool EnableAutoUpdates { get; set; }

        [SettingDisplayName("Enable Beta versions (warning: things may break)")]
        public bool EnableBetaUpdates { get; set; }

        #region Internal settings.
        [SettingIgnore]
        public List<HelmContext.Card> CardOrdering { get; set; }
        public List<HelmContext.Card> CollapsedCards { get; set; }
        #endregion


        public event PropertyChangedEventHandler PropertyChanged;
        internal void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
