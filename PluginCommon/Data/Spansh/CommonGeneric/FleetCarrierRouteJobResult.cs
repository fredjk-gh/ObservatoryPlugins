using Observatory.Framework.Files.ParameterTypes;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric
{
    public class FleetCarrierRouteJobResult : JobResult
    {
        [JsonPropertyName("parameters")]
        public Parameters Paramrs { get; set; }

        [JsonPropertyName("result")]
        public RouteResult Result { get; set; }

        public class Parameters : GenericJsonBase
        {
            [JsonIgnore]
            public bool CalculateStartingFuel
            {
                get => (CalculateStartingFuelInt >= 1);
                set => CalculateStartingFuelInt = (value ? 1 : 0);
            }

            [JsonPropertyName("calculate_starting_fuel")]
            public int CalculateStartingFuelInt { get; set; } = 1;

            [JsonPropertyName("capacity")]
            public int Capacity { get; set; } = 25000;

            [JsonPropertyName("capacity_used")]
            public int CapacityUsed { get; set; }

            [JsonPropertyName("current_fuel")]
            public int CurrentFuel { get; set; } = 0;

            [JsonPropertyName("mass")]
            public int Mass { get; set; } = 25000;

            [JsonPropertyName("source_system")]
            public string SourceSystem { get; set; }

            [JsonPropertyName("tritium_amount")]
            public int TritiumAmount { get; set; }

            [JsonPropertyName("refuel_destinations")]
            public List<string> RefuelDestinations { get; set; }

            [JsonPropertyName("destination_systems")]
            public List<string> DestinationSystems { get; set; }

            public bool IsValid()
            {
                if (string.IsNullOrWhiteSpace(SourceSystem)) return false;
                if (DestinationSystems != null && DestinationSystems.Count > 0 && DestinationSystems.Any(d => string.IsNullOrWhiteSpace(d))) return false;
                if (CapacityUsed <= 0 || CapacityUsed > Capacity) return false;
                if (CurrentFuel <= 0 || CurrentFuel > 1000) return false;
                if (!(Capacity == 25000 || Capacity == 60000)) return false;
                if (!(Mass == 15000 || Mass == 25000) || Mass > Capacity) return false;
                return true;
            }

            public string ToUrl()
            {
                if (!IsValid()) return null;

                StringBuilder sb = new("https://spansh.co.uk/api/fleetcarrier/route");

                bool first = true;
                foreach (var p in GetType().GetProperties())
                {
                    string paramName = "";
                    string value = "";

                    if (p.GetCustomAttribute(typeof(JsonPropertyNameAttribute)) is not JsonPropertyNameAttribute propNameAttr) continue;

                    paramName = propNameAttr.Name;
                    if (p.Name == "DestinationSystems")
                    {
                        if (DestinationSystems == null || DestinationSystems.Count == 0) continue;

                        // Cheat a bit
                        value = string.Join('&', DestinationSystems
                            .Where(v => !string.IsNullOrWhiteSpace(v))
                            .Select(v => $"{paramName}={Uri.EscapeDataString(v.Trim())}"));

                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            sb.Append(first ? "?" : "&");
                            sb.Append(value);
                            first = false;
                        }
                        continue;
                    }
                    if (p.Name == "RefuelDestinations") continue; // Not used.

                    string unescaped = p.GetValue(this).ToString().Trim();
                    if (!string.IsNullOrWhiteSpace(unescaped))
                    {
                        value = Uri.EscapeDataString(unescaped);
                    }

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        sb.Append(first ? "?" : "&");
                        sb.Append($"{paramName}={value}");
                        first = false;
                    }
                }
                return sb.ToString();
            }
        }

        public class RouteResult : GenericJsonBase
        {
            [JsonPropertyName("calculate_starting_fuel")]
            public bool CalculateStartingFuel { get; set; }

            [JsonPropertyName("capacity")]
            public int Capacity { get; init; }

            [JsonPropertyName("capacity_used")]
            public int CapacityUsed { get; init; }

            [JsonPropertyName("fuel_loaded")]
            public int FuelLoaded { get; init; }

            [JsonPropertyName("mass")]
            public int Mass { get; init; }

            [JsonPropertyName("source")]
            public string Source { get; init; }

            [JsonPropertyName("tritium_stored")]
            public int TritiumStored { get; init; }

            [JsonPropertyName("refuel_destinations")]
            public List<string> RefuelDestinations { get; set; }

            [JsonPropertyName("destinations")]
            public List<string> Destinations { get; set; }

            [JsonPropertyName("jumps")]
            public List<Jump> Jumps { get; init; }


            public Jump? Find(string systemName)
            {
                int index = IndexOf(systemName);
                if (index == -1) return null;

                return Jumps[index];
            }

            public Jump? GetNextJump(string systemName)
            {
                int index = IndexOf(systemName);
                var nextJumpIndex = index + 1;

                if (index == -1 || nextJumpIndex >= Jumps.Count) return null;

                return Jumps[nextJumpIndex];
            }

            public bool IsDestination(string systemName)
            {
                return systemName == Jumps[^1].SystemName;
            }

            public int IndexOf(string systemName)
            {
                int index = -1;
                if (string.IsNullOrWhiteSpace(systemName)) return index;

                for (int j = 0; j < Jumps.Count; j++)
                {
                    Jump jump = Jumps[j];

                    if (jump.SystemName == systemName)
                    {
                        index = j;
                        break;
                    }
                }

                return index;
            }
        }

        public class Jump : GenericJsonBase
        {
            [JsonPropertyName("distance")]
            public double Distance { get; init; }

            [JsonPropertyName("distance_to_destination")]
            public double DistanceToDestination { get; init; }

            [JsonPropertyName("fuel_in_tank")]
            public int FuelInTank { get; init; }

            [JsonPropertyName("fuel_used")]
            public int FuelUsed { get; init; }

            [JsonPropertyName("has_icy_ring")]
            public bool HasIcyRing { get; init; }

            [JsonPropertyName("id64")]
            public ulong Id64 { get; init; }

            [JsonIgnore]
            public bool IsDesiredDestination
            {
                get => (IsDesiredDestinationInt > 0);
                init => IsDesiredDestinationInt = (value ? 1 : 0);
            }

            [JsonPropertyName("is_desired_destination")]
            public int IsDesiredDestinationInt { get; set; }

            [JsonPropertyName("is_system_pristine")]
            public bool IsSystemPristine { get; init; }

            [JsonIgnore]
            public bool MustRestock 
            {
                get => MustRestockInt > 0; 
                set => MustRestockInt = (value ? 1 : 0);
            }

            [JsonPropertyName("must_restock")]
            public int MustRestockInt { get; set; }

            [JsonPropertyName("name")]
            public string SystemName { get; init; }

            [JsonPropertyName("restock_amount")]
            public int RstockAmount { get; init; }

            [JsonPropertyName("tritium_in_market")]
            public int TritiumInMarket { get; init; }

            [JsonPropertyName("x")]
            public double X { get; init; }

            [JsonPropertyName("y")]
            public double Y { get; init; }

            [JsonPropertyName("z")]
            public double Z { get; init; }

            [JsonIgnore]
            public StarPosition Position
            {
                get => new()
                {
                    x = X,
                    y = Y,
                    z = Z,
                };
            }
        }
    }
}
