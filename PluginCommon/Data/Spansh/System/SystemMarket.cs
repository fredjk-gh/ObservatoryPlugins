using com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric;
using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.System
{
    public class SystemMarket : UpdateTimeJsonBase
    {
        [JsonPropertyName("commodities")]
        public List<SystemCommodity> Commodities { get; init; }

        [JsonPropertyName("prohibitedCommodities")]
        public List<string> ProhibitedCommodities { get; init; }
    }
}