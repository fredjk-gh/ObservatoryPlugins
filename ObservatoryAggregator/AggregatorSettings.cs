using Observatory.Framework;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ObservatoryPlugins.Tests")]
namespace com.github.fredjk_gh.ObservatoryAggregator
{
    internal class AggregatorSettings
    {
        private string _filterSpec = "";
        private List<String> _filters = new();

        [SettingIgnore]
        public List<string> Filters { get => _filters; }

        [SettingDisplayName("Show content from current system only")]
        public bool ShowCurrentSystemOnly { get; set; }

        [SettingDisplayName("Exclude matching (| separated literals):")]
        public string FilterSpec {
            get => _filterSpec;
            set
            {
                _filterSpec = value;
                _filters = value?.Split('|').ToList() ?? new();
            }
        }

        // TODO: add button for filter help and/or UI for composing Filters?
    }
}
