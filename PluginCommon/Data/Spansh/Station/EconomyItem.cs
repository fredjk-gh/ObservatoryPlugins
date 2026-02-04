using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.Station
{
    public class EconomyItem : GenericJsonBase
    {
        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("share")]
        public int Share { get; init; }
    }
}
