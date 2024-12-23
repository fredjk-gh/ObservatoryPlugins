using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh
{
    public class SpanshMarket : UpdateTimeJsonBase
    {
        [JsonPropertyName("commodities")]
        public List<Commodity> Commodities { get; init; }

        [JsonPropertyName("prohibitedCommodities")]
        public List<string> ProhibitedCommodities { get; init; }
    }
}