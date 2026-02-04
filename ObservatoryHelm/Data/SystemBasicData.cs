using System.Text.Json.Serialization;
using com.github.fredjk_gh.PluginCommon.Data.Id64;
using Observatory.Framework.Files.ParameterTypes;

namespace com.github.fredjk_gh.ObservatoryHelm.Data
{
    internal class SystemBasicData
    {
        private Id64Details _id64Details = null;
        protected string _systemName;


        internal static SystemBasicData FromRouteJump(Route jump)
        {
            return new SystemBasicData(jump.SystemAddress, jump.StarSystem, jump.StarPos);
        }

        // for deserialization
        public SystemBasicData()
        { }

        public SystemBasicData(UInt64 id64, string systemName, StarPosition starPos)
        {
            SystemId64 = id64;
            _systemName = systemName;
            Position = starPos;
            _id64Details = Id64Details.FromId64(id64);
        }

        public UInt64 SystemId64 { get; init; }

        [JsonIgnore]
        public string SystemName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_systemName))
                {
                    return _systemName;
                }
                return Id64Details.ProcGenSystemName; // Fallback to proc-gen system name.
            }
        }
        // For deserialization
        public string InternalSystemName
        {
            get => _systemName;
            set => _systemName = value;
        }
        [JsonIgnore]
        public Id64Details Id64Details
        { 
            get =>_id64Details ??= Id64Details.FromId64(SystemId64);
        }
        public StarPosition Position { get; set; }


        public double? DistanceFrom(SystemBasicData other)
        {
            if (Position == null || other == null || other.Position == null) return null;

            return Id64CoordHelper.Distance(Position, other.Position);
        }
    }
}
