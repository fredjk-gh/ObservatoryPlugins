using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric
{
    public class TouristRouteJobResult : JobResult
    {
        [JsonPropertyName("parameters")]
        public Parameters Params { get; set; }

        [JsonPropertyName("result")]
        public RouteResult Result { get; set; }

        public class Parameters : GenericJsonBase
        {
            [JsonIgnore]
            public bool IsLoop
            {
                get => IsLoopInt > 0;
                set => IsLoopInt = (value ? 1 : 0);
            }

            [JsonPropertyName("loop")]
            public int IsLoopInt { get; set; } = 1;

            [JsonPropertyName("final_destination")]
            public string FinalDestination { get; set; }

            [JsonPropertyName("source")]
            public string Source { get; set; }

            [JsonPropertyName("range")]
            public double Range { get; set; }

            [JsonPropertyName("destinations")]
            public List<string> Destinations { get; set; }

            public bool IsValid()
            {
                if (string.IsNullOrWhiteSpace(Source)) return false;
                if (!IsLoop && string.IsNullOrWhiteSpace(FinalDestination)) return false;
                if (Range <= 20) return false;
                if (Destinations != null && Destinations.Count > 0 && Destinations.Any(s => string.IsNullOrWhiteSpace(s))) return false;
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
                    if (p.Name == "Destinations")
                    {
                        if (Destinations == null || Destinations.Count == 0) continue;

                        // Cheat a bit
                        value = string.Join('&', Destinations
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
            [JsonPropertyName("final_destination_system")]
            public string FinalDestinationSystem { get; init; }

            [JsonPropertyName("job")]
            public Guid Job { get; init; }

            [JsonPropertyName("range")]
            public double Range { get; init; }

            [JsonPropertyName("source_system")]
            public string SourceSystem { get; init; }

            [JsonPropertyName("destination_systems")]
            public List<string> DestinationSystems { get; init; }

            [JsonPropertyName("system_jumps")]
            public List<Jump> Jumps { get; init; }
        }

        public class Jump : GenericJsonBase
        {
            [JsonPropertyName("distance")]
            public double Distance { get; init; }

            [JsonPropertyName("id64")]
            public ulong Id64 { get; init; }

            [JsonPropertyName("jumps")]
            public int Jumps { get; init; }

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
