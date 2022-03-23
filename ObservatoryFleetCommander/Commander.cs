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

        private Location initialLocation = null;
        private string carrierName = null;
        private ulong? carrierId = null;
        private string carrierCallsign = null;
        private string carrierSystem = null;
        private int? carrierFuel = null;
        private string carrierBody = null;
        private string jumpTargetSystem = null;
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
                case LoadGame loadGame:
                    jumpTargetSystem = null;  // Suppress notifications from firing as a result of a jump completed while out of game.
                    break;
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
                    else if (location.StarSystem == jumpTargetSystem && (location.Docked || location.OnFoot))
                    {
                        // So, update 11 really made a mess of things. Location no longer has StationType/StationName properties, and still no CarrierJump eventis either.
                        // So if location is now set to jumpTargetSystem and we're docked/on-foot, it must be a carrier jump.
                        jumpTargetSystem = null;
                        MaybeUpdateCarrierLocation(location.TimestampDateTime, location.StarSystem, location.Body, true);
                    }
                    else if (initialLocation == null)
                    {
                        initialLocation = location;
                    }
                    break;
                case CarrierJumpRequest carrierJumpRequest:
                    jumpTargetSystem = carrierJumpRequest.SystemName;
                    var jumpTargetBody = (!string.IsNullOrEmpty(carrierJumpRequest.Body)) ? carrierJumpRequest.Body : carrierJumpRequest.SystemName;
                    AddToGrid(carrierJumpRequest.TimestampDateTime, carrierBody, carrierFuel, $"Requested a jump to {jumpTargetBody}");
                    break;
                case CarrierJump carrierJump: // these may be broken right now.
                    MaybeUpdateCarrierLocation(carrierJump.TimestampDateTime, carrierJump.StarSystem, carrierJump.Body, true);
                    break;
                case CarrierJumpCancelled carrierJumpCancelled:
                    if (jumpTargetSystem != null)
                    {
                        AddToGrid(carrierJumpCancelled.TimestampDateTime, carrierBody, carrierFuel, $"Cancelled requested jump to {jumpTargetSystem}");
                    }
                    else
                    {
                        AddToGrid(carrierJumpCancelled.TimestampDateTime, carrierBody, carrierFuel, $"Cancelled requested jump");
                    }
                    jumpTargetSystem = null;
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
                    if (MaybeInitializeCarrierInfo(carrierStats.TimestampDateTime, carrierStats.CarrierID, carrierStats.Callsign, carrierStats.Name, carrierStats.FuelLevel)) break;
                    // Ok, no initialization was required, we always need to check/update fuel levels on this event.
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
                || carrierSystem == null && jumpTargetSystem != null && jumpTargetSystem == starSystem)
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
                        });
                    }
                    if (settings.NotifyJumpCooldown)
                    {
                        carrierCooldownTimer = new System.Timers.Timer(TimeSpan.FromMinutes(4).TotalMilliseconds);
                        carrierCooldownTimer.Elapsed += CarrierCooldownTimer_Elapsed;
                        carrierCooldownTimer.Start();
                    }
                }
            }
            if (jumpTargetSystem != null && starSystem == jumpTargetSystem) jumpTargetSystem = null;
            carrierSystem = starSystem;
            carrierBody = body;
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
                    ? $"Carrier has {updatedCarrierFuel.Value} tons of tritium remaining and may require more for the next jump."
                    : "Fuel level has changed.";
                AddToGrid(dateTime, carrierBody, updatedCarrierFuel, fuelDetails);

                if (!Core.IsLogMonitorBatchReading && lowFuel)
                {
                    // Only notify if fuel is running low and not reading-all.
                    Core.SendNotification(new()
                    {
                        Title = "Low Fuel",
                        Detail = fuelDetails,
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
            });
            carrierCooldownTimer.Stop();
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
