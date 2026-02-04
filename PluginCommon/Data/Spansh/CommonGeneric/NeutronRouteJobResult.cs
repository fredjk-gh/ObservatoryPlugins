using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric
{
    public class NeutronRouteJobResult : JobResult
    {
        [JsonPropertyName("parameters")]
        public Parameters Params { get; set; }

        [JsonPropertyName("result")]
        public RouteResult Result { get; set; }

        public class Parameters : GenericJsonBase
        {
            [JsonPropertyName("efficiency")]
            public int Efficiency { get; set; } = 60;

            [JsonPropertyName("from")]
            public string From { get; set; }

            [JsonPropertyName("to")]
            public string To { get; set; }

            [JsonPropertyName("range")]
            public double Range { get; set; }

            [JsonPropertyName("supercharge_multiplier")]
            public int SuperchargeMultiplier { get; set; } = 4;

            [JsonPropertyName("via")]
            public List<string> Via { get; set; }

            public bool IsValid()
            {
                if (string.IsNullOrWhiteSpace(From)) return false;
                if (string.IsNullOrWhiteSpace(To)) return false;
                if (Efficiency <= 0 || Efficiency > 100) return false;
                if (Range <= 20) return false;
                if (!(SuperchargeMultiplier == 4 || SuperchargeMultiplier == 6)) return false;
                if (Via != null && Via.Count > 0 && Via.Any(s => string.IsNullOrWhiteSpace(s))) return false;
                return true;
            }

            public string ToUrl()
            {
                if (!IsValid()) return null;

                StringBuilder sb = new("https://spansh.co.uk/api/route");

                bool first = true;
                foreach (var p in GetType().GetProperties())
                {
                    string paramName = "";
                    string value = "";

                    if (p.GetCustomAttribute(typeof(JsonPropertyNameAttribute)) is not JsonPropertyNameAttribute propNameAttr) continue;

                    paramName = propNameAttr.Name;
                    if (p.Name == "Via")
                    {
                        if (Via == null || Via.Count == 0) continue;

                        // Cheat a bit
                        value = string.Join('&', Via
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

                    string unescaped = p.GetValue(this).ToString();
                    if (!string.IsNullOrWhiteSpace(unescaped))
                    {
                        value = Uri.EscapeDataString(unescaped.Trim());
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
            [JsonPropertyName("destination_system")]
            public string DestinationSystem { get; init; }

            [JsonPropertyName("distance")]
            public double Distance { get; init; }

            [JsonPropertyName("efficiency")]
            public int Efficiency { get; init; }

            [JsonPropertyName("job")]
            public Guid Job { get; init; }

            [JsonPropertyName("range")]
            public double Range { get; init; }

            [JsonPropertyName("source_system")]
            public string SourceSystem { get; init; }

            [JsonPropertyName("total_jumps")]
            public int TotalJumps { get; init; }

            [JsonPropertyName("via")]
            public List<string> Via { get; init; }

            [JsonPropertyName("system_jumps")]
            public List<Jump> Jumps { get; init; }
        }

        public class Jump : GenericJsonBase
        {
            [JsonPropertyName("distance_jumped")]
            public double DistanceJumped { get; init; }

            [JsonPropertyName("distance_left")]
            public double DistanceLeft { get; init; }

            [JsonPropertyName("id64")]
            public ulong Id64 { get; init; }

            [JsonPropertyName("jumps")]
            public int Jumps { get; init; }

            [JsonPropertyName("neutron_star")]
            public bool IsNeutronStar { get; init; }

            [JsonPropertyName("system")]
            public string SystemName { get; init; }

            [JsonPropertyName("x")]
            public double X { get; init; }

            [JsonPropertyName("y")]
            public double Y { get; init; }

            [JsonPropertyName("z")]
            public double Z { get; init; }
        }
    }
}
