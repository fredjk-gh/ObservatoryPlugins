using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    internal class CarrierData
    {
        private CarrierStats _lastStats;
        private CarrierBuy _buy;

        public CarrierData(string commander, CarrierBuy buy)
        {
            OwningCommander = commander;
            CarrierFuel = 500;
            _buy = buy;
        }

        public CarrierData(string commander, CarrierStats stats)
        {
            OwningCommander = commander;
            CarrierFuel = stats.FuelLevel;
            _lastStats = stats;
        }

        public string OwningCommander { get; }
        public string CarrierName { get => LastCarrierStats?.Name ?? "(unnamed)"; }
        public ulong CarrierId
        {
            get
            {
                if (LastCarrierStats != null) return LastCarrierStats.CarrierID;
                if (_buy != null) return _buy.CarrierID;
                throw new InvalidOperationException("Unexpected CarrierData state: neither CarrierStats nor CarrierBuy is present!");
            }
        }
        public string CarrierCallsign
        {
            get
            {
                if (LastCarrierStats != null) return LastCarrierStats.Callsign;
                if (_buy != null) return _buy.Callsign;
                throw new InvalidOperationException("Unexpected CarrierData state: neither CarrierStats nor CarrierBuy is present!");
            }
        }

        public int CarrierFuel { get; set; }
        public CarrierStats LastCarrierStats {
            get => _lastStats;
            set
            {
                if ((_lastStats?.CarrierID ?? _buy.CarrierID) == value.CarrierID)
                {
                    _lastStats = value;
                }
            }
        }

        public string CarrierSystem { get; set; }
        public string CarrierBody { get; set; }
        public CarrierJumpRequest LastCarrierJumpRequest { get; set; }
        public bool CooldownNotifyScheduled { get; set; }
        internal System.Timers.Timer CarrierCooldownTimer { get; set; }

        public void CancelCarrierJump()
        {
            LastCarrierJumpRequest = null;
            CooldownNotifyScheduled = false;
            if (CarrierCooldownTimer != null)
            {
                CarrierCooldownTimer.Stop();
                CarrierCooldownTimer = null;
            }
        }

        public bool MaybeUpdateLocation(string starSystem, string body)
        {
            if (CarrierSystem != null && CarrierBody != null && CarrierSystem == starSystem && CarrierBody == body) return false;

            CarrierSystem = starSystem;
            CarrierBody = body;
            return true;
        }
    }
}
