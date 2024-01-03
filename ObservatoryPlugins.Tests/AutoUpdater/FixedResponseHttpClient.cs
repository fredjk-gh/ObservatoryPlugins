using com.github.fredjk_gh.ObservatoryPluginAutoUpdater;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryPlugins.Tests.AutoUpdater
{
    internal class FixedResponseHttpClient : IHttpClientWrapper
    {
        internal Dictionary<string, List<PluginVersion>> JsonResponses = new();
        internal Dictionary<string, Stream> StreamResponses = new();

        internal int GetLatestVersionsCalls = 0;
        internal int GetStreamCalls = 0;

        public Dictionary<string, PluginVersion> GetLatestVersions(string latestReleasesUrl)
        {
            GetLatestVersionsCalls++;
            return JsonResponses[latestReleasesUrl].ToDictionary(v => v.PluginName, v => v);
        }

        public Stream GetStream(string url)
        {
            GetStreamCalls++;
            return StreamResponses[url];
        }
    }
}
