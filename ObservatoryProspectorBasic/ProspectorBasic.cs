using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static ObservatoryProspectorBasic.ProspectorSettings;
using Observatory.Framework.Files.ParameterTypes;
using System.Diagnostics;

namespace ObservatoryProspectorBasic
{
    public class ProspectorBasic : IObservatoryWorker
    {
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

        private PluginUI pluginUI;
        private IObservatoryCore Core;
        ObservableCollection<object> GridCollection = new();
        private ProspectorSettings settings = new()
        {
            ShowProspectorNotifications = true,
            ShowCargoNotification = true,
            MinimumPercent = 10,
            ProspectTritium = true,
            ProspectPlatinum = true,
        };

        private string currentSystem = null;
        private string currentLocation = null;
        private bool currentLocationShown = false;
        private HashSet<string> alreadyReportedScansSaaSignals = new();
        private int goodRocks = 0;
        private int prospectorsEngaged = 0;
        private int limpetsAbandoned = 0;
        private int limpetsUsed = 0;
        private int limpetsSynthed = 0;
        private readonly Guid[] prospectorNotifications = new Guid[2];
        private Guid cargoNotification = Guid.Empty;
        private int? cargoMax = null;
        private int? cargoCur = null;
        private readonly Dictionary<string, int> cargo = new();

        public string Name => "Observatory Prospector Basic";

        public string ShortName => "Prospector";

        public string Version => typeof(ProspectorBasic).Assembly.GetName().Version.ToString();

        public PluginUI PluginUI => pluginUI;

        public object Settings
        {
            get => settings;
            set => settings = (ProspectorSettings)value;
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
                    break;
                case SAASignalsFound saaSignalsFound:
                    OnRingPing(saaSignalsFound);
                    break;
                case SupercruiseEntry scEntry:
                    // Reset when we jump to supercruise or another system.
                    Reset("SupercruiseEntry");
                    MaybeUpdateCurrentSystem(scEntry.StarSystem);
                    if (scEntry is SupercruiseExit)
                    {
                        SupercruiseExit scExit = (SupercruiseExit)scEntry;
                        MaybeUpdateCurrentLocation(scExit.Body);
                    }
                    break;
                case FSDJump fsdJump:
                    // Reset when we jump to supercruise or another system.
                    Reset("FSDJump");
                    MaybeUpdateCurrentSystem(fsdJump.StarSystem);
                    break;
                case MiningRefined miningRefined:
                    string miningKey = CargoKey(miningRefined.Type, miningRefined.Type_Localised);
                    if (!cargo.ContainsKey(miningKey)) cargo[miningKey] = 0;
                    cargo[miningKey] += 1;
                    Debug.WriteLine("MiningRefined: {0} += 1", (object)miningKey);  // The (object) cast here is to force the correct overload of Debug.Writeline.
                    UpdateCargoNotification();
                    break;
                case BuyDrones buyDrones:
                    if (!cargo.ContainsKey(LimpetDronesKey)) cargo[LimpetDronesKey] = 0;
                    cargo[LimpetDronesKey] += buyDrones.Count;
                    Debug.WriteLine("BuyDrones: Limpets += {0}", (object)buyDrones.Count);  // The (object) cast here is to force the correct overload of Debug.Writeline.
                    UpdateCargoNotification(false /* newNotification */);
                    break;
                case CollectCargo collectCargo:
                    string collectedKey = CargoKey(collectCargo.Type, collectCargo.Type_Localised);
                    if (!cargo.ContainsKey(collectedKey)) cargo[collectedKey] = 0;
                    cargo[collectedKey] += 1;
                    Debug.WriteLine("CollectCargo: {0} += 1", (object)collectedKey);  // The (object) cast here is to force the correct overload of Debug.Writeline.
                    UpdateCargoNotification(false /* newNotification */);
                    break;
                case Synthesis synth:
                    if (CargoKey(synth.Name) == LimpetDronesKey)
                    {
                        // Not always 4 limpets synthed -- depends on available cargo space.
                        var limpetsSynthedGuess = Math.Min(4, (cargoMax ?? 4) - (cargoCur ?? 0));
                        limpetsSynthed += limpetsSynthedGuess;
                        if (!cargo.ContainsKey(LimpetDronesKey)) cargo[LimpetDronesKey] = 0;
                        cargo[LimpetDronesKey] += limpetsSynthedGuess; 
                        Debug.WriteLine("Synthesis: Limpets += 4");
                        UpdateCargoNotification(false /* newNotification */);
                    }
                    break;
                case SellDrones sellDrones:
                    if (cargo.ContainsKey(LimpetDronesKey))
                    {
                        Debug.WriteLine("SellDrones: Limpets -= Min( {0}, {1} )", sellDrones.Count, cargo[LimpetDronesKey]);
                        cargo[LimpetDronesKey] -= Math.Min(sellDrones.Count, cargo[LimpetDronesKey]);
                        UpdateCargoNotification(false /* newNotification */);
                    }
                    break;
                case EjectCargo eject:
                    string ejectedKey = CargoKey(eject.Type, eject.Type_Localised);
                    if (cargo.ContainsKey(ejectedKey))
                    {
                        Debug.WriteLine("EjectCargo: {0} -= Min of ( {1}, {2} )", ejectedKey, eject.Count, cargo[ejectedKey]);
                        cargo[ejectedKey] -= Math.Min(eject.Count, cargo[ejectedKey]);
                    }
                    if (ejectedKey == LimpetDronesKey) // Ditching limpets:
                        limpetsAbandoned += eject.Count;
                    UpdateCargoNotification(false /* newNotification */);
                    break;
                case LaunchDrone launchDrone:
                    // Ignore if unset (we can't subtract from a value we don't know).
                    Debug.WriteLine("LaunchDrone: Limpets -= 1");
                    if (cargo.ContainsKey(LimpetDronesKey) && cargo[LimpetDronesKey] > 0) cargo[LimpetDronesKey] -= 1;
                    limpetsUsed++;
                    UpdateCargoNotification(false /* newNotification */);
                    break;
                case Loadout loadout:
                    cargoMax = loadout.CargoCapacity;
                    if (cargoMax > 0) Debug.WriteLine("Loadout: New cargoMax: {0}", loadout.CargoCapacity);
                    UpdateCargoNotification(false /* newNotification */);
                    break;
                case Cargo cargoEvent:
                    cargoCur = cargoEvent.Count;
                    if (cargoEvent.Inventory != null && !cargoEvent.Inventory.IsEmpty) // Usually on game load or loadout change.
                    {
                        if (cargoCur > 0) Debug.WriteLine("Cargo w/Inventory: cargoCur: {0}", cargoCur);
                        Reset("Cargo w/Inventory");
                        cargo.Clear(); // This is a cargo state reset.
                        foreach (CargoType inventoryItem in cargoEvent.Inventory)
                        {
                            string inventoryKey = CargoKey(inventoryItem.Name);
                            // replace any value 
                            cargo[inventoryKey] = inventoryItem.Count;
                        }
                        UpdateCargoNotification(false /* newNotification */);
                    }
                    else if (cargoEvent.Count == 0 && cargo.Values.Sum() > 0)
                    {
                        // On rare occasion we'll find that the game doesn't properly account for all launched limpets and we'll
                        // end up with limpets "stuck" in our inventory. When the cargoEvent reports 0, we have an opportunity to
                        // fix. So clear the cargo contents and effectively reset.
                        Debug.WriteLine("Cargo event with 0 count but we think we still have {0} items... Correction!", cargo.Values.Sum());
                        Reset("Cargo");
                        cargo.Clear();
                        UpdateCargoNotification(false /* newNotification */);
                    }
                    break;
                case MarketSell marketSell:
                    string sellKey = CargoKey(marketSell.Type, marketSell.Type_Localised);
                    if (cargo.ContainsKey(sellKey))
                    {
                        Debug.WriteLine("MarketSell: {0} -= Min of ( {1}, {2} )", sellKey, marketSell.Count, cargo[sellKey]);
                        cargo[sellKey] -= Math.Min(marketSell.Count, cargo[sellKey]);
                    }
                    UpdateCargoNotification(false /* newNotification */);
                    break;
                case MarketBuy marketBuy:
                    string buyKey = CargoKey(marketBuy.Type, marketBuy.Type_Localised);
                    if (!cargo.ContainsKey(buyKey)) cargo[buyKey] = 0;
                    cargo[buyKey] += marketBuy.Count;
                    Debug.WriteLine("MarketBuy: {0} += {1}", buyKey, marketBuy.Count);
                    UpdateCargoNotification(false /* newNotification */);
                    break;
                case CargoTransfer transfer:
                    foreach (CargoTransferDetail transferred in transfer.Transfers)
                    {
                        string transferKey = CargoKey(transferred.Type, transferred.Type_Localised);
                        if (transferred.Direction == CargoTransferDirection.ToShip)
                        {
                            Debug.WriteLine("CargoTransfer (to Ship): {0} += {1}", transferKey, transferred.Count);
                            if (!cargo.ContainsKey(transferKey)) cargo[transferKey] = 0;
                            cargo[transferKey] += transferred.Count;
                        }
                        else if (cargo.ContainsKey(transferKey)) // tocarrier and tosrv; either way, off the ship
                        {
                            Debug.WriteLine("CargoTransfer (off Ship): {0} -= Min of ( {1}, {2} )", transferKey, transferred.Count, cargo[transferKey]);
                            cargo[transferKey] -= Math.Min(transferred.Count, cargo[transferKey]);
                        }
                    }
                    UpdateCargoNotification(false /* newNotification */);
                    break;
                case CarrierDepositFuel tritiumDonation:
                    if (cargo.ContainsKey(TritiumKey))
                    {
                        Debug.WriteLine("CarrierDepositFuel: Tritium -= Min of ( {0}, {1} )", tritiumDonation.Amount, cargo[TritiumKey]);
                        cargo[TritiumKey] -= Math.Min(tritiumDonation.Amount, cargo[TritiumKey]);
                        UpdateCargoNotification(false /* newNotification */);
                    }
                    break;
                case Shutdown shutdown:
                    Reset("Shutdown", true);
                    break;
            }
        }

        private void UpdateCargoNotification(bool newNotification = true)
        {
            if (!settings.ShowCargoNotification) return;

            int cargoEstimate = cargo.Values.Sum();
            if (cargoEstimate == 0 || (cargo.Count == 1 && cargo.ContainsKey(LimpetDronesKey)))
            {
                if (MaybeCloseCargoNotification())
                    Debug.WriteLine("\t--Cargo Notification closed; no cargo or only limpets remaining.");
                return;
            }

            string cargoTitle = cargoMax == null ? "Cargo" : $"Cargo ({cargoEstimate} / {cargoMax})";
            string cargoDetail = string.Join(Environment.NewLine, cargo.Where(kvp => kvp.Value > 0).Select(kvp =>
            {
                if (kvp.Key == LimpetDronesKey)
                {
                    if (limpetsAbandoned > 0)
                    {
                        int totalLimpets = limpetsAbandoned + limpetsUsed;
                        return $"{CargoName(kvp.Key)}: {kvp.Value} ({(limpetsAbandoned * 100.0 / totalLimpets):N0}% wasted)";
                    }
                    else if (limpetsSynthed > 0)
                    {
                        return $"{CargoName(kvp.Key)}: {kvp.Value} ({limpetsSynthed} short)";
                    }
                }
                return $"{CargoName(kvp.Key)}: {kvp.Value}";
            }));
            Debug.WriteLine("\t--Cargo Notification update: {0}; {1}", cargoTitle, cargoDetail.Replace(Environment.NewLine, "; "));

            if (Core.IsLogMonitorBatchReading || (cargoNotification == Guid.Empty && !newNotification))
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
            };
            if (cargoNotification == Guid.Empty)
            {
                cargoNotification = Core.SendNotification(args);
            }
            else
            {
                Core.UpdateNotification(cargoNotification, args);
            }
        }

        private void AddOrUpdateProspectorNotification(int counter, NotificationArgs args)
        {
            Debug.WriteLine("ProspectedAsteroid: {0}; {1}", args.Title, args.Detail);

            if (!settings.ShowProspectorNotifications || Core.IsLogMonitorBatchReading) return;

            // This method supports notifications with timeout OR persistent notifications.
            if (args.Timeout > 0 && prospectorNotifications[counter % 2] != Guid.Empty)
            {
                // Notifications have a time out: Guid should be assumed invalid. Close it explicitly and clear the guid.
                // Would be nice if we could interrogate Core for the state of a notification ID (ie. is the Guid still associated with a valid notification)?
                Core.CancelNotification(prospectorNotifications[counter % 2]);
                prospectorNotifications[counter % 2] = Guid.Empty;
            }

            if (prospectorNotifications[counter % 2] == Guid.Empty)
            {
                prospectorNotifications[counter % 2] = Core.SendNotification(args);
            }
            else
            {
                Core.UpdateNotification(prospectorNotifications[counter % 2], args);
            }
        }

        private void Reset(string caller, bool closeStaticNotificationsToo = false)
        {
            bool notificationsClosed = false;
            for (int i = 0; i < prospectorNotifications.Length; i++)
            {
                Guid notification = prospectorNotifications[i];
                if (notification != Guid.Empty)
                {
                    Core.CancelNotification(notification);
                    prospectorNotifications[i] = Guid.Empty;
                    notificationsClosed = true;
                }
            }
            if (closeStaticNotificationsToo) notificationsClosed |= MaybeCloseCargoNotification();

            // Reset stats.
            if (notificationsClosed || prospectorsEngaged > 0)
                Debug.WriteLine("\t--{0} - Reset; Stats: prospectorsEngaged: {1}, limpetsUsed: {2}, limpetsSynthed: {3}, limpetsAbandoned: {4}, goodRocks: {5}", caller, prospectorsEngaged, limpetsUsed, limpetsSynthed, limpetsAbandoned, goodRocks);

            prospectorsEngaged = 0;
            limpetsSynthed = 0;
            limpetsUsed = 0;
            limpetsAbandoned = 0;
            goodRocks = 0;
            // cargoMax can get updated by the next Loadout.
        }
        
        private bool MaybeCloseCargoNotification()
        {
            if (cargoNotification != Guid.Empty)
            {
                // Close the notification.
                Core.CancelNotification(cargoNotification);
                cargoNotification = Guid.Empty;
                return true;
            }
            return false;
        }

        private void OnScan(Scan scan)
        {
            if (scan.Rings == null || scan.Rings.IsEmpty || alreadyReportedScansSaaSignals.Contains(scan.BodyName) || !settings.MentionPotentiallyMineableRings || !settings.MentionableRings.HasValue) return;

            alreadyReportedScansSaaSignals.Add(scan.BodyName);
            List<Tuple<string,string,string>> ringsOfInterest = new();
            RingType mentionableRings = settings.MentionableRings.Value;
            foreach (Ring ring in scan.Rings)
            {
                // Ignore belts.
                if (!ring.Name.Contains("Ring")) continue;

                // Find rings of interest
                foreach (RingType rt in Enum.GetValues(typeof(RingType)))
                {
                    if (mentionableRings.HasFlag(rt) && ring.RingClass.Contains(rt.MatchString()))
                    {
                        double densityMTperkm3 = Math.Round(ring.MassMT / ((Math.PI * Math.Pow(ring.OuterRad / 1000, 2)) - (Math.PI * Math.Pow(ring.InnerRad / 1000, 2.0))));
                        if (densityMTperkm3 < minRingDensity)
                        {
                            Debug.WriteLine("Scan: Ignoring interesting ring with low density: {0}", densityMTperkm3);
                            break;
                        }
                        string desiredCommodities = string.Join(", ", settings.DesirableCommonditiesByRingType(rt).Select(c => c.ToString()));
                        var tuple = new Tuple<string, string, string>(
                            $"{rt.DisplayString()} Ring", $"[{desiredCommodities}]", $"Density: {Math.Floor(densityMTperkm3)}");
                        if (!ringsOfInterest.Contains(tuple)) ringsOfInterest.Add(tuple);
                        break;
                    }
                }
            }
            if (ringsOfInterest.Count == 0) return;

            var shortBodyName = GetBodyName(scan.BodyName);
            var detailsCommaSeparated = string.Join(", ", ringsOfInterest.Select(t => $"{t.Item1} {t.Item2}, {t.Item3}"));
            var bodyDistance = $", distance: {Math.Floor(scan.DistanceFromArrivalLS)} Ls";
            Debug.WriteLine("Scan: Interesting rings at body {0}: {1}", shortBodyName, detailsCommaSeparated + bodyDistance);

            // Recreate details string without density for the grid.
            detailsCommaSeparated = string.Join(", ", ringsOfInterest.Select(t => $"{t.Item1} {t.Item2}"));
            Core.AddGridItem(this, new ProspectorGrid()
            {
                Location = scan.BodyName,
                Details = detailsCommaSeparated + bodyDistance,
            });
            if (!Core.IsLogMonitorBatchReading)
            {
                Core.SendNotification(new NotificationArgs()
                {
                    Title = $"Body {shortBodyName}",
                    Detail = string.Join(Environment.NewLine, string.Join(", ", ringsOfInterest.Select(t => t.Item1))),
                });
            }
        }

        private void OnProspectedAsteroid(ProspectedAsteroid prospected)
        {
            int prospectorId = prospectorsEngaged++; // Kind-of assumes things land in the order they're launched. But meh.
            if (prospected.Remaining < 100)
            {
                AddOrUpdateProspectorNotification(prospectorId, MakeProspectorNotificationArgs("Depleted", "", prospectorId, NotificationRendering.NativeVisual));
                return;
            }

            // Separate grid entry, but combine it with other notifications.
            string highMaterialContent = "";
            if (settings.ProspectHighMaterialContent && prospected.Content == "$AsteroidMaterialContent_High;")
            {
                highMaterialContent = " and has high material content";
                Core.AddGridItem(this, new ProspectorGrid
                {
                    Location = !currentLocationShown ? currentLocation : string.Empty,
                    Commodity = "Raw materials",
                    Percentage = "High",
                    Details = string.Empty,
                });
                if (!currentLocationShown) currentLocationShown = true;
            }

            Commodities ml;
            if (Enum.TryParse<Commodities>(prospected.MotherlodeMaterial, true, out ml) && settings.getFor(ml))
            {
                // Found a core of interest!
                goodRocks++;
                string name = CommodityName(prospected.MotherlodeMaterial, prospected.MotherlodeMaterial_Localised);
                string cumulativeStats = $"{(goodRocks * 1000) / (prospectorsEngaged * 10.0):N1}% good rocks ({goodRocks}/{prospectorsEngaged})";
                Core.AddGridItem(this, new ProspectorGrid
                {
                    Location = !currentLocationShown ? currentLocation : string.Empty,
                    Commodity = name,
                    Percentage = "Core found",
                    Details = cumulativeStats,
                });
                if (!currentLocationShown) currentLocationShown = true;

                NotificationArgs args = MakeProspectorNotificationArgs(
                    "Core found", $"Asteroid contains core of {name}{highMaterialContent}", prospectorId, NotificationRendering.All);
                AddOrUpdateProspectorNotification(prospectorId, args);
                Debug.WriteLine("\t--Grid Update: {0}; {1}, {2}", name, "Core found", cumulativeStats);
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

                if (settings.getFor(c))
                {
                    string name = CommodityName(m.Name, m.Name_Localised);
                    desireableCommodities.Add(name, m.Proportion);
                    desireableCommoditiesPercentSum += m.Proportion;
                }
            }

            if (desireableCommodities.Count > 0 && desireableCommoditiesPercentSum > settings.MinimumPercent)
            {
                goodRocks++;
                string title = "";
                string details = "";
                string commodities = String.Join(", ", desireableCommodities.Keys);
                string percentageString = $"{desireableCommoditiesPercentSum:N2}%";
                string cumulativeStats = $"{(goodRocks * 1000) / (prospectorsEngaged * 10.0):N1}% good rocks ({goodRocks}/{prospectorsEngaged})";

                Core.AddGridItem(this, new ProspectorGrid
                {
                    Location = !currentLocationShown ? currentLocation : string.Empty,
                    Commodity = commodities,
                    Percentage = percentageString,
                    Details = cumulativeStats
                });
                if (!currentLocationShown) currentLocationShown = true;

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
                Debug.WriteLine("\t--Grid Update: {0}; {1}, {2}", commodities, percentageString, cumulativeStats);
                return;
            }
            // If we got here, we didn't find anything interesting.
            if (string.IsNullOrEmpty(highMaterialContent))
            {
                AddOrUpdateProspectorNotification(prospectorId, MakeProspectorNotificationArgs("Nothing here", "", prospectorId, NotificationRendering.NativeVisual));
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
            if (String.IsNullOrEmpty(currentSystem)) return;
            var ringName = GetBodyName(saaSignalsFound.BodyName);
            if (saaSignalsFound.Signals == null || saaSignalsFound.Signals.Count == 0 || alreadyReportedScansSaaSignals.Contains(ringName))
                return;

            alreadyReportedScansSaaSignals.Add(ringName);
            Dictionary<string, int> desireableCommodities = new();
            List<string> notificationDetail = new();

            foreach (var m in saaSignalsFound.Signals)
            {
                Commodities c;
                if (!Enum.TryParse<Commodities>(m.Type, true, out c))
                {
                    continue;
                }

                if (settings.getFor(c))
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
                    if (!currentLocationShown) currentLocationShown = true;
                }
            }

            if (notificationDetail.Count > 0)
            {
                Debug.WriteLine("SAASignalsFound: {0}: {1} contains: {2}", "Hotspots of interest", ringName, string.Join(", ", notificationDetail));

                if (Core.IsLogMonitorBatchReading) return;

                Core.SendNotification(new NotificationArgs
                {
                    Title = "Hotspots of interest",
                    Detail = $"{ringName} contains:{Environment.NewLine}{string.Join(Environment.NewLine, notificationDetail)}",
                });
            }
        }

        private void MaybeUpdateCurrentSystem(string starSystem)
        {
            if (starSystem != null && currentSystem != starSystem)
            {
                //Debug.WriteLine("MaybeUpdateCurrentSystem: Updating to {0}, clearing grid", (object)currentSystem);
                currentSystem = starSystem;
                currentLocation = null;
                currentLocationShown = false;
                alreadyReportedScansSaaSignals.Clear();
                if (!Core.IsLogMonitorBatchReading) Core.ClearGrid(this, new ProspectorGrid());
            }
        }

        private void MaybeUpdateCurrentLocation(string body)
        {
            //if (body != currentLocation && !string.IsNullOrEmpty(body)) Debug.WriteLine("MaybeUpdateCurrentLocation: Updating to {0}", (object)body);
            currentLocation = body;
            currentLocationShown = false;
        }

        private static NotificationArgs MakeProspectorNotificationArgs(string title, string detail, int index, NotificationRendering rendering)
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
            };
            return args;
        }

        private string GetBodyName(string bodyName, string baseName = "")
        {
            return string.IsNullOrEmpty(baseName) ? bodyName.Replace(currentSystem, "").Trim() : bodyName.Replace(baseName, "").Trim();
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
        public string Location { get; set; }
        public string Commodity { get; set; }
        public string Percentage { get; set; }
        public string Details { get; set; }
    }
}
