using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;

namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    public class Commander : IObservatoryWorker
    {
        private PluginUI pluginUI;
        private IObservatoryCore Core;
        ObservableCollection<object> GridCollection = new();
        private FleetCommanderSettings settings = FleetCommanderSettings.DEFAULT;
        private Grid _lastShown = new();
        private string _currentCommander;
        private Location _initialLocation = null;
        private CarrierManager _manager = new();

        public string Name => "Observatory Fleet Commander";
        public string ShortName => "Commander";
        public string Version => typeof(Commander).Assembly.GetName().Version.ToString();
        public PluginUI PluginUI => pluginUI;

        public object Settings
        {
            get => settings;
            set => settings = (FleetCommanderSettings)value;
        }

        public void Load(IObservatoryCore observatoryCore)
        {
            GridCollection = new();
            Grid uiObject = new();

            GridCollection.Add(uiObject);
            pluginUI = new PluginUI(GridCollection);

            Core = observatoryCore;
        }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            CarrierData carrierData;
            switch (journal)
            {
                case LoadGame loadGame:
                    _currentCommander = loadGame.Commander;
                    _lastShown = new(); // Reset last show to get periodic refresh of data in some columns.
                    break;
                case Docked docked:
                    // This isn't a great position signal.
                    if (docked.StationType == "FleetCarrier" && _manager.IsCallsignKnown(docked.StationName))
                    {
                        MaybeUpdateCarrierLocation(docked.TimestampDateTime, docked.StationName, new(docked.StarSystem, docked.SystemAddress), /* notifyIfChanged */ false, /* carrierMoveEvent */ false);
                    }
                    break;
                case Location location:
                    // On game startup, a location event fires but we may not yet know if the carrier they're docked on is theirs, so
                    // only keep track of this initial location until we can confirm carrier ownership from a future CarrierStats or any
                    // other carrier action for that matter.
                    if (location.StationType == "FleetCarrier" && _manager.IsCallsignKnown(location.StationName))
                    {
                        MaybeUpdateCarrierLocation(location.TimestampDateTime, location.StationName, new(location), true);
                    }
                    else if (_initialLocation == null)
                    {
                        
                        _initialLocation = location;
                    }
                    break;
                case CarrierBuy buy:
                    carrierData = _manager.RegisterCarrier(_currentCommander, buy);
                    break;
                case CarrierJumpRequest carrierJumpRequest:
                    carrierData = _manager.GetById(carrierJumpRequest.CarrierID);
                    if (carrierData == null)
                    {
                        // Odd that we wouldn't know about it yet.
                        break;
                    }

                    if (carrierData.LastCarrierJumpRequest != null)
                    {
                        // We may be looking at journals from a period of time where we did NOT get CarrierJump events. This shouldn't happen in
                        // realtime mode anymore. Furthermore, older journals don't have DepartureTime so we can't rely on that either.
                        // For simplicity sake, let's assume it happened (since you can't jump again unless cooldown is over or cancelled)
                        // thus, we'll advance the carrier position to the previously requested location. If this is wrong, it will eventually self-correct.
                        //
                        // This is a low fidelity location, but it's better than output being stuck in the wrong spot. We cache coordinates
                        // so maybe, hopefully, we've cached this one. In realtime, we'll look it up. Being a likely stale event, no notification
                        // is raised, but it is treated as a carrier move situation (and should be output to the grid). This has a side effect
                        // of clearing the LastCarrierJumpRequest, so don't update till after.
                        MaybeUpdateCarrierLocation(carrierJumpRequest.TimestampDateTime, carrierData.CarrierCallsign, new(carrierData.LastCarrierJumpRequest.SystemName, carrierData.LastCarrierJumpRequest.SystemAddress), false, true);
                    }

                    carrierData.LastCarrierJumpRequest = carrierJumpRequest;

                    var jumpTargetBody = (!string.IsNullOrEmpty(carrierJumpRequest.Body)) ? carrierJumpRequest.Body : carrierJumpRequest.SystemName;
                    string departureTime = "";
                    if (!String.IsNullOrEmpty(carrierJumpRequest.DepartureTime))
                    {
                        DateTime carrierDepartureTime = carrierJumpRequest.DepartureTimeDateTime;
                        double departureTimeMinutes = carrierDepartureTime.Subtract(DateTime.Now).TotalMinutes;
                        if (departureTimeMinutes > 0) {
                            departureTime = $" Jump in {departureTimeMinutes:#.0} minutes (@ {carrierDepartureTime.ToShortTimeString()}).";

                            // Force timer update for new cooldown time.
                            carrierData.CooldownNotifyScheduled = false;
                            MaybeScheduleCooldownNotification(carrierData);
                        }
                    }
                    AddToGrid(carrierJumpRequest.TimestampDateTime, carrierData, carrierData.Position.BodyName, $"{carrierData.CarrierFuel}", $"Requested a jump to {jumpTargetBody}.{departureTime}");
                    break;
                case CarrierJump carrierJump: // These have been broken in the past.
                    MaybeUpdateCarrierLocation(carrierJump.TimestampDateTime, carrierJump.StationName, new(carrierJump), true);
                    break;
                case CarrierJumpCancelled carrierJumpCancelled:
                    carrierData = _manager.GetById(carrierJumpCancelled.CarrierID);
                    if (carrierData.LastCarrierJumpRequest != null)
                    {
                        AddToGrid(carrierJumpCancelled.TimestampDateTime, carrierData, carrierData.Position.BodyName, $"{carrierData.CarrierFuel}", $"Cancelled requested jump to {carrierData.LastCarrierJumpRequest.SystemName}");
                    }
                    else
                    {
                        AddToGrid(carrierJumpCancelled.TimestampDateTime, carrierData, carrierData.Position.BodyName, $"{carrierData.CarrierFuel}", $"Cancelled requested jump");
                    }
                    carrierData.CancelCarrierJump();
                    break;
                case CarrierDepositFuel carrierDepositFuel:
                    // Make sure the Cmdr is donating to their carrier for fuel updates as this event could trigger for
                    // any commander's carrier when donating tritium. Technically this could wait until the next CarrierStats event.
                    MaybeUpdateCarrierFuel(carrierDepositFuel.TimestampDateTime, null, carrierDepositFuel);
                    break;
                case CarrierStats carrierStats:
                    if (MaybeInitializeCarrierInfo(carrierStats.TimestampDateTime, carrierStats))
                        // Initialization also sets fuel levels. Don't duplicate efforts.
                        break;

                    MaybeUpdateCarrierFuel(carrierStats.TimestampDateTime, carrierStats);
                    break;
            }
        }

        private void MaybeUpdateCarrierLocation(DateTime dateTime, string callsign, CarrierPositionData position, bool notifyIfChanged, bool carrierMoveEvent = true)
        {
            var data = _manager.GetByCallsign(callsign);
            if (data == null || position == null) return;

            if (data.IsPositionKnown && data.Position.IsSamePosition(position))
            {
                // Nothing has changed. Maybe update position precision.
                data.MaybeUpdateLocation(position);
                return;
            }
            else if (data.IsPositionKnown && data.Position.SystemName == position.SystemName && !carrierMoveEvent)
            {
                // This is low quality/untrusted bit of info (ie. it's Docked event or something) which may not have a useful body. Skip.
                return;
            }
            else if (carrierMoveEvent
                || !data.IsPositionKnown
                || (data.LastCarrierJumpRequest != null && data.LastCarrierJumpRequest.SystemName == position.SystemName))
            {
                // We've moved.
                string locationUpdateDetails = "Carrier jump complete";

                int estFuelUsage = 0;
                // Skip fuel estimate for in-system jumps (as it appears sometimes it doesn't actually deduct the 5 T of fuel.
                if (data.IsPositionKnown && data.Position.SystemName != position.SystemName)
                {
                    estFuelUsage = EstimateFuelUsageForCompletedJump(data, position);
                    data.CarrierFuel -= estFuelUsage;
                }

                AddToGrid(dateTime, data, position.BodyName, $"{data.CarrierFuel}{(estFuelUsage > 0 ? " T (estimated)" : "")}", locationUpdateDetails);
                // Notify if not initial values and context requests it and user has this notification enabled.
                if (notifyIfChanged)
                {
                    if (settings.NotifyJumpComplete) {
                        Core.SendNotification(new()
                        {
                            Title = locationUpdateDetails,
                            Detail = "",
                            Sender = ShortName,
                        });
                    }

                    MaybeScheduleCooldownNotification(data);
                }
                data.LastCarrierJumpRequest = null;
            }
            data.MaybeUpdateLocation(position);
        }

        private int EstimateFuelUsageForCompletedJump(CarrierData data, CarrierPositionData newPosition)
        {
            if (!data.IsPositionKnown || newPosition == null || data.LastCarrierStats == null) return 0; // Not enough data.

            // TODO: Consider using ID64CoordHelper to minimize lookups from edastro (with a cache in-play, do NOT set estimated coords to the StarPos property).
            if (!data.Position.StarPos.HasValue) data.Position.StarPos = MaybeGetStarPos(data.Position.SystemName);
            if (!newPosition.StarPos.HasValue) newPosition.StarPos = MaybeGetStarPos(newPosition.SystemName);

            // Ideal case: two detailed positions.
            double distanceLy = 0;
            (double, double, double)? pos1 = data.Position.StarPos;
            (double, double, double)? pos2 = newPosition.StarPos;
            if (pos1.HasValue && pos2.HasValue)
            {

                distanceLy = Math.Sqrt(Math.Pow(pos1.Value.Item1 - pos2.Value.Item1, 2) + Math.Pow(pos1.Value.Item2 - pos2.Value.Item2, 2) + Math.Pow(pos1.Value.Item3 - pos2.Value.Item3, 2));
                long capacityUsage = data.LastCarrierStats.SpaceUsage.TotalCapacity - data.LastCarrierStats.SpaceUsage.FreeSpace;
                double fuelCost = 5 + (distanceLy / 8.0) * (1.0 + ((capacityUsage + data.CarrierFuel) / 25000.0));

                return Convert.ToInt32(Math.Round(fuelCost, 0));
            }
            else if (!Core.IsLogMonitorBatchReading)
            {
                Debug.WriteLineIf(!data.Position.StarPos.HasValue, $"No coordinates for current system (after fetching!): {data.Position.SystemName}");
                Debug.WriteLineIf(!newPosition.StarPos.HasValue, $"No coordinates for new system (after fetching!): {newPosition.SystemName}");
            }

            return 0;
        }

        private (double, double, double)? MaybeGetStarPos(string systemName)
        {
            // We'll get rate-limited.
            if (Core.IsLogMonitorBatchReading || string.IsNullOrEmpty(systemName)) return null;

            string url = $"https://edastro.com/api/starsystem?q={systemName}";
            string jsonStr = "";

            var jsonFetchTask = Core.HttpClient.GetStringAsync(url);
            try
            {
                jsonStr = jsonFetchTask.Result;
            }
            catch (Exception ex)
            {
                Core.GetPluginErrorLogger(this)(ex, $"Failed to fetch data from edastro.com for system: {systemName}");
                return null;
            }

            using var jsonDoc = JsonDocument.Parse(jsonStr);
            var root = jsonDoc.RootElement;

            // I want [0].coordinates () which is an array of 3 doubles.
            if (root.GetArrayLength() > 0 && root[0].GetProperty("coordinates").GetArrayLength() == 3)
            {
                var coordsArray = root[0].GetProperty("coordinates");

                (double, double, double)? position = (
                    coordsArray[0].GetDouble(), coordsArray[1].GetDouble(), coordsArray[2].GetDouble());
                return position;
            }
            return null;
        }

        private void MaybeScheduleCooldownNotification(CarrierData data)
        {
            if (Core.IsLogMonitorBatchReading || data.LastCarrierJumpRequest == null) return;

            // Don't start a countdown if the cooldown is already over.
            // TODO: Update to use this once a new framework is released: lastCarrierJumpRequest.DepartureTimeDateTime
            DateTime carrierDepartureTime = DateTime.ParseExact(data.LastCarrierJumpRequest.DepartureTime, "yyyy-MM-ddTHH:mm:ssZ", null, System.Globalization.DateTimeStyles.AssumeUniversal);
            DateTime carrierJumpCooldownTime = carrierDepartureTime.AddMinutes(5);
            if (settings.NotifyJumpCooldown && !data.CooldownNotifyScheduled && DateTime.Now < carrierJumpCooldownTime)
            {
                data.CarrierCooldownTimer = new System.Timers.Timer(carrierJumpCooldownTime.Subtract(DateTime.Now).TotalMilliseconds);
                data.CarrierCooldownTimer.Elapsed += CarrierCooldownTimer_Elapsed;
                data.CarrierCooldownTimer.Start();
                data.CooldownNotifyScheduled = true;
            }
        }

        private bool MaybeInitializeCarrierInfo(DateTime dateTime, CarrierStats stats)
        {
            if (!_manager.IsCallsignKnown(stats.Callsign))
            {
                var data = _manager.RegisterCarrier(_currentCommander, stats);
                AddToGrid(dateTime, data, data.Position.BodyName, $"{data.CarrierFuel}", $"Carrier detected: {data.CarrierName} {data.CarrierCallsign}. Configured notifications are active.");
                return true;
            }
            return false;
        }

        private void MaybeUpdateCarrierFuel(DateTime dateTime, CarrierStats stats = null, CarrierDepositFuel fuel = null)
        {
            if (stats == null && fuel == null) return;

            ulong carrierId = stats?.CarrierID ?? fuel.CarrierID;
            var data = _manager.GetById(carrierId);

            if (data == null) return; // Not a known carrier.

            int updatedCarrierFuel = stats?.FuelLevel ?? fuel.Total;

            // First update on fuel level or fuel has dropped below previous value. Let 'em now if we're running low.
            if (data.CarrierFuel != updatedCarrierFuel)
            {
                bool lowFuel = updatedCarrierFuel < 135;
                string fuelDetails = lowFuel
                    ? $"Carrier has {updatedCarrierFuel} tons of tritium remaining and may require more soon."
                    : $"Fuel level has changed{(fuel != null ? " (deposited fuel)" : "")}.";
                // Only add "Fuel level has changed" output if we have a value for it already. At startup, the carrier detected line includes it and thus,
                // this line appears redundant.
                AddToGrid(dateTime, data, data.Position.BodyName, $"{updatedCarrierFuel}", fuelDetails);

                if (lowFuel)
                {
                    // Only notify if fuel is running low and not reading-all.
                    Core.SendNotification(new()
                    {
                        Title = "Low Fuel",
                        Detail = fuelDetails,
                        Sender = ShortName,
                    });
                }
            }
            data.CarrierFuel = updatedCarrierFuel;
            if (stats != null) data.LastCarrierStats = stats;
        }

        private void AddToGrid(DateTime dateTime, CarrierData data, string location, string fuelLevel, string details)
        {
            Grid gridItem = new()
            {
                Timestamp = dateTime.ToString("G"),
                Details = details,
            };

            if (string.IsNullOrEmpty(_lastShown.Commander) && !string.IsNullOrEmpty(data.OwningCommander)
                || data.OwningCommander != _lastShown.Commander)
            {
                gridItem.Commander = _lastShown.Commander = data.OwningCommander;
            }

            var carrierDisplay = $"{data.CarrierName} ({data.CarrierCallsign})";
            if (string.IsNullOrEmpty(_lastShown.Carrier) && !string.IsNullOrEmpty(carrierDisplay)
                || carrierDisplay != _lastShown.Carrier)
            {
                gridItem.Carrier = _lastShown.Carrier = carrierDisplay;
            }

            var displayLocation = location ?? "unknown";
            if (string.IsNullOrEmpty(_lastShown.CurrentLocation) && !string.IsNullOrEmpty(displayLocation)
                ||  displayLocation != _lastShown.CurrentLocation)
            {
                gridItem.CurrentLocation = _lastShown.CurrentLocation = displayLocation;
            }

            // HACK: Improve how we communicate "estimated" fuel levels.
            var displayFuel = string.IsNullOrEmpty(fuelLevel) ? "unknown" : (fuelLevel.Contains("estimated") ? fuelLevel : $"{fuelLevel} T");
            if (string.IsNullOrEmpty(_lastShown.FuelLevel) && !string.IsNullOrEmpty(displayFuel)
                || displayFuel != _lastShown.FuelLevel)
            {
                gridItem.FuelLevel = _lastShown.FuelLevel = displayFuel;
            }

            Core.AddGridItem(this, gridItem);
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args)
        {
            // * -> ReadAll
            if (args.NewState.HasFlag(LogMonitorState.Batch))
            {
                _manager.Clear();
                Core.ClearGrid(this, new Grid());
            }
        }

        private void CarrierCooldownTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var data = _manager.GetByTimer((System.Timers.Timer)sender);
            if (data == null) return;

            AddToGrid(DateTime.Now, data, data.Position.BodyName, $"{data.CarrierFuel}", "Carrier jump cooldown has ended. You may now schedule a new jump.");
            Core.SendNotification(new()
            {
                Title = "Carrier jump cooldown has ended",
                Detail = "You may now schedule a new jump.",
                Sender = ShortName,
            });
            data.CancelCarrierJump();
        }
    }

    public class Grid
    {
        [ColumnSuggestedWidth(300)]
        public string Timestamp { get; set; }
        [ColumnSuggestedWidth(250)]
        public string Commander { get; set; }
        [ColumnSuggestedWidth(400)]
        public string Carrier {  get; set; }
        [ColumnSuggestedWidth(350)]
        public string CurrentLocation { get; set; }
        [ColumnSuggestedWidth(200)]
        public string FuelLevel { get; set; }
        [ColumnSuggestedWidth(500)]
        public string Details { get; set; }
    }
}
