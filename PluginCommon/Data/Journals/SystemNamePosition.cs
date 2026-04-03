using com.github.fredjk_gh.PluginCommon.Data.Id64;
using Observatory.Framework.Files.ParameterTypes;
using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.Data.Journals
{
    public class SystemNamePosition
    {
        protected string _systemName;

        // for deserialization
        public SystemNamePosition() { }

        public SystemNamePosition(string systemName, StarPosition starPos)
        {
            _systemName = systemName;
            Position = starPos;
        }

        [JsonIgnore]
        public virtual string SystemName
        {
            get => _systemName;
        }

        // For deserialization
        public string InternalSystemName
        {
            get => _systemName;
            set => _systemName = value;
        }

        public StarPosition Position { get; set; }

        public double? DistanceFrom(SystemNamePosition other)
        {
            if (Position == null || other == null || other.Position == null) return null;

            return Id64CoordHelper.Distance(Position, other.Position);
        }
    }
}
