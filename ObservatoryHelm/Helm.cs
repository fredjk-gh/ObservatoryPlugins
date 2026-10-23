using System.Diagnostics;
using com.github.fredjk_gh.ObservatoryHelm.Data;
using com.github.fredjk_gh.ObservatoryHelm.UI;
using com.github.fredjk_gh.PluginCommon;
using com.github.fredjk_gh.PluginCommon.AutoUpdates;
using com.github.fredjk_gh.PluginCommon.Data;
using com.github.fredjk_gh.PluginCommon.Data.Id64;
using com.github.fredjk_gh.PluginCommon.Data.Journals;
using com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs;
using com.github.fredjk_gh.PluginCommon.Data.Prospecting;
using com.github.fredjk_gh.PluginCommon.PluginInterop.Messages;
using com.github.fredjk_gh.PluginCommon.UI;
using com.github.fredjk_gh.PluginCommon.Utilities;
using Observatory.Framework;
using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.Journal.Other;
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
            Description = @"Provides additional insights about your flight environment, route and some travel statistics plus prospecting information.

May use data from Spansh, EDGIS and/or EDAstro. Thanks to MattG for sharing some ProcGen Sector naming code which is used by this plugin.",
            AuthorName = "fredjk-gh",
            Links =
            [
                GH_LINK,
                GH_RELEASE_NOTES_LINK,
                DOC_LINK,
            ]
        };

        private const string VESSEL_SHIP = "Ship";
        private const string VESSEL_SRV = "SRV";


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
                case Commander cmdr:
                    OnNewCommander(cmdr.Name, cmdr.FID, cmdr.TimestampDateTime);
                    break;
                case LoadGame loadGame:
                    OnNewCommander(loadGame.Commander, loadGame.FID, loadGame.TimestampDateTime, loadGame);
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
                            CoalescingId = CoalescingIDs.DEFAULT_COALESCING_ID,
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
                        CoalescingId = CoalescingIDs.DEFAULT_COALESCING_ID,
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
                    if (location.SystemAddress == 0) break; // Really old journals do not have SystemAddress. :'( Ignore them.
                    if (!_c.Data.HasCurrentCommander) break;

                    _c.Data.SystemReset(location.SystemAddress, location.StarSystem, location.StarPos, TransitMode.FromJournal(location).ToEnum());

                    _c.UI.ChangeSystem(location.SystemAddress);

                    UpdateCache();
                    break;
                case FSDJump jump:
                    if (jump.SystemAddress == 0) break; // Really old journals do not have SystemAddress. :'( Ignore them.
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
                            CoalescingId = CoalescingIDs.DEFAULT_COALESCING_ID,
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
                                CoalescingId = CoalescingIDs.DEFAULT_COALESCING_ID,
                            });
                            _c.Data.CommanderData.FuelWarningNotifiedSystem = jump.StarSystem;
                        }
                    }
                    break;
                case Docked docked:
                    if (!_c.Data.HasCurrentCommander) break;

                    var commanderData = _c.Data.CommanderData;

                    commanderData.ActiveTransitMode = TransitMode.FromJournal(docked).ToEnum();
                    commanderData.IsDockedOnCarrier = _c.Data.CommanderData.ActiveTransitMode == TransitModeType.FleetCarrier;
                    commanderData.LastActive = docked.TimestampDateTime;
                    
                    break;
                case Undocked undocked:
                    if (!_c.Data.HasCurrentCommander) break;

                    _c.Data.CommanderData.ActiveTransitMode = TransitMode.FromJournal(undocked).ToEnum();
                    _c.Data.CommanderData.IsDockedOnCarrier = _c.Data.CommanderData.ActiveTransitMode == TransitModeType.FleetCarrier;
                    _c.Data.CommanderData.LastActive = undocked.TimestampDateTime;

                    if (_settings.LimpetReminderNotifications)
                    {
                        int limpetsCount = 0;
                        _c.Data.CommanderData?.Ships.Cargo.TryGetValue(CargoHelper.LimpetDronesKey, out limpetsCount);
                        var loadout = _c.Data.CommanderData?.Ships.CurrentShip?.Loadout;

                        if (loadout is not null)
                        {
                            var miningModuleCount = Constants.MiningModuleCount(loadout);

                            if (miningModuleCount >= Math.Max(1, _settings.LimpetNotificationMinMiningModules) && limpetsCount == 0)
                            {
                                _c.SendNotification(new()
                                {
                                    Sender = AboutInfo.ShortName,
                                    CoalescingId = CoalescingIDs.ALERT_COALESCING_ID,
                                    Title = "Don't forget limpets!",
                                    Detail = "You have mining modules equipped but no limpets in your cargo hold.",
                                    ExtendedDetails = $"Found {miningModuleCount} mining modules equipped.",
                                    Rendering = NotificationRendering.All,
                                });
                            }
                        }
                    }
                    break;
                case DockingCancelled cancelled: // And DockingDenied.
                    if (cancelled is DockingDenied denied && !_c.Core.IsLogMonitorBatchReading)
                        _c.AddMessage($"Docking Denied due to: {denied.Reason}");
                    break;
                case LaunchVessel launchVessel:
                    if (!_c.Data.HasCurrentCommander) break;

                    _c.Data.CommanderData.SRV.ActiveSRVType = SRVs.GetSRVDetails(launchVessel.VesselType)?.Type;
                    _c.Data.CommanderData.ActiveTransitMode = TransitModeType.ShipLaunchedVessel;
                    _c.Data.CommanderData.LastActive = launchVessel.TimestampDateTime;

                    _c.UI.ChangeTransitMode(TransitModeType.ShipLaunchedVessel);
                    break;
                case LaunchSRV launchSrv:
                    if (!_c.Data.HasCurrentCommander) break;

                    _c.Data.CommanderData.SRV.ActiveSRVType = SRVs.GetSRVDetails(launchSrv.SRVType)?.Type;
                    _c.Data.CommanderData.ActiveTransitMode = TransitModeType.ShipLaunchedVessel;
                    _c.Data.CommanderData.LastActive = launchSrv.TimestampDateTime;

                    _c.UI.ChangeTransitMode(TransitModeType.ShipLaunchedVessel);
                    break;
                case DockSRV dockSrv:
                    if (!_c.Data.HasCurrentCommander) break;

                    _c.Data.CommanderData.ActiveTransitMode = TransitModeType.OwnShip;
                    _c.Data.CommanderData.SRV.ActiveSRVType = null;
                    _c.Data.CommanderData.SRV.ClearCargo(); // Cargo is always transferred to the ship.
                    _c.Data.CommanderData.LastActive = dockSrv.TimestampDateTime;

                    _c.UI.ChangeTransitMode(TransitModeType.OwnShip);
                    break;
                case SRVDestroyed srvGoBoom:
                    if (!_c.Data.HasCurrentCommander) break;

                    // Let the next Location event determine new TransitMode.
                    _c.Data.CommanderData.SRV.ActiveSRVType = null;
                    _c.Data.CommanderData.SRV.ClearCargo(); // Cargo is always lost.
                    _c.Data.CommanderData.LastActive = srvGoBoom.TimestampDateTime;
                    break;
                case Disembark embarkOrDisembark:
                    _c.Data.CommanderData.ActiveTransitMode = TransitMode.FromJournal(embarkOrDisembark)
                        .WithPrevious(_c.Data.CommanderData.ActiveTransitMode)
                        .ToEnum();
                    _c.Data.CommanderData.LastActive = embarkOrDisembark.TimestampDateTime;

                    _c.UI.ChangeTransitMode(_c.Data.CommanderData.ActiveTransitMode);
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
                    if (scan.SystemAddress == 0) break; // Really old journals do not have SystemAddress. :'( Ignore them
                    OnScan(scan);
                    UpdateCache();
                    break;
                case FSSDiscoveryScan honk:
                    if (!_c.Data.HasCurrentCommander) break;
                    if (_c.Data.CommanderData.CurrentSystemData.SystemId64 == honk.SystemAddress)
                        _c.Data.CommanderData.CurrentSystemData.Honk = honk;

                    _c.UI.ChangeSystemBodyCount(honk.BodyCount);

                    UpdateCache();
                    break;
                case FSSAllBodiesFound allFound:
                    // By checking .IsFullyDiscovered here we ensure the system is fully scanned, regardless of false AllBodiesFound events.
                    // Only proceed if we don't actually know the body count-- sometimes the AllBodiesFound fires before the honk so we don't know the body count yet, but this provides it.
                    // Not much we can do in this situation but proceed -- but it's typically the right thing to do. This often happens in systems with a single star, no other bodies
                    // resulting in an inaccurate system panel.
                    // Alternatively, I could also handle the 
                    if (!_c.Data.HasCurrentCommander ||
                        ((_c.Data.CommanderData.CurrentSystemData?.BodyCount > 0) && !(_c.Data.CommanderData.CurrentSystemData?.IsFullyDiscovered ?? false)))
                        break;

                    if (_c.Data.CommanderData.FuelWarningNotifiedSystem != allFound.SystemName
                        && !_c.UIMgr.ReplayMode
                        && _c.Data.CommanderData.Ships.CurrentShip.FuelRemaining < (_c.Data.CommanderData.Ships.CurrentShip.MaxFuelPerJump * Constants.FuelWarningLevelFactor))
                    {
                        string extendedDetails = $"Warning! Low Fuel and no scoopable star! Be sure to jump to a system with a scoopable star!";
                        _c.SendNotification(new()
                        {
                            Title = "Warning! Low fuel!",
                            Detail = "There is no scoopable star in this system!",
                            ExtendedDetails = extendedDetails,
                            Sender = AboutInfo.ShortName,
                            CoalescingId = CoalescingIDs.DEFAULT_COALESCING_ID,
                        });
                    }
                    if (_c.Data.CommanderData.CurrentSystemData?.AllBodiesFound is null
                        && !_c.UIMgr.ReplayMode)
                    {
                        _c.Data.CommanderData.CurrentSystemData.AllBodiesFound = allFound;

                        // Now that we've set AllBodiesFound, double check IsFullyDiscovered, and proceed IFF that's true.
                        if (_c.Data.CommanderData.CurrentSystemData?.IsFullyDiscovered ?? false)
                        {
                            _c.UI.ChangeAllBodiesFound(true);

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
                                CoalescingId = CoalescingIDs.DEFAULT_COALESCING_ID,
                            });
                            _c.SendNotification(new()
                            {
                                Title = "FSS complete",
                                Detail = $"All {allFound.Count} bodies found",
                                Rendering = NotificationRendering.PluginNotifier,
                                Sender = AboutInfo.ShortName,
                                CoalescingId = CoalescingIDs.DEFAULT_COALESCING_ID,
                            });

                            if (_c.Data.CommanderData.LastStatus?.Destination?.Body > 0
                                && _c.Data.CommanderData.LastStatus?.Destination?.System == _c.UIMgr.Realtime.SystemId64
                                && _c.Data.CommanderData.LastStatus?.Destination?.Body != _c.UIMgr.Realtime.BodyId)
                            {
                                _c.UI.ChangeBody(_c.Data.CommanderData.LastStatus.Destination.Body);
                            }
                        }
                    }

                    UpdateCache();
                    break;
                case ApproachBody approachBody:
                    if (!_c.Data.HasCurrentCommander) break;

                    PlanetData p;
                    if (!_c.Settings.EnableHighGravityAdvisory) break;

                    if (!_c.Data.CommanderData.CurrentSystemData.Planets.TryGetValue(approachBody.BodyID, out p)) break;
                    var gravityG = p.Scan.SurfaceGravity / Conversions.CONV_MperS2_TO_G_DIVISOR;
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
                case SAASignalsFound signals: // And FSSBodySignals.
                    if (!_c.Data.HasCurrentCommander) break;

                    var sysData = _c.Data.CommanderData.CurrentSystemData;
                    if (sysData.AddSignals(signals))
                    {
                        if (sysData.Planets.ContainsKey(signals.BodyID))
                            _c.UI.ChangeBody(signals.BodyID);

                        OnRingPing(sysData, signals);
                        UpdateCache();
                    }
                    break;
                case Statistics stats:
                    if (!_c.Data.HasCurrentCommander) break;
                    _c.Data.CommanderData.LastStatistics = stats;
                    _c.Data.CommanderData.LastActive = stats.TimestampDateTime;
                    UpdateCache();
                    break;
                case Cargo cargo:
                    if (!_c.Data.HasCurrentCommander) break;
                    // Only use the startup version that has full inventory. If inventory count is also zero, then inventory is irrelevant.
                    if (cargo.Count > 0 && cargo.Inventory?.Count == 0) break;

                    if (cargo.Vessel == VESSEL_SHIP)
                    {
                        _c.Data.CommanderData.Ships.UpdateCargo(cargo);
                        _c.UI.ChangeShipCargo(cargo.Count);
                    }
                    else
                    {
                        _c.Data.CommanderData.SRV.UpdateCargo(cargo);
                        _c.UI.ChangeSRVCargo(cargo.Count);
                    }
                    UpdateCache();
                    break;
                case CargoFile cargo:
                    if (!_c.Data.HasCurrentCommander) break;

                    if (cargo.Vessel == VESSEL_SHIP)
                    {
                        _c.Data.CommanderData.ActiveTransitMode = TransitModeType.OwnShip;
                        _c.Data.CommanderData.Ships.UpdateCargo(cargo);
                        _c.UI.ChangeShipCargo(cargo.Count);
                    }
                    else
                    {
                        _c.Data.CommanderData.ActiveTransitMode = TransitModeType.ShipLaunchedVessel;
                        _c.Data.CommanderData.SRV.UpdateCargo(cargo);
                        _c.UI.ChangeSRVCargo(cargo.Count);
                    }
                    _c.Data.CommanderData.LastActive = cargo.TimestampDateTime;
                    UpdateCache();
                    break;
                case CargoTransfer xfer:
                    if (!_c.Data.HasCurrentCommander) break;

                    _c.Data.CommanderData.LastActive = xfer.TimestampDateTime;

                    foreach (var t in xfer.Transfers)
                    {
                        if (_c.Data.CommanderData.SRV.ActiveSRVType is not null)
                        {
                            // SRV -> Ship  OR Ship -> SRV.
                            // Update Ship inventory until we transfer back to the ship and CargoFile events fire for the ship again.
                            int newCount = _c.Data.CommanderData.Ships.UpdateCargo(t);
                            _c.UI.ChangeShipCargo(newCount);
                        }
                    }
                    break;
                case ProspectedAsteroid prospect:
                    if (!_c.Data.HasCurrentCommander) break;
                    _c.Data.CommanderData.LastActive = prospect.TimestampDateTime;

                    OnProspectedAsteroid(prospect);
                    break;
            }
        }

        private void OnNewCommander(string commanderName, string fid, DateTime lastActive, LoadGame loadGame = null)
        {
            bool commanderChanged = _c.Data.HasCurrentCommander
                && commanderName != _c.Data.CurrentCommander.Name;

            _c.Data.SessionReset(commanderName, fid, loadGame);
            _c.UIMgr.Realtime.ProspectedEvents.Clear();
            _c.Data.CommanderData.LastActive = lastActive;

            if (!_c.Core.IsLogMonitorBatchReading && commanderChanged)
            {
                MaybeSetDestinationInClipboard(commanderChanged);
                _c.UI.ChangeCommander(_c.Data.CurrentCommander);

                UpdateCache(); // ensure current commander is updated.
            }
            return;
        }

        private void OnScan(Scan scan)
        {
            if (!_c.Data.HasCurrentCommander) return;

            var cmdrData = _c.Data.CommanderData;
            var sysData = cmdrData.CurrentSystemData;
            var scanAlreadySeen = sysData.ContainsScan(scan);
            var usefulScan = sysData.AddScan(scan);

            if (!usefulScan) return;

            // When mapping, a second scan fires; don't re-target the UI.
            if (!scanAlreadySeen && usefulScan) _c.UI.ChangeBody(scan.BodyID);

            OnScanForNeutronHopping(scan, cmdrData, sysData);
            
            if (!scanAlreadySeen)
            {
                OnScanForRings(scan, sysData);
                if (sysData.Planets.TryGetValue(scan.BodyID, out PlanetData bodyData))
                    OnScanForBody(sysData, bodyData);
            }
        }

        private void OnScanForNeutronHopping(Scan scan, CommanderData cmdrData, SystemData sysData)
        {
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
                        CoalescingId = CoalescingIDs.SYSTEM_COALESCING_ID,
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

        private void OnScanForRings(Scan scan, SystemData sysData)
        {
            if (scan.Rings == null || scan.Rings.IsEmpty || _c.Data.AlreadyReportedScansSaaSignals.Contains(scan.BodyName) || !_settings.MentionPotentiallyMineableRings || _settings.MentionableRings == RingType.None)
                return;

            _c.Data.AlreadyReportedScansSaaSignals.Add(scan.BodyName);
            List<RingDetails> ringsOfInterest = [];
            RingType mentionableRings = _settings.MentionableRings;
            foreach (Ring ring in scan.Rings)
            {
                // Ignore belts.
                if (!ring.Name.Contains("Ring")) continue;

                // Find rings of interest
                foreach (RingType rt in Enum.GetValues(typeof(RingType)))
                {
                    if (rt == RingType.None) continue;

                    if (mentionableRings.HasFlag(rt) && ring.RingClass.Contains(rt.MatchString()))
                    {
                        double densityMTperkm3 = ring.MassMT / ((Math.PI * Math.Pow(ring.OuterRad / 1000, 2)) - (Math.PI * Math.Pow(ring.InnerRad / 1000, 2.0)));
                        if (densityMTperkm3 < _settings.MinimumDensity)
                        {
                            Debug.WriteLine(_c.Dlogger.EnableLogging, $"Scan: Ignoring interesting ring with low density: {UIFormatter.Density(densityMTperkm3)}");
                            break;
                        }
                        string desiredCommodities = string.Join(", ", _settings.DesireableCommoditiesByRingType(rt).Select(c => c.ToString()));
                        var details = new RingDetails()
                        {
                            ShortName = SharedLogic.GetBodyShortName(ring.Name, scan.BodyName),
                            RingType = rt.DisplayString(),
                            Commodities = $"[{desiredCommodities}]",
                            Density = $"Density: {UIFormatter.Density(densityMTperkm3)}",
                        };
                        if (!ringsOfInterest.Contains(details)) ringsOfInterest.Add(details);
                        break;
                    }
                }
            }
            if (ringsOfInterest.Count == 0) return;

            var shortBodyName = SharedLogic.GetBodyShortName(scan.BodyName, sysData.SystemName);
            var detailsCommaSeparated = string.Join(", ", ringsOfInterest.Select(t => t.ToString()));
            var bodyDistance = $"distance: {Math.Floor(scan.DistanceFromArrivalLS)} Ls";
            Debug.WriteLineIf(_c.Dlogger.EnableLogging, $"Scan: Interesting rings at body {shortBodyName}: {detailsCommaSeparated}, {bodyDistance}");

            _c.SendNotification(new NotificationArgs()
            {
                Title = SharedLogic.GetBodyDisplayName(shortBodyName),
                Detail = string.Join(", ", ringsOfInterest.Select(t => $"{t.RingType} Ring")),
                ExtendedDetails = $"{detailsCommaSeparated}", // Don't include distance here -- it's not consistent over multiple visits and produces dupe notifs in Archivist.
                Sender = AboutInfo.ShortName,
                CoalescingId = scan.BodyID,
            });
        }

        private void OnScanForBody(SystemData sysData, PlanetData details)
        {
            if (details.BodySignals is null || details.Scan is null || !_settings.MentionPotentiallyMineableBodies || _settings.MentionableBodies == BodyType.None)
                return; // Insufficient information.

            var planetarySignalCount = details.SignalCountByType(PlanetData.SignalType.PlanetaryMining);
            if (planetarySignalCount == 0)
                return;

            BodyType bodyTypes = _settings.MentionableBodies;

            foreach (BodyType bt in Enum.GetValues(typeof(BodyType)))
            {
                if (bt == BodyType.None) continue;

                if (_settings.MentionableBodies.HasFlag(bt) && details.Scan.PlanetClass.Contains(bt.MatchString()))
                {
                    var shortBodyName = SharedLogic.GetBodyShortName(details.Scan.BodyName, sysData.SystemName);
                    var bodyDistance = $"distance: {Math.Floor(details.Scan.DistanceFromArrivalLS)} Ls";
                    string desiredCommodities = string.Join(", ", _settings.DesireableCommoditiesByBodyType(bt).Select(c => c.ToString()));
                    string detailString = $"{planetarySignalCount} locations; [{desiredCommodities}]";

                    _c.SendNotification(new NotificationArgs()
                    {
                        Title = SharedLogic.GetBodyDisplayName(shortBodyName),
                        Detail = $"{bt.DisplayString()} body with planetary mining locations",
                        ExtendedDetails = $"{detailString}", // Don't include distance here -- it's not consistent over multiple visits and produces dupe notifications in Archivist.
                        Sender = AboutInfo.ShortName,
                        CoalescingId = details.BodyId,
                    });
                    break;
                }
            }
        }

        private void OnProspectedAsteroid(ProspectedAsteroid prospected)
        {
            if (prospected.Remaining < 100)
            {
                _c.SendNotification(MakeProspectorNotificationArgs("Depleted", "", NotificationRendering.NativeVisual));
                _c.UIMgr.Realtime.ProspectedEvents.Add(new()
                {
                    Event = prospected,
                });

                return;
            }

            ProspectorDetails d = new()
            {
                Event = prospected
            };
            bool suppressFurtherNotifications = false;

            // Separate grid entry, but combine it with other notifications.
            string highMaterialContent = "";
            if (_settings.MentionHighMaterialContent && prospected.Content == JournalConstants.MATERIAL_CONTENT_HIGH)
            {
                highMaterialContent = " and has high material content";
            }

            if (Enum.TryParse<Commodities>(prospected.MotherlodeMaterial, true, out Commodities ml) && _settings.GetFor(ml))
            {
                // Found a core of interest!
                string name = CargoHelper.CommodityName(prospected.MotherlodeMaterial, prospected.MotherlodeMaterial_Localised);
                string coreDetails = $"Asteroid contains core of {name}{highMaterialContent}";

                NotificationArgs args = MakeProspectorNotificationArgs( "Core found", coreDetails);
                _c.SendNotification(args);
                Debug.WriteLineIf(_c.Dlogger.EnableLogging, $"\t--Grid Update: {name}; Core found");
                suppressFurtherNotifications = true;

                d.CoreImage = new(Images.PlanetaryMiningSpotImage)
                {
                    Size = new(30, 30),
                    ToolTip = coreDetails,
                    Visible = true,
                };
            }

            Dictionary<string, float> desireableCommodities = [];
            float desireableCommoditiesPercentSum = 0.0f;
            foreach (var m in prospected.Materials)
            {
                if (!Enum.TryParse<Commodities>(m.Name, true, out Commodities c))
                {
                    continue;
                }

                if (_settings.GetFor(c))
                {
                    string name = CargoHelper.CommodityName(m.Name, m.Name_Localised);
                    desireableCommodities.Add(name, m.Proportion);
                    desireableCommoditiesPercentSum += m.Proportion;
                }
            }

            if (desireableCommodities.Count > 0 && desireableCommoditiesPercentSum > _settings.MinimumPercent)
            {
                string commodities = String.Join(", ", desireableCommodities.Keys);
                string percentageString = UIFormatter.Percent(desireableCommoditiesPercentSum, 0);
                string title;
                string details;

                if (desireableCommodities.Count > 1)
                {
                    // Ooh, multiple things here!
                    title = "Good multiple commodity rock";
                    details = $"Asteroid is a combined {UIFormatter.Percent(desireableCommoditiesPercentSum, 0)} {commodities}{highMaterialContent}";
                }
                else
                {
                    title = "Good rock";
                    details = $"Asteroid is {UIFormatter.Percent(desireableCommoditiesPercentSum, 0)} {commodities}{highMaterialContent}";
                }

                if (!suppressFurtherNotifications)
                {
                    _c.SendNotification(MakeProspectorNotificationArgs(title, details));
                    suppressFurtherNotifications = true;
                }
                d.CommoditiesImage = new(Images.PlanetaryMiningSpotImage)
                {
                    Size = new(30, 30),
                    ToolTip = details,
                    Visible = true,
                };
                Debug.WriteLineIf(_c.Dlogger.EnableLogging, $"\t--Grid Update: {commodities}; {percentageString}");
            }

            // If we got here, we didn't find anything interesting.
            if (!suppressFurtherNotifications && !string.IsNullOrEmpty(highMaterialContent))
            {
                string details = "";
                if (desireableCommodities.Count > 0)
                {
                    details = $"Asteroid also contains {UIFormatter.Percent(desireableCommoditiesPercentSum, 0)} of other desireable commodities";
                }
                _c.SendNotification(
                    MakeProspectorNotificationArgs("High raw material content", details));
            }

            _c.UIMgr.Realtime.ProspectedEvents.Add(d);
        }

        private void OnRingPing(SystemData sysData, SAASignalsFound saaSignalsFound)
        {
            if (String.IsNullOrEmpty(sysData.SystemName)) return;
            var ringName = SharedLogic.GetBodyShortName(saaSignalsFound.BodyName, sysData.SystemName);
            if (saaSignalsFound.Signals == null || saaSignalsFound.Signals.Count == 0 || _c.Data.AlreadyReportedScansSaaSignals.Contains(ringName)
                    || !ringName.Contains(" Ring", StringComparison.InvariantCultureIgnoreCase))
                return;

            _c.Data.AlreadyReportedScansSaaSignals.Add(ringName);
            Dictionary<string, int> desireableCommodities = [];
            List<string> notificationDetail = [];

            foreach (var m in saaSignalsFound.Signals)
            {
                if (!Enum.TryParse<Commodities>(m.Type, true, out Commodities c))
                {
                    continue;
                }

                if (_settings.GetFor(c))
                {
                    string name = CargoHelper.CommodityName(m.Type, m.Type);
                    desireableCommodities.Add(name, m.Count);

                    notificationDetail.Add($"{m.Count} {name}");
                }
            }

            if (notificationDetail.Count > 0)
            {
                Debug.WriteLineIf(_c.Dlogger.EnableLogging, $"SAASignalsFound: Hotspots of interest: {ringName} contains: {string.Join(", ", notificationDetail)}");

                if (_c.Core.IsLogMonitorBatchReading) return;

                _c.SendNotification(new NotificationArgs
                {
                    Title = "Hotspots of interest",
                    Detail = $"{ringName} contains:{Environment.NewLine}{string.Join(Environment.NewLine, notificationDetail)}",
                    Sender = AboutInfo.ShortName,
                    CoalescingId = saaSignalsFound.BodyID,
                });
            }
        }

        private NotificationArgs MakeProspectorNotificationArgs(string title, string detail, NotificationRendering rendering = NotificationRendering.NativeVocal | NotificationRendering.NativeVisual)
        {
            NotificationArgs args = new()
            {
                Title = title,
                Detail = $"[{DateTime.UtcNow:h:mm:ss}] {detail}",
                DetailSsml = $"<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en - US\"><voice name=\"\">{detail}</voice></speak>",
                Rendering = rendering,
                Sender = AboutInfo.ShortName,
                CoalescingId = CoalescingIDs.DEFAULT_COALESCING_ID,
            };
            return args;
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
                && !(_c.Data.CommanderData.CurrentSystemData?.IsFullyDiscovered ?? false)
                && (_c.Data.CommanderData.CurrentSystemData?.IsFirstDiscovery ?? false)
                && !incompleteSystemsNotified.Contains(_c.Data.CommanderData.CurrentSystemName))
            {
                incompleteSystemsNotified.Add(_c.Data.CommanderData.CurrentSystemName);
                _c.SendNotification(new()
                {
                    Title = "Incomplete system scan!",
                    Detail = $"The undiscovered system you are about to leave is not fully scanned!",
                    Sender = AboutInfo.ShortName,
                    CoalescingId = CoalescingIDs.SYSTEM_COALESCING_ID,
                    Rendering = NotificationRendering.NativeVisual | NotificationRendering.NativeVocal,
                });
            }

            if (_c.Data.CommanderData.LastStatus is not null
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
                    Task.Run(async () =>
                    {
                        await Task.Delay(3000);
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
                    CoalescingId = CoalescingIDs.DEFAULT_COALESCING_ID,
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
