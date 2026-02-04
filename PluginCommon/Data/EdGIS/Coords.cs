using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.EdGIS
{
    public class Coords
    {
        [JsonPropertyName("x")]
        public double X { get; set; }

        [JsonPropertyName("y")]
        public double Y { get; set; }

        [JsonPropertyName("z")]
        public double Z { get; set; }

        public override string ToString()
        {
            return $"{X}, {Y}, {Z}";
        }
    }
}
