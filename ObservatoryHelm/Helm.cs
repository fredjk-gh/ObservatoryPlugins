using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;
using Observatory.Framework.Interfaces;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace ObservatoryHelm
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
                    data.SessionReset(loadGame.Odyssey, loadGame.FuelCapacity, loadGame.FuelLevel);
                    //if (!Core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch))
                    Core.ClearGrid(this, new HelmGrid());
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
                case FSDJump jump:
                    if (jump is CarrierJump carrierJump && carrierJump.Docked)
                    {
                        // Do nothing else. We're missing fuel level, etc. here.
                        data.SystemResetDockedOnCarrier(carrierJump.StarSystem);
                        Core.AddGridItem(this, MakeGridItem(jump.Timestamp, "(docked on carrier)"));
                    }
                    else
                    {
                        data.SystemReset(jump.StarSystem, jump.FuelLevel, jump.JumpDist);
                        Core.AddGridItem(this, MakeGridItem(jump.Timestamp, (data.JumpsRemainingInRoute == 0 ? "(no route)" : "")));
                        Core.SendNotification(new()
                        {
                            Title = "Route Progress",
                            Detail = $"Jumps left: {data.JumpsRemainingInRoute}",
#if EXTENDED_EVENT_ARGS
                            Sender = this,
                            Rendering = NotificationRendering.PluginNotifier,
#endif
                        });
                        // This check is essential when travelling through areas where scans don't trigger (ie. known space).
                        if (data.FuelWarningNotifiedSystem != jump.StarSystem && data.FuelRemaining < (data.MaxFuelPerJump * Constants.FuelWarningLevelFactor))
                        {
                            Core.SendNotification(new()
                            {
                                Title = "Warning! Low fuel",
                                Detail = "Refuel soon!",
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
                    data.Scans[scan.BodyName] = scan;
                    if (data.FuelRemaining < (data.MaxFuelPerJump * Constants.FuelWarningLevelFactor) && data.FuelWarningNotifiedSystem != scan.StarSystem)
                    {
                        if (Constants.Scoopables.Contains(scan.StarType) && scan.DistanceFromArrivalLS < settings.MaxNearbyScoopableDistance)
                        {
                            string extendedDetails = $"Warning! Low Fuel! Scoopable star: {BodyShortName(scan.BodyName, data.CurrentSystem /*scan.StarSystem*/)}, {scan.DistanceFromArrivalLS:0.0} Ls";
                            Core.AddGridItem(this, MakeGridItem(scan.Timestamp, extendedDetails));
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
                        Core.AddGridItem(this, MakeGridItem(scan.Timestamp, extendedDetails));
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
                    if (data.FuelRemaining < (data.MaxFuelPerJump * Constants.FuelWarningLevelFactor) && data.FuelWarningNotifiedSystem != allFound.SystemName)
                    {
                        string extendedDetails = $"Warning! Low Fuel and no scoopable star! Be sure to jump to a system with a scoopable star!";
                        Core.AddGridItem(this, MakeGridItem(allFound.Timestamp, extendedDetails));
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
                    break;
                case ApproachBody approachBody:
                    Scan s;
                    if (settings.GravityAdvisoryThresholdx10 <= 0) break; // disabled
                    if (!data.Scans.TryGetValue(approachBody.Body, out s)) break;

                    string bodyShortName = BodyShortName(approachBody.Body, approachBody.StarSystem);
                    var gravityG = s.SurfaceGravity / Constants.CONV_MperS2_TO_G_DIVISOR;
                    if (gravityG >= settings.GravityWarningThresholdx10 / 10)
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
                    else if (gravityG > settings.GravityAdvisoryThresholdx10 / 10)
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

        private static string BodyShortName(string bodyName, string systemName)
        {
            return bodyName.Replace(systemName, "").Trim();
        }

        private HelmGrid MakeGridItem(string timestamp, string details = "")
        {
            HelmGrid gridItem = new()
            {
                Timestamp = timestamp,
                System = data.CurrentSystem ?? "",
                Fuel = data.IsDockedOnCarrier ? "" : $"{(100 * data.FuelRemaining / data.FuelCapacity):0}% ({Math.Round(data.FuelRemaining, 2)} T)",
                DistanceTravelled = data.IsDockedOnCarrier ? "" : $"{data.DistanceTravelled:0.#} ly",
                JumpsRemaining = data.IsDockedOnCarrier ? "" : (data.JumpsRemainingInRoute > 0 ? $"{data.JumpsRemainingInRoute}" : ""),
                Details = $"{details}",
            };
            return gridItem;
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
