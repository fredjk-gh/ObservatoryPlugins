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

        public string Name => "FredJKs Plugin AutoUpdater";
        public string ShortName => "FredJK AutoUpdater";

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
