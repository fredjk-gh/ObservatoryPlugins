using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.System
{
    public class SpanshSystemDump : GenericJsonBase
    {
        [JsonPropertyName("system")]
        public SpanshSystem System { get; init; }
    }
}
