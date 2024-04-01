using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    public class CarrierData
    {
        private const string UNNAMED = "(unnamed)";

        private bool _deserialized = false;
        private CarrierStats _lastStats;

        // Only to be used for JSON Deserialization.
        public CarrierData()
        {
            CarrierName = UNNAMED;
            _deserialized = true;
        }

        public CarrierData(string commander, CarrierBuy buy)
        {
            OwningCommander = commander;
            CarrierFuel = 500;
            CarrierCallsign = buy.Callsign;
            CarrierId = buy.CarrierID;
            CarrierName = UNNAMED;
        }

        public CarrierData(string commander, CarrierStats stats)
        {
            OwningCommander = commander;
            CarrierFuel = stats.FuelLevel;
            CarrierCallsign = stats.Callsign;
            CarrierId = stats.CarrierID;
            CarrierName = stats.Name;
            _lastStats = stats;
        }

        public string OwningCommander { get; set;  }
        public bool CommanderIsDockedOrOnFoot { get; set; }
        public string CarrierName { get; set; }
        public ulong CarrierId { get; set; }
        public string CarrierCallsign { get; set; }
        public int CarrierFuel { get; set; }
        public long CapacityUsed { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public CarrierStats LastCarrierStats {
            get => _lastStats;
            set
            {
                if ((CarrierId == 0 && _deserialized) || CarrierId == value.CarrierID)
                {
                    CarrierName = value.Name;
                    CapacityUsed = value.SpaceUsage.TotalCapacity - value.SpaceUsage.FreeSpace;
                    _lastStats = value;
                    _deserialized = false;
                }
            }
        }

        public CarrierJumpRequest LastCarrierJumpRequest { get; set; }

        public CarrierPositionData Position { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public bool IsPositionKnown
        {
            get => Position != null;
        }

        [System.Text.Json.Serialization.JsonIgnore]
        public bool CooldownNotifyScheduled { get => CarrierCooldownTimer != null; }

        [System.Text.Json.Serialization.JsonIgnore]
        internal System.Timers.Timer CarrierCooldownTimer { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public bool CarrierJumpTimerScheduled { get => CarrierJumpTimer != null; }

        [System.Text.Json.Serialization.JsonIgnore]
        internal System.Timers.Timer CarrierJumpTimer { get; set; }
        
        public void CancelCarrierJump()
        {
            LastCarrierJumpRequest = null;
            ClearTimers();
        }

        public void ClearTimers()
        {
            if (CarrierCooldownTimer != null)
            {
                CarrierCooldownTimer.Stop();
                CarrierCooldownTimer = null;
            }
            if (CarrierJumpTimer != null)
            {
                CarrierJumpTimer.Stop();
                CarrierJumpTimer = null;
            }
        }

        public bool MaybeUpdateLocation(CarrierPositionData newPosition)
        {
            if (newPosition == null
                || (Position?.IsSamePosition(newPosition) ?? false)) return false;

            Position = newPosition;
            return true;
        }

        public CarrierRoute Route { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public bool HasRoute { get => Route != null; }
    }
}
