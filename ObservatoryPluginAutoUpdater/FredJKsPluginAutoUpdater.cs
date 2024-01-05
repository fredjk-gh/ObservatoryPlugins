using Microsoft.VisualBasic;
using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using System.Diagnostics;
using System.Linq;


namespace com.github.fredjk_gh.ObservatoryPluginAutoUpdater
{
    public class FredJKsPluginAutoUpdater : IObservatoryWorker
    {
        private readonly HashSet<string> KnownPlugins = new()
        {
            "ObservatoryAggregator",
            "ObservatoryArchivist",
            "ObservatoryFleetCommander",
            "ObservatoryHelm",
            "ObservatoryPluginAutoUpdater",
            "ObservatoryProspectorBasic",
            "ObservatoryStatScanner",
        };

        internal const string CURRENT_RELEASES_URL = "https://raw.githubusercontent.com/fredjk-gh/ObservatoryPlugins/main/CurrentReleases.json";
        internal string PluginFolderPath = $"{AppDomain.CurrentDomain.BaseDirectory}plugins"; // Duplicated from Core. Maybe Core should expose this?
        internal IObservatoryCore Core;
        private AutoUpdaterSettings _settings = new();
        private List<NotificationArgs> _pendingNotifications = new List<NotificationArgs>();

        public string Name => "FredJKs Plugin AutoUpdater";

        public string Version => typeof(FredJKsPluginAutoUpdater).Assembly.GetName().Version.ToString();

        public PluginUI PluginUI => new PluginUI(PluginUI.UIType.None, new object());

        public object Settings {
            get => _settings;
            set => _settings = (AutoUpdaterSettings)value;
        }

        internal DirectoryInfo GetPluginFolder()
        {
            return new DirectoryInfo(PluginFolderPath);
        }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            // Nothing doing here.
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs eventArgs)
        {
            if (eventArgs.NewState.HasFlag(LogMonitorState.Realtime) && _pendingNotifications.Count > 0)
            {
                foreach(var n in _pendingNotifications)
                {
                    Core.SendNotification(n);
                }
                _pendingNotifications.Clear();
            }
        }

        public void Load(IObservatoryCore observatoryCore)
        {
            Core = observatoryCore;
            IHttpClientWrapper httpClient = new HttpClientWrapper(Core, this);

            CheckForUpdates(httpClient);
        }

        internal bool CheckForUpdates(IHttpClientWrapper httpClient)
        {
            if (!_settings.Enabled) return false;
            if (string.IsNullOrEmpty(PluginFolderPath) || !GetPluginFolder().Exists)
            {
                string title = "Configuration problem";
                string detail = "Plugin folder does not exist!";
                string extendedDetail = $"The expected plugins folder '{PluginFolderPath}' does not exist. This is most unexpected.";
                _pendingNotifications.Add(new()
                {
                    Title = title,
                    Detail = detail,
#if EXTENDED_EVENT_ARGS
                    Rendering = NotificationRendering.PluginNotifier,
                    ExtendedDetails = extendedDetail,
                    Sender = this,
#endif
                });
                Core.GetPluginErrorLogger(this)(new ArgumentNullException("PluginFolder", extendedDetail), "> Loading plugin");
                return false;
            }

            // Grab the file listing the latest versions from Github.
            List<PluginVersion> localVersions = GetLocalVersions();
            if (localVersions.Count == 0) return false; // Nothing to do.

            Dictionary<string, PluginVersion> latestVersions = httpClient.GetLatestVersions(CURRENT_RELEASES_URL);
            if (latestVersions.Count == 0) return false; // Nothing to do.

            int updateCount = 0;
            int failedCount = 0;
            foreach (var localVersion in localVersions)
            {
                if (!latestVersions.ContainsKey(localVersion.PluginName))
                {
                    Debug.WriteLine($"Unexpected unknown plugin name in downloaded latest versions: {localVersion.PluginName}");
                    continue;
                }

                var latest = latestVersions[localVersion.PluginName];
                var selectedVersion = PluginVersion.SelectVersion(localVersion, latest, _settings.UseBeta, Core.Version);
                if (selectedVersion != null)
                {
                    var downloadUrl = selectedVersion.Latest.DownloadURL;
                    if (string.IsNullOrEmpty(downloadUrl))
                    {
                        Debug.WriteLine($"No download URL for {selectedVersion.Name} version for plugin: {localVersion.PluginName}");
                        continue;
                    }
                    if (DownloadLatestPlugin(httpClient, downloadUrl, latest))
                    {
                        updateCount++;
                    }
                    else
                    {
                        failedCount++;
                    }
                }
            }

            if (updateCount > 0)
            {
                string extendedDetail = $"{updateCount} plugins were updated{(failedCount > 0 ? $" and {failedCount} failed; check crash log" : "")}";
                _pendingNotifications.Add(new()
                {
                    Title = "Pending plugin updates",
                    Detail = "Restart the app to complete the update",
#if EXTENDED_EVENT_ARGS
                    Rendering = NotificationRendering.PluginNotifier,
                    ExtendedDetails = extendedDetail,
                    Sender = this,
#endif
                });
            }
            else if (failedCount > 0)
            {
                string extendedDetail = $"{failedCount} plugins failed to update.";
                _pendingNotifications.Add(new()
                {
                    Title = "Plugin update failed",
                    Detail = "Please check crash log",
#if EXTENDED_EVENT_ARGS
                    Rendering = NotificationRendering.PluginNotifier,
                    ExtendedDetails = extendedDetail,
                    Sender = this,
#endif
                });
            }
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

                Debug.WriteLine($"Downloaded update and written: {outputFilename}");
                return true;
            }
            else
            {
                Debug.WriteLine($"Failed to fetch latest {(_settings.UseBeta ? "beta " : "")}version of {latest.PluginName} from {downloadUrl}");
                return false;
            }
        }

        internal virtual List<PluginVersion> GetLocalVersions()
        {
            DirectoryInfo dirInfo = GetPluginFolder();

            if (!dirInfo.Exists)
            {
                return new();
            }

            List<PluginVersion> versions = new();
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
                versions.Add(ver);
            }
            return versions;
        }
    }
}
