using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.System
{
    public class SystemShip : GenericJsonBase
    {
        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("shipId")]
        public long ShipId { get; init; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; init; }
    }
}