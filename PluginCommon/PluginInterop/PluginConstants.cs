using static com.github.fredjk_gh.PluginCommon.PluginInterop.PluginTracker;

namespace com.github.fredjk_gh.PluginCommon.PluginInterop
{
    public static class PluginConstants
    {
        // Matt G's
        public const string PLUGIN_ASTROANALYTICA = "AstroAnalytica"; // Double check
        public const string PLUGIN_BIOINSIGHTS = "Bioinsights"; // Double check
        public const string PLUGIN_EVALUATOR = "Evaluator";
        public const string PLUGIN_HECATE = "Hecate";
        public const string PLUGIN_STELLAR_OVERLAY = "Stellar Overlay"; // Double check

        // Fred JK's
        public const string PLUGIN_AGGREGATOR = "Notification Aggregator";
        public const string PLUGIN_ARCHIVIST = "Archivist";
        public const string PLUGIN_HELM = "Helm";
        public const string PLUGIN_PROSPECTOR = "Prospector Basic";
        public const string PLUGIN_FLEET_COMMANDER = "Fleet Commander";
        public const string PLUGIN_STAT_SCANNER = "Stat Scanner";

        // McMuttons'
        public const string PLUGIN_GEOPREDICTOR = "GeoPredictor";

        internal static readonly Dictionary<string, PluginType> _knownPluginStrings = new()
        {
            { "geopredictor", PluginType.mcmuttons_GeoPredictor },
            { "astroanalytica", PluginType.mattg_AstroAnalytica },
            { "boxelstats", PluginType.mattg_BoxelStats },
            { "boxel stats", PluginType.mattg_BoxelStats },
            { "bioinsights", PluginType.mattg_BioInsights },
            { "colliders", PluginType.mattg_Colliders },
            { "evaluator", PluginType.mattg_Evaluator },
            { "hecate", PluginType.mattg_Hecate },
            { "hecate - data vault", PluginType.mattg_Hecate },
            { "stellar overlay", PluginType.mattg_StellarOverlay },
            { "stellaroverlay", PluginType.mattg_StellarOverlay },
            { "notification aggregator", PluginType.fredjk_Aggregator },
            { "aggregator", PluginType.fredjk_Aggregator },
            { "archivist", PluginType.fredjk_Archivist },
            { "fleet commander", PluginType.fredjk_Commander },
            { "commander", PluginType.fredjk_Commander },
            { "helm", PluginType.fredjk_Helm },
            { "fredjk autoupdater", PluginType.fredjk_AutoUpdater },
            { "autoupdater", PluginType.fredjk_AutoUpdater },
            { "prospector basic", PluginType.fredjk_Prospector },
            { "prospector", PluginType.fredjk_Prospector },
            { "stat scanner", PluginType.fredjk_StatScanner },
            { "statscanner", PluginType.fredjk_StatScanner },
            { "observatory botanist", PluginType.vithigar_Botanist },
            { "botanist", PluginType.vithigar_Botanist },
            { "observatory explorer", PluginType.vithigar_Explorer },
            { "explorer", PluginType.vithigar_Explorer },
            { "observatory herald", PluginType.vithigar_Herald },
            { "herald", PluginType.vithigar_Herald },
        };

        public static readonly Dictionary<Guid, PluginType> PluginTypeByGuid = new()
        {
            //{ new(""), PluginType.mcmuttons_GeoPredictor },
            { new("4d617474-4704-4153-5452-4f414e414c59"), PluginType.mattg_AstroAnalytica },
            //{ new(""), PluginType.mattg_BoxelStats },
            { new("4d617474-4701-4249-4f49-4e5349474854"), PluginType.mattg_BioInsights },
            //{ new(""), PluginType.mattg_Colliders },
            { new("4d617474-4702-0045-5641-4c5541544f52"), PluginType.mattg_Evaluator },
            { new("4d617474-4707-0000-0000-484543415445"), PluginType.mattg_Hecate },
            { new("4d617474-4703-5354-454c-4c41524f5645"), PluginType.mattg_StellarOverlay },
            { new("b4654977-434b-424f-a9fd-b2ac5be9915a"), PluginType.fredjk_Aggregator },
            { new("0bec76f9-772b-4b0b-80fd-4809d23d394e"), PluginType.fredjk_Archivist },
            { new("95dcc3c8-e52f-47c1-ac16-4f548a54e030"), PluginType.fredjk_Commander },
            { new("38425041-6ff2-4b9d-a10c-420839972f79"), PluginType.fredjk_Helm },
            { new("a4a03c40-561f-4de9-878a-8ffd0285c17d"), PluginType.fredjk_AutoUpdater },
            { new("7e43a6bd-3ccc-486a-b027-3b9fb8258bcc"), PluginType.fredjk_Prospector },
            { new("398750b9-ffab-4d28-959b-3fc5648853eb"), PluginType.fredjk_StatScanner },
            { new("116D68F6-1C21-49AD-AE33-F0FEF619AF24"), PluginType.vithigar_Botanist },
            { new("B11E8725-3F71-4445-9B09-682F9EAB8B59"), PluginType.vithigar_Explorer },
            { new("AEA9A421-A19C-421F-A47B-5678956EC202"), PluginType.vithigar_Herald },
        };

        public static readonly Dictionary<PluginType, Guid> GuidByPluginType;

        static PluginConstants()
        {
            // This list is 1:1, so this should work!
            GuidByPluginType = PluginTypeByGuid.ToDictionary(e => e.Value, e => e.Key);
        }
    }
}
