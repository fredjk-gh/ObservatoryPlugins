using com.github.fredjk_gh.PluginCommon.PluginInterop.Messages;
using Observatory.Framework;
using Observatory.Framework.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace com.github.fredjk_gh.PluginCommon.AutoUpdates
{
    public static class AutoUpdateHelper
    {
#if DEBUG
        private const string CURRENT_RELEASES_URL = "http://www/Plugins/CurrentReleases.json";
#else
        private const string CURRENT_RELEASES_URL = "https://raw.githubusercontent.com/fredjk-gh/ObservatoryPlugins/main/CurrentReleases.json";
#endif
        private static readonly JsonSerializerOptions FetchOptions = new()
        {
            AllowTrailingCommas = true,
            WriteIndented = true,
        };
        private static Dictionary<string, PluginVersions> _latestVersions = [];
        private static DateTime? _latestVersionsFetchedDateTime = null;
        private static IObservatoryCore _core = null;
        private static readonly object _lock = new();

        public static void Init(IObservatoryCore core)
        {
            if (IsInitialized()) return;
            PluginFolderPath = core.UpdatedPluginsFolder;
            _core = core;
        }

        public static string PluginFolderPath { get; internal set; }

        public static bool IsInitialized()
        {
            return (_core != null);
        }

        public static PluginUpdateInfo CheckForPluginUpdate(IObservatoryPlugin plugin, string relnotesUrl, bool autoUpdatesEnabled = false, bool betaUpdatesEnabled = false)
        {
            if (!IsInitialized()) return null;

            lock (_lock) // Update checks are fired off on an async task. A lock is required to avoid
                         // spamming GitHub with fetches and concurrency issues when writing error logs.
            {
                var latestVersions = GetLatestVersion(plugin);
                if (latestVersions == null) return new();

                var localVersions = GetLocalVersions(plugin.AboutInfo, plugin.Version);
                var selected = PluginVersions.SelectVersion(
                    localVersions, latestVersions, betaUpdatesEnabled, _core.Version);

                if (selected == null) return new();

                string releaseTrack = selected.Latest.Label;
                if (autoUpdatesEnabled)
                {
                    if (DownloadLatestPlugin(plugin, selected.Latest, _core.UpdatedPluginsFolder))
                    {
                        // Or send a notification? Helm may not be installed. (And we probably can't track that yet at this stage of the plugin lifecycle.
                        _core.SendPluginMessage(
                            plugin,
                            HelmStatusMessage.New($"{plugin.AboutInfo.FullName} {selected.Latest.Label} version {selected.Latest.Version} is ready to use; restart Observatory Core.").ToPluginMessage());

                        // Successfully downloaded; provide release notes URL.
                        return new()
                        {
                            Status = PluginUpdateStatus.UpdateReady,
                            Url = relnotesUrl,
                            UrlText = $"{selected.Latest.Version} ({selected.Latest.Label}) release notes; Restart app",
                        };
                    }
                    else
                    {
                        _core.SendPluginMessage(
                            plugin,
                            HelmStatusMessage.New($"{plugin.AboutInfo.FullName} {selected.Latest.Label} version {selected.Latest.Version} is available but could not be automatically updated. See the Core plugin list to download.").ToPluginMessage());

                        // Failed to download, provide download link.
                        return new()
                        {
                            Status = PluginUpdateStatus.UpdateAvailable,
                            Url = selected.Latest.DownloadURL,
                            UrlText = "Update available (auto-update failed)",
                        };
                    }
                }
                else
                {
                    _core.SendPluginMessage(
                        plugin,
                        HelmStatusMessage.New($"{plugin.AboutInfo.FullName} {selected.Latest.Label} version {selected.Latest.Version} is available. See the Core plugin list to download.").ToPluginMessage());

                    return new()
                    {
                        Status = PluginUpdateStatus.UpdateAvailable,
                        Url = selected.Latest.DownloadURL,
                        UrlText = "Update available",
                    };
                }
            }
        }

        internal static PluginVersions GetLatestVersion(IObservatoryPlugin worker)
        {
            if (!IsInitialized()) return null;
            Dictionary<string, PluginVersions> versions = GetLatestVersions(worker);

            if (versions is null)
            {
                // This shouldn't happen often: Either the "CurrentReleases.json" file is corrupt or could not be downloaded.
                _core.GetPluginErrorLogger(worker)(
                    new InvalidDataException($"CurrentReleases.json is missing release information for plugin {worker.AboutInfo.FullName}; please contact the author."),
                    $"AutoUpdateHelper.CheckForPluginUpdate({worker.AboutInfo.FullName})");
                return null;
            }

            return versions.GetValueOrDefault(worker.AboutInfo.FullName, null);
        }

        internal static PluginVersions GetLocalVersions(AboutInfo about, string assemblyVersion)
        {
            return new()
            {
                PluginName = about.FullName,
                Production = VersionDetail.ForProduction(assemblyVersion),
                Beta = VersionDetail.ForBeta(assemblyVersion),
            };
        }

        internal static bool DownloadLatestPlugin(IObservatoryPlugin worker, VersionDetail versionDetail, string destPath)
        {
            if (!IsInitialized()) return false;

            var outputFilename = versionDetail.DownloadURL.Split('/').Last();
            var fileStream = GetStream(worker, versionDetail.DownloadURL);
            if (fileStream != null)
            {
                FileStream f = null;
                try
                {
                    f = File.Create($"{destPath}{Path.DirectorySeparatorChar}{outputFilename}");
                    fileStream.CopyTo(f);
                    fileStream.Flush();
                }
                catch (Exception ex)
                {
                    _core.GetPluginErrorLogger(worker)(
                        new PluginException(worker.AboutInfo.FullName, "Unable to download latest version from GitHub.", ex), "> AutoUpdateHelper.DownloadLatestPlugin()");
                    return false;
                }
                finally
                {
                    fileStream.Close();
                    f?.Close();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        internal static Dictionary<string, PluginVersions> GetLatestVersions(IObservatoryPlugin worker)
        {
            if (!IsInitialized()) return null;
            if (_latestVersionsFetchedDateTime.HasValue && _latestVersionsFetchedDateTime.Value.AddHours(2) > DateTime.Now)
            {
                return _latestVersions;
            }

            // Either we've not fetched yet or it's stale. Go fetch.
            try
            {
                var latestVersionsTask = _core.HttpClient.GetFromJsonAsync<List<PluginVersions>>(CURRENT_RELEASES_URL, FetchOptions);
                _latestVersions = latestVersionsTask.Result.ToDictionary(v => v.PluginName, v => v);
                AddVersionDetailLabels(_latestVersions);
            }
            catch (Exception ex)
            {
                _core.GetPluginErrorLogger(worker)(
                    new PluginException(worker.AboutInfo.FullName, "Unable to download or parse latest version information from GitHub.", ex), "> AutoUpdateHelper.GetLatestVersions()");
                return [];
            }
            finally
            {
                // Always update fetch attempt timestamp (after success or failure) to avoid frequent retries.
                _latestVersionsFetchedDateTime = DateTime.Now;
            }
            return _latestVersions;
        }

        internal static Stream GetStream(IObservatoryPlugin worker, string url)
        {
            var downloadTask = _core.HttpClient.GetStreamAsync(url);
            try
            {
                return downloadTask.Result;
            }
            catch (Exception ex)
            {
                _core.GetPluginErrorLogger(worker)(
                    new PluginException(worker.AboutInfo.FullName, "Unable to download latest version from GitHub.", ex), "> AutoUpdateHelper.GetStream()");
                return null;
            }
        }

        private static void AddVersionDetailLabels(Dictionary<string, PluginVersions> latestVersions)
        {
            foreach (var pv in latestVersions.Values)
            {
                if (pv.Production != null)
                {
                    pv.Production.Label = VersionDetail.LABEL_PRODUCTION;
                }
                if (pv.Beta != null)
                {
                    pv.Beta.Label = VersionDetail.LABEL_BETA;
                }
            }
        }
    }
}
