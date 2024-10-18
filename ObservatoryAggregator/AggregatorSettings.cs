using Observatory.Framework;
using System.ComponentModel;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ObservatoryPlugins.Tests")]
namespace com.github.fredjk_gh.ObservatoryAggregator
{
    [SettingSuggestedColumnWidth(450)]
    internal class AggregatorSettings : INotifyPropertyChanged
    {
        public static readonly AggregatorSettings DEFAULT = new()
        {
            ShowAllBodySummaries = false,
            AutoMarkBodiesWhenTargeted = true,
            FontSizeAdjustment = 0.0,
            FilterSpec = "",
        };

        private string _filterSpec = "";
        private List<String> _filters = new();
        private bool _showAllBodySummaries = false;
        private bool _autoMarkOnTarget = true;
        private double _fontSizeAdjustment = 0.0;

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
        [SettingDisplayName("Font Size adjustment.")]
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

        public event PropertyChangedEventHandler PropertyChanged;
        internal void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
