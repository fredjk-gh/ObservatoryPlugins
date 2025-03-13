using com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric;
using com.github.fredjk_gh.PluginCommon.Data.Spansh.System.Station;
using Observatory.Framework.Files.ParameterTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.Station
{
    public class Station : UpdateTimeJsonBase
    {
        [JsonPropertyName("controlling_minor_faction")]
        public string ControllingMinorFaction { get; init; }

        [JsonPropertyName("distance_to_arrival")]
        public double DistanceToArrival { get; init; }

        [JsonPropertyName("economies")]
        public List<EconomyItem> Economies { get; init; }

        [JsonPropertyName("export_commodities")]
        public List<NamedItem> ExportCommodities { get; init; }

        [JsonPropertyName("government")]
        public string Government { get; init; }

        [JsonPropertyName("has_large_pad")]
        public bool HasLargePad { get; init; }

        [JsonPropertyName("has_market")]
        public bool HasMarket { get; init; }

        [JsonPropertyName("has_outfitting")]
        public bool HasOutfitting { get; init; }

        [JsonPropertyName("has_shipyard")]
        public bool HasShipyard { get; init; }

        [JsonPropertyName("import_commodities")]
        public List<NamedItem> ImportCommodities { get; init; }

        [JsonPropertyName("is_planetary")]
        public bool IsPlanetary { get; init; }

        [JsonPropertyName("large_pads")]
        public int LargePads { get; init; }

        [JsonPropertyName("market")]
        public List<StationMarketItem> Market { get; init; }

        [JsonPropertyName("market_id")]
        public long MarketId { get; init; }

        [JsonPropertyName("market_updated_at")]
        public string MarketUpdatedAt { get; init; }

        [JsonIgnore]
        public DateTime? MarketUpdatedAtDateTime
        {
            get => ParseDateTime(MarketUpdatedAt);
        }

        [JsonPropertyName("material_trader")]
        public string MaterialTrader { get; init; }

        [JsonPropertyName("medium_pads")]
        public int MediumPads { get; init; }

        [JsonPropertyName("modules")]
        public List<StationOutfittingModule> OutfittingModules { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("outfitting_updated_at")]
        public string OutfittingUpdatedAt { get; init; }

        [JsonIgnore]
        public DateTime? OutfittingUpdatedAtDateTime
        {
            get => ParseDateTime(OutfittingUpdatedAt);
        }

        [JsonPropertyName("primary_economy")]
        public string PrimaryEconomy { get; init; }

        [JsonPropertyName("secondary_economy")]
        public string SecondaryEconomy { get; init; }

        [JsonPropertyName("services")]
        public List<NamedItem> Services { get; init; }

        [JsonPropertyName("ships")]
        public List<StationShip> ShipyardShips { get; init; }

        [JsonPropertyName("shipyard_updated_at")]
        public string ShipyardUpdatedAt { get; init; }

        [JsonIgnore]
        public DateTime? ShipyardUpdatedAtDateTime
        {
            get => ParseDateTime(ShipyardUpdatedAt);
        }

        [JsonPropertyName("small_pads")]
        public int SmallPads { get; init; }

        [JsonPropertyName("system_controlling_power")]
        public string SystemControllingPower { get; init; }

        [JsonPropertyName("system_id64")]
        public ulong SystemId64 { get; init; }

        [JsonPropertyName("system_name")]
        public string SystemName { get; init; }

        [JsonPropertyName("system_power")]
        public List<string> SystemPowers { get; init; }

        [JsonPropertyName("system_power_state")]
        public string SystemPowerState { get; init; }

        [JsonPropertyName("system_primary_economy")]
        public string SystemPrimaryEconomy { get; init; }

        [JsonPropertyName("system_secondary_economy")]
        public string SystemSecondaryEconomy { get; init; }

        [JsonPropertyName("system_x")]
        public double SystemX { get; init; }

        [JsonPropertyName("system_y")]
        public double SystemY { get; init; }

        [JsonPropertyName("")]
        public double SystemZ { get; init; }

        [JsonPropertyName("technology_broker")]
        public string TechnologyBroker { get; init; }

        [JsonPropertyName("type")]
        public string StationType { get; init; }

        [JsonPropertyName("updated_at")]
        public override string UpdateTime { get; init; }
    }
}
