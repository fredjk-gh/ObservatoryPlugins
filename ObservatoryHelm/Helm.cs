using Observatory.Framework;
using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;
using Observatory.Framework.Interfaces;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace com.github.fredjk_gh.ObservatoryHelm
{
    public class Helm : IObservatoryWorker
    {
        private IObservatoryCore Core;
        /// <summary>
        /// Error logger; string param is an arbitrary context hint.
        /// </summary>
        private Action<Exception, string> ErrorLogger;
        private PluginUI pluginUI;
        ObservableCollection<object> gridCollection = new();
        private HelmSettings settings = HelmSettings.DEFAULT;
        private TrackedData data = new();
        private HashSet<string> incompleteSystemsNotified = new();

        private static readonly int MAX_FUEL = 99;

        public string Name => "Observatory Helm";
        public string ShortName => "Helm";
        public string Version => typeof(Helm).Assembly.GetName().Version.ToString();

        public PluginUI PluginUI => pluginUI;

        public object Settings
        {
            get => settings;
            set
            {
                HelmSettings newSettings = (HelmSettings)value;
                if (newSettings.GravityWarningThresholdx10 > newSettings.GravityAdvisoryThresholdx10)
                {
                    settings = newSettings;
                };
            }
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args)
        {
            if (args.NewState.HasFlag(LogMonitorState.Batch))
            {
                Core.ClearGrid(this, new HelmGrid());
                data = new();
            }
            else if (args.PreviousState.HasFlag(LogMonitorState.Batch))
            {
                // Exiting read-all: Spit out summary of most recent session:
                MaybeMakeGridItemForSummary(data.LastJumpEvent?.Timestamp ?? "");
            }
        }

        public void Load(IObservatoryCore observatoryCore)
        {
            Core = observatoryCore;
            ErrorLogger = Core.GetPluginErrorLogger(this);

            gridCollection = new();
            HelmGrid uiObject = new();
            gridCollection.Add(uiObject);
            pluginUI = new PluginUI(gridCollection);
        }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            switch (journal)
            {
                // TODO: on ApproachBody before glide (ie ~ApproachBody): alert to the gravity of the planet.
                // Would require saving scans of at least landable bodies in a dictionary keyed by either body id or body name.
                // {"timestamp":"2023-03-06T01:36:05Z","event":"ApproachBody","StarSystem":"Viqs KC-M d7-2","SystemAddress":79297529027,"Body":"Viqs KC-M d7-2 4","BodyID":5}
                // {"timestamp":"2023-03-06T01:36:48Z","event":"SupercruiseExit","Taxi":false,"Multicrew":false,"StarSystem":"Viqs KC-M d7-2","SystemAddress":79297529027,"Body":"Viqs KC-M d7-2 4","BodyID":5,"BodyType":"Planet"}
                // Consider alerting only if gravity exceeds a certain threshold (1g?)
                // Other?
                case LoadGame loadGame:
                    if (Core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch))
                    {
                        // Summarize the last session:
                        MaybeMakeGridItemForSummary(data.LastJumpEvent?.Timestamp ?? "");

                    }
                    data.SessionReset(loadGame.Odyssey, loadGame.FuelCapacity, loadGame.FuelLevel);
                    break;
                case Location location:
                    data.CurrentSystem = location.StarSystem;
                    break;
                case Loadout loadOut:
                    var fsdModule = findFSD(loadOut.Modules);
                    if (fsdModule != null && Constants.MaxFuelPerJumpByFSDSizeClass.ContainsKey(fsdModule.Item))
                        data.MaxFuelPerJump = Constants.MaxFuelPerJumpByFSDSizeClass[fsdModule.Item];
                    // TODO: If FuelCapacity is null, it's probably an old journal that had a different format: "FuelCapacity":2.000000
                    // Consider parsing this out manually using System.Text.Json.
                    data.FuelCapacity = (loadOut.FuelCapacity != null ?  loadOut.FuelCapacity.Main : MAX_FUEL);
                    break;
                case FuelScoop fuelScoop:
                    data.FuelRemaining = Math.Min(data.FuelCapacity, data.FuelRemaining + fuelScoop.Scooped);
                    break;
                case NavRouteClear clear:
                    data.JumpsRemainingInRoute = 0;
                    break;
                case StartJump startJump:
                    // data.LastStartJumpEvent = null;
                    if (startJump.JumpType == "Hyperspace")
                    {
                        data.LastStartJumpEvent = startJump;
                    }
                    break;
                case FSDJump jump:
                    incompleteSystemsNotified.Clear();
                    if (jump is CarrierJump carrierJump && carrierJump.Docked) // (carrierJump.Docked || carrierJump.OnFoot))
                    {
                        // Do nothing else. We're missing fuel level, etc. here.
                        if (data.LastJumpEvent != null)
                        {
                            double distance = Id64.Id64CoordHelper.Distance(carrierJump.StarPos, data.LastJumpEvent.StarPos);

                            //double distance = Id64CoordHelper.Distance((long) carrierJump.SystemAddress, (long)data.LastJumpEvent.SystemAddress);
                            data.SystemResetDockedOnCarrier(carrierJump.StarSystem, distance);
                        }
                        else
                        {
                            data.SystemResetDockedOnCarrier(carrierJump.StarSystem);
                        }
                        data.LastJumpEvent = jump;
                        data.LastStartJumpEvent = null;
                        MakeGridItem(jump.Timestamp, "(docked on carrier)");
                    }
                    else
                    {
                        data.SystemReset(jump.StarSystem, jump.FuelLevel, jump.JumpDist);
                        data.LastJumpEvent = jump;
                        if (Core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch))
                        {
                            // Don't output anything further during read-all. LoadGame handler spits out session summaries.
                            break;
                        }

                        MakeGridItem(jump.Timestamp, (data.JumpsRemainingInRoute == 0 ? "(no route)" : ""));
                        string arrivalStarScoopableStr = (data.LastStartJumpEvent != null && !Constants.Scoopables.Contains(data.LastStartJumpEvent.StarClass))
                                ? $"Arrival star (type: {data.LastStartJumpEvent.StarClass}) is NOT scoopable!"
                                : "";
                        Core.SendNotification(new()
                        {
                            Title = "Route Progress",
                            Detail = $"Jumps left: {data.JumpsRemainingInRoute}",
#if EXTENDED_EVENT_ARGS
                            Sender = this,
                            ExtendedDetails = arrivalStarScoopableStr,
                            Rendering = NotificationRendering.PluginNotifier,
#endif
                        });
                        // This check is essential when travelling through areas where scans don't trigger (ie. known space).
                        if (data.FuelWarningNotifiedSystem != jump.StarSystem && data.FuelRemaining < (data.MaxFuelPerJump * Constants.FuelWarningLevelFactor))
                        {
                            Core.SendNotification(new()
                            {
                                Title = "Warning! Low fuel",
                                Detail = $"Refuel soon! {arrivalStarScoopableStr}",
#if EXTENDED_EVENT_ARGS
                                Sender = this,
#endif
                            });
                            data.FuelWarningNotifiedSystem = jump.StarSystem;
                        }
                    }
                    break;
                case Docked docked:
                    if (docked.StationType == "FleetCarrier")
                    {
                        data.IsDockedOnCarrier = true;
                    }
                    break;
                case Undocked undocked:
                    data.IsDockedOnCarrier = false;
                    break;
                case FSDTarget target:
                    data.JumpsRemainingInRoute = target.RemainingJumpsInRoute;
                    break;
                case ReservoirReplenished replenished:
                    data.FuelRemaining = replenished.FuelMain;
                    break;
                case RefuelAll refuel:
                    if (refuel is RefuelPartial)
                    {
                        data.FuelRemaining += refuel.Amount;
                    }
                    else
                    {
                        data.FuelRemaining = data.FuelCapacity;
                    }
                    break;
                case Scan scan:
                    if (scan.ScanType != "NavBeaconDetail" && scan.PlanetClass != "Barycentre" && !scan.WasDiscovered && scan.DistanceFromArrivalLS == 0)
                    {
                        data.UndiscoveredSystem = true;
                    }
                    data.Scans[scan.BodyName] = scan;
                    if (data.FuelRemaining < (data.MaxFuelPerJump * Constants.FuelWarningLevelFactor) && data.FuelWarningNotifiedSystem != scan.StarSystem)
                    {
                        if (Constants.Scoopables.Contains(scan.StarType) && scan.DistanceFromArrivalLS < settings.MaxNearbyScoopableDistance)
                        {
                            string extendedDetails = $"Warning! Low Fuel! Scoopable star: {BodyShortName(scan.BodyName, data.CurrentSystem /*scan.StarSystem*/)}, {scan.DistanceFromArrivalLS:0.0} Ls";
                            MakeGridItem(scan.Timestamp, extendedDetails);
                            Core.SendNotification(new()
                            {
                                Title = "Warning! Low fuel",
                                Detail = $"There is a scoopable star in this system!",
#if EXTENDED_EVENT_ARGS
                                ExtendedDetails = extendedDetails,
                                Sender = this,
#endif
                            });
                            data.FuelWarningNotifiedSystem = data.CurrentSystem; // scan.StarSystem;
                        }
                    }

                    if (scan.StarType == "N" && scan.DistanceFromArrivalLS == 0) data.NeutronPrimarySystem = data.CurrentSystem; // scan.StarSystem;

                    if (data.NeutronPrimarySystem == null || data.NeutronPrimarySystem != data.CurrentSystem /*scan.StarSystem*/) break;

                    if ((Constants.Scoopables.Contains(scan.StarType) && scan.DistanceFromArrivalLS < settings.MaxNearbyScoopableDistance && data.ScoopableSecondaryCandidateScan?.StarSystem != data.CurrentSystem /*scan.StarSystem*/)
                        || (data.ScoopableSecondaryCandidateScan?.StarSystem == data.CurrentSystem /*scan.StarSystem*/ && scan.DistanceFromArrivalLS < data.ScoopableSecondaryCandidateScan.DistanceFromArrivalLS))
                    {
                        data.ScoopableSecondaryCandidateScan = scan;
                    }

                    if (data.NeutronPrimarySystem == scan.StarSystem && data.ScoopableSecondaryCandidateScan?.StarSystem == data.CurrentSystem /*scan.StarSystem*/
                        && data.NeutronPrimarySystemNotified != data.CurrentSystem /*scan.StarSystem*/)
                    {
                        data.NeutronPrimarySystemNotified = data.CurrentSystem; // scan.StarSystem;
                        string extendedDetails = $"Nearby scoopable secondary star; Type: {data.ScoopableSecondaryCandidateScan?.StarType}, {Math.Round(data.ScoopableSecondaryCandidateScan?.DistanceFromArrivalLS ?? 0, 1)} Ls";
                        MakeGridItem(scan.Timestamp, extendedDetails);
                        Core.SendNotification(new()
                        {
                            Title = "Nearby scoopable secondary star",
                            Detail = $"Body {BodyShortName(scan.BodyName, data.CurrentSystem /*scan.StarSystem*/)}{Environment.NewLine}Type: {data.ScoopableSecondaryCandidateScan?.StarType}{Environment.NewLine}{Math.Round(data.ScoopableSecondaryCandidateScan?.DistanceFromArrivalLS ?? 0, 1)} Ls",
#if EXTENDED_EVENT_ARGS
                            ExtendedDetails = extendedDetails,
                            Sender = this,
#endif
                        });
                    }
                    break;
                case FSSAllBodiesFound allFound:
                    if (data.FuelWarningNotifiedSystem == allFound.SystemName) break;

                    if (data.FuelRemaining < (data.MaxFuelPerJump * Constants.FuelWarningLevelFactor))
                    {
                        string extendedDetails = $"Warning! Low Fuel and no scoopable star! Be sure to jump to a system with a scoopable star!";
                        MakeGridItem(allFound.Timestamp, extendedDetails);
                        Core.SendNotification(new()
                        {
                            Title = "Warning! Low fuel!",
                            Detail = "There is NO scoopable star in this system!",
#if EXTENDED_EVENT_ARGS
                            ExtendedDetails = extendedDetails,
                            Sender = this,
#endif
                        });
                    }
                    if (!data.AllBodiesFound)
                    {
                        Core.SendNotification(new()
                        {
                            Title = "FSS completed",
                            Detail = $"All {allFound.Count} bodies found",
                            Rendering = NotificationRendering.PluginNotifier,
#if EXTENDED_EVENT_ARGS
                            Sender = this,
#endif
                        });
                    }
                    data.AllBodiesFound = true;
                    break;
                case ApproachBody approachBody:
                    Scan s;
                    if (settings.GravityAdvisoryThresholdx10 <= 0) break; // disabled
                    if (!data.Scans.TryGetValue(approachBody.Body, out s)) break;

                    string bodyShortName = BodyShortName(approachBody.Body, approachBody.StarSystem);
                    var gravityG = s.SurfaceGravity / Constants.CONV_MperS2_TO_G_DIVISOR;
                    if (gravityG >= settings.GravityWarningThresholdx10 / 10.0)
                    {
                        Core.SendNotification(new()
                        {
                            Title = $"Body {bodyShortName}",
                            Detail = $"Warning! High gravity: {gravityG:0.0}g",
#if EXTENDED_EVENT_ARGS
                            Sender = this,
#endif
                        });
                    }
                    else if (gravityG > settings.GravityAdvisoryThresholdx10 / 10.0)
                    {
                        Core.SendNotification(new()
                        {
                            Title = $"Body {bodyShortName}",
                            Detail = $"Heads up! Relatively high gravity: {gravityG:0.0}g",
#if EXTENDED_EVENT_ARGS
                            Sender = this,
#endif
                        });
                    }
                    break;
            }
        }

        public void StatusChange(Status status)
        {
            // Update fuel level with latest, greatest data.
            if (status.Fuel != null) 
                data.FuelRemaining = status.Fuel.FuelMain;

            // TODO: Add a setting for this.
            if (status.Flags2.HasFlag(StatusFlags2.FsdHyperdriveCharging)
                && !data.AllBodiesFound && data.UndiscoveredSystem && !incompleteSystemsNotified.Contains(data.CurrentSystem))
            {
                incompleteSystemsNotified.Add(data.CurrentSystem);
                Core.SendNotification(new()
                {
                    Title = "Incomplete system scan!",
                    Detail = $"Heads up! The undiscovered system you are about to leave is not fully scanned!",
#if EXTENDED_EVENT_ARGS
                    Sender = this,
#endif
                });
            }
        }

        private static string BodyShortName(string bodyName, string systemName)
        {
            string shortName = bodyName.Replace(systemName, "").Trim();

            return (string.IsNullOrEmpty(shortName) ? "Primary star" : shortName);
        }

        private void MakeGridItem(string timestamp, string details = "")
        {
            if (Core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch)) return;

            HelmGrid gridItem = new()
            {
                Timestamp = timestamp,
                System = data.CurrentSystem ?? "",
                Fuel = (data.IsDockedOnCarrier ? "" : $"{(100 * data.FuelRemaining / data.FuelCapacity):0}% ({Math.Round(data.FuelRemaining, 2)} T)"),
                DistanceTravelled = $"{data.DistanceTravelled:n1} ly ({data.JumpsCompleted} jumps" + (data.DockedCarrierJumpsCompleted > 0 ? $", {data.DockedCarrierJumpsCompleted} jumps on a carrier)" : ")"),
                JumpsRemaining = data.IsDockedOnCarrier ? "" : (data.JumpsRemainingInRoute > 0 ? $"{data.JumpsRemainingInRoute}" : ""),
                Details = $"{details}",
            };
            Core.AddGridItem(this, gridItem);
        }

        private void MaybeMakeGridItemForSummary(string timestamp)
        {
            if (data.JumpsCompleted == 0)
            {
                // Do nothing.
                return;
            }

            HelmGrid gridItem = new()
            {
                Timestamp = timestamp,
                System = data.CurrentSystem ?? "",
                Fuel = "",
                DistanceTravelled = $"{data.DistanceTravelled:n1} ly ({data.JumpsCompleted} jumps" + (data.DockedCarrierJumpsCompleted > 0 ? $", {data.DockedCarrierJumpsCompleted} jumps on a carrier)" : ")"),
                JumpsRemaining = data.IsDockedOnCarrier ? "" : (data.JumpsRemainingInRoute > 0 ? $"{data.JumpsRemainingInRoute}" : ""),
                Details = "Previous session summary",
            };

            Core.AddGridItem(this, gridItem);
        }

        private static Modules? findFSD(ImmutableList<Modules> modules)
        {
            foreach (var m in modules)
            {
                if (m.Slot.Equals("FrameShiftDrive", StringComparison.InvariantCultureIgnoreCase))
                {
                    return m;
                }
            }
            return null;
        }

        class HelmGrid
        {
            public string Timestamp { get; set; }
            public string System { get; set; }
            public string Fuel { get; set; }
            public string DistanceTravelled { get; set; }
            public string JumpsRemaining { get; set; }
            public string Details { get; set; }
        }
    }
}
