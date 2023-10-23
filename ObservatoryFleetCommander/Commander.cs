using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ObservatoryFleetCommander.Worker
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

        private CarrierJumpRequest lastCarrierJumpRequest = null;
        private bool cooldownNotifyScheduled = false;
        private Location initialLocation = null;
        private string carrierName = null;
        private ulong? carrierId = null;
        private string carrierCallsign = null;
        private string carrierSystem = null;
        private int? carrierFuel = null;
        private string carrierBody = null;
        private System.Timers.Timer carrierCooldownTimer;
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
            switch (journal)
            {

                case Docked docked:
                    if (docked.StationType == "FleetCarrier" && carrierId.HasValue && docked.StationName == carrierCallsign && carrierSystem == null)
                    {
                        MaybeUpdateCarrierLocation(docked.TimestampDateTime, docked.StarSystem, docked.StarSystem, /* notifyIfChanged */ false, /* carrierMoveEvent */ false);
                    }
                    break;
                case Location location:
                    // On game startup, a location event fires but we may not yet know if the carrier they're docked on is theirs, so
                    // only keep track of this initial location until we can confirm carrier ownership from a future CarrierStats or any
                    // other carrier action for that matter.
                    if (location.StationType == "FleetCarrier" && carrierId.HasValue && location.StationName == carrierCallsign)
                    {
                        MaybeUpdateCarrierLocation(location.TimestampDateTime, location.StarSystem, location.Body, true);
                    }
                    else if (initialLocation == null)
                    {
                        initialLocation = location;
                    }
                    break;
                case CarrierJumpRequest carrierJumpRequest:
                    lastCarrierJumpRequest = carrierJumpRequest;
                    var jumpTargetBody = (!string.IsNullOrEmpty(carrierJumpRequest.Body)) ? carrierJumpRequest.Body : carrierJumpRequest.SystemName;
                    string departureTime = "";
                    if (!String.IsNullOrEmpty(carrierJumpRequest.DepartureTime))
                    {
                        // TODO: Update to use this once a new framework is released: lastCarrierJumpRequest.DepartureTimeDateTime
                        DateTime carrierDepartureTime = DateTime.ParseExact(lastCarrierJumpRequest.DepartureTime, "yyyy-MM-ddTHH:mm:ssZ", null, System.Globalization.DateTimeStyles.AssumeUniversal);
                        double departureTimeMinutes = carrierDepartureTime.Subtract(DateTime.Now).TotalMinutes;
                        if (departureTimeMinutes > 0) {
                            departureTime = $" Jump in {departureTimeMinutes:#.0} minutes (@ {carrierDepartureTime.ToShortTimeString()}).";

                            // Force timer update for new cooldown time.
                            cooldownNotifyScheduled = false;
                            MaybeScheduleCooldownNotification();
                        }
                    }
                    AddToGrid(carrierJumpRequest.TimestampDateTime, carrierBody, carrierFuel, $"Requested a jump to {jumpTargetBody}.{departureTime}");
                    break;
                case CarrierJump carrierJump: // these may be broken right now.
                    MaybeUpdateCarrierLocation(carrierJump.TimestampDateTime, carrierJump.StarSystem, carrierJump.Body, true);
                    break;
                case CarrierJumpCancelled carrierJumpCancelled:
                    if (lastCarrierJumpRequest != null)
                    {
                        AddToGrid(carrierJumpCancelled.TimestampDateTime, carrierBody, carrierFuel, $"Cancelled requested jump to {lastCarrierJumpRequest.SystemName}");
                    }
                    else
                    {
                        AddToGrid(carrierJumpCancelled.TimestampDateTime, carrierBody, carrierFuel, $"Cancelled requested jump");
                    }
                    CancelCarrierJump();
                    break;
                case CarrierDepositFuel carrierDepositFuel:
                    // Make sure the Cmdr is donating to their carrier for fuel updates as this event could trigger for
                    // any commander's carrier when donating tritium.
                    if (carrierId == carrierDepositFuel.CarrierID)
                    {
                        // Technically this could wait until the next CarrierStats event.
                        MaybeUpdateCarrierFuel(carrierDepositFuel.TimestampDateTime, carrierDepositFuel.Total);
                    }
                    break;
                case CarrierStats carrierStats:
                    if (MaybeInitializeCarrierInfo(carrierStats.TimestampDateTime, carrierStats.CarrierID, carrierStats.Callsign, carrierStats.Name, carrierStats.FuelLevel))
                        // Initialization also checks fuel levels. Don't duplicate efforts.
                        break;

                    MaybeUpdateCarrierFuel(carrierStats.TimestampDateTime, carrierStats.FuelLevel);
                    break;
            }
        }

        private void MaybeUpdateCarrierLocation(DateTime dateTime, string starSystem, string body, bool notifyIfChanged, bool carrierMoveEvent = true)
        {
            if (carrierSystem != null && carrierBody != null && starSystem == carrierSystem && body == carrierBody)
            {
                // Everything is set and nothing has changed. Nothing to do.
                return;
            }
            else if ((carrierSystem != null && carrierBody != null && carrierMoveEvent && carrierSystem != starSystem && carrierBody != body)
                || carrierSystem == null && lastCarrierJumpRequest != null && lastCarrierJumpRequest.SystemName == starSystem)
            {
                // We've moved.
                string locationUpdateDetails = "Carrier jump complete";
                AddToGrid(dateTime, body, carrierFuel, locationUpdateDetails);
                // Notify if not initial values and context requests it and user has this notification enabled.
                if (notifyIfChanged && carrierSystem != null && !Core.IsLogMonitorBatchReading)
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

                    MaybeScheduleCooldownNotification();
                }
                lastCarrierJumpRequest = null;
            }
            carrierSystem = starSystem;
            carrierBody = body;
        }

        private void MaybeScheduleCooldownNotification()
        {
            // Don't start a countdown if the cooldown is already over.
            // TODO: Update to use this once a new framework is released: lastCarrierJumpRequest.DepartureTimeDateTime
            DateTime carrierDepartureTime = DateTime.ParseExact(lastCarrierJumpRequest.DepartureTime, "yyyy-MM-ddTHH:mm:ssZ", null, System.Globalization.DateTimeStyles.AssumeUniversal);
            DateTime carrierJumpCooldownTime = carrierDepartureTime.AddMinutes(5);
            if (settings.NotifyJumpCooldown && !cooldownNotifyScheduled && DateTime.Now < carrierJumpCooldownTime)
            {
                carrierCooldownTimer = new System.Timers.Timer(carrierJumpCooldownTime.Subtract(DateTime.Now).TotalMilliseconds);
                carrierCooldownTimer.Elapsed += CarrierCooldownTimer_Elapsed;
                carrierCooldownTimer.Start();
                cooldownNotifyScheduled = true;
            }
        }

        private void CancelCarrierJump()
        {
            lastCarrierJumpRequest = null;
            cooldownNotifyScheduled = false;
            if (carrierCooldownTimer != null) carrierCooldownTimer.Stop();
            carrierCooldownTimer = null;
        }

        private bool MaybeInitializeCarrierInfo(DateTime dateTime, ulong updatedCarrierId, string updatedCarrierCallsign, string updatedCarrierName, int? updatedCarrierFuel)
        {
            if (!carrierId.HasValue || carrierCallsign == null || carrierName == null || !carrierFuel.HasValue)
            {
                if (carrierCallsign == null)
                {
                    AddToGrid(dateTime, carrierBody, updatedCarrierFuel, $"Carrier detected: {updatedCarrierName} {updatedCarrierCallsign}. Configured notifications are active.");
                }
                carrierCallsign = updatedCarrierCallsign;
                carrierId = updatedCarrierId;
                carrierName = updatedCarrierName;
                MaybeUpdateCarrierFuel(dateTime, updatedCarrierFuel);
                if (initialLocation != null && initialLocation.StationName == updatedCarrierCallsign && (initialLocation.Docked || initialLocation.OnFoot))
                {
                    MaybeUpdateCarrierLocation(initialLocation.TimestampDateTime, initialLocation.StarSystem, initialLocation.Body, false /* notifyIfChanged */, false /* carrierMoveEvent */);
                }
                return true;
            }
            return false;
        }

        private void MaybeUpdateCarrierFuel(DateTime dateTime, int? updatedCarrierFuel)
        {
            // First update on fuel level or fuel has dropped below previous value. Let 'em now if we're running low.
            if (updatedCarrierFuel.HasValue && (!carrierFuel.HasValue || carrierFuel.Value != updatedCarrierFuel.Value))
            {
                bool lowFuel = updatedCarrierFuel.Value < 135;
                string fuelDetails = lowFuel
                    ? $"Carrier has {updatedCarrierFuel.Value} tons of tritium remaining and may require more soon."
                    : "Fuel level has changed.";
                // Only add "Fuel level has changed" output if we have a value for it already. At startup, the carrier detected line includes it and thus,
                // this line appears redundant.
                if (carrierFuel.HasValue) AddToGrid(dateTime, carrierBody, updatedCarrierFuel, fuelDetails);

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
            carrierFuel = updatedCarrierFuel;
        }

        private void AddToGrid(DateTime dateTime, string location, int? fuelLevel, string details)
        {
            Core.AddGridItem(this, new FleetCommanderGrid
            {
                Timestamp = dateTime.ToString("G"),
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
            AddToGrid(DateTime.Now, carrierSystem, carrierFuel, "Carrier jump cooldown has ended. You may now schedule a new jump.");
            Core.SendNotification(new()
            {
                Title = "Carrier jump cooldown has ended",
                Detail = "You may now schedule a new jump.",
#if EXTENDED_EVENT_ARGS
                Sender = this,
#endif
            });
            CancelCarrierJump();
        }
    }

    public class FleetCommanderGrid
    {
        public string Timestamp { get; set; }
        public string CurrentLocation { get; set; }
        public string CurrentFuelLevel { get; set; }
        public string Details { get; set; }
    }
}
