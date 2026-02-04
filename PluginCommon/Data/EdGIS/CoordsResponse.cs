using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.EdGIS
{
    public class CoordsResponse
    {
        [JsonPropertyName("id64")]
        public ulong SystemId64 { get; set; }

        [JsonPropertyName("name")]
        public string SystemName { get; set; }

        [JsonPropertyName("mainstar")]
        public string MainStarType { get; set; }

        [JsonPropertyName("coords")]
        public Coords Coords { get; set; }
    }
}
