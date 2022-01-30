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
            MinimumPercent = 10,
            ProspectTritium = true,
            ProspectPlatinum = true,
        };
        private bool readAllInProgress = false;

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
                case SupercruiseEntry supercruiseEntry:
                    // Reset when we jump to supercruise or another system.
                    Reset();
                    Debug.WriteLine("SupercruiseEntry: Closing prospector notifications, resetting stats...");
                    break;
                case FSDJump fsdJump:
                    // Reset when we jump to supercruise or another system.
                    Reset();
                    Debug.WriteLine("FSDJump: Closing prospector notifications, resetting stats...");
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
                    Debug.WriteLine("Loadout: New cargoMax: {0}", loadout.CargoCapacity);
                    UpdateCargoNotification(false /* newNotification */);
                    break;
                case Cargo cargoEvent:
                    cargoCur = cargoEvent.Count;
                    if (cargoEvent.Inventory != null && !cargoEvent.Inventory.IsEmpty) // Usually on game load or loadout change.
                    {
                        Debug.WriteLine("Cargo w/Inventory: cargoCur: {0}", cargoCur);
                        Reset();
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
                        Reset();
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
                    Debug.WriteLine("Game client shutdown: Closing notifications, resetting stats...");
                    Reset(true);
                    break;
            }
        }

        private void UpdateCargoNotification(bool newNotification = true)
        {
            int cargoEstimate = cargo.Values.Sum();
            if (cargoEstimate == 0 || (cargo.Count == 1 && cargo.ContainsKey(LimpetDronesKey)))
            {
                MaybeCloseCargoNotification();
                Debug.WriteLine("\t--Cargo Notification closed; no cargo or only limpets remaining.");
                return;
            }

            string cargoTitle = cargoMax == null ? "Cargo" : string.Format("Cargo ({0} / {1})", cargoEstimate, cargoMax);
            string cargoDetail = string.Join("\n", cargo.Where(kvp => kvp.Value > 0).Select(kvp =>
            {
                if (kvp.Key == LimpetDronesKey)
                {
                    if (limpetsAbandoned > 0)
                    {
                        int totalLimpets = limpetsAbandoned + limpetsUsed;
                        return string.Format("{0}: {1} ({2:N0}% wasted)", CargoName(kvp.Key), kvp.Value, limpetsAbandoned * 100.0 / totalLimpets);
                    }
                    else if (limpetsSynthed > 0)
                    {
                        return string.Format("{0}: {1} ({2} short)", CargoName(kvp.Key), kvp.Value, limpetsSynthed);
                    }
                }
                return string.Format("{0}: {1}", CargoName(kvp.Key), kvp.Value);
            }));
            Debug.WriteLine("\t--Cargo Notification update: {0}; {1}", cargoTitle, cargoDetail.Replace("\n", "; "));

            if (readAllInProgress || (cargoNotification == Guid.Empty && !newNotification))
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

            if (readAllInProgress) return;

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

        private void Reset(bool closeStaticNotificationsToo = false)
        {
            for (int i = 0; i < prospectorNotifications.Length; i++)
            {
                Guid notification = prospectorNotifications[i];
                if (notification != Guid.Empty)
                {
                    Core.CancelNotification(notification);
                    prospectorNotifications[i] = Guid.Empty;
                }
            }
            if (closeStaticNotificationsToo) MaybeCloseCargoNotification();

            // Reset stats.
            Debug.WriteLine("\t--Reset; Stats: prospectorsEngaged: {0}, limpetsUsed: {1}, limpetsSynthed: {2}, limpetsAbandoned: {3}, goodRocks: {4}", prospectorsEngaged, limpetsUsed, limpetsSynthed, limpetsAbandoned, goodRocks);

            prospectorsEngaged = 0;
            limpetsSynthed = 0;
            limpetsUsed = 0;
            limpetsAbandoned = 0;
            goodRocks = 0;
            // cargoMax can get updated by the next Loadout.
        }
        
        private void MaybeCloseCargoNotification()
        {
            if (cargoNotification != Guid.Empty)
            {
                // Close the notification.
                Core.CancelNotification(cargoNotification);
                cargoNotification = Guid.Empty;
            }
        }

        private void OnProspectedAsteroid(ProspectedAsteroid prospected)
        {
            int prospectorId = prospectorsEngaged++; // Kind-of assumes things land in the order they're launched. But meh.
            if (prospected.Remaining < 100)
            {
                AddOrUpdateProspectorNotification(prospectorId, makeProspectorNotificationArgs("Depleted", "", prospectorId, NotificationRendering.NativeVisual));
                return;
            }

            // Separate grid entry, but combine it with other notifications.
            string highMaterialContent = "";
            if (settings.ProspectHighMaterialContent && prospected.Content == "$AsteroidMaterialContent_High;")
            {
                highMaterialContent = " and has high material content";
                Core.AddGridItem(this, new ProspectorGrid
                {
                    Commodity = "Raw materials",
                    Percentage = "High",
                    CumulativeStats = "",
                });
            }

            Commodities ml;
            if (Enum.TryParse<Commodities>(prospected.MotherlodeMaterial, true, out ml) && settings.getFor(ml))
            {
                // Found a core of interest!
                goodRocks++;
                string name = CommodityName(prospected.MotherlodeMaterial, prospected.MotherlodeMaterial_Localised);
                string cumulativeStats = string.Format("{2:N1}% good rocks ({0}/{1})", goodRocks, prospectorsEngaged, (goodRocks * 1000) / (prospectorsEngaged * 10.0));
                Core.AddGridItem(this, new ProspectorGrid
                {
                    Commodity = name,
                    Percentage = "Core found",
                    CumulativeStats = cumulativeStats,
                });

                NotificationArgs args = makeProspectorNotificationArgs(
                    "Core found",
                    string.Format("Asteroid contains core of {0}{1}", name, highMaterialContent), prospectorId, NotificationRendering.All);
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
                string percentageString = String.Format("{0:N2}%", desireableCommoditiesPercentSum);
                string cumulativeStats = string.Format("{2:N1}% good rocks ({0}/{1})", goodRocks, prospectorsEngaged, (goodRocks * 1000) / (prospectorsEngaged * 10.0));

                Core.AddGridItem(this, new ProspectorGrid
                {
                    Commodity = commodities,
                    Percentage = percentageString,
                    CumulativeStats = cumulativeStats
                }); ;

                if (desireableCommodities.Count > 1)
                {
                    // Ooh, multiple things here!
                    title = "Good multiple commodity rock";
                    details = string.Format("Asteroid is a combined {0:N0} percent {1}{2}", desireableCommoditiesPercentSum, commodities, highMaterialContent);
                }
                else
                {
                    title = "Good rock";
                    details = string.Format("Asteroid is {0:N0} percent {1}{2}", desireableCommoditiesPercentSum, commodities, highMaterialContent);
                }
                AddOrUpdateProspectorNotification(prospectorId, makeProspectorNotificationArgs(title, details, prospectorId, NotificationRendering.All));
                Debug.WriteLine("\t--Grid Update: {0}; {1}, {2}", commodities, percentageString, cumulativeStats);
                return;
            }
            // If we got here, we didn't find anything interesting.
            if (string.IsNullOrEmpty(highMaterialContent))
            {
                AddOrUpdateProspectorNotification(prospectorId, makeProspectorNotificationArgs("Nothing here", "", prospectorId, NotificationRendering.NativeVisual));
            }
            else
            {
                string details = "";
                if (desireableCommodities.Count > 0)
                {
                    details = string.Format("Asteroid also contains {0:N0} percent of other desireable commodities", desireableCommoditiesPercentSum);
                }
                AddOrUpdateProspectorNotification(prospectorId, makeProspectorNotificationArgs("High raw material content", details, prospectorId, NotificationRendering.All));
            }
        }

        private NotificationArgs makeProspectorNotificationArgs(string title, string detail, int index, NotificationRendering rendering)
        {
            NotificationArgs args = new()
            {
                Title = title,
                Detail = detail,
                Timeout = 300 * 1000, // 5 minutes
                XPos = 1,
                YPos = ((index % 2) + 1) * 15,
                Rendering = rendering,
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

        public void ReadAllStarted()
        {
            readAllInProgress = true;
            Core.ClearGrid(this, new ProspectorGrid());
        }

        public void ReadAllFinished()
        {
            readAllInProgress = false;
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
        public string Commodity { get; set; }
        public string Percentage { get; set; }
        public string CumulativeStats { get; set; }
    }
}
