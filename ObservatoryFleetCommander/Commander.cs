using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    public class Commander : IObservatoryWorker
    {
        private PluginUI pluginUI;
        private IObservatoryCore Core;
        ObservableCollection<object> GridCollection = new();
        private FleetCommanderSettings settings = new()
        {
            NotifyJumpComplete = false,
            NotifyJumpCooldown = true,
            NotifyLowFuel = true,
        };

        private string _currentCommander;
        private Dictionary<ulong, CarrierJumpRequest> _jumpRequestsById = new();
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

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            CarrierData carrierData;
            switch (journal)
            {
                case LoadGame loadGame:
                    _currentCommander = loadGame.Commander;
                    break;
                case Docked docked:
                    if (docked.StationType == "FleetCarrier" && _manager.IsCallsignKnown(docked.StationName))
                    {
                        MaybeUpdateCarrierLocation(docked.TimestampDateTime, docked.StationName, docked.StarSystem, docked.StarSystem, /* notifyIfChanged */ false, /* carrierMoveEvent */ false);
                    }
                    break;
                case Location location:
                    // On game startup, a location event fires but we may not yet know if the carrier they're docked on is theirs, so
                    // only keep track of this initial location until we can confirm carrier ownership from a future CarrierStats or any
                    // other carrier action for that matter.
                    if (location.StationType == "FleetCarrier" && _manager.IsCallsignKnown(location.StationName))
                    {
                        MaybeUpdateCarrierLocation(location.TimestampDateTime, location.StationName, location.StarSystem, location.Body, true);
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
                    carrierData.LastCarrierJumpRequest = carrierJumpRequest;

                    var jumpTargetBody = (!string.IsNullOrEmpty(carrierJumpRequest.Body)) ? carrierJumpRequest.Body : carrierJumpRequest.SystemName;
                    string departureTime = "";
                    if (!String.IsNullOrEmpty(carrierJumpRequest.DepartureTime))
                    {
                        // TODO: Update to use this once a new framework is released: lastCarrierJumpRequest.DepartureTimeDateTime
                        DateTime carrierDepartureTime = DateTime.ParseExact(carrierJumpRequest.DepartureTime, "yyyy-MM-ddTHH:mm:ssZ", null, System.Globalization.DateTimeStyles.AssumeUniversal);
                        double departureTimeMinutes = carrierDepartureTime.Subtract(DateTime.Now).TotalMinutes;
                        if (departureTimeMinutes > 0) {
                            departureTime = $" Jump in {departureTimeMinutes:#.0} minutes (@ {carrierDepartureTime.ToShortTimeString()}).";

                            // Force timer update for new cooldown time.
                            carrierData.CooldownNotifyScheduled = false;
                            MaybeScheduleCooldownNotification(carrierData);
                        }
                    }
                    AddToGrid(carrierJumpRequest.TimestampDateTime, carrierData, carrierData.CarrierBody, carrierData.CarrierFuel, $"Requested a jump to {jumpTargetBody}.{departureTime}");
                    break;
                case CarrierJump carrierJump: // These have been broken in the past.
                    MaybeUpdateCarrierLocation(carrierJump.TimestampDateTime, carrierJump.StationName, carrierJump.StarSystem, carrierJump.Body, true);
                    break;
                case CarrierJumpCancelled carrierJumpCancelled:
                    carrierData = _manager.GetById(carrierJumpCancelled.CarrierID);
                    if (carrierData.LastCarrierJumpRequest != null)
                    {
                        AddToGrid(carrierJumpCancelled.TimestampDateTime, carrierData, carrierData.CarrierBody, carrierData.CarrierFuel, $"Cancelled requested jump to {carrierData.LastCarrierJumpRequest.SystemName}");
                    }
                    else
                    {
                        AddToGrid(carrierJumpCancelled.TimestampDateTime, carrierData, carrierData.CarrierBody, carrierData.CarrierFuel, $"Cancelled requested jump");
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

        private void MaybeUpdateCarrierLocation(DateTime dateTime, string callsign, string starSystem, string body, bool notifyIfChanged, bool carrierMoveEvent = true)
        {
            var data = _manager.GetByCallsign(callsign);
            if (data == null) return;

            if (data.CarrierSystem != null && data.CarrierBody != null && starSystem == data.CarrierSystem && body == data.CarrierBody)
            {
                // Everything is set and nothing has changed. Nothing to do.
                return;
            }
            else if ((data.CarrierSystem != null && data.CarrierBody != null && carrierMoveEvent && data.CarrierSystem != starSystem && data.CarrierBody != body)
                || data.CarrierSystem == null && data.LastCarrierJumpRequest != null && data.LastCarrierJumpRequest.SystemName == starSystem)
            {
                // We've moved.
                string locationUpdateDetails = "Carrier jump complete";
                AddToGrid(dateTime, data, body, data.CarrierFuel, locationUpdateDetails);
                // Notify if not initial values and context requests it and user has this notification enabled.
                if (notifyIfChanged && data.CarrierSystem != null && !Core.IsLogMonitorBatchReading)
                {
                    if (settings.NotifyJumpComplete) {
                        Core.SendNotification(new()
                        {
                            Title = locationUpdateDetails,
                            Detail = "",
#if EXTENDED_EVENT_ARGS
                            Sender = this,
#endif
                        });
                    }

                    MaybeScheduleCooldownNotification(data);
                }
                data.LastCarrierJumpRequest = null;
            }
            data.MaybeUpdateLocation(starSystem, body);
        }

        private void MaybeScheduleCooldownNotification(CarrierData data)
        {
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
                AddToGrid(dateTime, data, data.CarrierBody, data.CarrierFuel, $"Carrier detected: {data.CarrierName} {data.CarrierCallsign}. Configured notifications are active.");
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
                    : "Fuel level has changed.";
                // Only add "Fuel level has changed" output if we have a value for it already. At startup, the carrier detected line includes it and thus,
                // this line appears redundant.
                AddToGrid(dateTime, data, data.CarrierBody, updatedCarrierFuel, fuelDetails);

                if (!Core.IsLogMonitorBatchReading && lowFuel)
                {
                    // Only notify if fuel is running low and not reading-all.
                    Core.SendNotification(new()
                    {
                        Title = "Low Fuel",
                        Detail = fuelDetails,
#if EXTENDED_EVENT_ARGS
                        Sender = this,
#endif
                    });
                }
            }
            data.CarrierFuel = updatedCarrierFuel;
            if (stats != null) data.LastCarrierStats = stats;
        }

        private void AddToGrid(DateTime dateTime, CarrierData data, string location, int? fuelLevel, string details)
        {
            Core.AddGridItem(this, new FleetCommanderGrid
            {
                Timestamp = dateTime.ToString("G"),
                Commander = data.OwningCommander,
                Carrier = $"{data.CarrierName} ({data.CarrierCallsign})",
                CurrentLocation = location ?? "unknown",
                CurrentFuelLevel = fuelLevel == null ? "unknown" : $"{fuelLevel.Value} T",
                Details = details,
            });
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args)
        {
            if (LogMonitorStateChangedEventArgs.IsBatchRead(args.NewState))
            {
                Core.ClearGrid(this, new FleetCommanderGrid());
            }
        }

        public void Load(IObservatoryCore observatoryCore)
        {
            GridCollection = new();
            FleetCommanderGrid uiObject = new();

            GridCollection.Add(uiObject);
            pluginUI = new PluginUI(GridCollection);

            Core = observatoryCore;
        }

        private void CarrierCooldownTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var data = _manager.GetByTimer((System.Timers.Timer)sender);
            if (data == null) return;

            AddToGrid(DateTime.Now, data, data.CarrierSystem, data.CarrierFuel, "Carrier jump cooldown has ended. You may now schedule a new jump.");
            Core.SendNotification(new()
            {
                Title = "Carrier jump cooldown has ended",
                Detail = "You may now schedule a new jump.",
#if EXTENDED_EVENT_ARGS
                Sender = this,
#endif
            });
            data.CancelCarrierJump();
        }
    }

    public class FleetCommanderGrid
    {
        public string Timestamp { get; set; }
        public string Commander { get; set; }
        public string Carrier {  get; set; }
        public string CurrentLocation { get; set; }
        public string CurrentFuelLevel { get; set; }
        public string Details { get; set; }
    }
}
