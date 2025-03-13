using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.Station
{
    public class StationOutfittingModule : GenericJsonBase
    {
        [JsonPropertyName("category")]
        public string Category { get; init; }

        [JsonPropertyName("class")]
        public int Class { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("price")]
        public int Price { get; init; }

        [JsonPropertyName("rating")]
        public string Rating { get; init; }

        [JsonPropertyName("ship")]
        public string Ship { get; init; }

        [JsonPropertyName("ed_symbol")]
        public string Symbol { get; init; }

        [JsonPropertyName("weapon_mode")]
        public string WeaponMode { get; init; }
    }
}