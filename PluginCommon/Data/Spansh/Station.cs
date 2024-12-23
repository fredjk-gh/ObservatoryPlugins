﻿using com.github.fredjk_gh.PluginCommon.Data.Spansh.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh
{
    public class Station : UpdateTimeJsonBase
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
        [JsonConverter(typeof(NameIntItemConverter))]
        public List<NameIntItem> Economies { get; init; }

        [JsonPropertyName("government")]
        public string Government { get; init; }

        [JsonPropertyName("id")]
        public ulong Id { get; init; }

        [JsonPropertyName("landingPads")]
        [JsonConverter(typeof(NameIntItemConverter))]
        public List<NameIntItem> LandingPads { get; init; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; init; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; init; }

        [JsonPropertyName("market")]
        [JsonIgnore] // Large data, gets stale.
        public SpanshMarket Market { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("outfitting")]
        [JsonIgnore] // Large data, don't care.
        public SpanshOutfitting Outfitting { get; init; }

        [JsonPropertyName("primaryEconomy")]
        public string PrimaryEconomy { get; init; }

        [JsonPropertyName("services")]
        public List<string> Services { get; init; }

        [JsonPropertyName("shipyard")]
        [JsonIgnore]
        public StationShipyard Shipyard { get; init; }

        [JsonPropertyName("type")]
        public string Type { get; init; }
    }
}