using Observatory.Framework;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ObservatoryPlugins.Tests")]
namespace com.github.fredjk_gh.ObservatoryAggregator
{
    [SettingSuggestedColumnWidth(450)]
    internal class AggregatorSettings
    {
        public static readonly AggregatorSettings DEFAULT = new ()
        {
            ShowAllBodySummaries = false,
            FilterSpec = "",
        };

        private string _filterSpec = "";
        private List<String> _filters = new();

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
        // TODO: add button for filter help and/or UI for composing Filters?

        [SettingNewGroup("Display")]
        [SettingDisplayName("Show all body summaries even if no notification")]
        public bool ShowAllBodySummaries { get; set; }

        [SettingDisplayName("Open Wiki (help, legend and info)")]
        [System.Text.Json.Serialization.JsonIgnore]
        public Action OpenAggregatorWiki { get; internal set; }
    }
}
