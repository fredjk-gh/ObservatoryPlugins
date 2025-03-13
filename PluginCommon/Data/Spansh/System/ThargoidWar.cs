using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.PluginCommon.Data.Spansh.System
{
    public class ThargoidWar : GenericJsonBase
    {
        [JsonPropertyName("currentState")]
        public string CurrentState { get; init; }

        [JsonPropertyName("daysRemaining")]
        public int DaysRemaining { get; init; }

        [JsonPropertyName("failureState")]
        public string FailureState { get; init; }

        [JsonPropertyName("portsRemaining")]
        public int PortsRemaining { get; init; }

        [JsonPropertyName("progress")]
        public double Progress { get; init; }

        [JsonPropertyName("successReached")]
        public bool SuccessReached { get; init; }

        [JsonPropertyName("successState")]
        public string SuccessState { get; init; }
    }
}
