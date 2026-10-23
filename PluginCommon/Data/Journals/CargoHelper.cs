using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;

namespace com.github.fredjk_gh.PluginCommon.Data.Journals
{
    public class CargoHelper
    {
        public const string LimpetDronesKey = "drones";
        private const string LimpetDronesName = "Limpets";
        private const string TritiumKey = "tritium";
        private const string TritiumName = "Tritium";

        // This cache is updated by CommodityName. Keys should be lower-case for maximum matchy-ness.
        private static readonly Dictionary<string, string> DisplayNames = new()
        {
            { "$tritium_name;", TritiumName },
            { TritiumKey, TritiumName },
            { "limpet", LimpetDronesName },
            { LimpetDronesKey, LimpetDronesName },
        };

        public static string GetCargoKey(string name, string localizedName = "")
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

        public static string CommodityName(string fallbackName, string localizedName = "")
        {
            // Anything cached?
            if (!string.IsNullOrEmpty(fallbackName) && DisplayNames.TryGetValue(fallbackName, out string displayName1)) return displayName1;
            if (!string.IsNullOrEmpty(localizedName) && DisplayNames.TryGetValue(localizedName, out string displayName2)) return displayName2;

            // Start with fallback, use localized if available.
            string displayName = fallbackName;
            if (!string.IsNullOrEmpty(localizedName))
            {
                displayName = localizedName;
            }

            // A couple overrides:
            if (displayName.Contains("limpet", StringComparison.OrdinalIgnoreCase))
            {
                DisplayNames[displayName] = LimpetDronesName;
                return LimpetDronesName;
            }
            if (!string.IsNullOrEmpty(fallbackName) && fallbackName != displayName) DisplayNames[fallbackName] = displayName;
            return displayName;
        }


        public static void UpdateCargo(Dictionary<string, int> existingInventory, CargoFile cargo)
        {
            UpdateCargoInventory(existingInventory, cargo.Inventory);
        }

        public static void UpdateCargo(Dictionary<string, int> existingInventory, Cargo cargo)
        {
            if (cargo.Inventory is null)
            {
                if (cargo.Count == 0)
                    existingInventory.Clear();
                return;
            }
            UpdateCargoInventory(existingInventory, cargo.Inventory);
        }

        public static int UpdateCargo(Dictionary<string, int> existingInventory, CargoTransferDetail xfer)
        {
            string key = GetCargoKey(xfer.Type, xfer.Type_Localised);

            if (xfer.Direction == CargoTransferDirection.ToShip)
                existingInventory[key] = existingInventory.GetValueOrDefault(key) + xfer.Count;
            else if (xfer.Direction == CargoTransferDirection.ToSRV)
                existingInventory[key] = existingInventory.GetValueOrDefault(key) +- xfer.Count;

            return existingInventory.Values.Sum();
        }

        public static void UpdateCargoInventory(Dictionary<string, int> existingInventory, IEnumerable<CargoType> updatedInventory)
        {
            existingInventory.Clear();
            foreach (var item in updatedInventory)
            {
                string key = CargoHelper.GetCargoKey(item.Name, item.Name_Localised);
                // You may get multiple items with the same key if you're transporting mission-specific cargo.
                // Therefore, the key may already exist.
                if (!existingInventory.ContainsKey(key))
                    existingInventory.Add(key, item.Count);
                else
                    existingInventory[key] += item.Count;
            }
        }
    }
}
