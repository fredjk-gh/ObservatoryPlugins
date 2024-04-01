using Observatory.Framework.Files.Converters;
using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    public class CarrierPositionData
    {
        // A global cache of system positions to improve availability of these values and reduce the need to request.
        private static Dictionary<string, (double x, double y, double z)> _knownSystemPositions = new();

        private bool _deserialized = false;
        private string _body = "";

        public static void CachePosition(string systemName, (double x, double y, double z) coords)
        {
            if (!_knownSystemPositions.ContainsKey(systemName))
                _knownSystemPositions[systemName] = coords;
        }

        // Used only for deserialization.
        public CarrierPositionData()
        {
            _deserialized = true;
        }

        public CarrierPositionData(Location location) : this(location.StarSystem, location.SystemAddress, location.Body)
        {
            StarPos = location.StarPos;
            BodyID = location.BodyID;
        }

        public CarrierPositionData(CarrierJump carrierJump) : this(carrierJump.StarSystem, carrierJump.SystemAddress, carrierJump.Body)
        {
            StarPos = carrierJump.StarPos;
            BodyID = carrierJump.BodyID;
        }

        public CarrierPositionData(CarrierJumpRequest jumpRequest) : this(jumpRequest.SystemName, jumpRequest.SystemAddress, jumpRequest.Body)
        {
            BodyID = jumpRequest.BodyID;
        }

        public CarrierPositionData(string carrierSystem, ulong systemAddress, string carrierBody = "")
        {
            SystemName = carrierSystem;
            SystemAddress = systemAddress;
            _body = string.IsNullOrEmpty(carrierBody) ? carrierSystem : carrierBody;
        }

        public string SystemName { get; set; }
        public ulong SystemAddress { get; set; }

        [JsonConverter(typeof(CoordConverter))]
        public (double x, double y, double z)? StarPos
        {
            get
            {
                if (_knownSystemPositions.ContainsKey(SystemName))
                    return _knownSystemPositions[SystemName];
                return null;
            }
            set
            {
                // If it's a non-null system position; cache it!
                if (value.HasValue && !_knownSystemPositions.ContainsKey(SystemName))
                    _knownSystemPositions[SystemName] = value.Value;
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

            return SystemName == other.SystemName && BodyName == other.BodyName;
        }
    }
}
