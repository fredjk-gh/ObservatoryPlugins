using com.github.fredjk_gh.ObservatoryPluginAutoUpdater.UI;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ObservatoryPlugins.Tests")]
namespace com.github.fredjk_gh.ObservatoryPluginAutoUpdater
{
    internal class PluginVersion
    {
        public string PluginName { get; set; }
        public VersionDetail Production { get; set; }
        public VersionDetail Beta { get; set; }

        public static VersionPair SelectVersion(PluginVersion local, PluginVersion latest, bool allowBeta, string coreVersionStr)
        {
            List<VersionPair> availableVersions = [];
            if (allowBeta && latest.Beta != null && latest.Beta?.Version != VersionDetail.NO_VERSION)
            {
                availableVersions.Add(new()
                {
                    Name = "Beta",
                    Local = local?.Beta ?? local?.Production,
                    Latest = latest.Beta,
                    Action = PluginAction.UpdateBeta,
                });
            }
            if (latest.Production != null && latest.Production?.Version != VersionDetail.NO_VERSION)
            {
                availableVersions.Add(new()
                {
                    Name = "Production",
                    Local = local?.Production,
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
                var localVersion = considered.Local?.VersionParsed ?? VersionDetail.NO_VERSION_PARSED;
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

    internal class VersionDetail
    {
        internal const string NO_VERSION = "0.0.0.0";
        internal static readonly int[] NO_VERSION_PARSED = [0, 0, 0, 0];
        private string _version = NO_VERSION;
        private string _downloadUrl = "";

        public string Version { get => _version; set => _version = value; }
        public string DownloadURL { get => _downloadUrl; set => _downloadUrl = value; }
        public string MinRequiredCoreVersion { get; set; }

        internal int[] VersionParsed { get => ParseVersion(Version); }
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

    internal class VersionPair
    {
        public string Name { get; set; }
        public VersionDetail Local { get; set; }
        public VersionDetail Latest { get; set; }

        public PluginAction Action { get; set; }
    }
}
