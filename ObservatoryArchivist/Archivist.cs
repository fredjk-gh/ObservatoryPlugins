using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using System.Collections.ObjectModel;

namespace com.github.fredjk_gh.ObservatoryArchivist
{
    public class Archivist : IObservatoryWorker
    {
        private IObservatoryCore Core;
        /// <summary>
        /// Error logger; string param is an arbitrary context hint.
        /// </summary>
        private Action<Exception, string> ErrorLogger;
        private PluginUI pluginUI;
        private ObservableCollection<object> gridCollection = new();
        private ArchivistSettings settings = ArchivistSettings.DEFAULT;
        private ArchiveManager manager;


        private FileHeaderInfo lastFileHeaderInfo = new();
        private CurrentSystemInfo currentSystemInfo = null;
        
        public string Name => "Observatory Archivist";
        public string ShortName => "Archivist";
        public string Version => typeof(Archivist).Assembly.GetName().Version.ToString();

        public PluginUI PluginUI => pluginUI;

        public object Settings
        {
            get => settings;
            set => settings = (ArchivistSettings)value;
        }

        public void Load(IObservatoryCore observatoryCore)
        {
            Core = observatoryCore;
            ErrorLogger = Core.GetPluginErrorLogger(this);

            gridCollection = new() { new ArchivistGrid() };
            pluginUI = new PluginUI(gridCollection);
            
            manager = new(Core.PluginStorageFolder, ErrorLogger);
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args)
        {
            if (args.NewState.HasFlag(LogMonitorState.Batch))
            {
                lastFileHeaderInfo = new();
                currentSystemInfo = null;

                // Re-connect in Direct mode for performance.
                manager.Connect(ConnectionMode.Direct);
                manager.Clear();

                Core.AddGridItem(this, new ArchivistGrid()
                {
                    Timestamp = DateTime.UtcNow.ToString(),
                    Details = $"Read All started",
                });
            }
            else
            {
                if (args.PreviousState.HasFlag(LogMonitorState.Batch))
                {
                    Core.AddGridItem(this, new ArchivistGrid()
                    {
                        Timestamp = DateTime.UtcNow.ToString(),
                        Details = $"Read All completed",
                    });
                }
                MaybeFlushSystemData(currentSystemInfo);
                manager.Connect(ConnectionMode.Shared);
            }
        }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            switch (journal)
            {
                case FileHeader fileHeader:
                    lastFileHeaderInfo = new();
                    lastFileHeaderInfo.FileHeader = fileHeader;
                    break;
                case LoadGame loadGame:
                    lastFileHeaderInfo.Commander = loadGame.Commander;
                    // Keep only latest value to avoid relogging from clogging up the works.
                    lastFileHeaderInfo.LoadGame = loadGame;
                    break;
                case Statistics statistics:
                    // Keep only latest value to avoid relogging from clogging up the works.
                    lastFileHeaderInfo.Statistics = statistics;
                    break;
                case FSDJump fsdJump:
                    if (!Core.CurrentLogMonitorState.HasFlag(LogMonitorState.PreRead)
                        && currentSystemInfo != null && fsdJump.StarSystem != currentSystemInfo.SystemName)
                    {
                        MaybeFlushSystemData(currentSystemInfo);
                    }

                    if (currentSystemInfo == null || fsdJump.StarSystem != currentSystemInfo.SystemName)
                    {
                        // Initialize the current system during pre-read in case we find/do more stuff or haven't flushed it yet.
                        // Duplicate entries should be filtered out by AddSystemJournalJason.
                        currentSystemInfo = LoadOrInitSystemInfo(
                            lastFileHeaderInfo, fsdJump.StarSystem, fsdJump.SystemAddress, fsdJump.TimestampDateTime);
                    }
                    // Don't add extraneous jumps.
                    if (currentSystemInfo.SystemJournalEntries.Count == 0)
                        currentSystemInfo.AddSystemJournalJson(fsdJump.Json, fsdJump.TimestampDateTime);
                    break;
                case Location location:
                    if (!Core.CurrentLogMonitorState.HasFlag(LogMonitorState.PreRead)
                        && currentSystemInfo != null && location.StarSystem != currentSystemInfo.SystemName)
                    {
                        MaybeFlushSystemData(currentSystemInfo);
                    }

                    if (currentSystemInfo == null || location.StarSystem != currentSystemInfo.SystemName)
                    {
                        // Initialize the current system during pre-read in case we find/do more stuff or haven't flushed it yet.
                        // Duplicate entries should be filtered out by AddSystemJournalJason.
                        currentSystemInfo = LoadOrInitSystemInfo(
                            lastFileHeaderInfo, location.StarSystem, location.SystemAddress, location.TimestampDateTime);
                    }
                    // Don't add extraneous locations.
                    if (currentSystemInfo.SystemJournalEntries.Count == 0)
                        currentSystemInfo.AddSystemJournalJson(location.Json, location.TimestampDateTime);
                    break;
                case DiscoveryScan discoveryScan:
                case FSSDiscoveryScan fssDiscoveryScan:
                case FSSBodySignals fssBodySignals:
                // case FSSSignalDiscovered fssSignalDiscovered:
                case FSSAllBodiesFound fssAllBodiesFound:
                case Scan scan:
                case ScanBaryCentre scanBaryCentre:
                case ScanOrganic scanOrganic:
                case NavBeaconScan navBeaconScan:
                case SAASignalsFound saaSignalsFound:
                case SAAScanComplete saaScanComplete:
                case CodexEntry codexEntry:
                    if (currentSystemInfo != null && !Core.CurrentLogMonitorState.HasFlag(LogMonitorState.PreRead))
                        currentSystemInfo.AddSystemJournalJson(journal.Json, journal.TimestampDateTime);
                    break;
            }
        }

        private CurrentSystemInfo LoadOrInitSystemInfo(FileHeaderInfo lastFileHeaderInfo, string starSystem, ulong systemAddress, DateTime timestampDateTime)
        {
            CurrentSystemInfo systemInfo = null;
            VisitedSystem systemData = manager.Get(starSystem, lastFileHeaderInfo.Commander);
            if (systemData != null)
            {
                systemInfo = new(systemData);
                systemInfo.VisitCount++;
            }
            else
            {
                systemInfo = new(lastFileHeaderInfo, starSystem, systemAddress, timestampDateTime);
            }
            return systemInfo;
        }

        private void MaybeFlushSystemData(CurrentSystemInfo currentSystemInfo)
        {
            if (currentSystemInfo == null) return;

            manager.Upsert(currentSystemInfo.ToSystemInfo());
        }

        public class ArchivistGrid
        {
            public string Timestamp { get; set; }
            public string Details { get; set; }
        }
    }
}