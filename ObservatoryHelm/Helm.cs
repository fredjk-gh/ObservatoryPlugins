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
        private bool hasLoadedDestinations = false;

        private static readonly int MAX_FUEL = 99;

        public string Name => "Observatory Helm";
        public string ShortName => "Helm";
        public string Version => typeof(Helm).Assembly.GetName().Version.ToString();
        public PluginUI PluginUI => pluginUI;

        public object Settings
        {
            get => settings;
            set =>  settings = (HelmSettings)value;
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

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args)
        {
            // * -> ReadAll
            if (args.NewState.HasFlag(LogMonitorState.Batch))
            {
                Core.ClearGrid(this, new HelmGrid());
                data = new();
            }
            // ReadAll -> 
            else if (args.PreviousState.HasFlag(LogMonitorState.Batch))
            {
                // Exiting read-all: Spit out summary of most recent session:
                MaybeMakeGridItemForSummary(data.CommanderData?.LastJumpEvent?.Timestamp ?? "");

                if (args.NewState.HasFlag(LogMonitorState.Realtime))
                {
                    MaybeRestoreDestinations();
                }
            }
            // Exiting Preread
            else if (args.PreviousState.HasFlag(LogMonitorState.PreRead)
                && !args.NewState.HasFlag(LogMonitorState.PreRead))
            {
                MaybeRestoreDestinations();
            }
        }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            switch (journal)
            {
                case LoadGame loadGame:
                    if (Core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch))
                    {
                        // Summarize the last session:
                        MaybeMakeGridItemForSummary(data.CommanderData?.LastJumpEvent?.Timestamp ?? "");
                    }

                    bool commanderChanged = loadGame.Commander != data.CurrentCommander && !string.IsNullOrWhiteSpace(data.CurrentCommander);

                    data.SessionReset(loadGame.Odyssey, loadGame.Commander, loadGame.FuelCapacity, loadGame.FuelLevel);

                    MaybeRestoreDestinations(commanderChanged);
                    break;
                case Location location:
                    if (!data.IsCommanderKnown) break;

                    data.CommanderData.CurrentSystem = location.StarSystem;
                    break;
                case Loadout loadOut:
                    if (!data.IsCommanderKnown) break;

                    var fsdModule = findFSD(loadOut.Modules);
                    if (fsdModule != null && Constants.MaxFuelPerJumpByFSDSizeClass.ContainsKey(fsdModule.Item))
                        data.CommanderData.MaxFuelPerJump = Constants.MaxFuelPerJumpByFSDSizeClass[fsdModule.Item];
                    // TODO: If FuelCapacity is null, it's probably an old journal that had a different format: "FuelCapacity":2.000000
                    // Consider parsing this out manually using System.Text.Json.
                    data.CommanderData.FuelCapacity = (loadOut.FuelCapacity != null ?  loadOut.FuelCapacity.Main : MAX_FUEL);
                    break;
                case FuelScoop fuelScoop:
                    if (!data.IsCommanderKnown) break;

                    data.CommanderData.FuelRemaining = Math.Min(data.CommanderData.FuelCapacity, data.CommanderData.FuelRemaining + fuelScoop.Scooped);
                    break;
                case NavRouteFile navRoute:
                    if (!data.IsCommanderKnown) break;

                    data.CommanderData.LastNavRoute = navRoute;
                    Core.SendNotification(new()
                    {
                        Title = "New route",
                        Detail = "",
                        Sender = ShortName,
                        ExtendedDetails = $"{data.CommanderData.JumpsRemainingInRoute} jumps to {data.CommanderData.Destination}",
                        Rendering = NotificationRendering.PluginNotifier,
                        CoalescingId = Constants.COALESCING_ID_POST_SYSTEM,
                    });

                    if (!Core.IsLogMonitorBatchReading)
                        data.SaveRouteDestinations(Core.PluginStorageFolder);
                    break;
                case NavRouteClear clear:
                    if (!data.IsCommanderKnown) break;

                    data.CommanderData.JumpsRemainingInRoute = 0;
                    data.CommanderData.Destination = "";

                    if (!Core.IsLogMonitorBatchReading)
                        data.SaveRouteDestinations(Core.PluginStorageFolder);
                    break;
                case StartJump startJump:
                    if (!data.IsCommanderKnown) break;

                    if (startJump.JumpType == "Hyperspace")
                    {
                        data.CommanderData.LastStartJumpEvent = startJump;
                    }
                    break;
                case FSDJump jump:
                    if (!data.IsCommanderKnown) break;

                    incompleteSystemsNotified.Clear();
                    if (jump is CarrierJump carrierJump && (
                        carrierJump.Docked || carrierJump.OnFoot))
                    {
                        // Do nothing else. We're missing fuel level, etc. here.
                        if (data.CommanderData.LastJumpEvent != null)
                        {
                            double distance = Id64.Id64CoordHelper.Distance(carrierJump.StarPos, data.CommanderData.LastJumpEvent.StarPos);

                            //double distance = Id64CoordHelper.Distance((long) carrierJump.SystemAddress, (long)data.CommanderData.LastJumpEvent.SystemAddress);
                            data.SystemResetDockedOnCarrier(carrierJump.StarSystem, distance);
                        }
                        else
                        {
                            data.SystemResetDockedOnCarrier(carrierJump.StarSystem);
                        }
                        data.CommanderData.LastJumpEvent = jump;
                        data.CommanderData.LastStartJumpEvent = null;
                        MakeGridItem(jump.Timestamp, "(docked on carrier)");
                    }
                    else
                    {
                        data.SystemReset(jump.StarSystem, jump.FuelLevel, jump.JumpDist);
                        data.CommanderData.LastJumpEvent = jump;
                        if (Core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch))
                        {
                            // Don't output anything further during read-all. LoadGame handler spits out session summaries.
                            break;
                        }

                        MakeGridItem(jump.Timestamp, (data.CommanderData.JumpsRemainingInRoute == 0 ? "(no route)" : ""));
                        string arrivalStarScoopableStr = (data.CommanderData.LastStartJumpEvent != null && !Constants.Scoopables.Contains(data.CommanderData.LastStartJumpEvent.StarClass))
                                ? $"Arrival star (type: {data.CommanderData.LastStartJumpEvent.StarClass}) is not scoopable!"
                                : "";
                        Core.SendNotification(new()
                        {
                            Title = "Route Progress",
                            Detail = $"{data.CommanderData.JumpsRemainingInRoute} jumps remaining{(!string.IsNullOrEmpty(data.CommanderData.Destination) ? $" to {data.CommanderData.Destination}" : "")}",
                            Sender = ShortName,
                            ExtendedDetails = arrivalStarScoopableStr,
                            Rendering = NotificationRendering.PluginNotifier,
                            CoalescingId = Constants.COALESCING_ID_POST_SYSTEM,
                        });
                        // This check is essential when travelling through areas where scans don't trigger (ie. known space).
                        if (data.CommanderData.FuelWarningNotifiedSystem != jump.StarSystem && data.CommanderData.FuelRemaining < (data.CommanderData.MaxFuelPerJump * Constants.FuelWarningLevelFactor))
                        {
                            Core.SendNotification(new()
                            {
                                Title = "Warning! Low fuel",
                                Detail = $"Refuel soon! {arrivalStarScoopableStr}",
                                Sender = ShortName,
                                CoalescingId = Constants.COALESCING_ID_POST_SYSTEM,
                            });
                            data.CommanderData.FuelWarningNotifiedSystem = jump.StarSystem;
                        }
                    }
                    break;
                case Docked docked:
                    if (!data.IsCommanderKnown) break;

                    if (docked.StationType == "FleetCarrier")
                    {
                        data.CommanderData.IsDockedOnCarrier = true;
                    }
                    break;
                case Undocked undocked:
                    if (!data.IsCommanderKnown) break;

                    data.CommanderData.IsDockedOnCarrier = false;
                    break;
                case FSDTarget target:
                    if (!data.IsCommanderKnown) break;

                    data.CommanderData.JumpsRemainingInRoute = target.RemainingJumpsInRoute;
                    break;
                case ReservoirReplenished replenished:
                    if (!data.IsCommanderKnown) break;

                    data.CommanderData.FuelRemaining = replenished.FuelMain;
                    break;
                case RefuelAll refuel:
                    if (!data.IsCommanderKnown) break;

                    if (refuel is RefuelPartial)
                    {
                        data.CommanderData.FuelRemaining += refuel.Amount;
                    }
                    else
                    {
                        data.CommanderData.FuelRemaining = data.CommanderData.FuelCapacity;
                    }
                    break;
                case Scan scan:
                    if (!data.IsCommanderKnown) break;

                    if (scan.ScanType != "NavBeaconDetail" && scan.PlanetClass != "Barycentre" && !scan.WasDiscovered && scan.DistanceFromArrivalLS == 0)
                    {
                        data.CommanderData.UndiscoveredSystem = true;
                    }
                    data.CommanderData.Scans[scan.BodyName] = scan;
                    if (data.CommanderData.FuelRemaining < (data.CommanderData.MaxFuelPerJump * Constants.FuelWarningLevelFactor) && data.CommanderData.FuelWarningNotifiedSystem != scan.StarSystem)
                    {
                        if (Constants.Scoopables.Contains(scan.StarType) && scan.DistanceFromArrivalLS < settings.MaxNearbyScoopableDistance)
                        {
                            string extendedDetails = $"Warning! Low Fuel! Scoopable star: {BodyShortName(scan.BodyName, data.CommanderData.CurrentSystem)}, {scan.DistanceFromArrivalLS:0.0} Ls";
                            MakeGridItem(scan.Timestamp, extendedDetails);
                            Core.SendNotification(new()
                            {
                                Title = "Warning! Low fuel",
                                Detail = $"There is a scoopable star in this system!",
                                ExtendedDetails = extendedDetails,
                                Sender = ShortName,
                                CoalescingId = scan.BodyID,
                            });
                            data.CommanderData.FuelWarningNotifiedSystem = data.CommanderData.CurrentSystem;
                        }
                    }

                    if (scan.StarType == "N" && scan.DistanceFromArrivalLS == 0) data.CommanderData.NeutronPrimarySystem = data.CommanderData.CurrentSystem;

                    if (data.CommanderData.NeutronPrimarySystem == null || data.CommanderData.NeutronPrimarySystem != data.CommanderData.CurrentSystem) break;

                    if ((Constants.Scoopables.Contains(scan.StarType) && scan.DistanceFromArrivalLS < settings.MaxNearbyScoopableDistance && data.CommanderData.ScoopableSecondaryCandidateScan?.StarSystem != data.CommanderData.CurrentSystem)
                        || (data.CommanderData.ScoopableSecondaryCandidateScan?.StarSystem == data.CommanderData.CurrentSystem && scan.DistanceFromArrivalLS < data.CommanderData.ScoopableSecondaryCandidateScan.DistanceFromArrivalLS))
                    {
                        data.CommanderData.ScoopableSecondaryCandidateScan = scan;
                    }

                    if (data.CommanderData.NeutronPrimarySystem == scan.StarSystem && data.CommanderData.ScoopableSecondaryCandidateScan?.StarSystem == data.CommanderData.CurrentSystem
                        && data.CommanderData.NeutronPrimarySystemNotified != data.CommanderData.CurrentSystem)
                    {
                        data.CommanderData.NeutronPrimarySystemNotified = data.CommanderData.CurrentSystem;
                        string extendedDetails = $"Nearby scoopable secondary star; Type: {data.CommanderData.ScoopableSecondaryCandidateScan?.StarType}, {Math.Round(data.CommanderData.ScoopableSecondaryCandidateScan?.DistanceFromArrivalLS ?? 0, 1)} Ls";
                        MakeGridItem(scan.Timestamp, extendedDetails);
                        Core.SendNotification(new()
                        {
                            Title = "Nearby scoopable secondary star",
                            Detail = $"Body {BodyShortName(scan.BodyName, data.CommanderData.CurrentSystem)}{Environment.NewLine}Type: {data.CommanderData.ScoopableSecondaryCandidateScan?.StarType}{Environment.NewLine}{Math.Round(data.CommanderData.ScoopableSecondaryCandidateScan?.DistanceFromArrivalLS ?? 0, 1)} Ls",
                            ExtendedDetails = extendedDetails,
                            Sender = ShortName,
                            CoalescingId = scan.BodyID,
                        });
                    }
                    break;
                case FSSAllBodiesFound allFound:
                    if (!data.IsCommanderKnown) break;

                    if (data.CommanderData.FuelWarningNotifiedSystem == allFound.SystemName) break;

                    if (data.CommanderData.FuelRemaining < (data.CommanderData.MaxFuelPerJump * Constants.FuelWarningLevelFactor))
                    {
                        string extendedDetails = $"Warning! Low Fuel and no scoopable star! Be sure to jump to a system with a scoopable star!";
                        MakeGridItem(allFound.Timestamp, extendedDetails);
                        Core.SendNotification(new()
                        {
                            Title = "Warning! Low fuel!",
                            Detail = "There is no scoopable star in this system!",
                            ExtendedDetails = extendedDetails,
                            Sender = ShortName,
                            CoalescingId = Constants.COALESCING_ID_POST_SYSTEM,
                        });
                    }
                    if (!data.CommanderData.AllBodiesFound)
                    {
                        Core.SendNotification(new()
                        {
                            Title = "FSS completed",
                            Detail = $"All {allFound.Count} bodies found",
                            Rendering = NotificationRendering.PluginNotifier,
                            Sender = ShortName,
                            CoalescingId = Constants.COALESCING_ID_POST_SYSTEM,
                        });
                    }
                    data.CommanderData.AllBodiesFound = true;
                    break;
                case ApproachBody approachBody:
                    if (!data.IsCommanderKnown) break;

                    Scan s;
                    if (!settings.EnableHighGravityAdvisory) break;

                    if (!data.CommanderData.Scans.TryGetValue(approachBody.Body, out s)) break;

                    string bodyShortName = BodyShortName(approachBody.Body, approachBody.StarSystem);
                    var gravityG = s.SurfaceGravity / Constants.CONV_MperS2_TO_G_DIVISOR;
                    if (gravityG > settings.GravityAdvisoryThreshold)
                    {
                        Core.SendNotification(new()
                        {
                            Title = $"Use caution when landing!",
                            Detail = $"Relatively high gravity: {gravityG:n1}g",
                            Sender = ShortName,
                            CoalescingId = approachBody.BodyID,
                        });
                    }
                    break;
            }
        }

        public void StatusChange(Status status)
        {
            // Update fuel level with latest, greatest data.
            if (status.Fuel != null) 
                data.CommanderData.FuelRemaining = status.Fuel.FuelMain;

            if (status.Flags2.HasFlag(StatusFlags2.FsdHyperdriveCharging) && settings.WarnIncompleteUndiscoveredSystemScan
                && !data.CommanderData.AllBodiesFound && data.CommanderData.UndiscoveredSystem && !incompleteSystemsNotified.Contains(data.CommanderData.CurrentSystem))
            {
                incompleteSystemsNotified.Add(data.CommanderData.CurrentSystem);
                Core.SendNotification(new()
                {
                    Title = "Incomplete system scan!",
                    Detail = $"The undiscovered system you are about to leave is not fully scanned!",
                    Sender = ShortName,
                    CoalescingId = Constants.COALESCING_ID_SYSTEM,
                });
            }
        }



        private static string BodyShortName(string bodyName, string systemName)
        {
            string shortName = bodyName.Replace(systemName, "").Trim();
            // TODO handle barycenters
            return (string.IsNullOrEmpty(shortName) ? "Primary star" : shortName);
        }

        private void MakeGridItem(string timestamp, string details = "")
        {
            if (Core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch)) return;

            HelmGrid gridItem = new()
            {
                Timestamp = timestamp,
                System = data.CommanderData.CurrentSystem ?? "",
                Fuel = (data.CommanderData.IsDockedOnCarrier ? "" : $"{(100 * data.CommanderData.FuelRemaining / data.CommanderData.FuelCapacity):0}% ({Math.Round(data.CommanderData.FuelRemaining, 2)} T)"),
                DistanceTravelled = $"{data.CommanderData.DistanceTravelled:n1} ly ({data.CommanderData.JumpsCompleted} jumps" + (data.CommanderData.DockedCarrierJumpsCompleted > 0 ? $", {data.CommanderData.DockedCarrierJumpsCompleted} jumps on a carrier)" : ")"),
                JumpsRemaining = data.CommanderData.IsDockedOnCarrier ? "" : 
                        (data.CommanderData.JumpsRemainingInRoute > 0 ? $"{data.CommanderData.JumpsRemainingInRoute}{(!string.IsNullOrEmpty(data.CommanderData.Destination) ? $" en route to {data.CommanderData.Destination}" : "")}" : ""),
                Details = $"{details}",
            };
            Core.AddGridItem(this, gridItem);
        }

        private void MaybeMakeGridItemForSummary(string timestamp)
        {
            if ((data.CommanderData?.JumpsCompleted ?? 0) == 0)
            {
                // Do nothing.
                return;
            }

            HelmGrid gridItem = new()
            {
                Timestamp = timestamp,
                System = data.CommanderData.CurrentSystem ?? "",
                Fuel = "",
                DistanceTravelled = $"{data.CommanderData.DistanceTravelled:n1} ly ({data.CommanderData.JumpsCompleted} jumps" + (data.CommanderData.DockedCarrierJumpsCompleted > 0 ? $", {data.CommanderData.DockedCarrierJumpsCompleted} jumps on a carrier)" : ")"),
                JumpsRemaining = data.CommanderData.IsDockedOnCarrier ? "" : (data.CommanderData.JumpsRemainingInRoute > 0 ? $"{data.CommanderData.JumpsRemainingInRoute}" : ""),
                Details = $"Previous session summary for {data.CommanderData.Commander}",
            };

            Core.AddGridItem(this, gridItem);
        }

        private void MaybeRestoreDestinations(bool forCommanderChange = false)
        {
            if (Core.IsLogMonitorBatchReading) return;

            if (!hasLoadedDestinations && data.RestoreRouteDestinations(Core.PluginStorageFolder)
                && data.IsCommanderKnown && !string.IsNullOrWhiteSpace(data.CommanderData.Destination))
            {
                hasLoadedDestinations = true;
            }

            MaybeSetDestinationInClipboard(forCommanderChange);
        }

        private void MaybeSetDestinationInClipboard(bool forCommanderChange)
        {
            if (Core.IsLogMonitorBatchReading) return;
            if (!string.IsNullOrWhiteSpace(data.CommanderData?.Destination))
            {
                // We have a known destination; Set the current destination into the clipboard.
                Core.ExecuteOnUIThread(delegate() {
                    Clipboard.SetText(data.CommanderData.Destination);
                });

                var detailStr = $"Last destination system for Cmdr {data.CurrentCommander} inserted into clipboard for re-plot";
                if (forCommanderChange)
                {
                    detailStr = $"Switched to Cmdr {data.CurrentCommander}; last destination system inserted into clipboard for re-plot";
                }
                HelmGrid gridItem = new()
                {
                    Timestamp = DateTime.UtcNow.ToString(),
                    System = data.CommanderData.Destination,
                    Fuel = "",
                    DistanceTravelled = "",
                    JumpsRemaining = "",
                    Details = detailStr,
                };
                Core.AddGridItem(this, gridItem);

                Core.SendNotification(new()
                {
                    Title = "Ready to re-plot route",
                    Detail = "Destination is in the system clipboard",
                    Sender = ShortName,
                    ExtendedDetails = $"Destination: {data.CommanderData.Destination}",
                    Rendering = NotificationRendering.PluginNotifier,
                    CoalescingId = Constants.COALESCING_ID_HEADER,
                });
            }
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
            [ColumnSuggestedWidth(300)]
            public string Timestamp { get; set; }
            [ColumnSuggestedWidth(350)]
            public string System { get; set; }
            [ColumnSuggestedWidth(150)]
            public string Fuel { get; set; }
            [ColumnSuggestedWidth(250)]
            public string DistanceTravelled { get; set; }
            [ColumnSuggestedWidth(250)]
            public string JumpsRemaining { get; set; }
            [ColumnSuggestedWidth(500)]
            public string Details { get; set; }
        }
    }
}
