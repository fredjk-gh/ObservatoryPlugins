using System.Text.Json.Serialization;
using Observatory.Framework.Files;

namespace com.github.fredjk_gh.ObservatoryHelm.Data
{
    internal class DerivedRouteData
    {
        internal static DerivedRouteData FromNavRouteFile(NavRouteFile route)
        {
            return new(
                SystemBasicData.FromRouteJump(route.Route[0]),
                SystemBasicData.FromRouteJump(route.Route[^1]),
                route.Route.Count - 1);
        }

        private readonly string _destSystemName = null;

        // only for deserialization
        public DerivedRouteData()
        { }

        internal DerivedRouteData(SystemBasicData origin, SystemBasicData dest, int jumps)
        {
            OriginSystem = origin;
            DestinationSystem = dest;
            Jumps = jumps;
        }
        internal DerivedRouteData(SystemBasicData origin, string dest, int jumps)
        {
            OriginSystem = origin;
            DestinationSystem = null;
            _destSystemName = dest;
            Jumps = jumps;
        }

        public SystemBasicData OriginSystem { get; init; }

        public SystemBasicData DestinationSystem { get; init; }

        [JsonIgnore]
        public string DestinationSystemName
        {
            get => DestinationSystem?.SystemName ?? _destSystemName;
        }

        public int Jumps { get; set; }
    }
}
