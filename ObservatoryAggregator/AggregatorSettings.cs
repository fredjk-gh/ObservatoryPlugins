using Observatory.Framework;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ObservatoryPlugins.Tests")]
namespace com.github.fredjk_gh.ObservatoryAggregator
{
    public class AggregatorSettings
    {
        [SettingDisplayName("Show content from current system only")]
        public bool ShowCurrentSystemOnly { get; set; }

        [SettingDisplayName("Exclude matching (| separated literals):")]
        public string FilterSpec {  get; set; }

        // TODO: add button for filter help and/or UI for composing it.
    }
}