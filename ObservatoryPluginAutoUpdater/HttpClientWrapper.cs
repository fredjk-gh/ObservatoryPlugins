using Observatory.Framework.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace com.github.fredjk_gh.ObservatoryPluginAutoUpdater
{
    internal class HttpClientWrapper(IObservatoryCore core, IObservatoryWorker worker) : IHttpClientWrapper
    {
        public Dictionary<string, PluginVersion> GetLatestVersions(string latestReleasesUrl)
        {
            Dictionary<string, PluginVersion> latestVersions;
            var options = new JsonSerializerOptions() {
                AllowTrailingCommas = true,
            };
            var latestVersionsTask = core.HttpClient.GetFromJsonAsync<List<PluginVersion>>(latestReleasesUrl, options);
            try
            {
                latestVersions = latestVersionsTask.Result.ToDictionary(v => v.PluginName, v => v);
            }
            catch (Exception ex)
            {
                core.GetPluginErrorLogger(worker)(ex, "Failed to fetch latest versions");
                return [];
            }
            return latestVersions;
        }

        public Stream GetStream(string url)
        {
            var downloadTask = core.HttpClient.GetStreamAsync(url);
            try
            {
                return downloadTask.Result;
            }
            catch (Exception ex)
            {
                core.GetPluginErrorLogger(worker)(ex, $"Failed to fetch from {url}");
                return null;
            }
        }
    }
}