using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ObservatoryPlugins.Tests")]
namespace com.github.fredjk_gh.ObservatoryPluginAutoUpdater
{
    internal class PluginVersion
    {
        public string PluginName { get; set; }
        public VersionDetail Production { get; set; }
        public VersionDetail Beta { get; set; }

        public bool IsNewerThan(PluginVersion other, bool allowBeta)
        {
            var thisVersion = parseVersion(
                allowBeta && !string.IsNullOrEmpty(Beta?.Version) ? Beta.Version : Production?.Version);
            var otherVersion = parseVersion(
                allowBeta && !string.IsNullOrEmpty(other.Beta?.Version) ? other.Beta.Version : other.Production?.Version);

            for (int i = 0; i < thisVersion.Length && i < otherVersion.Length; i++)
            {
                if (thisVersion[i] > otherVersion[i])
                {
                    return true;
                }
                else if (thisVersion[i] < otherVersion[i])
                {
                    return false; // All previous were equal, thus we can't be newer.
                }
            }

            return false;
        }

        private int[] parseVersion(string version)
        {
            if (string.IsNullOrEmpty(version)) return new int[0];

            string[] parts = version.Split('.');
            int[] numericParts = new int[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                numericParts[i] = int.Parse(parts[i]);
            }
            return numericParts;
        }
    }

    internal class VersionDetail
    {
        internal const string NO_VERSION = "0.0.0.0";
        private string _version = NO_VERSION;
        private string _downloadUrl = "";

        public string Version { get => _version; set => _version = value; }
        public string DownloadURL { get => _downloadUrl; set => _downloadUrl = value; }
    }
}
