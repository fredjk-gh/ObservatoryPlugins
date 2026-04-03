using System.Text.Json.Serialization;
using com.github.fredjk_gh.PluginCommon.Data.Id64;
using Observatory.Framework.Files.ParameterTypes;

namespace com.github.fredjk_gh.PluginCommon.Data.Journals
{
    public class SystemBasicData : SystemNamePosition
    {
        private Id64Details _id64Details = null;

        public static SystemBasicData FromRouteJump(Route jump)
        {
            return new SystemBasicData(jump.SystemAddress, jump.StarSystem, jump.StarPos);
        }

        // for deserialization
        public SystemBasicData() : base()
        { }

        public SystemBasicData(UInt64 id64, string systemName, StarPosition starPos)
            : base(systemName, starPos)
        {
            SystemId64 = id64;
            _id64Details = Id64Details.FromId64(id64);
        }

        public UInt64 SystemId64 { get; init; }

        [JsonIgnore]
        public override string SystemName
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

        [JsonIgnore]
        public Id64Details Id64Details
        { 
            get =>_id64Details ??= Id64Details.FromId64(SystemId64);
        }
    }
}
