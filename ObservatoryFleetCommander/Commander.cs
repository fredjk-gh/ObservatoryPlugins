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
            NotifyJumpCooldown = true,
            NotifyLowFuel = true,
        };
        private bool readAllInProgress = false;

        private Location initialLocation = null;
        private string carrierName = null;
        private ulong? carrierId = null;
        private string carrierCallsign = null;
        private string carrierSystem = null;
        private int? carrierFuel = null;
        private string carrierBody = null;
        private string jumpTargetBody = null;
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
                    jumpTargetBody = carrierJumpRequest.Body;
                    AddToGrid(carrierJumpRequest.TimestampDateTime, carrierBody, carrierFuel, string.Format("Requested a jump to {0}", jumpTargetBody));
                    break;
                case CarrierJump carrierJump: // these may be broken right now.
                    MaybeUpdateCarrierLocation(carrierJump.TimestampDateTime, carrierJump.StarSystem, carrierJump.Body, true);
                    break;
                case CarrierJumpCancelled carrierJumpCancelled:
                    AddToGrid(carrierJumpCancelled.TimestampDateTime, carrierBody, carrierFuel, string.Format("Cancelled requested jump to {0}", jumpTargetBody));
                    jumpTargetBody = null;
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
                    if (MaybeInitiailizeCarrierInfo(carrierStats.TimestampDateTime, carrierStats.CarrierID, carrierStats.Callsign, carrierStats.Name, carrierStats.FuelLevel)) break;
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
                || carrierSystem == null && jumpTargetBody != null && jumpTargetBody == body)
            {
                // We've moved.
                string locationUpdateDetails = "Carrier jump complete";
                AddToGrid(dateTime, body, carrierFuel, locationUpdateDetails);
                // Notify if not initial values and context requests it.
                if (notifyIfChanged && carrierSystem != null && !readAllInProgress)
                {
                    Core.SendNotification(new()
                    {
                        Title = locationUpdateDetails,
                        Detail = "",
                    });
                    if (settings.NotifyJumpCooldown)
                    {
                        carrierCooldownTimer = new System.Timers.Timer(TimeSpan.FromMinutes(4).TotalMilliseconds);
                        carrierCooldownTimer.Elapsed += CarrierCooldownTimer_Elapsed;
                        carrierCooldownTimer.Start();
                    }
                }
            }
            if (jumpTargetBody != null && body == jumpTargetBody) jumpTargetBody = null;
            carrierSystem = starSystem;
            carrierBody = body;
        }
        private bool MaybeInitiailizeCarrierInfo(DateTime dateTime, ulong updatedCarrierId, string updatedCarrierCallsign, string updatedCarrierName, int? updatedCarrierFuel)
        {
            if (!carrierId.HasValue || carrierCallsign == null || carrierName == null || !carrierFuel.HasValue)
            {
                if (carrierCallsign == null)
                {
                    AddToGrid(dateTime, carrierBody, updatedCarrierFuel, string.Format("Carrier detected: {0} {1}. Configured notifications are active.", updatedCarrierName, updatedCarrierCallsign));
                }
                carrierCallsign = updatedCarrierCallsign;
                carrierId = updatedCarrierId;
                carrierName = updatedCarrierName;
                MaybeUpdateCarrierFuel(dateTime, updatedCarrierFuel);
                if (initialLocation != null && initialLocation.StationName == updatedCarrierCallsign && initialLocation.Docked)
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
                    ? string.Format("Carrier has {0} tons of tritium remaining and may require more for the next jump.", updatedCarrierFuel.Value)
                    : "Fuel level has changed.";
                AddToGrid(dateTime, carrierBody, updatedCarrierFuel, fuelDetails);

                if (!readAllInProgress && lowFuel)
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
                CurrentFuelLevel = fuelLevel == null ? "unknown" : string.Format("{0} T", fuelLevel.Value),
                Details = details,
            });
        }

        public void ReadAllStarted()
        {
            readAllInProgress = true;
            Core.ClearGrid(this, new FleetCommanderGrid());
        }

        public void ReadAllFinished()
        {
            readAllInProgress = false;
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
