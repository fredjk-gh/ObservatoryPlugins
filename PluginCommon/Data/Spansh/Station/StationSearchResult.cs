using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.Station
{
    /// <summary>
    /// This is the root object for deserializing a response from a request to: https://spansh.co.uk/api/station/<marketId>
    /// </summary>
    public class StationSearchResult : GenericJsonBase
    {
        [JsonPropertyName("record")]
        public Station Station { get; init; }
    }
}
