using System.Diagnostics;

namespace com.github.fredjk_gh.PluginCommon.PluginInterop
{
    public class PluginTracker(PluginTracker.PluginType watcher)
    {
        private readonly Dictionary<PluginType, PluginVersionNumber> _activePlugins = [];

        public enum PluginType
        {
            Unknown,
            vithigar_Botanist,
            vithigar_Explorer,
            vithigar_Herald,
            fredjk_Archivist,
            fredjk_Aggregator,
            fredjk_AutoUpdater,
            fredjk_Commander,
            fredjk_Helm,
            fredjk_Prospector,
            fredjk_StatScanner,
            mattg_AstroAnalytica,
            mattg_BioInsights,
            mattg_BoxelStats,
            mattg_Colliders,
            mattg_Evaluator,
            mattg_Hecate,
            mattg_StellarOverlay,
            mcmuttons_GeoPredictor,
        }

        public PluginType MarkActive(Guid pluginGuid, PluginVersionNumber version)
        {
            if (pluginGuid == Guid.Empty) return PluginType.Unknown;
            PluginType type = PluginConstants.PluginTypeByGuid.GetValueOrDefault(pluginGuid);

            if (type == PluginType.Unknown)
            {
                Debug.WriteLine($"PluginTracker({watcher}): Unrecognized plugin guid: {pluginGuid}");
                return type;
            }

            _activePlugins.Add(type, version);

            Debug.WriteLine($"PluginTracker({watcher}): Added {type} to known active plugins: {string.Join(",", _activePlugins)}");
            return type;
        }

        public void MarkActive(PluginType type, PluginVersionNumber version)
        {
            if (type == PluginType.Unknown) return;

            _activePlugins.Add(type, version);
        }

        // TODO: Deprecate.
        public PluginType MarkActive(string pluginString, PluginVersionNumber version)
        {
            if (string.IsNullOrEmpty(pluginString)) return PluginType.Unknown;
            PluginType type = PluginConstants._knownPluginStrings.GetValueOrDefault(pluginString.ToLower(), PluginType.Unknown);

            if (type == PluginType.Unknown)
            {
                Debug.WriteLine($"PluginTracker({watcher}): Unrecognized plugin string: {pluginString}");
                return type;
            }

            _activePlugins.Add(type, version);

            Debug.WriteLine($"PluginTracker({watcher}): Added {type} to known active plugins: {string.Join(",", _activePlugins)}");
            return type;
        }

        public bool IsActive(PluginType type)
        {
            if (type == PluginType.Unknown) return false;

            return _activePlugins.ContainsKey(type);
        }

        public int? ComparePluginActiveVersionTo(PluginType type, PluginVersionNumber minVer)
        {
            if (type == PluginType.Unknown || minVer == null) return null;

            var activeVer = GetActiveVersion(type);

            if (!_activePlugins.ContainsKey(type) || activeVer == null) return null;

            return activeVer.CompareTo(minVer);
        }

        public PluginVersionNumber GetActiveVersion(PluginType type)
        {
            if (type == PluginType.Unknown)return null;

            return _activePlugins.GetValueOrDefault(type, null);
        }
    }
}
