using Observatory.Framework;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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

        private Dictionary<string, object> GRID_SIZING_MODES = new()
        {
            { "AutoFit", GridSizingMode.AutoFit },
            { "Manual", GridSizingMode.Manual },
        };

        public static readonly AggregatorSettings DEFAULT = new()
        {
            ShowAllBodySummaries = false,
            AutoMarkBodiesWhenTargeted = true,
            FontSizeAdjustment = 0.0,
            FilterSpec = "",
            GridSizingModeEnum = GridSizingMode.AutoFit,
            ColumnSizes = new(),
            ColumnOrder = new(),
        };

        private string _filterSpec = "";
        private List<String> _filters = new();
        private bool _showAllBodySummaries = false;
        private bool _autoMarkOnTarget = true;
        private double _fontSizeAdjustment = 0.0;
        private string _gridSizingModeValue = "";
        private Dictionary<string, int> _columnSizes = new();
        private Dictionary<string, int> _columnOrder = new();

        [SettingIgnore]
        public List<string> Filters { get => _filters; }

        [SettingNewGroup("Filtering")]
        [SettingDisplayName("Exclude matching (| separated literals)")]
        public string FilterSpec {
            get => _filterSpec;
            set
            {
                _filterSpec = value;
                _filters = value?.Split('|').ToList() ?? new();
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
        
        [SettingDisplayName("Font Size adjustment")]
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
                GridSizingMode v = GridSizingMode.AutoFit;
                Enum.TryParse<GridSizingMode>(GridSizingModeString, out v);
                return v;
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

        public event PropertyChangedEventHandler PropertyChanged;
        internal void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
