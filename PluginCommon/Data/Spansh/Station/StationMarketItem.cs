using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.Station
{
    public class StationMarketItem : GenericJsonBase
    {
        [JsonPropertyName("buy_price")]
        public int BuyPrice { get; init; }

        [JsonPropertyName("category")]
        public string Category { get; init; }

        [JsonPropertyName("commodity")]
        public string Commodity { get; init; }

        [JsonPropertyName("demand")]
        public int Demand { get; init; }

        [JsonPropertyName("sell_price")]
        public int SellPrice { get; init; }

        [JsonPropertyName("supply")]
        public int Supply { get; init; }
    }
}
