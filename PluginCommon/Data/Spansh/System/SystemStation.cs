using com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric;
using com.github.fredjk_gh.PluginCommon.Data.Spansh.Converters;
using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.System
{
    public class SystemStation : UpdateTimeJsonBase
    {
        [JsonPropertyName("allegiance")]
        public string Allegiance { get; init; }

        [JsonPropertyName("controllingFaction")]
        public string ControllingFaction { get; init; }

        [JsonPropertyName("controllingFactionState")]
        public string ControllingFactionState { get; init; }

        [JsonPropertyName("distanceToArrival")]
        public double DistanceToArrival { get; init; }

        [JsonPropertyName("economies")]
        [JsonConverter(typeof(NameIntItemConverter<NameIntItem>))]
        public List<NameIntItem> Economies { get; init; }

        [JsonPropertyName("government")]
        public string Government { get; init; }

        [JsonPropertyName("id")]
        public ulong Id { get; init; }

        [JsonPropertyName("landingPads")]
        [JsonConverter(typeof(NameIntItemConverter<NameIntItem>))]
        public List<NameIntItem> LandingPads { get; init; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; init; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; init; }

        [JsonPropertyName("market")]
        [JsonIgnore] // Large data, gets stale.
        public SystemMarket Market { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("outfitting")]
        [JsonIgnore] // Large data, don't care.
        public SystemOutfitting Outfitting { get; init; }

        [JsonPropertyName("primaryEconomy")]
        public string PrimaryEconomy { get; init; }

        [JsonPropertyName("services")]
        public List<string> Services { get; init; }

        [JsonPropertyName("shipyard")]
        [JsonIgnore] // Large data, don't care.
        public SystemStationShipyard Shipyard { get; init; }

        [JsonPropertyName("type")]
        public string Type { get; init; }
    }
}
