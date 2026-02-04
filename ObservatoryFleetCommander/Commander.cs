using com.github.fredjk_gh.ObservatoryFleetCommander.Data;
using com.github.fredjk_gh.ObservatoryFleetCommander.UI;
using com.github.fredjk_gh.PluginCommon.AutoUpdates;
using com.github.fredjk_gh.PluginCommon.PluginInterop.Messages;
using com.github.fredjk_gh.PluginCommon.Utilities;
using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;
using Observatory.Framework.Interfaces;
using Observatory.Framework.ParameterTypes;
using System.Diagnostics;
using System.Text;
using System.Timers;
using static com.github.fredjk_gh.PluginCommon.PluginInterop.PluginTracker;

namespace com.github.fredjk_gh.ObservatoryFleetCommander
{
    public class Commander : IObservatoryWorker
    {
        private readonly static Guid PLUGIN_GUID = new("95dcc3c8-e52f-47c1-ac16-4f548a54e030");
        private readonly static AboutLink GH_LINK = new("github", "https://github.com/fredjk-gh/ObservatoryPlugins");
        private readonly static AboutLink GH_RELEASE_NOTES_LINK = new("github release notes", "https://github.com/fredjk-gh/ObservatoryPlugins/wiki/Plugin:-Fleet-Commander");
        private readonly static AboutLink DOC_LINK = new("Documentation", "https://observatory.xjph.net/usage/plugins/thirdparty/fredjk-gh/fleetcommander");
        private readonly static AboutInfo ABOUT_INFO = new()
        {
            FullName = "Fleet Commander",
            ShortName = "Commander",
            Description = @"Fleet Commander is an essential assistant for Carrier owners.

May use data from Spansh, EDGIS and/or EDAstro.",
            AuthorName = "fredjk-gh",
            Links =
            [
                GH_LINK,
                GH_RELEASE_NOTES_LINK,
                DOC_LINK,
            ]
        };

        private readonly CommanderContext _c = new();
        private PluginUI pluginUI;
        private Location _initialLocation = null;
        private bool _initialRouteStuffingDone = false;

        #region Worker/Plugin Interface 
        public static Guid Guid => PLUGIN_GUID;
        public AboutInfo AboutInfo => ABOUT_INFO;
        public string Version => typeof(Commander).Assembly.GetName().Version.ToString();
        public PluginUI PluginUI => pluginUI;

        public object Settings
        {
            get => _c.Settings;
            set => _c.Settings = (FleetCommanderSettings)value;
        }

        public void Load(IObservatoryCore observatoryCore)
        {
            _c.Initialize(observatoryCore, this, new Manager());
            _c.UI = new CommanderUI(_c);
            pluginUI = new PluginUI(PluginUI.UIType.Panel, _c.UI);
            _c.UI.Init();
        }

        public PluginUpdateInfo CheckForPluginUpdate()
        {
            AutoUpdateHelper.Init(_c.Core);
            return AutoUpdateHelper.CheckForPluginUpdate(
                this, GH_RELEASE_NOTES_LINK.Url, _c.Settings.EnableAutoUpdates, _c.Settings.EnableBetaUpdates);
        }

        public void ObservatoryReady()
        {
            var readyMsg = GenericPluginReadyMessage.New();
            _c.Dispatcher.SendMessage(readyMsg);
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args)
        {
            // * -> ReadAll
            if (args.NewState.HasFlag(LogMonitorState.Batch))
            {
                _c.Manager.Clear();
                _c.UI.Clear();
            }
            // ReadAll -> *
            else if (args.PreviousState.HasFlag(LogMonitorState.Batch))
            {
                _c.SerializeDataCacheV2();

                _c.DoUIAction(() =>
                {
                    _c.UI.Repaint();
                });
            }

            // Exit PreRead
            else if (args.PreviousState.HasFlag(LogMonitorState.PreRead) && !args.NewState.HasFlag(LogMonitorState.PreRead))
            {
                foreach (var carrierData in _c.Manager.Carriers)
                {
                    // We still have a jump request. And jump time is in the past. Assume the jump happened while game was closed to advance the current location.
                    if (carrierData?.LastCarrierJumpRequest != null && carrierData.DepartureTimeMinutes < 0)
                    {
                        MaybeUpdateCarrierLocation(
                            carrierData.CarrierId,
                            new(carrierData.LastCarrierJumpRequest),
                            /* notifyIfchanged= */ false,
                            /* carrierMoveEvent= */ true);
                        carrierData.ClearJumpState();
                    }
                    // This plugin prioritizes stuffing the current commander's own carrier's next jump vs. either a non-deterministic approach or a squadron carrier.
                    if (carrierData?.Owner == _c.CurrentCommander)
                        MaybeStuffInitialJumpInClipboard(carrierData);
                }
            }
        }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            switch (journal)
            {
                case LoadGame loadGame:
                    OnLoadGame(loadGame);
                    break;
                case SquadronCreated squadronCreated: // Also handles SquadronStartup and JoinedSquadron
                    _c.Manager.RegisterSquadron(squadronCreated.SquadronID, squadronCreated.SquadronName, _c.CurrentCommander);
                    _c.SerializeDataCacheV2();
                    break;
                case Docked docked:
                    OnDocked(docked);
                    break;
                case Undocked:
                    OnUndocked();
                    break;
                // TODO: Skip this?
                case Location location:
                    OnLocation(location);
                    break;
                case CarrierLocation carrierLocation:
                    OnCarrierLocation(carrierLocation);
                    break;
                case CarrierBuy buy:
                    OnCarrierBuy(buy);
                    break;
                case CarrierNameChange nameChange:
                    OnCarrierNameChange(nameChange);
                    break;
                case CarrierDecommission decomCarrier:
                    OnCarrierDecommission(decomCarrier);
                    break;
                case CarrierCancelDecommission canceldecom:
                    OnCarrierDecommissionCancel(canceldecom);
                    break;
                case CarrierJumpRequest carrierJumpRequest:
                    OnCarrierJumpRequest(carrierJumpRequest);
                    break;
                //case CarrierJump carrierJump: // These have been broken in the past. Thus we don't rely on this for notification or state.
                //    OnCarrierJump(carrierJump);
                //    break;
                case CarrierJumpCancelled carrierJumpCancelled:
                    OnCarrierJumpCancelled(carrierJumpCancelled);
                    break;
                case CarrierDepositFuel carrierDepositFuel:
                    OnCarrierFuelDeposit(carrierDepositFuel);
                    break;
                case CarrierStats carrierStats:
                    OnCarrierStats(carrierStats);
                    break;
                case Statistics stats:
                    OnStats(stats);
                    break;
                case CarrierDockingPermission docPerm:
                    OnCarrierDockingPermission(docPerm);
                    break;
                case CarrierTradeOrder tradeOrder:
                    OnCarrierTradeOrder(tradeOrder);
                    break;
                case CargoTransfer transfer:
                    OnCargoTransfer(transfer);
                    break;
                case LaunchSRV:
                    OnLaunchSRV();
                    break;
                case DockSRV:
                    OnDockSRV();
                    break;
                // What we can't track is ALL market sales and purchases. That's where Manual updates may be required.
                case MarketBuy buy:
                    OnMarketBuy(buy);
                    break;
                case MarketSell sell:
                    OnMarketSell(sell);
                    break;
                case FSSSignalDiscovered fssSignal:
                    OnFssSignal(fssSignal);
                    break;
            }
        }
        #endregion

        #region Journal event handlers
        private void OnFssSignal(FSSSignalDiscovered fssSignal)
        {
            if (!_c.PluginTracker.IsActive(PluginType.fredjk_Archivist))
                return; // can't do much wtihout the position cache for this.

            CarrierData carrierData = _c.Manager.Carriers
                .Where(c => c.MatchesSignalName(fssSignal.SignalName))
                .FirstOrDefault();

            if (carrierData is not null)
            {
                if (carrierData.Position.SystemAddress == fssSignal.SystemAddress) return; // This isn't new info.

                ArchivistPositionCacheSingleLookup req = ArchivistPositionCacheSingleLookup.New("", fssSignal.SystemAddress, true);

                void del(PluginMessageWrapper resp)
                {
                    ArchivistPositionCacheSingle posCacheResp = resp as ArchivistPositionCacheSingle;
                    if (posCacheResp is not null)
                    {
                        StarPosition starPos = new()
                        {
                            x = posCacheResp.Position.X,
                            y = posCacheResp.Position.Y,
                            z = posCacheResp.Position.Z
                        };
                        CarrierPositionData pos = new(posCacheResp.Position.SystemName, posCacheResp.Position.SystemId64, starPos);
                        MaybeUpdateCarrierLocation(carrierData.CarrierId, pos, false, false);
                    }
                }
                _c.Dispatcher.SendMessageAndAwaitResponse(req, (Action<PluginMessageWrapper>)del, PluginType.fredjk_Archivist);

            }
        }

        private void OnMarketSell(MarketSell sell)
        {
            // This could be for the owning commander or an alternate commander -- both of which we can track.
            // What we won't see here is sales by unknown commanders.
            if (!_c.Manager.IsIDKnown(sell.MarketID)) return; // Not a carrier we track.

            CarrierData carrierData = _c.Manager.GetById(sell.MarketID);
            var updated = carrierData.InventoryAdjust(sell.Type, sell.Count, sell.Type_Localised);

            _c.DoUIAction(() =>
            {
                CarrierUI ui = _c.UI.Get(sell.MarketID);
                ui?.InventoryForm?.OnInventoryChange(updated);
            });
            _c.SerializeDataCacheV2();
        }

        private void OnMarketBuy(MarketBuy buy)
        {
            // This could be for the owning commander or an alternate commander -- both of which we can track.
            // What we won't see here is sales by unknown commanders.
            if (!_c.Manager.IsIDKnown(buy.MarketID)) return; // Not a carrier we track.

            CarrierData carrierData = _c.Manager.GetById(buy.MarketID);
            var updated = carrierData.InventoryAdjust(buy.Type, -1 * buy.Count, buy.Type_Localised);

            _c.DoUIAction(() =>
            {
                CarrierUI ui = _c.UI.Get(buy.MarketID);
                ui?.InventoryForm?.OnInventoryChange(updated);
            });
            _c.SerializeDataCacheV2();
        }

        private void OnCarrierDecommissionCancel(CarrierCancelDecommission canceldecom)
        {
            //TODO: Handle carrier decommissions.
        }

        private void OnCarrierDecommission(CarrierDecommission decomCarrier)
        {
            //TODO: Handle carrier decommissions.
        }

        private void OnDockSRV()
        {
            CommanderData cmdrData = _c.ForCommander();
            if (cmdrData is null) return;

            cmdrData.IsSRVDeployed = false;
            _c.SerializeDataCacheV2();
        }

        private void OnLaunchSRV()
        {
            CommanderData cmdrData = _c.ForCommander();
            if (cmdrData is null) return;

            cmdrData.IsSRVDeployed = true;
            _c.SerializeDataCacheV2();
        }

        private void OnCarrierJump(CarrierJump carrierJump)
        {
            CommanderData commanderData = _c.ForCommander();
            CarrierData carrierData = null;
            ulong carrierID = carrierJump.MarketID;
            if (carrierID <= 0)
            {
                // FDev forgot to include station details for on-foot carrier jumps. This could be either the squadron carrier OR a personal carrier that the commander
                // is docked on.
                if (commanderData.HasCarrier
                    && commanderData.LastDockedStationId.HasValue
                    && commanderData.Carrier.CarrierId == commanderData.LastDockedStationId.Value
                    && (carrierJump.Docked || carrierJump.OnFoot))
                {
                    // Things look promising that they're docked on their own carrier.
                    carrierData = commanderData.Carrier;
                    carrierData.CommandersOnBoard.Add(commanderData.Name);
                }
                else
                {
                    // Check if See if we're on-board on the squadron carrier. However, with multiple commanders each being in potentially distinct squadrons, it
                    // will be very difficult to know which squadron carrier belongs to whom. Need to figure this bit out.
                    var squadronCarrier = _c.Manager.GetSquadronCarrier(commanderData.Name);
                    if (squadronCarrier is not null
                        && commanderData.LastDockedStationId.HasValue
                        && squadronCarrier.CarrierId == commanderData.LastDockedStationId.Value)
                    {
                        carrierData = squadronCarrier;
                        carrierData.CommandersOnBoard.Add(commanderData.Name);
                    }
                }
            }
            else
            {
                carrierData = _c.Manager.GetById(carrierID);
            }

            if (carrierData is null) return;
            MaybeUpdateCarrierLocation(carrierData.CarrierId, new(carrierJump), false, true);
        }

        private void OnCargoTransfer(CargoTransfer transfer)
        {
            CommanderData cmdrData = _c.ForCommander();
            Dictionary<ulong, List<InventoryItem>> _carrierIdsWithChanges = [];

            // We're not on the ship.
            if (cmdrData is null || cmdrData.IsSRVDeployed) return;

            // Care about toship (only when docked on a carrier!!) and tocarrier
            foreach (var t in transfer.Transfers)
            {
                if (t.Direction == CargoTransferDirection.ToSRV) continue; // don't care
                else if (t.Direction == CargoTransferDirection.ToShip || t.Direction == CargoTransferDirection.ToCarrier)
                {
                    if (t.Direction == CargoTransferDirection.ToShip && !cmdrData.LastDockedStationId.HasValue) continue; // SRV -> ship?
                    if (!cmdrData.LastDockedStationId.HasValue || !_c.Manager.IsIDKnown(cmdrData.LastDockedStationId.Value))
                    {
                        Debug.Fail("OnCargoTransfer: A CargoTransfer event for an unknown station or we lost track of where we are docked.");
                        return;
                    }

                    // We're docked on a carrier we know about. Presumably the commander's carrier.
                    Debug.Assert(cmdrData.LastDockedStationId.Value == cmdrData.Carrier?.CarrierId, $"OnCargoTransfer[{t.Direction}] currently docked station is not commander's carrier!?");

                    CarrierData carrierData = _c.Manager.GetById(cmdrData.LastDockedStationId.Value);
                    _carrierIdsWithChanges.TryAdd(cmdrData.LastDockedStationId.Value, []);

                    // Removing from Carrier; multiply quantity by -1.
                    int qtysign = (t.Direction == CargoTransferDirection.ToShip ? -1 : 1);
                    var ii = carrierData.InventoryAdjust(t.Type, qtysign * t.Count, t.Type_Localised);
                    _carrierIdsWithChanges[cmdrData.LastDockedStationId.Value].Add(ii);
                }
            }

            _c.DoUIAction(() =>
            {
                foreach (var change in _carrierIdsWithChanges)
                {
                    CarrierUI ui = _c.UI.Get(change.Key);
                    foreach (var ii in change.Value)
                    {
                        ui?.InventoryForm?.OnInventoryChange(ii);
                    }
                    ui?.Draw();
                }
            });
            _c.SerializeDataCacheV2();
        }

        private void OnCarrierTradeOrder(CarrierTradeOrder tradeOrder)
        {
            CarrierData carrierData = _c.Manager.GetById(tradeOrder.CarrierID);
            if (carrierData is null) return;

            carrierData.TradeOrderAdjust(tradeOrder);

            _c.DoUIAction(() =>
            {
                CarrierUI ui = _c.UI.Get(tradeOrder.CarrierID);
                ui?.InventoryForm?.OnTradeOrderChange();
                ui?.Draw(); // Update capacity info (reserved cargo capacity may be changed).
            });
            _c.SerializeDataCacheV2();
        }

        private void OnCarrierDockingPermission(CarrierDockingPermission dockPerm)
        {
            // When docking perms are changed
            var carrierData =_c.Manager.GetById(dockPerm.CarrierID);
            if (carrierData is null) return;

            carrierData.DockingAccess = dockPerm.DockingAccess;

            _c.DoUIAction(() =>
            {
                _c.UI.Get(dockPerm.CarrierID)?.InventoryForm?.OnBasicInfoChange();
            });
            _c.SerializeDataCacheV2();
        }

        private void OnStats(Statistics stats)
        {
            if (!string.IsNullOrWhiteSpace(_c.CurrentCommander))
            {
                _c.Manager.GetCommander(_c.CurrentCommander).LastStatisticsEvent = stats;

                CarrierData carrierData = _c.Manager.GetByOwner(_c.CurrentCommander);
                if (carrierData == null) return;

                carrierData.DistanceTravelledLy = stats.FleetCarrier.DistanceTravelled;
                carrierData.TotalJumps = stats.FleetCarrier.TotalJumps;

                _c.DoUIAction(() =>
                {
                    _c.UI.Get(carrierData)?.UpdateStats();
                    _c.UI.Get(carrierData)?.InventoryForm?.OnBasicInfoChange();
                });
                _c.SerializeDataCacheV2();
            }
        }

        private void OnCarrierStats(CarrierStats carrierStats)
        {
            if (MaybeInitializeCarrierInfo(carrierStats))
            {
                // Initialization also sets fuel levels. Don't duplicate efforts.
                return;
            }

            CarrierData carrierData = _c.Manager.GetById(carrierStats.CarrierID);

            if (carrierData != null) MaybeStuffInitialJumpInClipboard(carrierData);

            MaybeUpdateCarrierFuel(carrierStats);
        }

        private void OnCarrierFuelDeposit(CarrierDepositFuel carrierDepositFuel)
        {
            // Make sure the Cmdr is donating to their carrier for fuel updates as this event could trigger for
            // any commander's carrier when donating tritium. Technically this could wait until the next CarrierStats event.
            var data = _c.Manager.GetById(carrierDepositFuel.CarrierID);
            if (data != null)
            {
                MaybeUpdateCarrierFuel(null, carrierDepositFuel);
            }
        }

        private void OnCarrierJumpCancelled(CarrierJumpCancelled carrierJumpCancelled)
        {
            CarrierData carrierData = _c.Manager.GetById(carrierJumpCancelled.CarrierID);
            string msg = "Cancelled requested jump";
            if (carrierData.LastCarrierJumpRequest != null)
                msg = $"Cancelled requested jump to {carrierData.LastCarrierJumpRequest.SystemName}";
            carrierData.ClearJumpState();

            carrierData.LastCarrierJumpCancelled = carrierJumpCancelled;
            MaybeScheduleTimers(carrierData);

            _c.DoUIAction(() =>
            {
                _c.UI.Get(carrierData)?.JumpCanceled(msg);
            });
        }

        private void OnCarrierJumpRequest(CarrierJumpRequest carrierJumpRequest)
        {
            CarrierData carrierData = _c.Manager.GetById(carrierJumpRequest.CarrierID);
            // Odd that we wouldn't know about it yet.
            if (carrierData is null) return;

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
                MaybeUpdateCarrierLocation(carrierData.CarrierId, new(carrierData.LastCarrierJumpRequest.SystemName, carrierData.LastCarrierJumpRequest.SystemAddress), false, true);
            }

            carrierData.LastCarrierJumpRequest = carrierJumpRequest;
            double departureTimeMinutes = carrierData.DepartureTimeMinutes;
            if (departureTimeMinutes > 0)
            {
                // Force timer update for new cooldown time.
                carrierData.ClearTimers();
                MaybeScheduleTimers(carrierData);

                _c.Core.SendNotification(new()
                {
                    Title = "Carrier Jump Scheduled",
                    Detail = $"Jump is in {departureTimeMinutes:#.0} minutes",
                    Sender = AboutInfo.ShortName,
                });
                _c.DoUIAction(() =>
                {
                    _c.UI.Get(carrierData)?.JumpScheduled();
                });
            }
        }

        private void OnCarrierNameChange(CarrierNameChange nameChange)
        {
            CarrierData carrierData = _c.Manager.GetById(nameChange.CarrierID);
            if (carrierData == null) return;

            carrierData.CarrierName = nameChange.Name;
            carrierData.CarrierCallsign = nameChange.Callsign;
            _c.SerializeDataCacheV2();

            _c.DoUIAction(() =>
            {
                _c.UI.Get(carrierData)?.Draw();
            });
        }

        private void OnCarrierLocation(CarrierLocation carrierLocation)
        {
            var squadron = _c.Manager.GetSquadronForCommander(_c.CurrentCommander);
            if (!_c.Manager.IsIDKnown(carrierLocation.CarrierID) && carrierLocation.CarrierType == CarrierType.SquadronCarrier && squadron != null)
            {
                // We've discovered a new squadron carrier. When we know the squadron, we can register it (although discovering it in this situation means we don't
                // know much about it).
                _c.Manager.RegisterCarrier(squadron, carrierLocation);
            }

            MaybeUpdateCarrierLocation(
                carrierLocation.CarrierID,
                new(carrierLocation),
                /* notifyIfChanged= */ false,
                /* carrierMoveEvent= */ true);

        }

        private void OnCarrierBuy(CarrierBuy buy)
        {
            _c.Manager.RegisterCarrier(_c.CurrentCommander, buy);
            _c.SerializeDataCacheV2();
        }

        private void OnLocation(Location location)
        {
            // On game startup, a location event fires but we may not yet know if the carrier they're docked on is theirs, so
            // only keep track of this initial location until we can confirm carrier ownership from a future CarrierStats or any
            // other carrier action for that matter.
            ulong? marketId = location.MarketID == 0 ? null : location.MarketID;

            if (marketId.HasValue && (location.Docked || location.OnFoot))
            {
                _c.ForCommander().LastDockedStationId = marketId.Value;
            }

            if (location.StationType == "FleetCarrier" && _c.Manager.IsIDKnown(location.MarketID))
            {
                MaybeUpdateCarrierLocation(location.MarketID, new(location), true);
            }
            else _initialLocation ??= location;

            if (_c.Manager.SetCommanderAboard(_c.CurrentCommander, marketId))
            {
                // Something changed; update all commander on-board states.
                _c.DoUIAction(() =>
                {
                    _c.UI.UpdateCommanderOnboardState();
                });
                _c.SerializeDataCacheV2();
            }
        }

        private void OnUndocked()
        {
            CommanderData cmdrData = _c.ForCommander();
            if (cmdrData is null) return;

            // It doesn't matter where they're undocking from, they're no longer on any carrier.
            cmdrData.LastDockedStationId = null;
            if (_c.Manager.SetCommanderAboard(cmdrData.Name, /*marketId=*/null))
            {
                // Something changed; update all commander on-board states.
                _c.DoUIAction(() =>
                {
                    _c.UI.UpdateCommanderOnboardState();
                });
                _c.SerializeDataCacheV2();
            }
        }

        private void OnDocked(Docked docked)
        {
            CommanderData cmdrData = _c.ForCommander();
            if (cmdrData is null) return;

            cmdrData.LastDockedStationId = docked.MarketID;

            if (_c.Manager.SetCommanderAboard(cmdrData.Name, /*marketId=*/cmdrData.LastDockedStationId))
            {
                // Something changed; update all commander on-board states.
                _c.DoUIAction(() =>
                {
                    _c.UI.UpdateCommanderOnboardState();
                });
                _c.SerializeDataCacheV2();
            }

            // IF docked on a either their own carrier or the squadron's carreir, set that carrier's 
            // docked state. Otherwise, the commander is not docked on relevant carriers.
            if (docked.StationType != "FleetCarrier" || !_c.Manager.IsIDKnown(docked.MarketID)) return;
            // This isn't a great position signal.
            MaybeUpdateCarrierLocation(docked.MarketID, new(docked.StarSystem, docked.SystemAddress), false, /* notifyIfChanged */ false);

        }

        private void OnLoadGame(LoadGame loadGame)
        {
            if (_c.CurrentCommander != loadGame.Commander)
            {
                _initialRouteStuffingDone = false;
                _initialLocation = null;
            }
            _c.CurrentCommander = loadGame.Commander;
            CommanderData commanderData;
            if (!_c.Manager.IsCommanderKnown(loadGame.Commander))
            {
                commanderData = _c.Manager.AddCommander(loadGame.FID, loadGame.Commander);
                _c.SerializeDataCacheV2();
            }
            else
                commanderData = _c.Manager.GetCommander(loadGame.Commander);

            if (commanderData.Carrier is not null) MaybeStuffInitialJumpInClipboard(commanderData.Carrier);
        }

        public byte[] ExportContent(string delimiter, ref string filetype)
        {
            StringBuilder content = new();
            content.AppendLine("Carrier Name\tCallsign\tOwner\tFuel Level (T)\tLocation\tNext Jump");

            foreach (var c in _c.Manager.Carriers)
            {
                var nextJumpDetail = "(none scheduled)";
                if (c.HasRoute && c.IsPositionKnown)
                {
                    var nextJumpInfo = c.Route.GetNextJump(c.Position?.SystemName);
                    string departureTime = "";
                    if (c.LastCarrierJumpRequest != null && !string.IsNullOrWhiteSpace(c.LastCarrierJumpRequest.DepartureTime))
                    {
                        departureTime = $"@ {c.LastCarrierJumpRequest.DepartureTimeDateTime}";
                    }
                    nextJumpDetail = $"{nextJumpInfo.SystemName}{departureTime}";

                }

                content.AppendLine(
                    $"{c.CarrierName}\t{c.CarrierCallsign}\t{c.Owner}\t{c.CarrierFuel}\t{c.Position?.BodyName}\t{nextJumpDetail}");
            }

            return Encoding.UTF8.GetBytes(content.ToString());
        }
        #endregion

        #region PluginMessage handling

        public void HandlePluginMessage(MessageSender sender, PluginMessage messageArgs)
        {
            _c.Dispatcher.MaybeHandlePluginMessage(sender, messageArgs, out _);
        }

        #endregion

        #region Private methods
        private void MaybeUpdateCarrierLocation(ulong carrierId, CarrierPositionData position, bool notifyIfChanged, bool carrierMoveEvent = true)
        {
            var data = _c.Manager.GetById(carrierId);
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
                    // TODO: Spawn this off in a dedicated task to allow for making an external lookup.
                    estFuelUsage = data.EstimateFuelForJumpFromCurrentPosition(_c, position);
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

                _c.DoUIAction(() =>
                {
                    _c.UI.Get(data)?.UpdatePosition(position, locationUpdateDetails);
                    _c.UI.Get(data)?.UpdateFuel();
                });

                // Notify if not initial values and context requests it and user has this notification enabled.
                if (notifyIfChanged)
                {
                    var fromPositionText = (data.IsPositionKnown ? $" from {data.Position.BodyName}" : "");

                    _c.Core.SendNotification(new()
                    {
                        Title = "Carrier Status Update",
                        Detail = locationUpdateDetails,
                        ExtendedDetails = $"Carrier has jumped{fromPositionText} to {position.BodyName}.",
                        Sender = AboutInfo.ShortName,
                        Rendering = (_c.Settings.NotifyJumpComplete ? NotificationRendering.All : NotificationRendering.PluginNotifier),
                    });
                }
                MaybeScheduleTimers(data);
                //data.LastCarrierJumpRequest = null;
            }
            data.MaybeUpdateLocation(position);
            _c.SerializeDataCacheV2();
        }

        private void MaybeScheduleTimers(CarrierData data)
        {
            if (_c.Core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch)
                || (data.LastCarrierJumpRequest == null && data.LastCarrierJumpCancelled == null)
                || (data.LastCarrierJumpRequest != null && string.IsNullOrWhiteSpace(data.LastCarrierJumpRequest.DepartureTime))) return;

            if (data.LastCarrierJumpRequest != null) // We have a jump requested.
            {
                // Don't start a countdown if the cooldown is already over.
                DateTime carrierDepartureTime = data.LastCarrierJumpRequest.DepartureTimeDateTime;
                DateTime carrierJumpCooldownTime = carrierDepartureTime.AddMinutes(CarrierData.JUMP_COOLDOWN_MINUTES);
                if (_c.Settings.NotifyJumpCooldown && !data.CooldownNotifyScheduled && DateTime.Now < carrierJumpCooldownTime)
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
            }
            if (data.LastCarrierJumpCancelled != null) // We have a jump cancelled.
            {
                DateTime carrierCancelCooldownTime = data.LastCarrierJumpCancelled.TimestampDateTime.AddMinutes(CarrierData.CANCEL_COOLDOWN_MINUTES);
                if (_c.Settings.NotifyJumpCooldown && !data.CooldownNotifyScheduled && DateTime.Now < carrierCancelCooldownTime)
                {
                    data.CarrierCooldownTimer = new System.Timers.Timer(carrierCancelCooldownTime.Subtract(DateTime.Now).TotalMilliseconds);
                    data.CarrierCooldownTimer.Elapsed += CarrierCooldownTimer_Elapsed;
                    data.CarrierCooldownTimer.Start();
                    Debug.WriteLine($"Cooldown notification scheduled for {carrierCancelCooldownTime}.");
                }
            }
            _c.DoUIAction(() =>
            {
                _c.UI.Get(data)?.InitCountdown();
            });
        }

        private bool MaybeInitializeCarrierInfo(CarrierStats stats)
        {
            if (!_c.Manager.IsIDKnown(stats.CarrierID))
            {
                var data = _c.Manager.RegisterCarrier(_c.CurrentCommander, stats);
                _c.DoUIAction(() =>
                {
                    _c.UI.Add(stats.CarrierID)?.Draw($"Carrier detected: {data.CarrierName} {data.CarrierCallsign}. Configured notifications are active.");
                });
                _c.SerializeDataCacheV2();
                return true;
            }
            else
            {
                if (stats.CarrierType == CarrierType.SquadronCarrier && !_c.Manager.IsCallsignKnown(stats.Callsign))
                {
                    EnrichSquadronCarrierData(stats.CarrierID, stats.Name, stats.Callsign);
                    _c.SerializeDataCacheV2();
                }
                return false;
            }
        }

        private void EnrichSquadronCarrierData(ulong carrierId, string name, string callsign)
        {
            // We have potentially new info about this carrier.
            var data = _c.Manager.GetById(carrierId);
            if (!data.IsSquadronCarrier) return;

            data.CarrierName = name;
            data.CarrierCallsign = callsign;
            var squadron = _c.Manager.GetAssociatedSquadron(data.CarrierId);
            if (squadron != null)
                squadron.Tag = data.CarrierCallsign;
        }

        private void MaybeUpdateCarrierFuel(CarrierStats stats = null, CarrierDepositFuel fuel = null)
        {
            if (stats == null && fuel == null) return;

            ulong carrierId = stats?.CarrierID ?? fuel.CarrierID;
            var data = _c.Manager.GetById(carrierId);

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
                _c.DoUIAction(() =>
                {
                    _c.UI.Get(data)?.UpdateFuel(fuelDetails);
                });

                if (lowFuel)
                {
                    // Only notify if fuel is running low and not reading-all.
                    _c.Core.SendNotification(new()
                    {
                        Title = "Carrier Low Fuel",
                        Detail = fuelDetails,
                        Sender = AboutInfo.ShortName,
                    });
                }
            }

            if (stats != null) data.LastCarrierStats = stats;
            _c.DoUIAction(() =>
            {
                _c.UI.Get(data)?.UpdateStats();
            });
            _c.SerializeDataCacheV2();
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
                        MaybeUpdateCarrierLocation(carrierData.CarrierId, new(carrierData.LastCarrierJumpRequest), false, true);
                    }
                    // Otherwise, In our cache, position == next jump destination. So the last jump request appears stale; just clear it.
                    carrierData.LastCarrierJumpRequest = null;
                    _c.SerializeDataCacheV2();
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
                _c.SerializeDataCacheV2();
                _c.DoUIAction(() =>
                {
                    _c.UI.Get(carrierData)?.UpdatePosition(carrierData.Position);
                });
                return;
            }

            var nextJumpInfo = carrierData.Route.GetNextJump(carrierData.Position.SystemName);
            if (nextJumpInfo != null && carrierData.LastCarrierJumpRequest == null)
            {
                _c.DoUIAction(() =>
                {
                    Misc.SetTextToClipboard(nextJumpInfo.SystemName);
                    _c.UI.Get(carrierData)?.SetMessage("System name for the next jump in your carrier route is in the clipboard.");
                });
            }
        }

        #endregion

        #region Events
        private void CarrierCooldownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var data = _c.Manager.GetByTimer((System.Timers.Timer)sender);
            if (data == null) return;

            _c.DoUIAction(() =>
            {
                _c.UI.Get(data)?.SetMessage("Carrier jump cooldown is over. You may now schedule a new jump.");
            });
            _c.Core.SendNotification(new()
            {
                Title = "Carrier Status Update",
                Detail = "Jump cooldown is complete. You may now schedule a new jump.",
                Sender = AboutInfo.ShortName,
            });
            data.ClearJumpState();

            if (!string.IsNullOrEmpty(_c.Settings.CooldownNotificationSoundFile)
                && File.Exists(_c.Settings.CooldownNotificationSoundFile))
            {
                _c.Core.PlayAudioFile(_c.Settings.CooldownNotificationSoundFile, new AudioOptions()
                {
                    DeleteAfterPlay = false,
                    Instant = true,
                });
            }

            if (data.HasRoute)
            {
                MaybeSetNextJumpInClipboard(data);
            }
        }

        private void CarrierJumpTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // If we didn't get cancelled, assume the carrier jump occurred.
            var data = _c.Manager.GetByTimer((System.Timers.Timer)sender);
            if (data == null || data.LastCarrierJumpRequest == null) return;

            MaybeUpdateCarrierLocation(
                data.CarrierId, new(data.LastCarrierJumpRequest), true, true);
        }
        #endregion
    }
}
