using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.System.Station
{
    public class EconomyItem : GenericJsonBase
    {
        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("share")]
        public int Share { get; init; }
    }
}
