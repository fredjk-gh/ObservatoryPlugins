using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using System.Timers;

namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    public class Commander : IObservatoryWorker
    {
        private const string CARRIER_DATA_CACHE_FILENAME = "carrierDataCache.json";

        private PluginUI pluginUI;
        private IObservatoryCore Core;
        ObservableCollection<object> GridCollection = new();
        private FleetCommanderSettings settings = FleetCommanderSettings.DEFAULT;
        private Grid _lastShown = new();
        private string _currentCommander;
        private bool _initialRouteStuffingDone = false;
        private Location _initialLocation = null;
        private CarrierManager _manager = new();
        private CountdownManager _countdownManager;

        #region Worker Interface 

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

            settings.PlotCarrierRoute = PlotCarrierRoute;
            settings.FixCarrierRoute = FixCarrierRoute;
            MaybeDeserializeDataCache();
            _countdownManager = new(Core, this);
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args)
        {
            // * -> ReadAll
            if (args.NewState.HasFlag(LogMonitorState.Batch))
            {
                _manager.Clear();
                Core.ClearGrid(this, new Grid());
            }
            // ReadAll -> *
            else if (args.PreviousState.HasFlag(LogMonitorState.Batch))
            {
                SerializeDataCache();
            }
            else if (args.PreviousState.HasFlag(LogMonitorState.PreRead))
            {
                if (_currentCommander != null)
                {
                    CarrierData carrierData = _manager.GetByCommander(_currentCommander);
                    if (carrierData != null)
                    {
                        // We still have a jump request. And jump time is in the past. Assume the jump happened while game was closed to advance the current location.
                        if (carrierData.LastCarrierJumpRequest != null
                            && string.IsNullOrWhiteSpace(carrierData.LastCarrierJumpRequest.DepartureTime)
                            && carrierData.LastCarrierJumpRequest.DepartureTimeDateTime.CompareTo(DateTime.Now) < 0)
                        {
                            MaybeUpdateCarrierLocation(
                                carrierData.LastCarrierJumpRequest.DepartureTimeDateTime, carrierData.CarrierCallsign, new(carrierData.LastCarrierJumpRequest), false, true);
                            carrierData.CancelCarrierJump();
                        }
                        MaybeStuffInitialJumpInClipboard(carrierData);
                    }
                }
            }
        }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            CarrierData carrierData;
            switch (journal)
            {
                case LoadGame loadGame:
                    if (_currentCommander != loadGame.Commander)
                    {
                        _initialRouteStuffingDone = false;
                        _initialLocation = null;
                    }
                    _currentCommander = loadGame.Commander;
                    _lastShown = new(); // Reset last show to get periodic refresh of data in some columns.

                    carrierData = _manager.GetByCommander(_currentCommander);
                    if (carrierData != null) MaybeStuffInitialJumpInClipboard(carrierData);
                    break;
                case Docked docked:
                    // This isn't a great position signal.
                    if (docked.StationType == "FleetCarrier" && _manager.IsCallsignKnown(docked.StationName))
                    {
                        MaybeUpdateCarrierLocation(docked.TimestampDateTime, docked.StationName, new(docked.StarSystem, docked.SystemAddress), /* notifyIfChanged */ false, /* carrierMoveEvent */ false);

                        carrierData = _manager.GetByCommander(_currentCommander);
                        if (carrierData != null && carrierData.OwningCommander == _currentCommander)
                        {
                            carrierData.CommanderIsDockedOrOnFoot = true;
                        }
                    }
                    break;
                case Undocked undocked:
                    if (undocked.StationType == "FleetCarrier" && _manager.IsCallsignKnown(undocked.StationName))
                    {
                        carrierData = _manager.GetByCommander(_currentCommander);
                        if (carrierData != null && carrierData.OwningCommander == _currentCommander)
                        {
                            carrierData.CommanderIsDockedOrOnFoot = false;
                        }
                    }
                    break;
                case Location location:
                    // On game startup, a location event fires but we may not yet know if the carrier they're docked on is theirs, so
                    // only keep track of this initial location until we can confirm carrier ownership from a future CarrierStats or any
                    // other carrier action for that matter.
                    if (location.StationType == "FleetCarrier" && _manager.IsCallsignKnown(location.StationName))
                    {
                        MaybeUpdateCarrierLocation(location.TimestampDateTime, location.StationName, new(location), true);

                        carrierData = _manager.GetByCommander(_currentCommander);
                        if (carrierData != null && carrierData.OwningCommander == _currentCommander)
                        {
                            carrierData.CommanderIsDockedOrOnFoot = location.Docked;
                        }
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

                    if (carrierData.LastCarrierJumpRequest != null && (string.IsNullOrWhiteSpace(carrierJumpRequest.DepartureTime) || DateTime.Now.CompareTo(carrierJumpRequest.DepartureTimeDateTime) < 0))
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
                            carrierData.ClearTimers();
                            MaybeScheduleTimers(carrierData);

                            Core.SendNotification(new()
                            {
                                Title = "Carrier Jump Scheduled",
                                Detail = $"Jump is in {departureTimeMinutes:#.0} minutes",
                                Sender = ShortName,
                            });
                        }
                    }
                    AddToGrid(carrierJumpRequest.TimestampDateTime, carrierData, carrierData.Position?.BodyName, $"{carrierData.CarrierFuel}", $"Requested a jump to {jumpTargetBody}.{departureTime}");
                    break;
                case CarrierJump carrierJump: // These have been broken in the past. Thus we don't rely on this for notification or state.
                    string callSign = carrierJump.StationName;
                    if (string.IsNullOrWhiteSpace(callSign))
                    {
                        // FDev forgot to include station details for on-foot carrier jumps.
                        carrierData = _manager.GetByCommander(_currentCommander);
                        if (carrierData != null && carrierData.LastCarrierJumpRequest != null) // Ok we had a jump for this carrier. We assume this is it.
                            callSign = carrierData.CarrierCallsign;
                        else break;
                    }
                    else
                    {
                        carrierData = _manager.GetByCallsign(carrierJump.StationName);
                    }

                    MaybeUpdateCarrierLocation(carrierJump.TimestampDateTime, callSign, new(carrierJump), false, true);

                    if (carrierData != null && carrierData.OwningCommander == _currentCommander)
                    {
                        carrierData.CommanderIsDockedOrOnFoot = (carrierJump.Docked || carrierJump.OnFoot);
                    }
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
                    _countdownManager.Cancel();
                    break;
                case CarrierDepositFuel carrierDepositFuel:
                    // Make sure the Cmdr is donating to their carrier for fuel updates as this event could trigger for
                    // any commander's carrier when donating tritium. Technically this could wait until the next CarrierStats event.
                    var data = _manager.GetByCommander(_currentCommander);
                    if (data != null && data.CarrierId == carrierDepositFuel.CarrierID)
                    {
                        MaybeUpdateCarrierFuel(carrierDepositFuel.TimestampDateTime, null, carrierDepositFuel);
                    }
                    break;
                case CarrierStats carrierStats:
                    if (MaybeInitializeCarrierInfo(carrierStats.TimestampDateTime, carrierStats))
                    {
                        // Initialization also sets fuel levels. Don't duplicate efforts.
                        break;
                    }

                    carrierData = _manager.GetByCallsign(carrierStats.Callsign);
                    if (carrierData != null) MaybeStuffInitialJumpInClipboard(carrierData);

                    MaybeUpdateCarrierFuel(carrierStats.TimestampDateTime, carrierStats);
                    break;
            }
        }

        #endregion

        #region Private methods
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
                string locationUpdateDetails = "Jump successful";

                int estFuelUsage = 0;

                // Skip fuel estimate for in-system jumps (as it appears sometimes it doesn't actually deduct the 5 T of fuel.
                if (data.IsPositionKnown && data.Position.SystemName != position.SystemName)
                {
                    if (estFuelUsage == 0) estFuelUsage = EstimateFuelUsageForCompletedJump(data, position);
                    data.CarrierFuel -= estFuelUsage;
                }

                if (data.HasRoute)
                {
                    var lastJump = data.Route.Find(position.SystemName);
                    if (lastJump != null && estFuelUsage == 0) estFuelUsage = lastJump.FuelRequired;

                    if (data.Route.IsDestination(position.SystemName))
                    {
                        // Clear the route; it will be saved later.
                        data.Route = null;
                    }
                }

                AddToGrid(dateTime, data, position.BodyName, $"{data.CarrierFuel}{(estFuelUsage > 0 ? " T (estimated)" : "")}", locationUpdateDetails);

                // Notify if not initial values and context requests it and user has this notification enabled.
                if (notifyIfChanged)
                {
                    var fromPositionText = (data.IsPositionKnown ? $" from {data.Position.BodyName}" : "");

                    Core.SendNotification(new()
                    {
                        Title = "Carrier Status Update",
                        Detail = locationUpdateDetails,
                        ExtendedDetails = $"Carrier has jumped{fromPositionText} to {position.BodyName}.",
                        Sender = ShortName,
                        Rendering = (settings.NotifyJumpComplete ? NotificationRendering.All : NotificationRendering.PluginNotifier),
                    });
                }
                MaybeScheduleTimers(data);
                data.LastCarrierJumpRequest = null;
            }
            data.MaybeUpdateLocation(position);
            SerializeDataCache();
        }

        private int EstimateFuelUsageForCompletedJump(CarrierData data, CarrierPositionData newPosition)
        {
            if (!data.IsPositionKnown || newPosition == null || data.LastCarrierStats == null) return 0; // Not enough data.

            // TODO: Consider using ID64CoordHelper to minimize lookups from edastro (with a cache in-play, do NOT set estimated coords to the StarPos property).
            if (!data.Position.StarPos.HasValue) data.Position.StarPos = MaybeGetStarPos(data.Position.SystemName);
            if (!newPosition.StarPos.HasValue) newPosition.StarPos = MaybeGetStarPos(newPosition.SystemName);

            // Ideal case: two detailed positions.
            double distanceLy = 0;
            (double x, double y, double z)? pos1 = data.Position.StarPos;
            (double x, double y, double z)? pos2 = newPosition.StarPos;
            if (pos1.HasValue && pos2.HasValue)
            {

                distanceLy = Math.Sqrt(Math.Pow(pos1.Value.x - pos2.Value.x, 2) + Math.Pow(pos1.Value.y - pos2.Value.y, 2) + Math.Pow(pos1.Value.z - pos2.Value.z, 2));
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

        private (double x, double y, double z)? MaybeGetStarPos(string systemName)
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

            if (string.IsNullOrWhiteSpace(jsonStr)) return null;

            using var jsonDoc = JsonDocument.Parse(jsonStr);
            var root = jsonDoc.RootElement;

            // root[0].coordinates is an array of 3 doubles.
            if (root.GetArrayLength() > 0 && root[0].GetProperty("coordinates").GetArrayLength() == 3)
            {
                var coordsArray = root[0].GetProperty("coordinates");

                (double x, double y, double z)? position = (
                    coordsArray[0].GetDouble(), coordsArray[1].GetDouble(), coordsArray[2].GetDouble());
                return position;
            }
            return null;
        }

        private void MaybeScheduleTimers(CarrierData data)
        {
            if (Core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch)
                || data.LastCarrierJumpRequest == null
                || string.IsNullOrWhiteSpace(data.LastCarrierJumpRequest.DepartureTime)) return;

            // Don't start a countdown if the cooldown is already over.
            DateTime carrierDepartureTime = data.LastCarrierJumpRequest.DepartureTimeDateTime;
            DateTime carrierJumpCooldownTime = carrierDepartureTime.AddMinutes(5);
            if (settings.NotifyJumpCooldown && !data.CooldownNotifyScheduled && DateTime.Now < carrierJumpCooldownTime)
            {
                data.CarrierCooldownTimer = new System.Timers.Timer(carrierJumpCooldownTime.Subtract(DateTime.Now).TotalMilliseconds);
                data.CarrierCooldownTimer.Elapsed += CarrierCooldownTimer_Elapsed;
                data.CarrierCooldownTimer.Start();
                Debug.WriteLine($"Cooldown notification scheduled for {carrierJumpCooldownTime}.");
            }

            if (!data.CarrierJumpTimerScheduled && DateTime.Now < carrierDepartureTime)
            {
                data.CarrierJumpTimer = new System.Timers.Timer(carrierDepartureTime.Subtract(DateTime.Now).TotalMilliseconds);
                data.CarrierJumpTimer.Elapsed += CarrierJumpTimer_Elapsed;
                data.CarrierJumpTimer.Start();
                Debug.WriteLine($"Carrier jump timer scheduled for {carrierDepartureTime}.");
            }
            if (settings.EnableRealtimeCountdown && (data.CarrierCooldownTimer != null || data.CarrierJumpTimer != null))
            {
                if (data.HasRoute)
                {
                    int jumpNumber = data.Route.IndexOf(data.LastCarrierJumpRequest.SystemName);
                    int jumpsTotal = data.Route.Jumps.Count - 1; // [0] is origin system; and does not represent a jump.

                    _countdownManager.InitCountdown(carrierDepartureTime, carrierJumpCooldownTime, jumpNumber, jumpsTotal);
                }
                else
                {
                    _countdownManager.InitCountdown(carrierDepartureTime, carrierJumpCooldownTime);
                }
            }
        }

        private bool MaybeInitializeCarrierInfo(DateTime dateTime, CarrierStats stats)
        {
            if (!_manager.IsCallsignKnown(stats.Callsign))
            {
                var data = _manager.RegisterCarrier(_currentCommander, stats);
                AddToGrid(dateTime, data, data.Position?.BodyName, $"{data.CarrierFuel}", $"Carrier detected: {data.CarrierName} {data.CarrierCallsign}. Configured notifications are active.");
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
            SerializeDataCache();
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
            if (string.IsNullOrEmpty(_lastShown.SystemName) && !string.IsNullOrEmpty(displayLocation)
                ||  displayLocation != _lastShown.SystemName)
            {
                gridItem.SystemName = _lastShown.SystemName = displayLocation;
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

        private void MaybeStuffInitialJumpInClipboard(CarrierData carrierData)
        {
            if (!_initialRouteStuffingDone)
            {
                if (carrierData.IsPositionKnown && carrierData.LastCarrierJumpRequest != null
                    && DateTime.Now.CompareTo(carrierData.LastCarrierJumpRequest.DepartureTimeDateTime) > 0)
                {
                    if (carrierData.Position.SystemName != carrierData.LastCarrierJumpRequest.SystemName)
                    {
                        // Last jump request happened in the past, but position doesn't match. So either a stale request or a stale position.
                        // I'm going to assume the latter and update the position and clear the jump request.
                        MaybeUpdateCarrierLocation(carrierData.LastCarrierJumpRequest.TimestampDateTime, carrierData.CarrierCallsign, new(carrierData.LastCarrierJumpRequest), false, true);
                    }
                    // Otherwise, In our cache, position == next jump destination. So the last jump request appears stale; just clear it.
                    carrierData.LastCarrierJumpRequest = null;
                    SerializeDataCache(/* forceWrite */ true);
                }

                MaybeSetNextJumpInClipboard(carrierData);
                _initialRouteStuffingDone = true;
            }
        }

        private void MaybeSetNextJumpInClipboard(CarrierData carrierData)
        {
            if (!carrierData.HasRoute || !carrierData.IsPositionKnown) return;
            if (carrierData.Route.IsDestination(carrierData.Position.SystemName))
            {
                carrierData.Route = null;
                SerializeDataCache();
                return;
            }

            var nextJumpInfo = carrierData.Route.GetNextJump(carrierData.Position.SystemName);
            if (nextJumpInfo != null && carrierData.LastCarrierJumpRequest == null)
            {
                Core.ExecuteOnUIThread(() => {
                    Clipboard.SetText(nextJumpInfo.SystemName);
                });

                AddToGrid(DateTime.Now, carrierData, $"Next in route: {nextJumpInfo.SystemName}", $"{carrierData.CarrierFuel}", "System name for the next jump in your carrier route is in the clipboard.");
            }
        }

        private void MaybeDeserializeDataCache()
        {
            string dataCacheFile = $"{Core.PluginStorageFolder}{CARRIER_DATA_CACHE_FILENAME}";
            if (!File.Exists(dataCacheFile)) return;

            try
            {
                string jsonString = File.ReadAllText(dataCacheFile);
                List<CarrierData> carrierCache = JsonSerializer.Deserialize<List<CarrierData>>(jsonString)!;

                foreach (CarrierData carrierData in carrierCache)
                {
                    _manager.RegisterCarrier(carrierData);
                }
            }
            catch (Exception ex)
            {
                Core.GetPluginErrorLogger(this)(ex, "Deserializing CarrierData cache");
            }
        }

        private void SerializeDataCache(bool forceWrite = false)
        {
            if (Core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch) && !forceWrite) return;

            string dataCacheFile = $"{Core.PluginStorageFolder}{CARRIER_DATA_CACHE_FILENAME}";
            string jsonString = JsonSerializer.Serialize(_manager.Carriers,
                new JsonSerializerOptions() { AllowTrailingCommas = true, WriteIndented = true });
            File.WriteAllText(dataCacheFile, jsonString);
        }
        #endregion

        #region Events
        private void CarrierCooldownTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var data = _manager.GetByTimer((System.Timers.Timer)sender);
            if (data == null) return;

            AddToGrid(DateTime.Now, data, data.Position.BodyName, $"{data.CarrierFuel}", "Carrier jump cooldown is over. You may now schedule a new jump.");
            Core.SendNotification(new()
            {
                Title = "Carrier Status Update",
                Detail = "Jump cooldown is complete. You may now schedule a new jump.",
                Sender = ShortName,
            });
            data.CancelCarrierJump();

            if (data.HasRoute)
            {
                MaybeSetNextJumpInClipboard(data);
            }
        }

        private void CarrierJumpTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // If we didn't get cancelled, assume the carrier jump occurred.
            var data = _manager.GetByTimer((System.Timers.Timer)sender);
            if (data == null || data.LastCarrierJumpRequest == null) return;

            MaybeUpdateCarrierLocation(
                data.LastCarrierJumpRequest.DepartureTimeDateTime, data.CarrierCallsign, new(data.LastCarrierJumpRequest), true, true);
        }
        #endregion

        #region Settings Actions
        private void PlotCarrierRoute() // UI Thread
        {
            SpanshCarrierRouterForm dlgSpanshOptions = new(Core, this, _currentCommander, _manager);

            DialogResult result = dlgSpanshOptions.ShowDialog();

            if (result == DialogResult.OK)
            {
                // The result is stored on the CarrierData for the selected Commander (indicated on the "SelectedCommander" property).
                CarrierData data = _manager.GetByCommander(dlgSpanshOptions.SelectedCommander);
                if (data != null)
                {
                    // Freshen the contents of the cache for next run of the application.
                    SerializeDataCache();

                    // Pre-cache the positions of systems in the route.
                    foreach (var jump in data.Route.Jumps)
                    {
                        CarrierPositionData.CachePosition(jump.SystemName, jump.Position);
                    }

                    // Stuff the first waypoint in the route into the clipboard.
                    var firstJump = data.Route.GetNextJump(data.Position?.SystemName ?? "");
                    if (firstJump != null)
                    {
                        Core.ExecuteOnUIThread(() => {
                            Clipboard.SetText(firstJump.SystemName);
                        });

                        // Spit out an update to the grid indicating a route is set.
                        AddToGrid(DateTime.Now, data, $"Next jump: {firstJump.SystemName}", $"{data.CarrierFuel}", "Route plotted via Spansh. First jump system name is in the clipboard.");
                    }
                }
            }
            else if (result == DialogResult.Abort) // Cleared route.
            {
                dlgSpanshOptions.CarrierDataForSelectedCommander.Route = null;
                SerializeDataCache();
            }
            dlgSpanshOptions.Dispose();
        }

        private void FixCarrierRoute() // UI Thread
        {
            FixRouteForm dlgFixRoute = new(Core, this, _currentCommander, _manager);

            DialogResult result = dlgFixRoute.ShowDialog();

            if (result == DialogResult.OK)
            {
                CarrierData data = dlgFixRoute.CarrierDataForSelectedCommander;
                if (data != null)
                {
                    // Write back the updated current location. Next jump system is already on the clipboard.
                    SerializeDataCache();
                }
            }
            dlgFixRoute.Dispose();
        }
        #endregion
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
        public string SystemName { get; set; }
        [ColumnSuggestedWidth(200)]
        public string FuelLevel { get; set; }
        [ColumnSuggestedWidth(500)]
        public string Details { get; set; }
    }
}
