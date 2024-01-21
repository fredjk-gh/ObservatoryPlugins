using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    internal class CarrierPositionData
    {
        // A global cache of system positions to improve availability of these values and reduce the need to request.
        private static Dictionary<string, (double, double, double)> _knownSystemPositions = new();

        private string _body = "";

        public CarrierPositionData(Location location)
        {
            SystemName = location.StarSystem;
            SystemAddress = location.SystemAddress;
            StarPos = location.StarPos;

            _body = location.Body;
            BodyID = location.BodyID;
        }

        public CarrierPositionData(CarrierJump carrierJump)
        {
            SystemName = carrierJump.StarSystem;
            SystemAddress = carrierJump.SystemAddress;
            StarPos = carrierJump.StarPos;

            _body = carrierJump.Body;
            BodyID = carrierJump.BodyID;
        }

        public CarrierPositionData(string carrierSystem, ulong systemAddress, string carrierBody = "")
        {
            SystemName = carrierSystem;
            SystemAddress = systemAddress;
            _body = string.IsNullOrEmpty(carrierBody) ? carrierSystem : carrierBody;
        }

        public string SystemName { get; set; }
        public ulong SystemAddress { get; set; }
        public (double, double, double)? StarPos
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
