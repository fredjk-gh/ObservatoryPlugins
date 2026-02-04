using Observatory.Framework;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static com.github.fredjk_gh.ObservatoryAggregator.AggregatorSettings;

[assembly: InternalsVisibleTo("ObservatoryPlugins.Tests")]
namespace com.github.fredjk_gh.ObservatoryAggregator
{
    internal class AggregatorSettings : INotifyPropertyChanged
    {
        public enum GridSizingMode
        {
            AutoFit,
            Manual
        }

        private static readonly Dictionary<string, object> GRID_SIZING_MODES = new()
        {
            { "AutoFit", GridSizingMode.AutoFit },
            { "Manual", GridSizingMode.Manual },
        };

        public static readonly AggregatorSettings DEFAULT = new()
        {

        };

        private string _filterSpec = "";
        private List<String> _filters = [];
        private bool _showAllBodySummaries = false;
        private bool _autoMarkOnTarget = true;
        private double _fontSizeAdjustment = 0.0;
        private string _gridSizingModeValue = "";
        private Dictionary<string, int> _columnSizes = [];
        private Dictionary<string, int> _columnOrder = [];

        public AggregatorSettings()
        {
            // Defaults go here (unless there's a backing variable above, which suffices).
            GridSizingModeEnum = GridSizingMode.AutoFit;
            EnableAutoUpdates = true;
        }

        [SettingIgnore]
        public List<string> Filters { get => _filters; }

        [SettingNewGroup("Filtering")]
        [SettingDisplayName("Exclude matching (| separated literals)")]
        public string FilterSpec
        {
            get => _filterSpec;
            set
            {
                _filterSpec = value;
                _filters = value?.Split('|').ToList() ?? [];
            }
        }

        [SettingNewGroup("Display")]
        [SettingDisplayName("Show all body summaries even if no notification")]
        public bool ShowAllBodySummaries
        {
            get => _showAllBodySummaries;
            set
            {
                _showAllBodySummaries = value;
                OnPropertyChanged("ShowAllBodySummaries");
            }
        }

        [SettingDisplayName("Mark bodies as interesting when targeted")]
        public bool AutoMarkBodiesWhenTargeted
        {
            get => _autoMarkOnTarget;
            set
            {
                _autoMarkOnTarget = value;
                OnPropertyChanged("AutoMarkBodiesWhenTargeted");
            }
        }

        [SettingDisplayName("Font Size Adjustment")]
        [SettingNumericBounds(-5.0, 10.0)]
        public double FontSizeAdjustment
        {
            get => _fontSizeAdjustment;
            set
            {
                _fontSizeAdjustment = value;
                OnPropertyChanged("FontSizeAdjustment");
            }
        }

        [SettingDisplayName("Grid column sizing mode")]
        [SettingBackingValue("GridSizingModeString")]
        public Dictionary<string, object> GridSizingModeOptions
        {
            get => GRID_SIZING_MODES;
        }

        [SettingIgnore]
        public string GridSizingModeString
        {
            get => _gridSizingModeValue;
            set
            {
                _gridSizingModeValue = value;
                OnPropertyChanged("GridSizingMode");
            }
        }

        [SettingIgnore]
        public GridSizingMode GridSizingModeEnum
        {
            get
            {
                if (Enum.TryParse<GridSizingMode>(GridSizingModeString, out GridSizingMode v))
                    return v;
                return GridSizingMode.AutoFit;
            }
            set => GridSizingModeString = value.ToString();
        }

        [SettingIgnore]
        public Dictionary<string, int> ColumnSizes
        {
            get => _columnSizes;
            set => _columnSizes = value; // Always programmatically set; no notify to avoid loops.
        }

        [SettingIgnore]
        public Dictionary<string, int> ColumnOrder
        {
            get => _columnOrder;
            set => _columnOrder = value; // Always programmatically set; no notify to avoid loops.
        }

        [SettingNewGroup("Updates")]
        [SettingDisplayName("Enable automatic updates")]
        public bool EnableAutoUpdates { get; set; }

        [SettingDisplayName("Enable Beta versions (warning: things may break)")]
        public bool EnableBetaUpdates { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        internal void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
