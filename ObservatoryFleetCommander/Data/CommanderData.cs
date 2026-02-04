using Observatory.Framework.Files.Journal;
using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.ObservatoryFleetCommander.Data
{
    internal class CommanderData
    {
        public string FID { get; set; }

        public string Name { get; set; }

        public CarrierData Carrier { get; set; }

        public bool HasCarrier { get => Carrier is not null; }

        public ulong? LastDockedStationId { get; set; } = null;

        public bool IsSRVDeployed { get; set; } = false;

        [JsonIgnore]
        public Statistics LastStatisticsEvent { get; set; } = null;
    }
}
