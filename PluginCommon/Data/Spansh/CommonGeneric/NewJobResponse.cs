using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric
{
    public class NewJobResponse
    {
        [JsonPropertyName("job")]
        public string Job { get; init; }

        [JsonPropertyName("status")]
        public string Status { get; init; }
    }
}
