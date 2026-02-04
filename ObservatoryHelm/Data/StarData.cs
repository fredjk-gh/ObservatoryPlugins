using System.Text.Json.Serialization;
using com.github.fredjk_gh.PluginCommon.Data.Journals;
using com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryHelm.Data
{
    internal class StarData
    {
        private SystemData _system;

        public StarData() // For deserialization
        { }

        internal StarData(SystemData parentSystem, Scan scan)
        {
            _system = parentSystem;
            Scan = scan;
        }

        internal void Init(SystemData parentSystem)
        {
            _system = parentSystem;
        }

        [JsonIgnore]
        public int BodyId { get => Scan.BodyID; }
        public Scan Scan { get; init; }

        [JsonIgnore]
        public string ShortName
        {
            get
            {
                if (Scan.BodyName == _system.SystemName)
                {
                    return "Primary star";
                }
                return Scan.BodyName.Replace(_system.SystemName, "").Trim();
            }
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
                else if (Scan.BodyName == _system.SystemName)
                {
                    return "Primary star";
                }
                return $"Body {Scan.BodyName.Replace(_system.SystemName, "").Trim()}";
            }
        }

        [JsonIgnore]
        public string TypeDescription
        {
            get
            {
                if (FDevIDs.BodiesById.TryGetValue(Scan.StarType, out IdName result))
                {
                    return result.Name;
                }
                else
                {
                    return Scan.StarType;
                }
            }
        }

        [JsonIgnore]
        public bool IsScoopable
        {
            get => JournalConstants.Scoopables.Contains(Scan.StarType);
        }

        [JsonIgnore]
        public bool IsFirstDiscovery
        {
            get => Scan.ScanType != "NavBeaconDetail" && !Scan.WasDiscovered;
        }
    }
}
