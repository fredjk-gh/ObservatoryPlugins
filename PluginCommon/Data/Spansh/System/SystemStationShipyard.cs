using com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric;
using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.System
{
    public class SystemStationShipyard : UpdateTimeJsonBase
    {
        [JsonPropertyName("ships")]
        public List<SystemShip> Ships { get; init; }
    }
}