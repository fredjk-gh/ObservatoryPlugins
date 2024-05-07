using com.github.fredjk_gh.ObservatoryFleetCommander.Data;
using com.github.fredjk_gh.ObservatoryFleetCommander.UI;
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
        internal FleetCommanderSettings settings = FleetCommanderSettings.DEFAULT;
        private string _currentCommander;
        private bool _initialRouteStuffingDone = false;
        private Location _initialLocation = null;
        private CarrierManager _manager = new();
        private CommanderUI _ui;

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
            Core = observatoryCore;

            MaybeDeserializeDataCache();

            pluginUI = new PluginUI(PluginUI.UIType.Panel, _ui = new CommanderUI(Core, this, _manager));
            _ui.Init();
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args)
        {
            // * -> ReadAll
            if (args.NewState.HasFlag(LogMonitorState.Batch))
            {
                _manager.Clear();
                _ui.Clear();
            }
            // ReadAll -> *
            else if (args.PreviousState.HasFlag(LogMonitorState.Batch))
            {
                SerializeDataCache();

                Core.ExecuteOnUIThread(() =>
                {
                    _ui.Repaint();
                });
            }
            else if (args.PreviousState.HasFlag(LogMonitorState.PreRead))
            {
                if (_currentCommander != null)
                {
                    CarrierData carrierData = _manager.GetByCommander(_currentCommander);
                    if (carrierData != null)
                    {
                        // We still have a jump request. And jump time is in the past. Assume the jump happened while game was closed to advance the current location.
                        if (carrierData.LastCarrierJumpRequest != null && carrierData.DepartureTimeMinutes < 0)
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
                    double departureTimeMinutes = carrierData.DepartureTimeMinutes;
                    if (departureTimeMinutes > 0)
                    {
                        // Force timer update for new cooldown time.
                        carrierData.ClearTimers();
                        MaybeScheduleTimers(carrierData);

                        Core.SendNotification(new()
                        {
                            Title = "Carrier Jump Scheduled",
                            Detail = $"Jump is in {departureTimeMinutes:#.0} minutes",
                            Sender = ShortName,
                        });
                        Core.ExecuteOnUIThread(() =>
                        {
                            _ui.Get(carrierData)?.JumpScheduled();
                        });
                    }

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
                    string msg = "Cancelled requested jump";
                    if (carrierData.LastCarrierJumpRequest != null)
                        msg = $"Cancelled requested jump to {carrierData.LastCarrierJumpRequest.SystemName}";

                    carrierData.CancelCarrierJump();
                    Core.ExecuteOnUIThread(() =>
                    {
                        _ui.Get(carrierData)?.JumpCanceled(msg);
                    });
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
                    estFuelUsage = data.EstimateFuelForJumpFromCurrentPosition(position, Core);
                    data.CarrierFuel -= estFuelUsage;
                }

                if (data.HasRoute)
                {
                    if (data.Route.IsDestination(position.SystemName))
                    {
                        // Clear the route; it will be saved later.
                        data.Route = null;
                    }
                }

                Core.ExecuteOnUIThread(() =>
                {
                    _ui.Get(data)?.UpdatePosition(position, locationUpdateDetails);
                    _ui.Get(data)?.UpdateFuel();
                });

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
            Core.ExecuteOnUIThread(() =>
            {
                _ui.Get(data)?.InitCountdown(data.LastCarrierJumpRequest);
            });
        }

        private bool MaybeInitializeCarrierInfo(DateTime dateTime, CarrierStats stats)
        {
            if (!_manager.IsCallsignKnown(stats.Callsign))
            {
                var data = _manager.RegisterCarrier(_currentCommander, stats);
                Core.ExecuteOnUIThread(() =>
                {
                    _ui.Add(stats.Callsign)?.Draw($"Carrier detected: {data.CarrierName} {data.CarrierCallsign}. Configured notifications are active.");
                });
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
                data.CarrierFuel = updatedCarrierFuel;
                bool lowFuel = updatedCarrierFuel < 135;
                string fuelDetails = lowFuel
                    ? $"Carrier has {updatedCarrierFuel} tons of tritium remaining and may require more soon."
                    : $"Fuel level has changed{(fuel != null ? " (deposited fuel)" : "")}.";
                // Only add "Fuel level has changed" output if we have a value for it already. At startup, the carrier detected line includes it and thus,
                // this line appears redundant.
                Core.ExecuteOnUIThread(() =>
                {
                    _ui.Get(data)?.UpdateFuel(fuelDetails);
                });

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

            if (stats != null) data.LastCarrierStats = stats;
            Core.ExecuteOnUIThread(() =>
            {
                _ui.Get(data)?.UpdateStats();
            });
            SerializeDataCache();
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
                Core.ExecuteOnUIThread(() =>
                {
                    _ui.Get(carrierData)?.UpdatePosition(carrierData.Position);
                });
                return;
            }

            var nextJumpInfo = carrierData.Route.GetNextJump(carrierData.Position.SystemName);
            if (nextJumpInfo != null && carrierData.LastCarrierJumpRequest == null)
            {
                Core.ExecuteOnUIThread(() =>
                {
                    Clipboard.SetText(nextJumpInfo.SystemName);
                    _ui.Get(carrierData)?.SetMessage("System name for the next jump in your carrier route is in the clipboard.");
                });
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
                    var data = _manager.RegisterCarrier(carrierData);
                }
            }
            catch (Exception ex)
            {
                Core.GetPluginErrorLogger(this)(ex, "Deserializing CarrierData cache");
            }
        }

        public void SerializeDataCache(bool forceWrite = false)
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

            Core.ExecuteOnUIThread(() =>
            {
                _ui.Get(data)?.SetMessage("Carrier jump cooldown is over. You may now schedule a new jump.");
            });
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
    }
}
