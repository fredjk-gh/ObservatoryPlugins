using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric
{
    public class NamedItem : GenericJsonBase
    {
        [JsonPropertyName("name")]
        public string Name { get; init; }
    }
}
