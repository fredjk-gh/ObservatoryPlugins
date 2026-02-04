using System.Text;
using System.Text.Json.Serialization;
using com.github.fredjk_gh.PluginCommon.Data.Id64;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;

namespace com.github.fredjk_gh.ObservatoryHelm.Data
{
    internal class SystemData : SystemBasicData
    {
        internal Dictionary<int, List<SAASignalsFound>> _bodySignals = [];
        internal Dictionary<int, SAAScanComplete> _dssScans = [];
        internal FSDJump _jump = null;

        // For deserialization.
        public SystemData() : base()
        { }

        public SystemData(UInt64 id64, string systemName, StarPosition starPos)
            : base(id64, systemName, starPos)
        { }

        // For deserialization.
        public Dictionary<int, List<SAASignalsFound>> InternalBodySignals
        { 
            get => _bodySignals;
            set => _bodySignals = value;
        }
        // For deserialization.
        public Dictionary<int, SAAScanComplete> DSSScans
        {
            get => _dssScans;
            set => _dssScans = value;
        }

        public Dictionary<int, StarData> Stars { get; set; } = [];
        public Dictionary<int, ScanBaryCentre> BaryCentreScans { get; set; } = [];
        public Dictionary<int, PlanetData> Planets { get; set; } = [];
        public FSSDiscoveryScan Honk { get; set; }
        public FSSAllBodiesFound AllBodiesFound { get; set; }
        public FSDJump FSDJump { get; set; }
        [JsonIgnore]
        public int BodyCount { get => Honk?.BodyCount ?? AllBodiesFound?.Count ?? -1; }
        [JsonIgnore]
        public bool IsFullyDiscovered { get => AllBodiesFound != null; }
        [JsonIgnore]
        public bool IsFirstDiscovery
        {
            get
            {
                StarData entryStar = Stars.Values.FirstOrDefault(s => s.Scan.DistanceFromArrivalLS == 0);
                return (entryStar != null && entryStar.IsFirstDiscovery);
            }
        }
        public bool AddScan(ScanBaryCentre scanObj) // return true if it's a type of scan we find useful.
        {
            if (scanObj.SystemAddress != SystemId64) return false;
            if (scanObj is not Scan scan) // it's just a barycentre.
            {
                BaryCentreScans[scanObj.BodyID] = scanObj;
                return true;
            }
            if (!string.IsNullOrWhiteSpace(scan.StarType))
            {
                Stars[scan.BodyID] = new(this, scan);
                return true;
            }
            if (!string.IsNullOrWhiteSpace(scan.PlanetClass))
            {
                Planets[scan.BodyID] = new(this, scan);
                return true;
            }
            return false;
        }

        public void AddScan(SAAScanComplete scanComplete)
        {
            if (scanComplete.SystemAddress != SystemId64) return;

            _dssScans[scanComplete.BodyID] = scanComplete;
        }

        public bool ContainsScan(ScanBaryCentre scanObj)
        {
            if (scanObj.SystemAddress != SystemId64) return false;

            if (scanObj is not Scan scan) // it's just a barycentre.
            {
                return BaryCentreScans.ContainsKey(scanObj.BodyID);
            }
            if (!string.IsNullOrWhiteSpace(scan.StarType))
            {
                return Stars.ContainsKey(scan.BodyID);
            }
            if (!string.IsNullOrWhiteSpace(scan.PlanetClass))
            {
                return Planets.ContainsKey(scan.BodyID);
            }
            return false;
        }

        public void AddSignals(SAASignalsFound signals)
        {
            if (signals.SystemAddress != SystemId64) return;

            _bodySignals.TryAdd(signals.BodyID, []);
            _bodySignals[signals.BodyID].Add(signals);
        }

        [JsonIgnore]
        public bool? IsInSuppressionZone
        {
            get
            {
                if (Position == null) return null;
                if (IsInBubble.HasValue && IsInBubble.Value) return false;

                // This is a gross area that approximately encompasses all the known suppression zones.
                return ((Position.x > -1500 && Position.x < 1500) || (Position.z > -2500 && Position.z < 2500));
            }
        }

        [JsonIgnore]
        public string SuppressionZoneDetails
        {
            get
            {
                if (!IsInSuppressionZone ?? false) return string.Empty;
                bool hasResults = false;
                StringBuilder sb = new();
                sb.AppendLine("This system may be in a star-type suppression zone. Potentially affected star types:");

                foreach (var spec in SuppressionSpec.StarType)
                {
                    if (spec.Process(Position, out string detail))
                    {
                        sb.AppendLine($"- {detail}");
                        hasResults = true;
                    }
                }
                if (hasResults) return sb.ToString();

                return "This system may be in a star-type suppression zone."; // This should never happen.
            }
        }
        [JsonIgnore]
        public bool? IsInHeSuppressionBubble
        {
            get
            {
                if (Position == null) return null;

                return (Id64CoordHelper.Distance(new() { x = 0, y = 0, z = 27500 }, Position) < 7000
                    || Id64CoordHelper.Distance(new() { x = -3500, y = 0, z = 24000 }, Position) < 6000);
            }
        }

        [JsonIgnore]
        public bool? IsInBubble
        {
            get
            {
                if (Position == null) return null;

                var radius = HelmContext.Instance.Settings.BubbleRadiusLy;
                if (Math.Abs(Position.y) < radius && Math.Abs(Position.x) < radius && Math.Abs(Position.z) < radius) return true;

                return false;
            }
        }
    }
}
