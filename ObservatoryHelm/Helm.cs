using System.Diagnostics;
using com.github.fredjk_gh.ObservatoryHelm.Data;
using com.github.fredjk_gh.ObservatoryHelm.UI;
using com.github.fredjk_gh.PluginCommon.AutoUpdates;
using com.github.fredjk_gh.PluginCommon.Data;
using com.github.fredjk_gh.PluginCommon.Data.Id64;
using com.github.fredjk_gh.PluginCommon.Data.Journals;
using com.github.fredjk_gh.PluginCommon.PluginInterop.Messages;
using com.github.fredjk_gh.PluginCommon.Utilities;
using Observatory.Framework;
using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;
using Observatory.Framework.Interfaces;

namespace com.github.fredjk_gh.ObservatoryHelm
{
    public class Helm : IObservatoryWorker
    {
        private static readonly Guid PLUGIN_GUID = new("38425041-6ff2-4b9d-a10c-420839972f79");

        private static readonly AboutLink GH_LINK = new("github", "https://github.com/fredjk-gh/ObservatoryPlugins");
        private static readonly AboutLink GH_RELEASE_NOTES_LINK = new("github release notes", "https://github.com/fredjk-gh/ObservatoryPlugins/wiki/Plugin:-Helm");
        private static readonly AboutLink DOC_LINK = new("Documentation", "https://observatory.xjph.net/usage/plugins/thirdparty/fredjk-gh/helm");
        private static readonly AboutInfo ABOUT_INFO = new()
        {
            FullName = "Helm",
            ShortName = "Helm",
            Description = @"Provides additional insights about your flight environment, route and some travel statistics.

May use data from Spansh, EDGIS and/or EDAstro.",
            AuthorName = "fredjk-gh",
            Links =
            [
                GH_LINK,
                GH_RELEASE_NOTES_LINK,
                DOC_LINK,
            ]
        };

        private PluginUI pluginUI;
        private HelmSettings _settings = new();
        private HelmContext _c;
        private readonly HashSet<string> incompleteSystemsNotified = [];
        private bool hasLoadedDestinations = false;

        public static Guid Guid => PLUGIN_GUID;
        public AboutInfo AboutInfo => ABOUT_INFO;
        public string Version => typeof(Helm).Assembly.GetName().Version.ToString();
        public PluginUI PluginUI => pluginUI;

        public object Settings
        {
            get => _c?.Settings ?? _settings;
            set
            {
                _settings = (HelmSettings)value;
                if (_c != null) _c.Settings = _settings;
            }
        }

        public void Load(IObservatoryCore observatoryCore)
        {
            _c = HelmContext.Initialize(observatoryCore, this, _settings);
            _c.Dispatcher.RegisterHandler<HelmStatusMessage>(HandleHelmStatusMessage);
            _c.Dispatcher.RegisterHandler<ArchivistJournalsMessage>(HandleArchivistScansMessage);

            _c.Panel = new HelmPanel(_c);
            pluginUI = new PluginUI(PluginUI.UIType.Panel, _c.Panel);
        }

        public PluginUpdateInfo CheckForPluginUpdate()
        {
            AutoUpdateHelper.Init(_c.Core);
            return AutoUpdateHelper.CheckForPluginUpdate(
                this, GH_RELEASE_NOTES_LINK.Url, _c.Settings.EnableAutoUpdates, _c.Settings.EnableBetaUpdates);
        }

        public void ObservatoryReady()
        {
            _c.Core.ExecuteOnUIThread(() =>
            {
                _c.UI.MaybeAdjustFontSize();
            });

            _c.Dispatcher.SendMessage(GenericPluginReadyMessage.New());

            // Also populates commanders.
            ReadCacheFile();
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args)
        {
            // * -> ReadAll
            if (args.NewState.HasFlag(LogMonitorState.Batch))
            {
                _c.Clear();
            }
            // ReadAll -> 
            else if (args.PreviousState.HasFlag(LogMonitorState.Batch))
            {
                // Exiting read-all. Repaint things.
                if (args.NewState.HasFlag(LogMonitorState.Realtime))
                {
                    MaybeSetDestinationInClipboard();
                    _c.UI.ChangeCommander(_c.Data.CurrentCommander);
                    _c.UI.Draw();

                    UpdateCache();
                }
            }
            // Exiting Preread
            else if (args.PreviousState.HasFlag(LogMonitorState.PreRead)
                && !args.NewState.HasFlag(LogMonitorState.PreRead))
            {
                MaybeSetDestinationInClipboard();
                _c.UI.ChangeCommander(_c.Data.CurrentCommander);
                _c.UI.Draw();

                UpdateCache();
            }
        }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            switch (journal)
            {
                case LoadGame loadGame:
                    bool commanderChanged = _c.Data.HasCurrentCommander
                        && loadGame.FID != _c.Data.CurrentCommander.FID;

                    _c.Data.SessionReset(loadGame);
                    _c.Data.CommanderData.LastActive = loadGame.TimestampDateTime;

                    if (!_c.Core.IsLogMonitorBatchReading && commanderChanged)
                    {
                        MaybeSetDestinationInClipboard(commanderChanged);
                        _c.UI.ChangeCommander(_c.Data.CurrentCommander);

                        UpdateCache(); // ensure current commander is updated.
                    }
                    break;
                case Loadout loadOut:
                    if (!_c.Data.HasCurrentCommander) break;
                    _c.Data.CommanderData.Ships.UpdateLoadout(loadOut);
                    _c.UI.ChangeShip(loadOut.ShipID);
                    _c.Data.CommanderData.LastActive = loadOut.TimestampDateTime;

                    UpdateCache();
                    break;
                case StoredShips storedShips:
                    if (!_c.Data.HasCurrentCommander) break;
                    _c.Data.CommanderData.Ships.UpdateStoredShips(storedShips);
                    _c.Data.CommanderData.LastActive = storedShips.TimestampDateTime;

                    UpdateCache();
                    break;
                case ShipyardSwap:
                    // Do we care? We should get a loadout momentarily.
                    break;
                case FuelScoop fuelScoop:
                    if (!_c.Data.HasCurrentCommander) break;
                    _c.Data.CommanderData.Ships.CurrentShip.FuelRemaining += fuelScoop.Scooped;
                    _c.UI.ChangeFuelLevel(_c.Data.CommanderData.Ships.CurrentShip.FuelRemaining);

                    UpdateCache();
                    break;
                case JetConeBoost jetConeBoost:
                    _c.Data.CommanderData.Ships.CurrentShip.JetConeBoostFactor = jetConeBoost.BoostValue;
                    _c.UI.ChangeJetConeBoost(jetConeBoost.BoostValue);
                    if (_c.Settings.NotifyJetConeBoost)
                    {
                        _c.SendNotification(new()
                        {
                            Title = "FSD is Supercharged",
                            ExtendedDetails = $"Boost factor: {jetConeBoost.BoostValue}",
                            Sender = AboutInfo.ShortName,
                            CoalescingId = Constants.COALESCING_ID_POST_SYSTEM,
                        });
                    }

                    UpdateCache();
                    break;
                case NavRouteFile navRoute:
                    if (!_c.Data.HasCurrentCommander) break;

                    _c.Data.CommanderData.LastNavRoute = navRoute;
                    _c.Data.CommanderData.LastActive = navRoute.TimestampDateTime;
                    _c.SendNotification(new()
                    {
                        Title = "New route",
                        Detail = "",
                        Sender = AboutInfo.ShortName,
                        ExtendedDetails = $"{_c.Data.CommanderData.JumpsRemainingInRoute} jumps to {_c.Data.CommanderData.Destination}",
                        Rendering = NotificationRendering.PluginNotifier,
                        CoalescingId = Constants.COALESCING_ID_POST_SYSTEM,
                    });

                    UpdateCache();
                    _c.UI.ChangeRoute(navRoute);
                    break;
                case NavRouteClear:
                    if (!_c.Data.HasCurrentCommander) break;

                    _c.Data.CommanderData.JumpsRemainingInRoute = 0;
                    _c.Data.CommanderData.Destination = "";

                    UpdateCache();
                    _c.UI.ChangeRoute(null);
                    break;
                case SupercruiseEntry:
                    if (!_c.Data.HasCurrentCommander) break;

                    // Move to _c.UI.Change* method?
                    _c.UIMgr.Realtime.ProspectedEvents.Clear();

                    break;
                case StartJump startJump:
                    if (!_c.Data.HasCurrentCommander) break;
                    _c.Data.CommanderData.LastActive = startJump.TimestampDateTime;

                    if (startJump.JumpType == "Hyperspace")
                    {
                        _c.Data.CommanderData.LastStartJumpEvent = startJump;
                    }
                    break;
                case Location location:
                    if (!_c.Data.HasCurrentCommander) break;

                    _c.Data.SystemReset(location.SystemAddress, location.StarSystem, location.StarPos);

                    _c.UI.ChangeSystem(location.SystemAddress);

                    UpdateCache();
                    break;
                case FSDJump jump:
                    if (!_c.Data.HasCurrentCommander) break;

                    incompleteSystemsNotified.Clear();
                    if (jump is CarrierJump carrierJump && (
                        carrierJump.Docked || carrierJump.OnFoot))
                    {
                        // Do nothing else. We're missing fuel level, etc. here.
                        if (_c.Data.CommanderData.LastJumpEvent != null)
                        {
                            double distance = Id64CoordHelper.Distance(carrierJump.StarPos, _c.Data.CommanderData.LastJumpEvent.StarPos);

                            //double distance = Id64CoordHelper.Distance((long) carrierJump.SystemAddress, (long)data.CommanderData.LastJumpEvent.SystemAddress);
                            _c.Data.SystemResetDockedOnCarrier(carrierJump.SystemAddress, carrierJump.StarSystem, carrierJump.StarPos, distance);
                        }
                        else
                        {
                            _c.Data.SystemResetDockedOnCarrier(carrierJump.SystemAddress, carrierJump.StarSystem, carrierJump.StarPos);
                        }
                        _c.Data.CommanderData.LastJumpEvent = jump;
                        _c.Data.CommanderData.LastStartJumpEvent = null;
                        _c.UI.ChangeSystem(jump.SystemAddress);
                        // Move to _c.UI.Change* method?
                        _c.UIMgr.Realtime.ProspectedEvents.Clear();
                        UpdateCache();
                    }
                    else
                    {
                        _c.Data.SystemReset(jump.SystemAddress, jump.StarSystem, jump.StarPos, jump.FuelLevel, jump.JumpDist);
                        _c.Data.CommanderData.LastJumpEvent = jump;

                        if ((_c.Data.CommanderData.Ships.CurrentShip?.JetConeBoostFactor ?? 1) > 1)
                        {
                            _c.Data.CommanderData.Ships.CurrentShip.JetConeBoostFactor = 1.0;
                            _c.UI.ChangeJetConeBoost(1);
                        }

                        UpdateCache();
                        if (_c.Core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch) || _c.UIMgr.ReplayMode)
                        {
                            // Don't output anything further during read-all or replay mode due to archivist message.
                            break;
                        }

                        // This is noisy and now represented in the UI. Consider removing here altogether.
                        //string suppressionZoneDetail = (_c.Data.CommanderData.CurrentSystemData.IsInSuppressionZone ?? false
                        //    ? _c.Data.CommanderData.CurrentSystemData.SuppressionZoneDetails
                        //    : string.Empty);
                        //if (_c.Data.CommanderData.CurrentSystemData.IsInBubble ?? false)
                        //{
                        //    suppressionZoneDetail = $"This system is in the bubble (< {_settings.BubbleRadiusLy} Ly from Sol).";
                        //}
                        _c.UI.ChangeSystem(jump.SystemAddress);
                        _c.UI.ChangeFuelLevel(jump.FuelLevel);
                        string arrivalStarScoopableStr = (_c.Data.CommanderData.LastStartJumpEvent != null && !JournalConstants.Scoopables.Contains(_c.Data.CommanderData.LastStartJumpEvent.StarClass))
                                ? $"Arrival star (type: {_c.Data.CommanderData.LastStartJumpEvent.StarClass}) is not scoopable!"
                                : "";

                        _c.SendNotification(new()
                        {
                            Title = "Route Progress",
                            Detail = $"{_c.Data.CommanderData.JumpsRemainingInRoute} jumps remaining{(!string.IsNullOrEmpty(_c.Data.CommanderData.Destination) ? $" to {_c.Data.CommanderData.Destination}" : "")}",
                            Sender = AboutInfo.ShortName,
                            //ExtendedDetails = $"{arrivalStarScoopableStr} {suppressionZoneDetail}",
                            ExtendedDetails = arrivalStarScoopableStr,
                            Rendering = NotificationRendering.PluginNotifier,
                            CoalescingId = Constants.COALESCING_ID_POST_SYSTEM,
                        });
                        // This check is essential when travelling through areas where scans don't trigger (ie. known space).
                        // TODO: Figure out why this occasionally misfires.
                        if (_c.Data.CommanderData.FuelWarningNotifiedSystem != jump.StarSystem
                            && _c.Data.CommanderData.Ships.CurrentShip.FuelRemaining < (_c.Data.CommanderData.Ships.CurrentShip.MaxFuelPerJump * Constants.FuelWarningLevelFactor))
                        {
                            Debug.WriteLine($"Helm: Low Fuel details: Ship: {_c.Data.CommanderData.Ships.CurrentShip.ShipName}; Fuel remaining: {_c.Data.CommanderData.Ships.CurrentShip.FuelRemaining}; Per jump: {_c.Data.CommanderData.Ships.CurrentShip.MaxFuelPerJump}");
                            _c.SendNotification(new()
                            {
                                Title = "Warning! Low fuel",
                                Detail = $"Refuel soon! {arrivalStarScoopableStr}",
                                Sender = AboutInfo.ShortName,
                                CoalescingId = Constants.COALESCING_ID_POST_SYSTEM,
                            });
                            _c.Data.CommanderData.FuelWarningNotifiedSystem = jump.StarSystem;
                        }
                    }
                    break;
                case Docked docked:
                    if (!_c.Data.HasCurrentCommander) break;

                    if (docked.StationType == "FleetCarrier")
                    {
                        _c.Data.CommanderData.IsDockedOnCarrier = true;
                    }
                    _c.Data.CommanderData.LastActive = docked.TimestampDateTime;
                    break;
                case Undocked undocked:
                    if (!_c.Data.HasCurrentCommander) break;

                    _c.Data.CommanderData.IsDockedOnCarrier = false;
                    _c.Data.CommanderData.LastActive = undocked.TimestampDateTime;
                    break;
                case DockingCancelled cancelled: // And DockingDenied.
                    if (cancelled is DockingDenied denied && !_c.Core.IsLogMonitorBatchReading)
                    {
                        _c.AddMessage($"Docking Denied due to: {denied.Reason}");
                    }
                    break;
                case FSDTarget target:
                    if (!_c.Data.HasCurrentCommander) break;

                    _c.Data.CommanderData.JumpsRemainingInRoute = target.RemainingJumpsInRoute;
                    if (_c.Data.CommanderData.DerivedRoute == null && !string.IsNullOrEmpty(_c.Data.CommanderData.Destination))
                    {
                        _c.Data.CommanderData.DerivedRoute = new(
                            _c.Data.CommanderData.CurrentSystemData,
                            _c.Data.CommanderData.Destination,
                            target.RemainingJumpsInRoute);
                        // Use _c.UI.Change* method?
                        _c.UIMgr.Realtime.JumpsRemaining = target.RemainingJumpsInRoute;
                    }
                    break;
                case ReservoirReplenished replenished:
                    if (!_c.Data.HasCurrentCommander) break;

                    _c.Data.CommanderData.Ships.CurrentShip.FuelRemaining = replenished.FuelMain;
                    _c.UI.ChangeFuelLevel(_c.Data.CommanderData.Ships.CurrentShip.FuelRemaining);

                    UpdateCache();
                    break;
                case RefuelAll refuel:
                    if (!_c.Data.HasCurrentCommander) break;

                    if (refuel is RefuelPartial)
                    {
                        _c.Data.CommanderData.Ships.CurrentShip.FuelRemaining += refuel.Amount;
                    }
                    else
                    {
                        _c.Data.CommanderData.Ships.CurrentShip.FuelRemaining = _c.Data.CommanderData.Ships.CurrentShip.FuelCapacity;
                    }
                    _c.UI.ChangeFuelLevel(_c.Data.CommanderData.Ships.CurrentShip.FuelRemaining);

                    UpdateCache();
                    break;
                case Scan scan:
                    OnScan(scan);

                    UpdateCache();
                    break;
                case FSSDiscoveryScan honk:
                    if (!_c.Data.HasCurrentCommander) break;
                    if (_c.Data.CommanderData.CurrentSystemData.SystemId64 == honk.SystemAddress)
                        _c.Data.CommanderData.CurrentSystemData.Honk = honk;

                    UpdateCache();
                    break;
                case FSSAllBodiesFound allFound:
                    if (!_c.Data.HasCurrentCommander) break;

                    if (_c.Data.CommanderData.FuelWarningNotifiedSystem == allFound.SystemName) break;

                    if (!_c.UIMgr.ReplayMode && _c.Data.CommanderData.Ships.CurrentShip.FuelRemaining < (_c.Data.CommanderData.Ships.CurrentShip.MaxFuelPerJump * Constants.FuelWarningLevelFactor))
                    {
                        string extendedDetails = $"Warning! Low Fuel and no scoopable star! Be sure to jump to a system with a scoopable star!";
                        _c.SendNotification(new()
                        {
                            Title = "Warning! Low fuel!",
                            Detail = "There is no scoopable star in this system!",
                            ExtendedDetails = extendedDetails,
                            Sender = AboutInfo.ShortName,
                            CoalescingId = Constants.COALESCING_ID_POST_SYSTEM,
                        });
                    }
                    if (!_c.Data.CommanderData.AllBodiesFound && !_c.UIMgr.ReplayMode)
                    {
                        // Two distinct notifications. One has only a title and is vocal only. The other has additional detail and
                        // is plugin only. Alternatively: One notification, but put "All N bodies found" in Extended details which
                        // is never spoken anyway?
                        //
                        // Make Vocal notification an option?
                        _c.SendNotification(new()
                        {
                            Title = "FSS complete",
                            Rendering = NotificationRendering.NativeVocal,
                            Sender = AboutInfo.ShortName,
                            CoalescingId = Constants.COALESCING_ID_POST_SYSTEM,
                        });
                        _c.SendNotification(new()
                        {
                            Title = "FSS complete",
                            Detail = $"All {allFound.Count} bodies found",
                            Rendering = NotificationRendering.PluginNotifier,
                            Sender = AboutInfo.ShortName,
                            CoalescingId = Constants.COALESCING_ID_POST_SYSTEM,
                        });
                    }
                    _c.Data.CommanderData.AllBodiesFound = true;
                    _c.Data.CommanderData.CurrentSystemData.AllBodiesFound = allFound;
                    _c.UI.ChangeAllBodiesFound(true);

                    if (_c.Data.CommanderData.LastStatus?.Destination?.Body > 0
                        && _c.Data.CommanderData.LastStatus?.Destination?.System == _c.UIMgr.Realtime.SystemId64
                        && _c.Data.CommanderData.LastStatus?.Destination?.Body != _c.UIMgr.Realtime.BodyId)
                    {
                        _c.UI.ChangeBody(_c.Data.CommanderData.LastStatus.Destination.Body);
                    }

                    UpdateCache();
                    break;
                case ApproachBody approachBody:
                    if (!_c.Data.HasCurrentCommander) break;

                    PlanetData p;
                    if (!_c.Settings.EnableHighGravityAdvisory) break;

                    if (!_c.Data.CommanderData.CurrentSystemData.Planets.TryGetValue(approachBody.BodyID, out p)) break;
                    var gravityG = p.Scan.SurfaceGravity / Constants.CONV_MperS2_TO_G_DIVISOR;
                    if (gravityG > _c.Settings.GravityAdvisoryThreshold)
                    {
                        _c.SendNotification(new()
                        {
                            Title = $"Use caution when landing!",
                            Detail = $"Relatively high gravity: {gravityG:n1}g",
                            Sender = AboutInfo.ShortName,
                            CoalescingId = approachBody.BodyID,
                            Rendering = NotificationRendering.NativeVocal | NotificationRendering.NativeVisual,
                        });
                    }
                    _c.UI.ChangeBody(approachBody.BodyID);
                    UpdateCache();
                    break;
                case SAAScanComplete saaScan:
                    if (!_c.Data.HasCurrentCommander) break;

                    _c.Data.CommanderData.CurrentSystemData.AddScan(saaScan);
                    UpdateCache();
                    break;
                case SAASignalsFound signals:
                    if (!_c.Data.HasCurrentCommander) break;

                    _c.Data.CommanderData.CurrentSystemData.AddSignals(signals);
                    if (_c.Data.CommanderData.CurrentSystemData.Planets.ContainsKey(signals.BodyID))
                        _c.UI.ChangeBody(signals.BodyID);
                    UpdateCache();
                    break;
                case Statistics stats:
                    if (!_c.Data.HasCurrentCommander) break;
                    _c.Data.CommanderData.LastStatistics = stats;
                    _c.Data.CommanderData.LastActive = stats.TimestampDateTime;
                    UpdateCache();
                    break;
                case Cargo cargo:
                    if (!_c.Data.HasCurrentCommander) break;

                    _c.Data.CommanderData.Ships.UpdateCargo(cargo);
                    _c.UI.ChangeCargo(cargo.Count);
                    UpdateCache();
                    break;
                case CargoFile cargo:
                    if (!_c.Data.HasCurrentCommander) break;

                    _c.Data.CommanderData.Ships.UpdateCargo(cargo);
                    _c.UI.ChangeCargo(cargo.Count);
                    _c.Data.CommanderData.LastActive = cargo.TimestampDateTime;
                    UpdateCache();
                    break;
                case ProspectedAsteroid prospect:
                    if (!_c.Data.HasCurrentCommander) break;
                    // Move to _c.UI.Change* method?
                    _c.UIMgr.Realtime.ProspectedEvents.Add(prospect);
                    _c.Data.CommanderData.LastActive = prospect.TimestampDateTime;
                    break;
            }
        }

        private void OnScan(Scan scan)
        {
            if (!_c.Data.HasCurrentCommander) return;

            var cmdrData = _c.Data.CommanderData;
            var sysData = cmdrData.CurrentSystemData;

            if (scan.ScanType != "NavBeaconDetail"
                && scan.PlanetClass != "Barycentre"
                && !scan.WasDiscovered && scan.DistanceFromArrivalLS == 0)
            {
                cmdrData.UndiscoveredSystem = true;
            }

            var scanAlreadySeen = sysData.ContainsScan(scan);
            var usefulScan = sysData.AddScan(scan);
            // When mapping, a second scan fires; don't re-target the UI.
            if (!scanAlreadySeen && usefulScan) _c.UI.ChangeBody(scan.BodyID);
            if (cmdrData.Ships.CurrentShip.FuelRemaining < (cmdrData.Ships.CurrentShip.MaxFuelPerJump * Constants.FuelWarningLevelFactor)
                && cmdrData.FuelWarningNotifiedSystem != scan.StarSystem)
            {
                if (JournalConstants.Scoopables.Contains(scan.StarType) && scan.DistanceFromArrivalLS < _c.Settings.MaxNearbyScoopableDistance)
                {
                    string bodyDisplayName = SharedLogic.GetBodyDisplayName(SharedLogic.GetBodyShortName(scan.BodyName, cmdrData.CurrentSystemName));
                    string extendedDetails = $"Warning! Low Fuel! Scoopable star: {bodyDisplayName}, {scan.DistanceFromArrivalLS:0.0} Ls";
                    _c.SendNotification(new()
                    {
                        Title = "Warning! Low fuel",
                        Detail = $"There is a scoopable star in this system!",
                        ExtendedDetails = extendedDetails,
                        Sender = AboutInfo.ShortName,
                        CoalescingId = Constants.COALESCING_ID_HEADER,
                    });
                    cmdrData.FuelWarningNotifiedSystem = cmdrData.CurrentSystemName;
                }
            }

            if (scan.StarType == "N" && scan.DistanceFromArrivalLS == 0)
                cmdrData.NeutronPrimarySystem = cmdrData.CurrentSystemName;

            if (cmdrData.NeutronPrimarySystem == null
                || cmdrData.NeutronPrimarySystem != cmdrData.CurrentSystemName)
                return;

            if ((JournalConstants.Scoopables.Contains(scan.StarType) && scan.DistanceFromArrivalLS < _c.Settings.MaxNearbyScoopableDistance
                && cmdrData.ScoopableSecondaryCandidateScan?.StarSystem != cmdrData.CurrentSystemName)
                || (cmdrData.ScoopableSecondaryCandidateScan?.StarSystem == cmdrData.CurrentSystemName
                && scan.DistanceFromArrivalLS < cmdrData.ScoopableSecondaryCandidateScan.DistanceFromArrivalLS))
            {
                cmdrData.ScoopableSecondaryCandidateScan = scan;
            }

            if (cmdrData.NeutronPrimarySystem == scan.StarSystem
                && cmdrData.ScoopableSecondaryCandidateScan?.StarSystem == cmdrData.CurrentSystemName
                && cmdrData.NeutronPrimarySystemNotified != cmdrData.CurrentSystemName)
            {
                cmdrData.NeutronPrimarySystemNotified = cmdrData.CurrentSystemName;
                string extendedDetails = $"Nearby scoopable secondary star; Type: {cmdrData.ScoopableSecondaryCandidateScan?.StarType}, {Math.Round(cmdrData.ScoopableSecondaryCandidateScan?.DistanceFromArrivalLS ?? 0, 1)} Ls";
                _c.SendNotification(new()
                {
                    Title = SharedLogic.GetBodyDisplayName(SharedLogic.GetBodyShortName(scan.BodyName, cmdrData.CurrentSystemName)),
                    Detail = $"Nearby scoopable secondary star",
                    ExtendedDetails = extendedDetails,
                    Sender = AboutInfo.ShortName,
                    CoalescingId = scan.BodyID,
                });
            }
        }

        public void StatusChange(Status status)
        {
            // Update fuel level with latest, greatest data.
            if (status.Fuel != null)
            {
                var fuelChanged = (_c.Data.CommanderData.Ships.CurrentShip.FuelRemaining != status.Fuel.FuelMain);
                _c.Data.CommanderData.Ships.CurrentShip.FuelRemaining = status.Fuel.FuelMain;
                if (fuelChanged)
                {
                    _c.UI.ChangeFuelLevel(status.Fuel.FuelMain);
                }
            }

            if (status.Flags2.HasFlag(StatusFlags2.FsdHyperdriveCharging) && _c.Settings.WarnIncompleteUndiscoveredSystemScan
                && !_c.Data.CommanderData.AllBodiesFound && _c.Data.CommanderData.UndiscoveredSystem
                && !incompleteSystemsNotified.Contains(_c.Data.CommanderData.CurrentSystemName))
            {
                incompleteSystemsNotified.Add(_c.Data.CommanderData.CurrentSystemName);
                _c.SendNotification(new()
                {
                    Title = "Incomplete system scan!",
                    Detail = $"The undiscovered system you are about to leave is not fully scanned!",
                    Sender = AboutInfo.ShortName,
                    CoalescingId = Constants.COALESCING_ID_SYSTEM,
                    Rendering = NotificationRendering.NativeVisual | NotificationRendering.NativeVocal,
                });
            }

            if (_c.Data.CommanderData.LastStatus != null
                && status.Destination?.System == _c.UIMgr.Realtime.SystemId64
                && status.Destination?.Body >= 0
                && status.Destination?.Body != _c.Data.CommanderData.LastStatus.Destination?.Body
                && status.Destination?.Body != _c.UIMgr.Realtime.BodyId)
            {
                _c.UI.ChangeBody(status.Destination.Body);
            }

            if (status.Flags.HasFlag(StatusFlags.LatLongValid))
            {
                // Move to _c.UI.Change* method?
                _c.UIMgr.Realtime.SurfacePosition = SurfacePosition.FromStatus(status);
            }
            else
            {
                // Move to _c.UI.Change* method?
                _c.UIMgr.Realtime.SurfacePosition = null;
            }
            _c.Data.CommanderData.LastStatus = status;
        }

        public void HandlePluginMessage(MessageSender sender, PluginMessage messageArgs)
        {
            _c.Dispatcher.MaybeHandlePluginMessage(sender, messageArgs, out _);
        }

        private void HandleHelmStatusMessage(HelmStatusMessage statusMessage)
        {
            _c.AddMessage(statusMessage.Message, statusMessage.TimestampUtc, statusMessage.Sender.FullName);
        }

        private void ReadCacheFile()
        {
            if (_c.Core.IsLogMonitorBatchReading) return;

            if (!hasLoadedDestinations && _c.Data.RestoreFromCache())
            {
                hasLoadedDestinations = true;
            }

            if (_c.Data.CurrentCommander is null) return;

            // Rehydrate some UI state.
            // Move to _c.UI.Change* method(s)?
            _c.UIMgr.Realtime.SwitchCommander(_c.Data.CurrentCommander);
        }

        private bool _cacheWriteQueued = false;
        private void UpdateCache()
        {
            if (_c.Core.IsLogMonitorBatchReading) return;

            // Throttle writes by queuing a task to save in a few seconds so if there's a burst of
            // changes, we don't slow things down or write super often.
            if (!_cacheWriteQueued)
            {
                _cacheWriteQueued = true;
                try
                {
                    Task.Run(() =>
                    {
                        Task.Delay(3000);
                        try
                        {
                            _c.Data.SaveToCache();
                        }
                        finally
                        {
                            _cacheWriteQueued = false;
                        }
                    });
                }
                catch (Exception ex)
                {
                    _cacheWriteQueued = false;
                    Debug.Fail($"Helm: Failed to write cache file: {ex.Message}");
                }
            }
        }

        private void MaybeSetDestinationInClipboard(bool forCommanderChange = false)
        {
            if (_c.Core.IsLogMonitorBatchReading) return;
            if (!string.IsNullOrWhiteSpace(_c.Data.CommanderData?.Destination))
            {
                // We have a known destination; Set the current destination into the clipboard.
                _c.Core.ExecuteOnUIThread(delegate ()
                {
                    Misc.SetTextToClipboard(_c.Data.CommanderData.Destination);
                });

                var detailStr = $"Last destination system ({_c.Data.CommanderData.Destination}) for Cmdr {_c.Data.CurrentCommander.Name} inserted into clipboard for re-plot";
                if (forCommanderChange)
                {
                    detailStr = $"Switched to Cmdr {_c.Data.CurrentCommander.Name}; last destination system ({_c.Data.CommanderData.Destination}) inserted into clipboard for re-plot";
                }

                _c.AddMessage(detailStr);
                _c.SendNotification(new()
                {
                    Title = "Ready to re-plot route",
                    Detail = "Destination is in the system clipboard",
                    Sender = AboutInfo.ShortName,
                    ExtendedDetails = $"Destination: {_c.Data.CommanderData.Destination}",
                    Rendering = NotificationRendering.PluginNotifier,
                    CoalescingId = Constants.COALESCING_ID_HEADER,
                });
            }
        }

        private void HandleArchivistScansMessage(ArchivistJournalsMessage m)
        {
            _c.UIMgr.ReplayMode = true;

            try
            {
                foreach (var entry in m.SystemJournalEntries)
                {
                    JournalEvent(entry);
                }
            }
            finally
            {
                _c.UIMgr.ReplayMode = false;
                _c.AddMessage($"Got system data from Archivist for {m.SystemName}");
                _c.UI.Draw();
            }
        }
    }
}
