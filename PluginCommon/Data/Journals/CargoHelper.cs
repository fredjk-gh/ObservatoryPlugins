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
    }
}
