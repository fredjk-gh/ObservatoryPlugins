using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh
{
    public class SpanshOutfitting : UpdateTimeJsonBase
    {
        [JsonPropertyName("modules")]
        public List<SpanshOutfittingModule> Modules { get; init; }
    }
}