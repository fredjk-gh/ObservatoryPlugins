using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh
{
    public class Commodity : GenericJsonBase
    {
        [JsonPropertyName("buyPrice")]
        public int BuyPrice { get; init; }

        [JsonPropertyName("category")]
        public string Category { get; init; }

        [JsonPropertyName("commodityId")]
        public long CommodityId { get; init; }

        [JsonPropertyName("demand")]
        public int Demand { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("sellPrice")]
        public int SellPrice { get; init; }

        [JsonPropertyName("supply")]
        public int Supply { get; init; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; init; }
    }
}