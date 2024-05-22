using Observatory.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryPluginAutoUpdater.UI
{
    internal class UIContext
    {
        private Dictionary<string, PluginRowUI> _pluginUI = new();
        private List<string> _messages = new();
        private string _latestVersionCacheFilename;
        private DateTime? _latestVersionsFetchedDateTime = null;

        public readonly HashSet<string> KnownPlugins = new()
        {
            "ObservatoryAggregator",
            "ObservatoryArchivist",
            "ObservatoryFleetCommander",
            "ObservatoryHelm",
            "ObservatoryPluginAutoUpdater",
            "ObservatoryProspectorBasic",
            "ObservatoryStatScanner",
        };
        public const string WIKI_URL = "https://github.com/fredjk-gh/ObservatoryPlugins/wiki";
        public const string CURRENT_RELEASES_URL = "https://raw.githubusercontent.com/fredjk-gh/ObservatoryPlugins/main/CurrentReleases.json";
        public const string LATEST_VERSION_CACHE_FILENAME = "latest_version_cache.json";
        public string PluginFolderPath = $"{AppDomain.CurrentDomain.BaseDirectory}plugins"; // Duplicated from Core. Maybe Core should expose this?

        public UIContext(IObservatoryCore core, FredJKsPluginAutoUpdater worker)
        {
            RequiresRestart = false;
            Core = core;
            Worker = worker;
            HttpClient = new HttpClientWrapper(Core, worker);

            _latestVersionCacheFilename = $"{Core.PluginStorageFolder}{LATEST_VERSION_CACHE_FILENAME}";

            LocalVersions = GetLocalVersions();
            LatestVersions = LoadCachedLatestVersions();
        }

        public bool RequiresRestart { get; set; }

        public IObservatoryCore Core { get; init; }
        public FredJKsPluginAutoUpdater Worker { get; init; }
        public AutoUpdaterSettings Settings { get => (AutoUpdaterSettings)Worker.Settings; }
        public IHttpClientWrapper HttpClient { get; init; }
        public Dictionary<string, PluginVersion> LocalVersions { get; init; }
        public Dictionary<string, PluginVersion> LatestVersions { get; set; }

        // Not initialized by constructor.
        public PluginUpdaterUI UI { get; set; }

        public void AddRowControls(PluginRowUI row)
        {
            _pluginUI.Add(row.PluginName, row);
        }

        public PluginRowUI GetRowControls(string pluginName)
        {
            return _pluginUI[pluginName];
        }

        public string GetMessages()
        {
            StringBuilder sb = new();

            if (RequiresRestart)
            {
                sb.AppendLine("⚠️ Restart required! ⚠️");
            }
            if (Settings.UseBeta)
            {
                sb.AppendLine("Using beta versions, where available.");
            }
            sb.AppendLine(string.Join(Environment.NewLine, _messages));
            return sb.ToString();
        }

        public void AddMessage(string message)
        {
            _messages.Add(message);
        }

        public void ClearMessages()
        {
            _messages.Clear();
        }

        public void OpenUrl(string url)
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        internal DirectoryInfo GetPluginFolder()
        {
            return new DirectoryInfo(PluginFolderPath);
        }

        internal bool CheckForUpdates()
        {
            ClearMessages();

            if (string.IsNullOrEmpty(PluginFolderPath) || !GetPluginFolder().Exists)
            {
                string extendedDetail = $"The expected plugins folder '{PluginFolderPath}' does not exist. This is most unexpected.";
                AddMessage($"Configuration problem: Plugin folder does not exist or not found!");
                AddMessage(extendedDetail);
                Core.GetPluginErrorLogger(Worker)(new ArgumentNullException("PluginFolder", extendedDetail), "> Loading plugin");
                return false;
            }

            // Grab the file listing the latest versions from Github.
            if (!_latestVersionsFetchedDateTime.HasValue || _latestVersionsFetchedDateTime.Value.AddHours(2) < DateTime.Now)
            {
                try
                {
                    LatestVersions = HttpClient.GetLatestVersions(UIContext.CURRENT_RELEASES_URL);

                    // Cache results.
                    string jsonString = JsonSerializer.Serialize(LatestVersions,
                        new JsonSerializerOptions() { AllowTrailingCommas = true, WriteIndented = true });
                    File.WriteAllText(_latestVersionCacheFilename, jsonString);
                    _latestVersionsFetchedDateTime = DateTime.Now;
                }
                catch (Exception ex)
                {
                    AddMessage("Unable to download latest version information from GitHub. Nothing to do.");
                    Core.GetPluginErrorLogger(Worker)(
                        new ArgumentNullException("LatestVersions", "Unable to download latest version information from GitHub."), " > Fetching LatestVersions");
                    return false;
                }
            }

            int updateCount = 0;
            int failedCount = 0;
            int skippedCount = 0;
            foreach (var p in KnownPlugins)
            {
                if (!LocalVersions.ContainsKey(p))
                {
                    UI.UpdatePluginState(p, "", "Not installed", PluginAction.Install);
                    skippedCount++;
                    continue;
                }
                var local = LocalVersions[p];
                if (!LatestVersions.ContainsKey(p))
                {
                    if (LatestVersions.Count > 0)
                    {
                        AddMessage($"Known plugin {p} was not found in latest versions file!");
                        UI.UpdatePluginState(p, local.Production.Version, "Latest version unavailable", PluginAction.None);
                    }
                    else
                    {
                        AddMessage($"Latest versions file is empty. Failed fetch?");
                    }
                    skippedCount++;
                    continue;
                }

                var latest = LatestVersions[p];
                var selectedVersion = PluginVersion.SelectVersion(local, latest, Settings.UseBeta, Core.Version);
                if (selectedVersion != null)
                {
                    var downloadUrl = selectedVersion.Latest.DownloadURL;
                    if (string.IsNullOrEmpty(downloadUrl))
                    {
                        AddMessage($"No download URL for {selectedVersion.Name} version for plugin: {p}.");
                        skippedCount++;
                        continue;
                    }
                    if (DownloadLatestPlugin(HttpClient, downloadUrl, latest))
                    {
                        UI.UpdatePluginState(p, local.Production.Version, "Updated (pending restart)", PluginAction.None);
                        updateCount++;
                    }
                    else
                    {
                        UI.UpdatePluginState(p, local.Production.Version, "Update pending (download failed!)", PluginAction.Update);
                        failedCount++;
                    }
                }
                else
                {
                    skippedCount++;
                    UI.UpdatePluginState(p, local.Production.Version, "Up to date", PluginAction.None);
                }
            }

            if (updateCount > 0)
            {
                AddMessage($"{updateCount} plugins were updated{(failedCount > 0 ? $" and {failedCount} failed; check error log" : "")}{(skippedCount > 0 ? $" and {skippedCount} skipped" : "")}.");
                RequiresRestart = true;
            }
            else if (failedCount > 0)
            {
                AddMessage($"{failedCount} plugins failed to update. Please check the error log.");
            }
            else if (skippedCount > 0)
            {
                AddMessage($"{skippedCount} plugins were skipped.");
            }

            UI.ShowMessages(string.Join(Environment.NewLine, GetMessages()));

            return true;
        }

        internal bool DownloadLatestPlugin(IHttpClientWrapper httpClient, string downloadUrl, PluginVersion latest)
        {
            var outputFilename = downloadUrl.Split('/').Last();
            var fileStream = httpClient.GetStream(downloadUrl);
            if (fileStream != null)
            {
                var f = File.Create($"{PluginFolderPath}{Path.DirectorySeparatorChar}{outputFilename}");
                fileStream.CopyTo(f);
                fileStream.Flush();
                fileStream.Close();
                f.Close();

                AddMessage($"Downloaded update: {outputFilename}");
                return true;
            }
            else
            {
                AddMessage($"Failed to fetch latest {(Settings.UseBeta ? "beta " : "")}version of {latest.PluginName} from {downloadUrl}");
                return false;
            }
        }

        internal virtual Dictionary<string, PluginVersion> GetLocalVersions()
        {
            DirectoryInfo dirInfo = GetPluginFolder();

            if (!dirInfo.Exists)
            {
                return new();
            }

            Dictionary<string, PluginVersion> versions = new();
            foreach (var plugin in dirInfo.GetFiles("*.dll"))
            {
                var pluginName = plugin.Name.Replace(".dll", "");
                if (!KnownPlugins.Contains(pluginName)) continue;

                var fv = FileVersionInfo.GetVersionInfo(plugin.FullName);
                PluginVersion ver = new PluginVersion();
                ver.PluginName = pluginName;
                ver.Production = new() { Version = fv.FileVersion ?? "" };
                // Don't know if this is production or beta version; set both.
                ver.Beta = new() { Version = fv.FileVersion ?? "" };
                versions.Add(pluginName, ver);
            }
            return versions;
        }


        private Dictionary<string, PluginVersion> LoadCachedLatestVersions()
        {
            Dictionary<string, PluginVersion> versions = new();

            if (!File.Exists(_latestVersionCacheFilename)) return versions;

            try
            {
                string jsonString = File.ReadAllText(_latestVersionCacheFilename);
                var options = new JsonSerializerOptions()
                {
                    AllowTrailingCommas = true,
                };
                versions = JsonSerializer.Deserialize<Dictionary<string, PluginVersion>>(jsonString, options)!;

                FileInfo fileInfo = new FileInfo(_latestVersionCacheFilename);
                _latestVersionsFetchedDateTime = fileInfo.LastWriteTime;
            }
            catch (Exception ex)
            {
                Core.GetPluginErrorLogger(Worker)(ex, "Deserializing LatestVersions cache");
                AddMessage("Unable to read latest versions cache");
            }

            return versions;
        }
    }
}
