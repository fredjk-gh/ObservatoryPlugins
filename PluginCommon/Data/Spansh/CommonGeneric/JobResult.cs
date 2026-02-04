using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric
{
    public class JobResult : GenericJsonBase
    {
        [JsonPropertyName("error")]
        public string Error { get; init; }

        [JsonPropertyName("job")]
        public Guid Job { get; init; }

        [JsonPropertyName("state")]
        public string State { get; init; }

        [JsonPropertyName("status")]
        public string Status { get; init; }
    }
}
