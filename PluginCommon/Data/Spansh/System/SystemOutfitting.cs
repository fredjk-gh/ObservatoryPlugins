using com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric;
using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.System
{
    public class SystemOutfitting : UpdateTimeJsonBase
    {
        [JsonPropertyName("modules")]
        public List<SystemOutfittingModule> Modules { get; init; }
    }
}