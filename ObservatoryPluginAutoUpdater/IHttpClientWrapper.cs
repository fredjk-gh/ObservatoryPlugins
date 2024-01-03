
namespace com.github.fredjk_gh.ObservatoryPluginAutoUpdater
{
    internal interface IHttpClientWrapper
    {
        Dictionary<string, PluginVersion> GetLatestVersions(string latestReleasesUrl);
        Stream GetStream(string url);
    }
}