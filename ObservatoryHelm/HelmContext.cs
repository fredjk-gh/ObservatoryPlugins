using com.github.fredjk_gh.ObservatoryHelm.Data;
using com.github.fredjk_gh.ObservatoryHelm.UI;
using com.github.fredjk_gh.PluginCommon.PluginInterop;
using com.github.fredjk_gh.PluginCommon.Utilities;
using Observatory.Framework;
using Observatory.Framework.Interfaces;

namespace com.github.fredjk_gh.ObservatoryHelm
{
    internal class HelmContext : ICoreContext<Helm>
    {
        public static HelmContext Instance { get; private set; }

        public static HelmContext Initialize(IObservatoryCore core, Helm worker, HelmSettings settings)
        {
            Instance = new(core, worker, settings);

            return Instance;
        }

        internal enum Card
        {
            Commander,
            Ship,
            Route,
            System,
            Body,
            Station,
            Cargo,
            Prospector,
            Messages,
        }

        private readonly IObservatoryCore _c;
        private readonly Helm _w;
        private readonly MessageDispatcher _d;
        private readonly DebugLogger _l;
        /// <summary>
        /// Error logger; string param is an arbitrary context hint.
        /// </summary>
        private Action<Exception, string> _errorLogger;
        private HelmSettings _settings = new();
        private TrackedData _data;

        private HelmContext(IObservatoryCore core, Helm worker, HelmSettings settings)
        {
            _c = core;
            _w = worker;
            _d = new(core, worker, PluginTracker.PluginType.fredjk_Helm);
            _l = new(core, worker);
            _errorLogger = core.GetPluginErrorLogger(worker);
            Settings = settings;
            _data = new TrackedData(core.PluginStorageFolder);
            UIMgr = new();
        }

        public IObservatoryCore Core { get => _c; }
        public Helm Worker { get => _w; }
        public MessageDispatcher Dispatcher { get => _d; }
        public DebugLogger Dlogger { get => _l; }
        public Action<Exception, string> ErrorLogger { get => _errorLogger; }
        public HelmSettings Settings
        {
            get => _settings;
            set =>  _settings = MaybeFixUnsetSettings(value);
        }
        public TrackedData Data { get => _data; set => _data = value; }
        public HelmPanel Panel { get; set; }
        public HelmUI UI { get; set; }
        public UIStateManager UIMgr { get; }
        public PluginTracker PluginTracker { get => _d.PluginTracker; }

        public Guid SendNotification(NotificationArgs n)
        {
            if (UIMgr.ReplayMode)
            {
                n.Rendering &= ~NotificationRendering.NativeVocal; // Suppress vocalization on replay.
            }

            return Core.SendNotification(n);
        }

        public void AddMessage(string msg, DateTime? timestamp = null, string sender = null)
        {
            UIMgr.AddMessage(msg, timestamp ?? DateTime.UtcNow, sender ?? _w.AboutInfo.ShortName);
        }

        private HelmSettings MaybeFixUnsetSettings(HelmSettings newValue)
        {
            // Shouldn't need this anymore. Defaults should be set in the settings constructor.
            HelmSettings defaults = new();
            if (newValue.GravityAdvisoryThreshold == 0)
                newValue.GravityAdvisoryThreshold = defaults.GravityAdvisoryThreshold;
            if (newValue.BubbleRadiusLy == 0)
                newValue.BubbleRadiusLy = defaults.BubbleRadiusLy;
            if (newValue.MaxNearbyScoopableDistance == 0)
                newValue.MaxNearbyScoopableDistance = defaults.MaxNearbyScoopableDistance;
            newValue.CardOrdering ??= [];
            newValue.CollapsedCards ??= [];

            return newValue;
        }

        internal void Clear()
        {
            Data = new(Core.PluginStorageFolder);
            Data.SaveToCache();
            Core.ExecuteOnUIThread(() =>
            {
                UI.Clear();
            });
        }
    }
}
