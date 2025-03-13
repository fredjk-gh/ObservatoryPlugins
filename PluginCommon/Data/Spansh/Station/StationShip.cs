using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.Station
{
    public class StationShip : GenericJsonBase
    {
        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("price")]
        public long Price { get; init; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; init; }
    }
}