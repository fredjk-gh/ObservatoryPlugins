using com.github.fredjk_gh.ObservatoryPluginAutoUpdater.UI;
using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using System;
using System.Diagnostics;


namespace com.github.fredjk_gh.ObservatoryPluginAutoUpdater
{
    public class FredJKsPluginAutoUpdater : IObservatoryWorker
    {
        private AutoUpdaterSettings _settings = new();
        private UIContext _context;
        private PluginUI _pluginUI;
        private AboutInfo _aboutInfo = new()
        {
            FullName = "FredJK AutoUpdater",
            ShortName = "AutoUpdater",
            Description = "This plugin helps you install other plugins authored by fredjk-gh and keep them up-to-date after installation.",
            AuthorName = "fredjk-gh",
            Links = new()
            {
                new AboutLink("github", "https://github.com/fredjk-gh/ObservatoryPlugins"),
                new AboutLink("github release notes", "https://github.com/fredjk-gh/ObservatoryPlugins/wiki/Plugin:-AutoUpdater"),
                new AboutLink("Documentation", "https://observatory.xjph.net/usage/plugins/thirdparty/bioinsights"),
            }
        };

        public AboutInfo AboutInfo => _aboutInfo;

        public string Version => typeof(FredJKsPluginAutoUpdater).Assembly.GetName().Version.ToString();

        public PluginUI PluginUI => _pluginUI;

        public object Settings {
            get => _settings;
            set => _settings = (AutoUpdaterSettings)value;
        }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            // Nothing doing here.
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs eventArgs)
        {
            if (eventArgs.NewState.HasFlag(LogMonitorState.Realtime))
            {
                _context.CheckForUpdates();
            }
        }

        public void ObservatoryReady()
        {
            _context.CheckForUpdates();
        }

        public void Load(IObservatoryCore observatoryCore)
        {
            _context = new(observatoryCore, this);
            _context.UI = new(_context);
            _pluginUI = new PluginUI(PluginUI.UIType.Panel, new PluginUpdaterPanel(_context));
        }
    }
}
