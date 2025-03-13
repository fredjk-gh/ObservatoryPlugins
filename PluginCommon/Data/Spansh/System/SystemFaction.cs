using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.System
{
    public class SystemFaction : GenericJsonBase
    {
        [JsonPropertyName("allegiance")]
        public string Allegiance { get; init; }

        [JsonPropertyName("government")]
        public string Government { get; init; }

        [JsonPropertyName("influence")]
        public double Influence { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("state")]
        public string State { get; init; }
    }
}