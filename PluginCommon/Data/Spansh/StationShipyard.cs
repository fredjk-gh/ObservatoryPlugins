using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh
{
    public class StationShipyard : UpdateTimeJsonBase
    {
        [JsonPropertyName("ships")]
        public List<Ship> Ships { get; init; }
    }
}