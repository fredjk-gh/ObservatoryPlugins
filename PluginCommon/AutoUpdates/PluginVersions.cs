using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.PluginCommon.AutoUpdates
{
    public enum PluginAction
    {
        None,
        InstallStable,
        InstallBeta,
        UpdateStable,
        UpdateBeta,
    }

    public class PluginVersions
    {
        public string PluginName { get; set; }
        public VersionDetail Production { get; set; }
        public VersionDetail Beta { get; set; }

        public static SelectedVersion SelectVersion(PluginVersions local, PluginVersions latest, bool allowBeta, string coreVersionStr)
        {
            List<SelectedVersion> availableVersions = [];
            if (allowBeta && latest.Beta != null && latest.Beta?.Version != VersionDetail.NO_VERSION)
            {
                availableVersions.Add(new()
                {
                    Latest = latest.Beta,
                    Action = PluginAction.UpdateBeta,
                });
            }
            if (latest.Production != null && latest.Production?.Version != VersionDetail.NO_VERSION)
            {
                availableVersions.Add(new()
                {
                    Latest = latest.Production,
                    Action = PluginAction.UpdateStable,
                });
            }

            var coreVersion = VersionDetail.ParseVersion(coreVersionStr);
            foreach (var considered in availableVersions)
            {
                var minRequiredCoreVersion = considered.Latest.MinRequiredCoreVersionParsed;
                if (minRequiredCoreVersion.Length > 0 && VersionDetail.Compare(coreVersion, minRequiredCoreVersion) < 0)
                {
                    // Core version is too old; incompatible release.
                    continue;
                }

                var localVersion = local?.Production?.VersionParsed ?? VersionDetail.NO_VERSION_PARSED;
                if (considered.Action == PluginAction.UpdateBeta && local != null && local.Beta != null)
                {
                    localVersion = local.Beta.VersionParsed;
                }
                var latestVersion = considered.Latest.VersionParsed;

                if (VersionDetail.Compare(localVersion, latestVersion) < 0)
                {
                    // Local version is older and it's compatible. We have an update.
                    return considered;
                }
            }
            return null;
        }
    }

    public class VersionDetail
    {
        internal const string LABEL_PRODUCTION = "Stable";
        internal const string LABEL_BETA = "Beta";

        internal const string NO_VERSION = "0.0.0.0";
        internal static readonly int[] NO_VERSION_PARSED = [0, 0, 0, 0];
        private string _version = NO_VERSION;
        private string _minReqCoreVersion = NO_VERSION;
        private string _downloadUrl = "";

        public static VersionDetail ForProduction(string version)
        {
            return Create(LABEL_PRODUCTION, version);
        }

        public static VersionDetail ForBeta(string version)
        {
            return Create(LABEL_BETA, version);
        }

        internal static VersionDetail Create(string label, string version, string minReqCoreVersion = "", string url = "")
        {
            return new VersionDetail
            {
                Label = label,
                Version = version,
                MinRequiredCoreVersion = String.IsNullOrWhiteSpace(minReqCoreVersion) ? NO_VERSION : minReqCoreVersion,
                DownloadURL = url,
            };

        }
        public string Label { get; set; }
        public string Version
        {
            get => _version;
            set => _version = value?.Replace("v", "").Trim();
        }
        public string DownloadURL { get => _downloadUrl; set => _downloadUrl = value; }
        public string MinRequiredCoreVersion
        {
            get => _minReqCoreVersion;
            set => _minReqCoreVersion = value?.Replace("v", "").Trim();
        }

        [JsonIgnore]
        internal int[] VersionParsed { get => ParseVersion(Version); }
        [JsonIgnore]
        internal int[] MinRequiredCoreVersionParsed { get => ParseVersion(MinRequiredCoreVersion); }

        internal static int[] ParseVersion(string version)
        {
            if (string.IsNullOrEmpty(version) || version == NO_VERSION) return NO_VERSION_PARSED;

            if (version.StartsWith('v'))
            {
                version = version.Replace("v", ""); // Just in case I'm dumb and leave a v prefix.
            }
            string[] parts = version.Split('.');
            int[] numericParts = new int[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                numericParts[i] = int.Parse(parts[i]);
            }
            return numericParts;
        }

        internal static int Compare(int[] left, int[] right)
        {

            int compareResult = 0; // equal.
            for (int i = 0; i < left.Length && i < right.Length; i++)
            {
                if (left[i] > right[i])
                {
                    return 1;
                }
                else if (left[i] < right[i])
                {
                    return -1; // All previous were equal, thus we can't be newer.
                }
            }

            return compareResult;
        }
    }

    public class SelectedVersion
    {
        public VersionDetail Latest { get; set; }
        public PluginAction Action { get; set; }
    }
}
