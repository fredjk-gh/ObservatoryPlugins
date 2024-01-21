using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    internal class CarrierManager
    {
        private Dictionary<string, CarrierData> _knownCarriersByCallsign = new();

        public CarrierData RegisterCarrier(string commander, CarrierBuy buyData)
        {
            CarrierData data = new(commander, buyData); // Sets fuel too.
            data.MaybeUpdateLocation(new(buyData.Location, buyData.SystemAddress));

            _knownCarriersByCallsign.Add(buyData.Callsign, data);
            return data;
        }

        public CarrierData RegisterCarrier(string commander, CarrierStats stats, Location initialLocation = null)
        {
            CarrierData data = new(commander, stats); // Sets fuel too.
            if (initialLocation != null && initialLocation.StationName == stats.Callsign && (initialLocation.Docked || initialLocation.OnFoot))
            {
                data.MaybeUpdateLocation(new(initialLocation));
            }

            _knownCarriersByCallsign.Add(stats.Callsign, data);
            return data;
        }

        public bool IsCallsignKnown(string callSign)
        {
            return _knownCarriersByCallsign.ContainsKey(callSign);
        }

        public CarrierData GetByCallsign(string callSign)
        {
            if (_knownCarriersByCallsign.ContainsKey(callSign))
                return _knownCarriersByCallsign[callSign];

            return null;
        }
        public CarrierData GetById(ulong id)
        {
            return _knownCarriersByCallsign.Values.Where(c => c.CarrierId == id).FirstOrDefault();
        }

        public CarrierData GetByTimer(System.Timers.Timer timer)
        {
            return _knownCarriersByCallsign.Values.Where(c => c.CarrierCooldownTimer == timer).FirstOrDefault();
        }

        public void Clear()
        {
            _knownCarriersByCallsign.Clear();
        }
    }
}
