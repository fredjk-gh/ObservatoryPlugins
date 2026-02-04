using com.github.fredjk_gh.ObservatoryArchivist.DB;
using com.github.fredjk_gh.ObservatoryArchivist.UI;
using com.github.fredjk_gh.PluginCommon.Data;
using com.github.fredjk_gh.PluginCommon.PluginInterop;
using com.github.fredjk_gh.PluginCommon.Utilities;
using Observatory.Framework;
using Observatory.Framework.Interfaces;
using System.Text;
using System.Text.Json;

namespace com.github.fredjk_gh.ObservatoryArchivist
{
    internal class ArchivistContext : ICoreContext<Archivist>
    {
        private const string DATA_CACHE_FILENAME = "archivistStateCache.json";

        protected readonly IObservatoryCore _c;
        protected readonly Archivist _p;
        private readonly MessageDispatcher _d;
        private readonly DebugLogger _l;

        private bool _batchMode = false;

        public ArchivistContext(IObservatoryCore core, Archivist worker)
        {
            _c = core;
            _p = worker;
            _d = new(core, worker, PluginTracker.PluginType.fredjk_Archivist);
            _l = new(core, worker);
            Data = new();
            Search = new(this);
        }

        public IObservatoryCore Core { get => _c; }
        public Archivist Worker { get => _p; }
        public MessageDispatcher Dispatcher { get => _d; }
        public DebugLogger Dlogger { get => _l; }
        /// <summary>
        /// Error logger; string param is an arbitrary context hint.
        /// </summary>
        public Action<Exception, string> ErrorLogger { get; init; }
        public ArchivistSettings Settings { get; init; }
        public PluginTracker Tracker { get => _d.PluginTracker; }
        public JournalManager Journals { get; init; }
        public PositionCacheManager PositionCache { get; init; }
        public NotificationManager Notifications { get; init; }
        public ArchivistData Data { get; private set; }
        public ArchivistSearchData Search { get; init; }
        public ArchivistUI UI { get; internal set; }
        public bool IsReadAll { get => Core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch); }
        public bool IsResending { get; private set; }
        public ulong LastSystemId64DataShared { get; set; }
        public bool BatchModeProcessing
        {
            get => _batchMode;
            set
            {
                _batchMode = value;
                Journals.BatchModeProcessing = value;
                PositionCache.BatchModeProcessing = value;
                Notifications.BatchModeProcessing = value;
            }
        }

        public void FlushIfDirty(bool forceFlush = false)
        {
            if (IsReadAll && !forceFlush) return;

            foreach (var c in Data.KnownCommanders.Values)
            {
                if (c.CurrentSystem == null) continue;
                
                if (c.CurrentSystem.IsSystemInfoDirty)
                {
                    Journals.UpsertVisitedSystemData(c.CurrentSystem.ToVisitedSystem(true));
                }
                if (c.CurrentSystem.AreNotificationDirty)
                {
                    Notifications.UpsertNotifications(c.CurrentSystem.ToNotificationInfo(true));
                }
            }
        }

        public void SerializeState(bool forceWrite = false)
        {
            if (IsReadAll && !forceWrite) return;

            string dataCacheFile = $"{Core.PluginStorageFolder}{DATA_CACHE_FILENAME}";
            string jsonString = JsonSerializer.Serialize(Data, JsonHelper.PRETTY_PRINT_OPTIONS);
            File.WriteAllText(dataCacheFile, jsonString);
        }

        public void DeserializeState()
        {
            string dataCacheFile = $"{Core.PluginStorageFolder}{DATA_CACHE_FILENAME}";
            if (!File.Exists(dataCacheFile)) return;

            try
            {
                string jsonString = File.ReadAllText(dataCacheFile);
                ArchivistData dataCache = JsonSerializer.Deserialize<ArchivistData>(jsonString, JsonHelper.PRETTY_PRINT_OPTIONS);

                Data = dataCache;

                foreach (var cmdrData in Data.KnownCommanders)
                {
                    // Fetch current system data from DB (not serialized to the cache for brevity/simplicity).
                    if (!string.IsNullOrWhiteSpace(cmdrData.Value.CurrentSystemName) && cmdrData.Value.CurrentSystem == null)
                    {
                        var currentSystem = Journals.GetVisitedSystem(cmdrData.Value.CurrentSystemName, cmdrData.Key);
                        var notifs = Notifications.GetNotifications(cmdrData.Value.CurrentSystemName, cmdrData.Key);

                        if (currentSystem != null) // This may happen after an aborted Read-all.
                            cmdrData.Value.CurrentSystem = new(currentSystem, notifs);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger(ex, "Deserializing Archivist state cache");
            }
        }

        public void DisplaySummary(string prefix = "")
        {
            var summaryData = Journals.GetVisitedSystemSummary();
            var cachedSystems = PositionCache.CountCachedSystems();
            StringBuilder sb = new(prefix);

            if (!string.IsNullOrWhiteSpace(prefix)) sb.AppendLine();

            foreach (var r in summaryData)
            {
                int sysCount = r["SystemCount"];
                if ("Augmented".Equals(r["Cmdr"], StringComparison.OrdinalIgnoreCase))
                    sb.AppendLine($"{sysCount:#,###} systems augmented with Spansh data");
                else
                    sb.AppendLine($"{sysCount:#,###} known systems for Cmdr {r["Cmdr"]}.");
            }

            long notificationSystemCount = Notifications.CountSystemsWithNotifications();
            long notificationCount = Notifications.CountAll();
            sb.AppendLine($"Notification cache holds data for {notificationSystemCount:#,###} unique (system + commander) combinations for a total of {notificationCount:#,###} notifications.");

            sb.AppendLine($"System position cache contains {cachedSystems:#,###} systems.");

            UI.SetMessage(sb.ToString());
        }

        internal void SetResendAll(bool isResending)
        {
            IsResending = isResending;
        }

        internal void ClearForReadAll()
        {
            Journals.ClearAll();
            Notifications.ClearAll();
            // Don't ever clear the cache (or only do so manually). That would lose data we can't recover via re-reading all.
        }

        internal void FinishReadAll()
        {
            Journals.FinishReadAll();
            Notifications.FinishReadAll();
            PositionCache.FinishReadAll();
        }

        internal void Connect(ConnectionMode mode)
        {
            Journals.Connect(mode);
            Notifications.Connect(mode);
            PositionCache.Connect(mode);
        }
    }
}
