using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        }

        [SettingNewGroup("Notifications")]
        [SettingDisplayName("Notify if current undiscovered system is not fully scanned")]
        public bool WarnIncompleteUndiscoveredSystemScan { get; set; }

        [SettingDisplayName("Notify when FSD is supercharged")]
        public bool NotifyJetConeBoost { get; set; }

        [SettingDisplayName("Enable high gravity advisory on approach")]
        public bool EnableHighGravityAdvisory { get; set; }

        [SettingNewGroup("Tuning")]

        [SettingDisplayName("Gravity advisory threshold")]
        [SettingNumericBounds(0.0, 10.0, 0.1)]
        public double GravityAdvisoryThreshold { get; set; }

        [SettingDisplayName("Bubble radius (Ly)")]
        [SettingNumericBounds(100.0, 1500.0, 100.0)]
        public double BubbleRadiusLy { get; set; }


        [SettingNewGroup("Neutron Highway")]
        [SettingDisplayName("Maximum secondary scoopable star distance (Ls)")]
        [SettingNumericBounds(500, 50000, 500)]
        public int MaxNearbyScoopableDistance { get; set; }

        // TODO: Put notifiers on these properties and respond to them at run-time to avoid a restart?
        [SettingNewGroup("Displayed Cards - Requires restart")]
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
        [SettingDisplayName("Font Size Adjustmnet")]
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
