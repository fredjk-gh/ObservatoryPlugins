using System.Text.Json.Serialization;
using com.github.fredjk_gh.PluginCommon.Data.Journals;
using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryHelm.Data
{
    public class SRVData
    {
        private Dictionary<string, int> _cargo = [];

        public Dictionary<string, int> Cargo
        {
            get => _cargo;
            set => _cargo = value; // For deserialization
        }

        public SRVs.Type? ActiveSRVType { get; set; }

        [JsonIgnore]
        public SRVs.Details ActiveSRVDetails
        {
            get
            {
                if (!ActiveSRVType.HasValue) return null;

                return SRVs.GetSRVDetails(ActiveSRVType.Value);
            }
        }

        public void ClearCargo()
        {
            _cargo.Clear();
        }

        public void UpdateCargo(CargoFile cargo)
        {
            CargoHelper.UpdateCargo(_cargo, cargo);
        }

        public void UpdateCargo(Cargo cargo)
        {
            CargoHelper.UpdateCargo(_cargo, cargo);
        }
    }
}
