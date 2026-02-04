using com.github.fredjk_gh.PluginCommon.Data;
using com.github.fredjk_gh.PluginCommon.PluginInterop;
using Observatory.Framework.Interfaces;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using static com.github.fredjk_gh.PluginCommon.PluginInterop.PluginTracker;

namespace com.github.fredjk_gh.ObservatoryPluginAutoUpdater.UI
{
    internal class UIContext
    {
        private const string SELF_PLUGIN_KEY = "ObservatoryPluginAutoUpdater";
        private readonly List<string> _messages = [];
        private readonly string _latestVersionCacheFilename;
        private DateTime? _latestVersionsFetchedDateTime = null;
        private bool hasCheckedForUpdates = false;

        public readonly Dictionary<string, PluginType> KnownPlugins = new()
        {
            { "ObservatoryAggregator", PluginType.fredjk_Aggregator },
            { "ObservatoryArchivist", PluginType.fredjk_Archivist },
            { "ObservatoryFleetCommander", PluginType.fredjk_Commander },
            { "ObservatoryHelm", PluginType.fredjk_Helm },
            { SELF_PLUGIN_KEY, PluginType.fredjk_AutoUpdater },
            { "ObservatoryProspectorBasic", PluginType.fredjk_Prospector },
            { "ObservatoryStatScanner", PluginType.fredjk_StatScanner },
        };
        public const string WIKI_URL = "https://github.com/fredjk-gh/ObservatoryPlugins/wiki";
        public const string CURRENT_RELEASES_URL = "https://raw.githubusercontent.com/fredjk-gh/ObservatoryPlugins/main/CurrentReleases.json";
        public const string LATEST_VERSION_CACHE_FILENAME = "latest_version_cache.json";
        public string PluginFolderPath;

        public UIContext(IObservatoryCore core, FredJKsPluginAutoUpdater worker)
        {
            RequiresRestart = false;
            Core = core;
            Worker = worker;
            HttpClient = new HttpClientWrapper(Core, worker);
            PluginTracker = new PluginTracker(PluginType.fredjk_AutoUpdater);
            PluginFolderPath = Core.PluginStorageFolder;
            _latestVersionCacheFilename = $"{Core.PluginStorageFolder}{LATEST_VERSION_CACHE_FILENAME}";

            LocalVersions = GetLocalVersions();
            LatestVersions = LoadCachedLatestVersions();

            Settings.PropertyChanged += Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "UseBeta": // Refresh the list -- available actions may change.
                    CheckForUpdates(true);
                    break;
            }
        }

        public bool RequiresRestart { get; set; }

        public IObservatoryCore Core { get; init; }
        public FredJKsPluginAutoUpdater Worker { get; init; }
        public AutoUpdaterSettings Settings { get => (AutoUpdaterSettings)Worker.Settings; }
        public IHttpClientWrapper HttpClient { get; init; }
        public Dictionary<string, PluginVersion> LocalVersions { get; init; }
        public Dictionary<string, PluginVersion> LatestVersions { get; set; }
        public PluginTracker PluginTracker { get; init; }

        // Not initialized by constructor.
        public PluginUpdaterUI UI { get; set; }

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

        internal DirectoryInfo GetPluginFolder()
        {
            return new DirectoryInfo(PluginFolderPath);
        }

        internal bool CheckForUpdates(bool isExplicit = false)
        {
            if (!isExplicit && hasCheckedForUpdates) return false;

            ClearMessages();

            // Definitions and Assumptions:
            // * "legacy" plugins are pre-1.4 versions of each plugin which have no self-updating support NOR send ready messages.
            //     Assumption 1: These plugins were installed pre ObsCore 1.4 and thus are dlls in the plugins/dir.
            // * "modern" plugins have version 1.4 or higher and can self-update and send ready messages.
            //     Assumption 2: These plugins were installed post ObsCore 1.4 release and are loaded directly from .eop.
            //
            // Unfortunate corner case that violates :
            // * Legacy plugins (ie. stat scanner) installed/updated post 1.4 (so run as .eop) but can't update themselves
            //   nor make their presence known via Ready message.
            //
            HashSet<string> legacyInstalls = [.. KnownPlugins.Keys];
            HashSet<string> modernInstalls = [];
            Dictionary<string, PluginVersion> mergedLocalVersions = [];

            foreach (var p in KnownPlugins)
            {
                if (!LocalVersions.TryGetValue(p.Key, out PluginVersion ver)) // We couldn't find a dll to grab a version for this plugin
                {
                    // Either this plugin is not installed at all, or...
                    legacyInstalls.Remove(p.Key);
                    // ... it's installed and sending Ready messages which we can track.
                    if (PluginTracker.IsActive(p.Value))
                    {
                        modernInstalls.Add(p.Key);
                        var mv = PluginTracker.GetActiveVersion(KnownPlugins[p.Key]);
                        PluginVersion lv = new()
                        {
                            PluginName = p.Key,
                            Production = new() { Version = mv.ToString() },
                            Beta = new() { Version = mv.ToString() },
                        };
                        mergedLocalVersions.Add(p.Key, lv);
                    }
                    else if (p.Key == SELF_PLUGIN_KEY) // won't be in the plugin tracker; add our local version as well.
                    {
                        PluginVersion lv = new()
                        {
                            PluginName = p.Key,
                            Production = new() { Version = Worker.Version },
                            Beta = new() { Version = Worker.Version },
                        };
                        mergedLocalVersions.Add(p.Key, lv);
                    }
                }
                else
                {
                    mergedLocalVersions.Add(p.Key, ver);
                }
            }

            // What is left in legacyInstalls has things that have a local version (legacy install) or aren't installed.
            if (legacyInstalls.Count == 0)
            {
                AddMessage("NOTICE: The AutoUpdater is no longer managing any plugins -- they all appear to be self-updating now. Please remove ObservatoryPluginAutoUpdator from your installation.");

                UI.ShowMessages(string.Join(Environment.NewLine, GetMessages()));
                UI.Refresh();
            }
            else
            {
                AddMessage($"NOTICE: This plugin is deprecated and will become unsupported soon.{Environment.NewLine}"
                    + $"Moving forward, all plugins with version 1.4 and higher have will built-in auto-updater logic integrated into the Core plugin list.{Environment.NewLine}"
                    + $"Please check if any plugins are not automatically updating as this plugin may not be able to update them.{Environment.NewLine}");
            }

            if (string.IsNullOrEmpty(PluginFolderPath) || !GetPluginFolder().Exists)
            {
                string extendedDetail = $"The expected plugins folder '{PluginFolderPath}' does not exist. This is most unexpected.";
                AddMessage($"Configuration problem: Plugin folder does not exist or not found!");
                AddMessage(extendedDetail);
                Core.GetPluginErrorLogger(Worker)(new NullReferenceException(extendedDetail), "> Loading plugin");
                return false;
            }

            // Grab the file listing the latest versions from Github.
            var skipCacheAgeCheck = false;
#if DEBUG
            skipCacheAgeCheck = true;
#endif
            if (skipCacheAgeCheck || !_latestVersionsFetchedDateTime.HasValue || _latestVersionsFetchedDateTime.Value.AddHours(2) < DateTime.Now)
            {
                try
                {
                    LatestVersions = HttpClient.GetLatestVersions(UIContext.CURRENT_RELEASES_URL);

                    // Cache results.
                    string jsonString = JsonSerializer.Serialize(LatestVersions, JsonHelper.PRETTY_PRINT_OPTIONS);
                    File.WriteAllText(_latestVersionCacheFilename, jsonString);
                    _latestVersionsFetchedDateTime = DateTime.Now;
                }
                catch (Exception ex)
                {
                    AddMessage("Unable to download latest version information from GitHub. Nothing to do.");
                    Core.GetPluginErrorLogger(Worker)(
                        new InvalidDataException("Unable to download latest version information from GitHub.", ex), " > Fetching LatestVersions");
                    return false;
                }
            }

            if (LatestVersions.Count == 0)
            {
                AddMessage($"Latest versions file is empty. Failed fetch? Aborting check.");
                return false;
            }

            int updateCount = 0;
            int failedCount = 0;
            int skippedCount = 0;
            foreach (var p in KnownPlugins.Keys)
            {
                if (!LatestVersions.ContainsKey(p))
                {
                    string localVersion = mergedLocalVersions.ContainsKey(p) ? LocalVersions[p].Production.Version : "";
                    AddMessage($"Known plugin {p} was not found in latest versions file!");
                    UI.UpdatePluginState(p, localVersion, "Latest version unavailable", null, PluginAction.None);

                    skippedCount++;
                    continue;
                }
                var latest = LatestVersions[p];

                if (!mergedLocalVersions.ContainsKey(p))
                {
                    var action = PluginAction.None; // There may be no available version.
                    var status = "Not installed";
                    if (latest.Production != null && !Settings.UseBeta)
                    {
                        action = PluginAction.InstallStable;
                    }
                    else if (latest.Beta != null && Settings.UseBeta)
                    {
                        action = PluginAction.InstallBeta;
                    }
                    else if (latest.Production == null && latest.Beta != null && !Settings.UseBeta)
                    {
                        status += ". Beta versions available but disallowed.";
                    }

                    UI.UpdatePluginState(p, "", status, LatestVersions[p], action);
                    skippedCount++;
                    continue;
                }
                var local = mergedLocalVersions[p];
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
                    if (p != SELF_PLUGIN_KEY)
                    {
                        if (DownloadLatestPlugin(HttpClient, downloadUrl, latest))
                        {
                            UI.UpdatePluginState(p, local.Production.Version, "Updated (pending restart)", latest, PluginAction.None);
                            updateCount++;
                        }
                        else
                        {
                            UI.UpdatePluginState(p, local.Production.Version, "Update pending (download failed!)", latest, selectedVersion.Action);
                            failedCount++;
                        }
                    }
                    else
                    {
                        UI.UpdatePluginState(p, local.Production.Version, "Update skipped: self-updating", latest, PluginAction.None);
                        skippedCount++;
                    }
                }
                else
                {
                    skippedCount++;
                    VersionDetail SELF_UPDATING_MIN_VER = new() { Version = "1.4.0.0" };
                    if (VersionDetail.Compare(local.Production.VersionParsed, SELF_UPDATING_MIN_VER.VersionParsed) >= 0)
                    {
                        UI.UpdatePluginState(p, local.Production.Version, "Self-updating", latest, PluginAction.None);
                    }
                    else
                    {
                        UI.UpdatePluginState(p, local.Production.Version, "Up to date", latest, PluginAction.None);
                    }
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
            UI.Refresh();
            hasCheckedForUpdates = true;

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

                AddMessage($"Downloaded package: {outputFilename}");
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
                return [];
            }

            Dictionary<string, PluginVersion> versions = [];
            foreach (var plugin in dirInfo.GetFiles("*.dll"))
            {
                var pluginName = plugin.Name.Replace(".dll", "");
                if (!KnownPlugins.ContainsKey(pluginName)) continue;

                var fv = FileVersionInfo.GetVersionInfo(plugin.FullName);
                PluginVersion ver = new()
                {
                    PluginName = pluginName,
                    Production = new() { Version = fv.FileVersion ?? "" },
                    // Don't know if this is production or beta version; set both.
                    Beta = new() { Version = fv.FileVersion ?? "" }
                };
                versions.Add(pluginName, ver);
            }
            return versions;
        }
        

        private Dictionary<string, PluginVersion> LoadCachedLatestVersions()
        {
            Dictionary<string, PluginVersion> versions = [];

            if (!File.Exists(_latestVersionCacheFilename)) return versions;

            try
            {
                string jsonString = File.ReadAllText(_latestVersionCacheFilename);
                versions = JsonSerializer.Deserialize<Dictionary<string, PluginVersion>>(jsonString, JsonHelper.PRETTY_PRINT_OPTIONS)!;
                FileInfo fileInfo = new(_latestVersionCacheFilename);
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
