using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.System
{
    public class SystemOutfittingModule : GenericJsonBase
    {
        [JsonPropertyName("category")]
        public string Category { get; init; }

        [JsonPropertyName("class")]
        public int Class { get; init; }

        [JsonPropertyName("moduleId")]
        public long ModuleId { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("rating")]
        public string Rating { get; init; }

        [JsonPropertyName("ship")]
        public string Ship { get; init; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; init; }
    }
}