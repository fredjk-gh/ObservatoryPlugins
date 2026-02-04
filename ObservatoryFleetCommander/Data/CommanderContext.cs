using com.github.fredjk_gh.ObservatoryFleetCommander.UI;
using com.github.fredjk_gh.PluginCommon.Data;
using com.github.fredjk_gh.PluginCommon.PluginInterop;
using com.github.fredjk_gh.PluginCommon.Utilities;
using Observatory.Framework;
using Observatory.Framework.Interfaces;
using System.Text.Json;
using static com.github.fredjk_gh.PluginCommon.PluginInterop.PluginTracker;

namespace com.github.fredjk_gh.ObservatoryFleetCommander.Data
{
    internal class CommanderContext : ICoreContext<Commander>
    {
        private const string PREVIOUS_CARRIER_DATA_CACHE_FILENAME = "carrierDataCache.json";
        private readonly string CARRIER_DATA_CACHE_FILENAME = $"carrierDataCache.{FleetCommanderDataCache.CURRENT_FILE_VERSION:000}.json";

        private IObservatoryCore _c;
        private Commander _w;
        private MessageDispatcher _d;
        private DebugLogger _l;
        private Manager _manager;
        private FleetCommanderSettings _settings = new();
        private CommanderUI _ui;
        private string _currentCommander;

        public void Initialize(IObservatoryCore core, Commander worker, Manager manager)
        {
            // Settings should be already set.
            _c = core;
            _w = worker;
            _manager = manager;
            _d = new(core, worker, PluginType.fredjk_Commander);
            _l = new(core, worker);

            PrepSettings();
            MaybeDeserializeDataCacheV2();
        }

        public IObservatoryCore Core { get => _c; }
        public Commander Worker { get => _w; }
        internal Manager Manager { get => _manager; }
        public MessageDispatcher Dispatcher { get => _d; }
        public DebugLogger Dlogger { get => _l; }
        internal FleetCommanderSettings Settings
        {
            get => _settings;
            set => _settings = MaybeFixUnsetSettings(value);
        }
        internal CommanderUI UI { get => _ui; set => _ui = value; }
        internal PluginTracker PluginTracker { get => _d.PluginTracker; }
        internal bool IsReadAll { get => _c.IsLogMonitorBatchReading; }
        internal Action<Exception, string> ErrorLogger { get => Core.GetPluginErrorLogger(Worker); }
        internal string CurrentCommander { get => _currentCommander; set => _currentCommander = value; }

        internal CommanderData ForCommander(string commanderName = null)
        {
            var cmdr = commanderName ?? _currentCommander;
            if (string.IsNullOrWhiteSpace(cmdr)) return null;
            return Manager.GetCommander(cmdr);
        }

        internal void DoUIAction(Action uiAction)
        {
            if (Core.IsLogMonitorBatchReading) return;
            Core.ExecuteOnUIThread(uiAction);
        }

        #region CacheFile bits
        private void MaybeDeserializeDataCacheV2()
        {
            string dataCacheFile = $"{Core.PluginStorageFolder}{CARRIER_DATA_CACHE_FILENAME}";
            if (!File.Exists(dataCacheFile))
            {
                MaybeDeserializeDataCachev0();
                return;
            }

            try
            {
                string jsonString = File.ReadAllText(dataCacheFile);
                FleetCommanderDataCache cache = JsonSerializer.Deserialize<FleetCommanderDataCache>(jsonString, JsonHelper.PRETTY_PRINT_OPTIONS)!;

                _manager.Rehydrate(cache);
            }
            catch (Exception ex)
            {
                _manager.Clear(); // Start from scratch.
                SerializeDataCacheV2(true);
                Core.GetPluginErrorLogger(Worker)(ex, "Deserializing CarrierData cache failed; Read-all required");
            }
        }

        private void MaybeDeserializeDataCachev0()
        {
            // Original cache version. This only serialized the list of carriers. 
            string dataCacheFile = $"{Core.PluginStorageFolder}{PREVIOUS_CARRIER_DATA_CACHE_FILENAME}";
            if (!File.Exists(dataCacheFile)) return;

            try
            {
                string jsonString = File.ReadAllText(dataCacheFile);
                List<CarrierData> carrierCache = JsonSerializer.Deserialize<List<CarrierData>>(jsonString, JsonHelper.PRETTY_PRINT_OPTIONS)!;

                foreach (CarrierData carrierData in carrierCache)
                {
                    var data = _manager.RegisterCarrier(carrierData);
                }
            }
            catch (Exception ex)
            {
                Core.GetPluginErrorLogger(Worker)(ex, "Deserializing CarrierData cache");
            }
        }

        public void SerializeDataCacheV2(bool forceWrite = false)
        {
            if (Core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch) && !forceWrite) return;

            string dataCacheFile = $"{Core.PluginStorageFolder}{CARRIER_DATA_CACHE_FILENAME}";
            string jsonString = JsonSerializer.Serialize(FleetCommanderDataCache.FromManager(_manager), JsonHelper.PRETTY_PRINT_OPTIONS);
            File.WriteAllText(dataCacheFile, jsonString);
        }

        #endregion

        #region Settings bits
        private FleetCommanderSettings MaybeFixUnsetSettings(FleetCommanderSettings settings)
        {
            FleetCommanderSettings defaults = new();
            if (string.IsNullOrEmpty(settings.CooldownNotificationSoundFile))
            {
                settings.CooldownNotificationSoundFile = defaults.CooldownNotificationSoundFile;
            }
            return settings;
        }

        private void PrepSettings()
        {
            _settings.ClearCountdownWindowSizePosition = ResetCountdownWindowSizePosition;
        }

        private void ResetCountdownWindowSizePosition()
        {
            Settings.CountdownWindowX = 0;
            Settings.CountdownWindowY = 0;

            Settings.CountdownWindowWidth = 0;
            Settings.CountdownWindowHeight = 0;

            Core.SaveSettings(Worker);
        }
        #endregion
    }
}
