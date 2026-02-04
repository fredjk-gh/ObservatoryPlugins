using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric
{
    public class NamedTimestamp : GenericJsonBase
    {
        public string Name { get; init; }
        public string Value { get; init; }

        [JsonIgnore]
        public DateTime? ValueDateTime
        {
            get => ParseDateTime(Value);
        }
    }
}
