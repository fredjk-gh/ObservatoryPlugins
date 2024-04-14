using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Observatory.Framework.Files.ParameterTypes;
using System.Diagnostics;
using static com.github.fredjk_gh.ObservatoryProspectorBasic.SynthRecipes;

namespace com.github.fredjk_gh.ObservatoryProspectorBasic
{
    public class ProspectorBasic : IObservatoryWorker
    {
        private bool enableDebug = false; // Not const to avoid unreachable code warnings.

        private const int minRingDensity = 5;  // mT/km^3
        private const string LimpetDronesKey = "drones";
        private const string LimpetDronesName = "Limpets";
        private const string TritiumKey = "tritium";
        private const string TritiumName = "Tritium";
        // This is updated by CargoName. Keys should be lower-case for maximum matchy-ness.
        private readonly Dictionary<string, string> DisplayNames = new()
        {
            { "$tritium_name;", TritiumName },
            { TritiumKey, TritiumName },
            { "limpet", LimpetDronesName },
            { LimpetDronesKey, LimpetDronesName },
        };

        private ProspectorSettings _settings = ProspectorSettings.DEFAULT;
        private PluginUI pluginUI;
        private IObservatoryCore Core;
        ObservableCollection<object> GridCollection = new();
        private readonly TrackedData _data = new();
        private readonly TrackedStats _stats = new();
        private readonly Guid[] _prospectorNotifications = new Guid[2];
        private Guid _cargoNotification = Guid.Empty;

        public string Name => "Observatory Prospector Basic";

        public string ShortName => "Prospector";

        public string Version => typeof(ProspectorBasic).Assembly.GetName().Version.ToString();

        public PluginUI PluginUI => pluginUI;

        public object Settings
        {
            get => _settings;
            set => _settings = (ProspectorSettings)value;
        }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            switch (journal)
            {
                case ProspectedAsteroid prospected:
                    OnProspectedAsteroid(prospected);
                    break;
                case Scan scan:
                    OnScan(scan);
                    _data.AddScanRawMats(scan);
                    break;
                case SAASignalsFound saaSignalsFound:
                    OnRingPing(saaSignalsFound);
                    break;
                case FSSAllBodiesFound allBodies:
                    if (!_data.AllBodiesFound)
                    {
                        FindSynthMatRichBodies();
                        _data.AllBodiesFound = true;
                    }
                    break;
                case SupercruiseEntry scEntry:
                    // Reset when we jump to supercruise or another system.
                    Reset("SupercruiseEntry");
                    _data.LocationChanged(scEntry.StarSystem);
                    if (scEntry is SupercruiseExit)
                    {
                        SupercruiseExit scExit = (SupercruiseExit)scEntry;
                        _data.LocationChanged(scExit.Body);
                    }
                    break;
                case FSDJump fsdJump:
                    // Reset when we jump to supercruise or another system.
                    Reset("FSDJump");
                    _data.SystemChanged(fsdJump.StarSystem);
                    break;
                case Location location:
                    // Reset (this could be game startup or carrier jump)
                    Reset("Location");
                    _data.SystemChanged(location.StarSystem);
                    break;
                case MiningRefined miningRefined:
                    string miningKey = CargoKey(miningRefined.Type, miningRefined.Type_Localised);
                    _data.CargoAdd(miningKey, 1);
                    Debug.WriteLineIf(enableDebug, $"MiningRefined: {miningKey} += 1");
                    UpdateCargoNotification();
                    break;
                case BuyDrones buyDrones:
                    _data.CargoAdd(LimpetDronesKey, buyDrones.Count);
                    Debug.WriteLineIf(enableDebug, $"BuyDrones: Limpets += {buyDrones.Count}");
                    UpdateCargoNotification(false /* newNotification */);
                    break;
                case CollectCargo collectCargo:
                    string collectedKey = CargoKey(collectCargo.Type, collectCargo.Type_Localised);
                    _data.CargoAdd(collectedKey, 1);
                    Debug.WriteLineIf(enableDebug, $"CollectCargo: {collectedKey} += 1");
                    UpdateCargoNotification(false /* newNotification */);
                    break;
                case Synthesis synth:
                    if (CargoKey(synth.Name) == LimpetDronesKey)
                    {
                        // Not always 4 limpets synthed -- depends on available cargo space.
                        var limpetsSynthedGuess = Math.Min(4, (_data.CargoMax ?? 4) - (_data.CargoCur ?? 0));
                        _stats.LimpetsSynthed += limpetsSynthedGuess;
                        _data.CargoAdd(LimpetDronesKey, limpetsSynthedGuess);
                        Debug.WriteLineIf(enableDebug, $"Synthesis: Limpets += {limpetsSynthedGuess}");
                        UpdateCargoNotification(false /* newNotification */);
                    }
                    foreach (var mat in synth.Materials)
                    {
                        if (MaterialData.IsRawMat(mat.Name)) {
                            _data.RawMatInventorySubtract(mat.Name, mat.Count);
                        }
                    }
                    break;
                case SellDrones sellDrones:
                    if (_data.CargoGet(LimpetDronesKey) > 0)
                    {
                        Debug.WriteLineIf(enableDebug, $"SellDrones: Limpets -= {sellDrones.Count}");
                        _data.CargoRemove(LimpetDronesKey, sellDrones.Count);
                        UpdateCargoNotification(false /* newNotification */);
                    }
                    break;
                case EjectCargo eject:
                    string ejectedKey = CargoKey(eject.Type, eject.Type_Localised);
                    if (_data.CargoGet(ejectedKey) > 0)
                    {
                        Debug.WriteLineIf(enableDebug, $"EjectCargo: {ejectedKey} -= {eject.Count}");
                        _data.CargoRemove(ejectedKey, eject.Count);
                    }
                    if (ejectedKey == LimpetDronesKey) // Ditching limpets:
                        _stats.LimpetsAbandoned += eject.Count;
                    UpdateCargoNotification(false /* newNotification */);
                    break;
                case LaunchDrone launchDrone:
                    // Ignore if unset (we can't subtract from a value we don't know).
                    Debug.WriteLineIf(enableDebug, "LaunchDrone: Limpets -= 1");
                    _data.CargoRemove(LimpetDronesKey, 1);
                    _stats.LimpetsUsed++;
                    UpdateCargoNotification(false /* newNotification */);
                    break;
                case Loadout loadout:
                    _data.CargoMax = loadout.CargoCapacity;
                    Debug.WriteLineIf(_data.CargoMax > 0 && enableDebug, $"Loadout: New cargoMax: {loadout.CargoCapacity}");
                    UpdateCargoNotification(false /* newNotification */);
                    break;
                case Cargo cargoEvent:
                    _data.CargoCur = cargoEvent.Count;
                    if (cargoEvent.Inventory != null && !cargoEvent.Inventory.IsEmpty) // Usually on game load or loadout change.
                    {
                        Debug.WriteLineIf(_data.CargoCur > 0 && enableDebug, $"Cargo w/Inventory: cargoCur: {_data.CargoCur}");
                        Reset("Cargo w/Inventory");
                        _data.Cargo.Clear(); // This is a cargo state reset.
                        foreach (CargoType inventoryItem in cargoEvent.Inventory)
                        {
                            string inventoryKey = CargoKey(inventoryItem.Name);
                            _data.CargoAdd(inventoryKey, inventoryItem.Count);
                        }
                        UpdateCargoNotification(false /* newNotification */);
                    }
                    else if (cargoEvent.Count == 0 && _data.Cargo.Values.Sum() > 0)
                    {
                        // On rare occasion we'll find that the game doesn't properly account for all launched limpets and we'll
                        // end up with limpets "stuck" in our inventory. When the cargoEvent reports 0, we have an opportunity to
                        // fix. So clear the cargo contents and effectively reset.
                        Debug.WriteLineIf(enableDebug, $"Cargo event with 0 count but we think we still have {_data.Cargo.Values.Sum()} items... Correction!");
                        Reset("Cargo");
                        _data.Cargo.Clear();
                        UpdateCargoNotification(false /* newNotification */);
                    }
                    break;
                case MarketSell marketSell:
                    string sellKey = CargoKey(marketSell.Type, marketSell.Type_Localised);
                    if (_data.CargoGet(sellKey) > 0)
                    {
                        Debug.WriteLineIf(enableDebug, $"MarketSell: {sellKey} -= {marketSell.Count}");
                        _data.CargoRemove(sellKey, marketSell.Count);
                    }
                    UpdateCargoNotification(false /* newNotification */);
                    break;
                case MarketBuy marketBuy:
                    string buyKey = CargoKey(marketBuy.Type, marketBuy.Type_Localised);
                    _data.CargoAdd(buyKey, marketBuy.Count);
                    Debug.WriteLineIf(enableDebug, $"MarketBuy: {buyKey} += {marketBuy.Count}");
                    UpdateCargoNotification(false /* newNotification */);
                    break;
                case CargoTransfer transfer:
                    foreach (CargoTransferDetail transferred in transfer.Transfers)
                    {
                        string transferKey = CargoKey(transferred.Type, transferred.Type_Localised);
                        if (transferred.Direction == CargoTransferDirection.ToShip)
                        {
                            Debug.WriteLineIf(enableDebug, $"CargoTransfer (to Ship): {transferKey} += {transferred.Count}");
                            _data.CargoAdd(transferKey, transferred.Count);
                        }
                        else if (_data.CargoGet(transferKey) > 0) // tocarrier and tosrv; either way, off the ship
                        {
                            Debug.WriteLineIf(enableDebug, $"CargoTransfer (off Ship): {transferKey} -= {transferred.Count}");
                            _data.CargoRemove(transferKey, transferred.Count);
                        }
                    }
                    UpdateCargoNotification(false /* newNotification */);
                    break;
                case CarrierDepositFuel tritiumDonation:
                    if (_data.CargoGet(TritiumKey) > 0)
                    {
                        Debug.WriteLineIf(enableDebug, $"CarrierDepositFuel: Tritium -= {tritiumDonation.Amount}");
                        _data.CargoRemove(TritiumKey, tritiumDonation.Amount);
                        UpdateCargoNotification(false /* newNotification */);
                    }
                    break;
                case Materials mats:
                    _data.RawMatInventoryUpdate(mats);
                    break;
                case MaterialCollected matCollected:
                    if (matCollected.Category.ToLower() != "raw") break;
                    var discarded = matCollected as MaterialDiscarded;
                    if (discarded != null)
                        // Why would one ever do this!? Also, how?
                        _data.RawMatInventorySubtract(discarded.Name, discarded.Count);
                    else
                        _data.RawMatInventoryAdd(matCollected.Name, matCollected.Count);
                    break;
                case MaterialTrade matTrade:
                    if (matTrade.Paid.Category.ToLower() == "raw")
                        _data.RawMatInventorySubtract(matTrade.Paid.Material, matTrade.Paid.Quantity);
                    if (matTrade.Received.Category.ToLower() == "raw")
                        _data.RawMatInventorySubtract(matTrade.Received.Material, matTrade.Received.Quantity);

                    break;

                // Maybe todo: Handle other events which could affect mat inventory:
                // - Mission rewards
                // - Techbroker unlocks
                // - Engineer contributions
                // - Engineer crafting
                // - Scientific Research - is this even used?
                case Shutdown shutdown:
                    Reset("Shutdown", true);
                    break;
            }
        }

        private void FindSynthMatRichBodies()
        {
            HashSet<string> allAvailableMats = new();
            if (_settings.MatsFSDBoost) allAvailableMats.UnionWith(FindAvailableSynthRecipeLevels("FSD Boost", SynthRecipes.FSDBoost));
            if (_settings.MatsAFMURefill) allAvailableMats.UnionWith(FindAvailableSynthRecipeLevels("AFMU Refill", SynthRecipes.AFMURefill));
            if (_settings.MatsSRVRefuel) allAvailableMats.UnionWith(FindAvailableSynthRecipeLevels("SRV Refuel", SynthRecipes.SRVRefuel));
            if (_settings.MatsSRVRepair) allAvailableMats.UnionWith(FindAvailableSynthRecipeLevels("SRV Repair", SynthRecipes.SRVRepair));

            var bestMatBodies = _data.MatsInSystem.Where(e => allAvailableMats.Contains(e.Key))
                .Select(e => e.Value.OrderByDescending(bmc => bmc.Percent).First())
                .GroupBy(bmc => (bmc.BodyName, bmc.BodyID))
                .ToDictionary(g => g.Key, g => g.ToList());

            var items = new List<ProspectorGrid>();
            foreach (var e in bestMatBodies)
            {
                Core.SendNotification(new()
                {
                    Sender = ShortName,
                    CoalescingId =  e.Key.BodyID,
                    Title =  _data.GetBodyTitle(e.Key.BodyName),
                    Detail = $"Good source of synth mats",
                    ExtendedDetails = $"Materials: {string.Join(", ", e.Value.Select(bmc => $"{bmc.Material}: {bmc.Percent:n2}%"))}",
                    Rendering = NotificationRendering.PluginNotifier,
                });

                items.Add(new()
                {
                    Location = $"{_data.SystemName} {e.Key.BodyName}",
                    Commodity = $"{string.Join(", ", e.Value.Select(bmc => $"{bmc.Material}"))}",
                    Percentage = "",
                    Details = "",
                });
            }
            Core.AddGridItems(this, items);
        }

        private HashSet<string> FindAvailableSynthRecipeLevels(string recipeName, Dictionary<SynthLevel, HashSet<string>> recipe)
        {
            List<SynthLevel> synthLevelMatsFound = new();
            HashSet<string> availableMats = new();

            foreach (var r in recipe)
            {
                if (r.Value.All(m => _data.MatsInSystem.ContainsKey(m)))
                {
                    synthLevelMatsFound.Add(r.Key);
                    availableMats.UnionWith(r.Value);
                }
            }

            if (synthLevelMatsFound.Count > 0)
            {
                var levelsStr = $"{string.Join(" | ", synthLevelMatsFound.Select(l => SynthRecipes.SynthLevelSymbols[l]))}";
                List<ProspectorGrid> gridItems = new();
                Core.AddGridItem(this, new ProspectorGrid()
                {
                    Location = _data.SystemName,
                    Commodity = recipeName,
                    Percentage = levelsStr,
                });

                Core.SendNotification(new()
                {
                    Sender = ShortName,
                    CoalescingId = -22,
                    Title = "Surface materials available",
                    Detail = $"For {recipeName}",
                    ExtendedDetails = $"Levels: {levelsStr}",
                    Rendering = NotificationRendering.PluginNotifier,
                });
            }

            return availableMats;
        }

        private void UpdateCargoNotification(bool newNotification = true)
        {
            if (!_settings.ShowCargoNotification) return;

            int cargoEstimate = _data.Cargo.Values.Sum();
            if (cargoEstimate == 0 || (_data.Cargo.Count == 1 && _data.CargoGet(LimpetDronesKey) > 0))
            {
                Debug.WriteLineIf(MaybeCloseCargoNotification() && enableDebug, "\t--Cargo Notification closed; no cargo or only limpets remaining.");
                return;
            }

            string cargoTitle = _data.CargoMax == null ? "Cargo" : $"Cargo ({cargoEstimate} / {_data.CargoMax})";
            string cargoDetail = string.Join(Environment.NewLine, _data.Cargo.Where(kvp => kvp.Value > 0).Select(kvp =>
            {
                if (kvp.Key == LimpetDronesKey)
                {
                    if (_stats.LimpetsAbandoned > 0)
                    {
                        int totalLimpets = _stats.LimpetsAbandoned + _stats.LimpetsUsed;
                        return $"{CargoName(kvp.Key)}: {kvp.Value} ({(_stats.LimpetsAbandoned * 100.0 / totalLimpets):N0}% wasted)";
                    }
                    else if (_stats.LimpetsSynthed > 0)
                    {
                        return $"{CargoName(kvp.Key)}: {kvp.Value} ({_stats.LimpetsSynthed} short)";
                    }
                }
                return $"{CargoName(kvp.Key)}: {kvp.Value}";
            }));
            Debug.WriteLineIf(enableDebug, $"\t--Cargo Notification update: {cargoTitle}; {cargoDetail.Replace(Environment.NewLine, "; ")}");

            if (Core.IsLogMonitorBatchReading || (_cargoNotification == Guid.Empty && !newNotification))
            {
                // Read-all or this notification shouldn't spawn a new notification and that's what we'd do next -- so we're done here.
                return;
            }

            NotificationArgs args = new()
            {
                Title = cargoTitle,
                Detail = cargoDetail,
                Timeout = 0, // Persistent until explicitly cleared.
                XPos = 83,
                YPos = 15,
                Rendering = NotificationRendering.NativeVisual,
                Sender = ShortName,
            };
            if (_cargoNotification == Guid.Empty)
            {
                _cargoNotification = Core.SendNotification(args);
            }
            else
            {
                Core.UpdateNotification(_cargoNotification, args);
            }
        }

        private void AddOrUpdateProspectorNotification(int counter, NotificationArgs args)
        {
            Debug.WriteLineIf(enableDebug, "ProspectedAsteroid: {args.Title}; {args.Detail}");

            if (!_settings.ShowProspectorNotifications || Core.IsLogMonitorBatchReading) return;

            // This method supports notifications with timeout OR persistent notifications.
            if (args.Timeout > 0 && _prospectorNotifications[counter % 2] != Guid.Empty)
            {
                // Notifications have a time out: Guid should be assumed invalid. Close it explicitly and clear the guid.
                // Would be nice if we could interrogate Core for the state of a notification ID (ie. is the Guid still associated with a valid notification)?
                Core.CancelNotification(_prospectorNotifications[counter % 2]);
                _prospectorNotifications[counter % 2] = Guid.Empty;
            }

            if (_prospectorNotifications[counter % 2] == Guid.Empty)
            {
                _prospectorNotifications[counter % 2] = Core.SendNotification(args);
            }
            else
            {
                Core.UpdateNotification(_prospectorNotifications[counter % 2], args);
            }
        }

        private void Reset(string caller, bool closeStaticNotificationsToo = false)
        {
            bool notificationsClosed = false;
            for (int i = 0; i < _prospectorNotifications.Length; i++)
            {
                Guid notification = _prospectorNotifications[i];
                if (notification != Guid.Empty)
                {
                    Core.CancelNotification(notification);
                    _prospectorNotifications[i] = Guid.Empty;
                    notificationsClosed = true;
                }
            }
            if (closeStaticNotificationsToo) notificationsClosed |= MaybeCloseCargoNotification();

            // Reset stats.
            if ((notificationsClosed || _stats.ProspectorsEngaged > 0) && enableDebug)
                Debug.WriteLine($"\t--{caller} - Reset; Stats: prospectorsEngaged: {_stats.ProspectorsEngaged}, limpetsUsed: {_stats.LimpetsUsed}, limpetsSynthed: {_stats.LimpetsSynthed}, limpetsAbandoned: {_stats.LimpetsAbandoned}, goodRocks: {_stats.GoodRocks}");

            _stats.Reset();
            // cargoMax can get updated by the next Loadout.
        }
        
        private bool MaybeCloseCargoNotification()
        {
            if (_cargoNotification != Guid.Empty)
            {
                // Close the notification.
                Core.CancelNotification(_cargoNotification);
                _cargoNotification = Guid.Empty;
                return true;
            }
            return false;
        }

        private void OnScan(Scan scan)
        {
            if (scan.Rings == null || scan.Rings.IsEmpty || _data.AlreadyReportedScansSaaSignals.Contains(scan.BodyName) || !_settings.MentionPotentiallyMineableRings || !_settings.MentionableRings.HasValue) return;

            _data.AlreadyReportedScansSaaSignals.Add(scan.BodyName);
            List<RingDetails> ringsOfInterest = new();
            RingType mentionableRings = _settings.MentionableRings.Value;
            foreach (Ring ring in scan.Rings)
            {
                // Ignore belts.
                if (!ring.Name.Contains("Ring")) continue;

                // Find rings of interest
                foreach (RingType rt in Enum.GetValues(typeof(RingType)))
                {
                    if (mentionableRings.HasFlag(rt) && ring.RingClass.Contains(rt.MatchString()))
                    {
                        double densityMTperkm3 = ring.MassMT / ((Math.PI * Math.Pow(ring.OuterRad / 1000, 2)) - (Math.PI * Math.Pow(ring.InnerRad / 1000, 2.0)));
                        if (densityMTperkm3 < minRingDensity)
                        {
                            Debug.WriteLineIf(enableDebug, $"Scan: Ignoring interesting ring with low density: {densityMTperkm3:n1}");
                            break;
                        }
                        string desiredCommodities = string.Join(", ", _settings.DesirableCommonditiesByRingType(rt).Select(c => c.ToString()));
                        var details = new RingDetails()
                        {
                            ShortName = _data.GetShortBodyName(ring.Name, scan.BodyName),
                            RingType = rt.DisplayString(),
                            Commodities = $"[{desiredCommodities}]",
                            Density = $"Density: {densityMTperkm3:n1} mT/km^3",
                        };
                        if (!ringsOfInterest.Contains(details)) ringsOfInterest.Add(details);
                        break;
                    }
                }
            }
            if (ringsOfInterest.Count == 0) return;

            var shortBodyName = _data.GetShortBodyName(scan.BodyName);
            var detailsCommaSeparated = string.Join(", ", ringsOfInterest.Select(t => t.ToString()));
            var bodyDistance = $"distance: {Math.Floor(scan.DistanceFromArrivalLS)} Ls";
            Debug.WriteLineIf(enableDebug, $"Scan: Interesting rings at body {shortBodyName}: {detailsCommaSeparated}, {bodyDistance}");

            Core.AddGridItem(this, new ProspectorGrid()
            {
                Location = scan.BodyName,
                Details = detailsCommaSeparated + bodyDistance,
            });
            Core.SendNotification(new NotificationArgs()
            {
                Title = _data.GetBodyTitle(shortBodyName),
                Detail = string.Join(", ", ringsOfInterest.Select(t => $"{t.RingType} Ring")),
                ExtendedDetails = $"{detailsCommaSeparated}, {bodyDistance}",
                Sender = ShortName,
                CoalescingId = scan.BodyID,
            });
        }

        private void OnProspectedAsteroid(ProspectedAsteroid prospected)
        {
            int prospectorId = _stats.ProspectorsEngaged++; // Kind-of assumes things land in the order they're launched. But meh.
            if (prospected.Remaining < 100)
            {
                AddOrUpdateProspectorNotification(prospectorId, MakeProspectorNotificationArgs("Depleted", "", prospectorId, NotificationRendering.NativeVisual));
                return;
            }

            // Separate grid entry, but combine it with other notifications.
            string highMaterialContent = "";
            if (_settings.ProspectHighMaterialContent && prospected.Content == "$AsteroidMaterialContent_High;")
            {
                highMaterialContent = " and has high material content";
                Core.AddGridItem(this, new ProspectorGrid
                {
                    Location = !_data.CurrentLocationShown ? _data.LocationName : string.Empty,
                    Commodity = "Raw materials",
                    Percentage = "High",
                    Details = string.Empty,
                });
                if (!_data.CurrentLocationShown) _data.CurrentLocationShown = true;
            }

            Commodities ml;
            if (Enum.TryParse<Commodities>(prospected.MotherlodeMaterial, true, out ml) && _settings.getFor(ml))
            {
                // Found a core of interest!
                _stats.GoodRocks++;
                string name = CommodityName(prospected.MotherlodeMaterial, prospected.MotherlodeMaterial_Localised);
                string cumulativeStats = $"{(_stats.GoodRocks * 1000.0) / (_stats.ProspectorsEngaged * 10.0):N1}% good rocks ({_stats.GoodRocks}/{_stats.ProspectorsEngaged})";
                Core.AddGridItem(this, new ProspectorGrid
                {
                    Location = !_data.CurrentLocationShown ? _data.LocationName : string.Empty,
                    Commodity = name,
                    Percentage = "Core found",
                    Details = cumulativeStats,
                });
                if (!_data.CurrentLocationShown) _data.CurrentLocationShown = true;
                
                NotificationArgs args = MakeProspectorNotificationArgs(
                    "Core found", $"Asteroid contains core of {name}{highMaterialContent}", prospectorId, NotificationRendering.All);
                AddOrUpdateProspectorNotification(prospectorId, args);
                Debug.WriteLineIf(enableDebug, $"\t--Grid Update: {name}; Core found, {cumulativeStats}");
                return;
            }

            Dictionary<string, float> desireableCommodities = new();
            float desireableCommoditiesPercentSum = 0.0f;
            foreach (var m in prospected.Materials)
            {
                Commodities c;
                if (!Enum.TryParse<Commodities>(m.Name, true, out c))
                {
                    continue;
                }

                if (_settings.getFor(c))
                {
                    string name = CommodityName(m.Name, m.Name_Localised);
                    desireableCommodities.Add(name, m.Proportion);
                    desireableCommoditiesPercentSum += m.Proportion;
                }
            }

            if (desireableCommodities.Count > 0 && desireableCommoditiesPercentSum > _settings.MinimumPercent)
            {
                _stats.GoodRocks++;
                string title;
                string details;
                string commodities = String.Join(", ", desireableCommodities.Keys);
                string percentageString = $"{desireableCommoditiesPercentSum:N2}%";
                string cumulativeStats = $"{(_stats.GoodRocks * 1000) / (_stats.ProspectorsEngaged * 10.0):N1}% good rocks ({_stats.GoodRocks}/{_stats.ProspectorsEngaged})";

                Core.AddGridItem(this, new ProspectorGrid
                {
                    Location = !_data.CurrentLocationShown ? _data.LocationName : string.Empty,
                    Commodity = commodities,
                    Percentage = percentageString,
                    Details = cumulativeStats
                });
                if (!_data.CurrentLocationShown) _data.CurrentLocationShown = true;

                if (desireableCommodities.Count > 1)
                {
                    // Ooh, multiple things here!
                    title = "Good multiple commodity rock";
                    details = $"Asteroid is a combined {desireableCommoditiesPercentSum:N0} percent {commodities}{highMaterialContent}";
                }
                else
                {
                    title = "Good rock";
                    details = $"Asteroid is {desireableCommoditiesPercentSum:N0} percent {commodities}{highMaterialContent}";
                }
                AddOrUpdateProspectorNotification(prospectorId, MakeProspectorNotificationArgs(title, details, prospectorId, NotificationRendering.All));
                Debug.WriteLine(enableDebug, $"\t--Grid Update: {commodities}; {percentageString}, {cumulativeStats}");
                return;
            }
            // If we got here, we didn't find anything interesting.
            if (string.IsNullOrEmpty(highMaterialContent))
            {
                AddOrUpdateProspectorNotification(prospectorId, MakeProspectorNotificationArgs("Nothing of interest", "", prospectorId, NotificationRendering.NativeVisual));
            }
            else
            {
                string details = "";
                if (desireableCommodities.Count > 0)
                {
                    details = $"Asteroid also contains {desireableCommoditiesPercentSum:N0} percent of other desireable commodities";
                }
                AddOrUpdateProspectorNotification(prospectorId, MakeProspectorNotificationArgs("High raw material content", details, prospectorId, NotificationRendering.All));
            }
        }

        private void OnRingPing(SAASignalsFound saaSignalsFound)
        {
            if (String.IsNullOrEmpty(_data.SystemName)) return;
            var ringName = _data.GetShortBodyName(saaSignalsFound.BodyName);
            if (saaSignalsFound.Signals == null || saaSignalsFound.Signals.Count == 0 || _data.AlreadyReportedScansSaaSignals.Contains(ringName)
                    || !ringName.Contains(" Ring", StringComparison.InvariantCultureIgnoreCase))
                return;

            _data.AlreadyReportedScansSaaSignals.Add(ringName);
            Dictionary<string, int> desireableCommodities = new();
            List<string> notificationDetail = new();

            foreach (var m in saaSignalsFound.Signals)
            {
                Commodities c;
                if (!Enum.TryParse<Commodities>(m.Type, true, out c))
                {
                    continue;
                }

                if (_settings.getFor(c))
                {
                    string name = CommodityName(m.Type, m.Type);
                    desireableCommodities.Add(name, m.Count);

                    Core.AddGridItem(this, new ProspectorGrid
                    {
                        Location = saaSignalsFound.BodyName,
                        Commodity = name,
                        Percentage = string.Empty,
                        Details = $"{m.Count} hotspot{(m.Count > 1 ? "s" : string.Empty)}",
                    });
                    notificationDetail.Add($"{m.Count} {name}");
                    if (!_data.CurrentLocationShown) _data.CurrentLocationShown = true;
                }
            }

            if (notificationDetail.Count > 0)
            {
                Debug.WriteLineIf(enableDebug, $"SAASignalsFound: Hotspots of interest: {ringName} contains: {string.Join(", ", notificationDetail)}");

                if (Core.IsLogMonitorBatchReading) return;

                Core.SendNotification(new NotificationArgs
                {
                    Title = "Hotspots of interest",
                    Detail = $"{ringName} contains:{Environment.NewLine}{string.Join(Environment.NewLine, notificationDetail)}",
                    Sender = ShortName,
                    CoalescingId = saaSignalsFound.BodyID,
                });
            }
        }

        private NotificationArgs MakeProspectorNotificationArgs(string title, string detail, int index, NotificationRendering rendering)
        {
            NotificationArgs args = new()
            {
                Title = title,
                Detail = $"[{DateTime.UtcNow.ToString("h:mm:ss")}] {detail}",
                DetailSsml = $"<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en - US\"><voice name=\"\">{detail}</voice></speak>",
                Timeout = 300 * 1000, // 5 minutes
                XPos = 1,
                YPos = ((index % 2) + 1) * 15,
                Rendering = rendering,
                Sender = ShortName,
            };
            return args;
        }

        private string CargoKey(string name, string localizedName = "")
        {
            // Commodity name prefers LocalizedName but uses name as a fallback. However,
            // CargoKey prefers name and fallsback to localised name if not set (rare).
            // Always lower-case it (DisplayNames work best this way). And strip out the $..._name; junk.
            string displayName = CommodityName(name, localizedName);
            string candidateKey = name.ToLowerInvariant().Replace("$", "").Replace("_name;", "");

            if (candidateKey.Contains("limpet")) return LimpetDronesKey; // skip display name stuff here as it's pre-filled
            //if (KeySynonyms.ContainsKey(candidateKey)) candidateKey = KeySynonyms[candidateKey];

            if (!DisplayNames.ContainsKey(candidateKey))
            {
                DisplayNames[candidateKey] = displayName;
            }
            return candidateKey;
        }

        private string CargoName(string key)
        {
            return DisplayNames.ContainsKey(key) ? DisplayNames[key] : key;
        }

        private string CommodityName(string fallbackName, string localizedName = "")
        {
            // Anything cached?
            if (!string.IsNullOrEmpty(fallbackName) && DisplayNames.ContainsKey(fallbackName)) return DisplayNames[fallbackName];
            if (!string.IsNullOrEmpty(localizedName) && DisplayNames.ContainsKey(localizedName)) return DisplayNames[localizedName];

            // Start with fallback, use localized if available.
            string displayName = fallbackName;
            if (!string.IsNullOrEmpty(localizedName))
            {
                displayName = localizedName;
            }

            // A couple overrides:
            if (displayName.ToLowerInvariant().Contains("limpet"))
            {
                DisplayNames[displayName] = LimpetDronesName;
                return LimpetDronesName;
            }
            if (!string.IsNullOrEmpty(fallbackName) && fallbackName != displayName) DisplayNames[fallbackName] = displayName;
            return displayName;
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args)
        {
            if (LogMonitorStateChangedEventArgs.IsBatchRead(args.NewState))
            {
                Core.ClearGrid(this, new ProspectorGrid());
            }
        }

        public void Load(IObservatoryCore observatoryCore)
        {
            GridCollection = new();
            ProspectorGrid uiObject = new();

            GridCollection.Add(uiObject);
            pluginUI = new PluginUI(GridCollection);

            Core = observatoryCore;
        }
    }

    public class ProspectorGrid
    {
        [ColumnSuggestedWidth(400)]
        public string Location { get; set; }
        [ColumnSuggestedWidth(250)]
        public string Commodity { get; set; }
        [ColumnSuggestedWidth(150)]
        public string Percentage { get; set; }
        [ColumnSuggestedWidth(500)]
        public string Details { get; set; }
    }
}
