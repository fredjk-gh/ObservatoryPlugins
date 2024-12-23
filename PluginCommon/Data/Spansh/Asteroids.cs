using com.github.fredjk_gh.PluginCommon.Data.Spansh.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh
{
    public class Asteroids : GenericJsonBase
    {
        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("mass")]
        [JsonConverter(typeof(Int64Converter))]
        public long Mass { get; init; }

        [JsonPropertyName("innerRadius")]
        [JsonConverter(typeof(Int64Converter))]
        public long InnerRadius { get; init; }

        [JsonPropertyName("outerRadius")]
        [JsonConverter(typeof(Int64Converter))]
        public long OuterRadius { get; init; }

        [JsonPropertyName("type")]
        public string Type { get; init; }

        [JsonPropertyName("signals")]
        public BodySignals Signals { get; init; }

        [JsonPropertyName("id64")]
        public ulong Id64 { get; init; }
    }
}
