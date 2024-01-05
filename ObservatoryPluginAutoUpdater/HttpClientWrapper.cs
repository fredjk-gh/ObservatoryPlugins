using Observatory.Framework.Interfaces;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime;
using System.Text.Json;

namespace com.github.fredjk_gh.ObservatoryPluginAutoUpdater
{
    internal class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly IObservatoryCore Core;
        private readonly IObservatoryWorker Worker;

        public HttpClientWrapper(IObservatoryCore core, IObservatoryWorker worker)
        {
            Core = core;
            Worker = worker;
        }

        public Dictionary<string, PluginVersion> GetLatestVersions(string latestReleasesUrl)
        {
            Dictionary<string, PluginVersion> latestVersions;
            var options = new JsonSerializerOptions() {
                AllowTrailingCommas = true,
            };
            var latestVersionsTask = Core.HttpClient.GetFromJsonAsync<List<PluginVersion>>(latestReleasesUrl, options);
            try
            {
                latestVersions = latestVersionsTask.Result.ToDictionary(v => v.PluginName, v => v);
            }
            catch (Exception ex)
            {
                Core.GetPluginErrorLogger(Worker)(ex, "Failed to fetch latest versions");
                return new();
            }
            return latestVersions;
        }

        public Stream GetStream(string url)
        {
            var downloadTask = Core.HttpClient.GetStreamAsync(url);
            try
            {
                return downloadTask.Result;
            }
            catch (Exception ex)
            {
                Core.GetPluginErrorLogger(Worker)(ex, $"Failed to fetch from {url}");
                return null;
            }
        }
    }
}