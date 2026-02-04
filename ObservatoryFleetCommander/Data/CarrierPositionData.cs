using com.github.fredjk_gh.PluginCommon.Data.Spansh.CommonGeneric;
using Observatory.Framework.Files.Converters;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;
using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.ObservatoryFleetCommander.Data
{
    public class CarrierPositionData
    {
        // A global cache of system positions to improve availability of these values and reduce the need to request.
        private readonly static Dictionary<string, StarPosition> _knownSystemPositions = [];

        private string _body = "";

        public static void CachePosition(string systemName, StarPosition coords)
        {
            _knownSystemPositions.TryAdd(systemName, coords);
        }

        // Used only for deserialization.
        public CarrierPositionData() { }

        public CarrierPositionData(Location location) : this(location.StarSystem, location.SystemAddress, location.StarPos, location.Body)
        {
            BodyID = location.BodyID;
        }

        public CarrierPositionData(CarrierLocation location) : this(location.StarSystem, location.SystemAddress)
        {
            BodyID = location.BodyID;
        }

        public CarrierPositionData(CarrierJump carrierJump) : this(carrierJump.StarSystem, carrierJump.SystemAddress, carrierJump.StarPos, carrierJump.Body)
        {
            BodyID = carrierJump.BodyID;
        }

        public CarrierPositionData(CarrierJumpRequest jumpRequest) : this(jumpRequest.SystemName, jumpRequest.SystemAddress, /*starPos=*/ null, jumpRequest.Body)
        {
            BodyID = jumpRequest.BodyID;
        }

        public CarrierPositionData(JumpInfo jumpInfo) : this(jumpInfo.SystemName, jumpInfo.SystemAddress, jumpInfo.Position) { }

        public CarrierPositionData(FleetCarrierRouteJobResult.Jump jumpInfo) : this(jumpInfo.SystemName, jumpInfo.Id64, jumpInfo.Position) { }

        public CarrierPositionData(string carrierSystem, ulong systemAddress, StarPosition pos = null, string carrierBody = "")
        {
            SystemName = carrierSystem;
            SystemAddress = systemAddress;
            StarPos = pos;
            _body = string.IsNullOrEmpty(carrierBody) ? carrierSystem : carrierBody;
        }


        public string SystemName { get; set; }
        public ulong SystemAddress { get; set; }

        [JsonConverter(typeof(StarPosConverter))]
        public StarPosition StarPos
        {
            get => _knownSystemPositions.GetValueOrDefault(SystemName, null);
            set
            {
                // If it's a non-null system position; cache it!
                if (value is not null)
                    _knownSystemPositions.TryAdd(SystemName, value);
            }
        }

        public string BodyName
        {
            get => string.IsNullOrEmpty(_body) ? SystemName : _body;
            set => _body = value;
        }

        public long BodyID { get; set; }

        public bool IsSamePosition(CarrierPositionData other)
        {
            if (other == null) return false;

            return SystemName == other.SystemName && BodyName.Equals(other.BodyName);
        }
    }
}
