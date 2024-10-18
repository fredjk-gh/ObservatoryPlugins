using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryFleetCommander.Data
{
    public class CarrierData
    {
        private const int COOLDOWN_MINUTES = 5;
        private const string UNNAMED = "(unnamed)";

        private bool _deserialized = false;
        private CarrierStats _lastStats;
        private CarrierJumpRequest _lastJumpRequest;
        private System.Timers.Timer _jumpTimer;
        private System.Timers.Timer _cooldownTimer;
        private System.Timers.Timer _ticker = new(1000);
        private DateTime _lastCooldownDateTime = DateTime.MinValue;

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

        public string OwningCommander { get; set; }
        public bool CommanderIsDockedOrOnFoot { get; set; }
        public string CarrierName { get; set; }
        public ulong CarrierId { get; set; }
        public string CarrierCallsign { get; set; }
        public int CarrierFuel { get; set; }

        public int EstimateFuelForJumpFromCurrentPosition(CarrierPositionData newPosition, IObservatoryCore core, IObservatoryWorker worker)
        {
            if (!IsPositionKnown || newPosition == null || LastCarrierStats == null) return 0; // Not enough data.


            // TODO: Consider using ID64CoordHelper to minimize lookups from edastro (with a cache in-play, do NOT set estimated coords to the StarPos property).
            if (!Position.StarPos.HasValue) Position.StarPos = MaybeFetchStarPos(Position.SystemName, core, worker);
            if (!newPosition.StarPos.HasValue) newPosition.StarPos = MaybeFetchStarPos(newPosition.SystemName, core, worker);

            // Ideal case: two detailed positions.
            int estFuelUsage = 0;
            double distanceLy = 0;
            (double x, double y, double z)? pos1 = Position.StarPos;
            (double x, double y, double z)? pos2 = newPosition.StarPos;
            if (pos1.HasValue && pos2.HasValue)
            {
                distanceLy = Math.Sqrt(Math.Pow(pos1.Value.x - pos2.Value.x, 2) + Math.Pow(pos1.Value.y - pos2.Value.y, 2) + Math.Pow(pos1.Value.z - pos2.Value.z, 2));
                long capacityUsage = LastCarrierStats.SpaceUsage.TotalCapacity - LastCarrierStats.SpaceUsage.FreeSpace;
                double fuelCost = 5 + (distanceLy / 8.0) * (1.0 + ((capacityUsage + CarrierFuel) / 25000.0));

                estFuelUsage = Convert.ToInt32(Math.Round(fuelCost, 0));
            }
            else if (!core.IsLogMonitorBatchReading)
            {
                Debug.WriteLineIf(!Position.StarPos.HasValue, $"No coordinates for current system (after fetching!): {Position.SystemName}");
                Debug.WriteLineIf(!newPosition.StarPos.HasValue, $"No coordinates for new system (after fetching!): {newPosition.SystemName}");
            }

            // Fallback to the spansh estimate from the route.
            if (estFuelUsage == 0 && HasRoute)
            {
                var jumpInfo = Route.Find(newPosition.SystemName);
                if (jumpInfo != null) estFuelUsage = jumpInfo.FuelRequired;
            }

            return estFuelUsage;
        }

        private (double x, double y, double z)? MaybeFetchStarPos(string systemName, IObservatoryCore core, IObservatoryWorker worker)
        {
            // We'll get rate-limited.
            if (core.IsLogMonitorBatchReading || string.IsNullOrEmpty(systemName)) return null;

            string url = $"https://edastro.com/api/starsystem?q={systemName}";
            string jsonStr = "";
            JsonElement root;
            var jsonFetchTask = core.HttpClient.GetStringAsync(url);
            try
            {
                jsonStr = jsonFetchTask.Result;
                if (string.IsNullOrWhiteSpace(jsonStr)) return null;

                using (var jsonDoc = JsonDocument.Parse(jsonStr))
                {
                    root = jsonDoc.RootElement;
                    // root[0].coordinates is an array of 3 doubles.
                    if (root.GetArrayLength() > 0 && root[0].GetProperty("coordinates").GetArrayLength() == 3)
                    {
                        var coordsArray = root[0].GetProperty("coordinates");

                        (double x, double y, double z)? position = (
                            coordsArray[0].GetDouble(), coordsArray[1].GetDouble(), coordsArray[2].GetDouble());
                        return position;
                    }
                }
            }
            catch (Exception ex)
            {
                core.GetPluginErrorLogger(worker)(ex, $"Failed to fetch data from edastro.com for system: {systemName}");

                // Log the jsonStr to a file in plugin data for diagnosis.
                var edastroSysResponseFile = $"{core.PluginStorageFolder}lastEdastroSystemResponse.json";
                File.WriteAllText(edastroSysResponseFile, jsonStr);
                return null;
            }
            return null;
        }

        public long CapacityUsed { get; set; }

        public long CarrierBalance { get; set; }

        public DateTime StatsAsOfDate { get; set; }

        [JsonIgnore]
        public CarrierStats LastCarrierStats
        {
            get => _lastStats;
            set
            {
                if (CarrierId == 0 && _deserialized || CarrierId == value.CarrierID)
                {
                    CarrierName = value.Name;
                    CapacityUsed = value.SpaceUsage.TotalCapacity - value.SpaceUsage.FreeSpace;
                    CarrierBalance = value.Finance.CarrierBalance;
                    StatsAsOfDate = value.TimestampDateTime;

                    _lastStats = value;
                    _deserialized = false;
                }
            }
        }

        public CarrierJumpRequest LastCarrierJumpRequest
        {
            get => _lastJumpRequest;
            set
            {
                _lastCooldownDateTime = DateTime.MinValue; // Force update of cached value.
                _lastJumpRequest = value;
            }
        }

        [JsonIgnore]
        public DateTime DepartureDateTime {
            get
            {
                if (LastCarrierJumpRequest != null
                    && !string.IsNullOrEmpty(LastCarrierJumpRequest.DepartureTime))
                {
                    return LastCarrierJumpRequest.DepartureTimeDateTime;
                }
                return DateTime.MinValue;
            }
        }

        [JsonIgnore]
        public double DepartureTimeMinutes
        {
            get
            {
                if (DepartureDateTime == DateTime.MinValue) return Double.MinValue;
                return DepartureDateTime.Subtract(DateTime.Now).TotalMinutes;
            }
        }

        [JsonIgnore]
        public DateTime CooldownDateTime
        {
            get
            {
                if (LastCarrierJumpRequest != null
                    && !string.IsNullOrEmpty(LastCarrierJumpRequest.DepartureTime)
                    && _lastCooldownDateTime == DateTime.MinValue)
                {
                    // We have a jump request. Ensure our cached cooldown is up-to-date
                    _lastCooldownDateTime = LastCarrierJumpRequest.DepartureTimeDateTime.AddMinutes(COOLDOWN_MINUTES);
                }
                return _lastCooldownDateTime;
            }
        }

        public CarrierPositionData Position { get; set; }

        [JsonIgnore]
        public bool IsPositionKnown
        {
            get => Position != null;
        }

        [JsonIgnore]
        public bool CooldownNotifyScheduled { get => CarrierCooldownTimer != null; }

        [JsonIgnore]
        internal System.Timers.Timer CarrierCooldownTimer
        {
            get => _cooldownTimer;
            set => _cooldownTimer = value;
        }

        [JsonIgnore]
        public bool CarrierJumpTimerScheduled { get => CarrierJumpTimer != null; }

        [JsonIgnore]
        internal System.Timers.Timer CarrierJumpTimer
        { 
            get => _jumpTimer; 
            set => _jumpTimer = value;
        }

        [JsonIgnore]
        public System.Timers.Timer Ticker
        {
            get => _ticker;
        }

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
            _lastCooldownDateTime = DateTime.MinValue;
        }

        public bool MaybeUpdateLocation(CarrierPositionData newPosition)
        {
            if (newPosition == null
                || (Position?.IsSamePosition(newPosition) ?? false)) return false;

            Position = newPosition;
            return true;
        }

        public CarrierRoute Route { get; set; }

        [JsonIgnore]
        public bool HasRoute { get => Route != null; }
    }
}
