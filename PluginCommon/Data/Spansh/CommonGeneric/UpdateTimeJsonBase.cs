using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric
{
    public class UpdateTimeJsonBase : GenericJsonBase
    {
        [JsonPropertyName("updateTime")]
        public virtual string UpdateTime { get; init; }

        [JsonIgnore]
        public DateTime? UpdateDateTime
        {
            get => ParseDateTime(UpdateTime);
        }
    }
}
