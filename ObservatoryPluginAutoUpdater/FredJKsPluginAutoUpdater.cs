using com.github.fredjk_gh.ObservatoryPluginAutoUpdater.UI;
using com.github.fredjk_gh.PluginCommon.AutoUpdates;
using com.github.fredjk_gh.PluginCommon.PluginInterop;
using com.github.fredjk_gh.PluginCommon.PluginInterop.Marshalers;
using com.github.fredjk_gh.PluginCommon.PluginInterop.Messages;
using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using static com.github.fredjk_gh.PluginCommon.PluginInterop.PluginTracker;


namespace com.github.fredjk_gh.ObservatoryPluginAutoUpdater
{
    public class FredJKsPluginAutoUpdater : IObservatoryWorker
    {
        private static readonly Guid PLUGIN_GUID = new("a4a03c40-561f-4de9-878a-8ffd0285c17d");
        private static readonly AboutLink GH_LINK = new("github", "https://github.com/fredjk-gh/ObservatoryPlugins");
        private static readonly AboutLink GH_RELEASE_NOTES_LINK = new("github release notes", "https://github.com/fredjk-gh/ObservatoryPlugins/wiki/Plugin:-AutoUpdater");
        private static readonly AboutLink DOC_LINK = new("Documentation", "https://observatory.xjph.net/usage/plugins/thirdparty/fredjk-gh/fredjk-autoupdater");
        private static readonly AboutInfo _aboutInfo = new()
        {
            FullName = "FredJK AutoUpdater",
            ShortName = "AutoUpdater",
            Description = "This plugin helps you install other plugins authored by fredjk-gh and keep them up-to-date after installation.",
            AuthorName = "fredjk-gh",
            Links =
            [
                GH_LINK,
                GH_RELEASE_NOTES_LINK,
                DOC_LINK,
            ]
        };

        private AutoUpdaterSettings _settings = new();
        private UIContext _context;
        private PluginUI _pluginUI;

        public static Guid Guid => PLUGIN_GUID;
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

        public PluginUpdateInfo CheckForPluginUpdate()
        {
            AutoUpdateHelper.Init(_context.Core);
            return AutoUpdateHelper.CheckForPluginUpdate(
                this, GH_RELEASE_NOTES_LINK.Url, true, _settings.UseBeta);
        }

        public void ObservatoryReady()
        {
            var readyMsg = GenericPluginReadyMessage.New();
            _context.Core.SendPluginMessage(this, readyMsg.ToPluginMessage());

            // I'd like to be the last plugin to be called or delay running this for a few seconds
            // in order to wait for other plugins to ready up.
            Task.Run(() =>
            {
                Task.Delay(500).Wait(); // OK to wait on a task background thread.
                _context.CheckForUpdates();
            });
        }

        public void Load(IObservatoryCore observatoryCore)
        {
            _context = new(observatoryCore, this);
            _context.UI = new(_context);
            _pluginUI = new PluginUI(PluginUI.UIType.Panel, new PluginUpdaterPanel(_context));
        }


        public void HandlePluginMessage(MessageSender sender, PluginMessage messageArgs)
        {
            PluginMessageWrapper w = new(sender, messageArgs);

            if (w.Type != "LegacyPluginMessage")
            {
                if (!PluginMessageUnmarshaler.TryUnmarshal(w, out PluginMessageWrapper unMarshaled)) return;

                switch (unMarshaled)
                {
                    case GenericPluginReadyMessage readyMsg:
                        var pluginType = PluginConstants.PluginTypeByGuid.GetValueOrDefault(readyMsg.Sender.Guid, PluginType.Unknown);
                        _context.PluginTracker.MarkActive(pluginType, new(readyMsg.Sender.Version));
                        break;
                }
            }
        }
    }
}
