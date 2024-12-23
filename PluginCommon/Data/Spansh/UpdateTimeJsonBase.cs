using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh
{
    public class UpdateTimeJsonBase : GenericJsonBase
    {
        [JsonPropertyName("updateTime")]
        public string UpdateTime { get; init; }

        [JsonIgnore]
        public DateTime? UpdateDateTime
        {
            get => ParseDateTime(UpdateTime);
        }
    }
}
