using Observatory.Framework;

namespace ObservatoryAggregator
{
    internal class AggregatorSettings
    {
        [SettingDisplayName("Show content from current system only")]
        public bool ShowCurrentSystemOnly { get; set; }
    }
}