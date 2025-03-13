using com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric;
using com.github.fredjk_gh.PluginCommon.Data.Spansh.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.System
{
    /// <summary>
    /// This is the root object for deserializing a request to https://spansh.co.uk/api/dump/<system_id64></system_id64>
    /// </summary>
    public class SpanshSystem : GenericJsonBase
    {
        [JsonPropertyName("allegiance")]
        public string Allegiance { get; init; }

        [JsonPropertyName("bodies")]
        public List<SystemBody> Bodies { get; init; }

        [JsonPropertyName("bodyCount")]
        public int BodyCount { get; init; }

        [JsonPropertyName("controllingFaction")]
        public SystemFaction ControllingFaction { get; init; }

        [JsonPropertyName("coords")]
        public SystemCoords Coords { get; init; }

        [JsonPropertyName("date")]
        public string Date { get; init; }

        [JsonIgnore]
        public DateTime? DateTime
        {
            get => ParseDateTime(Date);
        }

        [JsonPropertyName("factions")]
        public List<SystemFaction> Factions { get; init; }

        [JsonPropertyName("government")]
        public string Government { get; init; }

        [JsonPropertyName("id64")]
        public ulong Id64 { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("population")]
        public long Population { get; init; }

        [JsonPropertyName("powerState")]
        public string PowerState { get; init; }

        [JsonPropertyName("powers")]
        public List<string> Powers { get; init; }

        [JsonPropertyName("primaryEconomy")]
        public string PrimaryEconomy { get; init; }

        [JsonPropertyName("secondaryEconomy")]
        public string SecondaryEconomy { get; init; }

        [JsonPropertyName("security")]
        public string Security { get; init; }

        [JsonPropertyName("stations")]
        [JsonIgnore] // Includes fleet carriers which are transient; stations not needed today. Maybe just filter FCs?
        public List<SystemStation> Stations { get; init; }

        [JsonPropertyName("thargoidWar")]
        [JsonIgnore] // Temporary.
        public ThargoidWar ThargoidWar { get; init; }

        [JsonPropertyName("timestamps")]
        [JsonConverter(typeof(TimestampsConverter))]
        public List<NamedTimestamp> Timestamps { get; init; }
    }
}
