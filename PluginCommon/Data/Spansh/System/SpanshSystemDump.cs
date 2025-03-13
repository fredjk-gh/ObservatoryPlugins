using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.System
{
    public class SpanshSystemDump : GenericJsonBase
    {
        [JsonPropertyName("system")]
        public SpanshSystem System { get; init; }
    }
}
