using com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric;
using com.github.fredjk_gh.PluginCommon.Data.Spansh.Converters;
using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.System
{
    public class BodySignals : UpdateTimeJsonBase

    {
        [JsonPropertyName("genuses")]
        public List<string> Genuses { get; init; }

        [JsonPropertyName("signals")]
        [JsonConverter(typeof(NameIntItemConverter<NameIntItem>))]
        public List<NameIntItem> Signals { get; init; }
    }
}
