using System.Text.Json.Serialization;
using com.github.fredjk_gh.PluginCommon.Data.Journals;
using com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryHelm.Data
{
    internal class PlanetData
    {
        public enum SignalType
        {
            Biological,
            Geological,
        }

        private SystemData _system;

        public PlanetData() // For deserialization.
        { }

        internal PlanetData(SystemData parentSystem, Scan scan)
        {
            _system = parentSystem;
            Scan = scan;
        }

        public void Init(SystemData parentSystem)
        {
            _system = parentSystem;
        }

        [JsonIgnore]
        public int BodyId { get => Scan.BodyID; }
        public Scan Scan { get; init; }


        [JsonIgnore]
        public List<SAASignalsFound> BodySignals
        {
            get => _system._bodySignals.GetValueOrDefault(BodyId, null);
        }

        public int SignalCountByType(SignalType type)
        {
            if (BodySignals == null) return 0;
            return type switch
            {
                SignalType.Biological => BodySignals
                                        .SelectMany(l => l.Signals)
                                        .Where(s => s.Type == "$SAA_SignalType_Biological;")
                                        .Select(s => s.Count)
                                        .FirstOrDefault(0),
                SignalType.Geological => BodySignals
                                        .SelectMany(l => l.Signals)
                                        .Where(s => s.Type == "$SAA_SignalType_Geological;")
                                        .Select(s => s.Count)
                                        .FirstOrDefault(0),
                _ => 0,
            };
        }

        [JsonIgnore]
        public string ShortName
        {
            get => Scan.BodyName.Replace(_system.SystemName, "").Trim();
        }
        [JsonIgnore]
        public string DisplayName
        {
            get
            {
                if (!Scan.BodyName.Contains(_system.SystemName))
                {
                    return Scan.BodyName;
                }
                return $"Body {Scan.BodyName.Replace(_system.SystemName, "").Trim()}";
            }
        }

        [JsonIgnore]
        public string TypeDescription
        {
            get
            {
                if (FDevIDs.BodiesById.TryGetValue(Scan.PlanetClass, out IdName result))
                {
                    return result.Name;
                }
                else
                {
                    return Scan.PlanetClass;
                }
            }
        }

        [JsonIgnore]
        public bool WasMapped
        {
            get => _system._dssScans.ContainsKey(BodyId);
        }

        [JsonIgnore]
        public bool IsLandable
        {
            get => Scan.Landable;
        }

        [JsonIgnore]
        public bool IsHighValue
        {
            get => JournalConstants.HighValueNonTerraformablePlanetClasses.Contains(Scan.PlanetClass)
                || !string.IsNullOrEmpty(Scan.TerraformState);
        }
    }
}
