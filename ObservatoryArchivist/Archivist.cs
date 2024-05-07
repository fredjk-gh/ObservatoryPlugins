using com.github.fredjk_gh.ObservatoryArchivist.UI;
using LiteDB;
using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace com.github.fredjk_gh.ObservatoryArchivist
{
    public class Archivist : IObservatoryWorker
    {
        private PluginUI pluginUI;
        private ArchivistPanel _archivistPanel;
        private ArchivistSettings settings = ArchivistSettings.DEFAULT;
        private ArchivistContext _context;

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
            
            _context = new ArchivistContext()
            {
                Core = observatoryCore,
                PluginWorker = this,
                Settings = ArchivistSettings.DEFAULT,
                ErrorLogger = observatoryCore.GetPluginErrorLogger(this),
                Manager = new(observatoryCore.PluginStorageFolder, observatoryCore.GetPluginErrorLogger(this)),
            };
            _context.DeserializeState();

            _archivistPanel = new ArchivistPanel(_context);
            pluginUI = new PluginUI(PluginUI.UIType.Panel, _archivistPanel);
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args)
        {
            // * -> ReadAll
            if (args.NewState.HasFlag(LogMonitorState.Batch))
            {
                _context.Data.ResetForReadAll();

                // Re-connect in Direct mode for performance.
                _context.Manager.Connect(ConnectionMode.Direct);
                _context.Manager.Clear();
                _context.UI.SetMessage("Read All started");
            }
            // ReadAll -> *
            else if (args.PreviousState.HasFlag(LogMonitorState.Batch))
            {
                _context.FlushIfDirty(/* force= */ true);
                _context.Manager.Connect(ConnectionMode.Shared);
                _context.Core.ExecuteOnUIThread(() =>
                {
                    _context.UI.Draw();
                    _context.DisplaySummary("Read All completed");
                });
                _context.SerializeState();
            }
            else if (args.NewState.HasFlag(LogMonitorState.Realtime))
            {
                _context.Core.ExecuteOnUIThread(() =>
                {
                    _context.UI.Draw();
                    _context.DisplaySummary();
                });
            }
        }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            switch (journal)
            {
                case FileHeader fileHeader:
                    // This happens before we know who the Commander it. Cache it until needed (LoadGame).
                    _context.Data.LastFileHeader = fileHeader;
                    break;
                case LoadGame loadGame:
                    bool isNewCommander = false;
                    bool isDifferentCommander = _context.Data.CurrentCommander != loadGame.Commander;
                    if (!_context.Data.KnownCommanders.ContainsKey(loadGame.Commander))
                    {
                        isNewCommander = true;
                    }

                    _context.Data.CurrentCommander = loadGame.Commander;
                    // Keep only latest value to avoid relogging from clogging up the works.
                    _context.Data.ForCommander().FileHeaderInfo.FileHeader = _context.Data.LastFileHeader;
                    _context.Data.ForCommander().FileHeaderInfo.LoadGame = loadGame;

                    if (isNewCommander || isDifferentCommander)
                    {
                        _context.Core.ExecuteOnUIThread(() =>
                        {
                            _context.UI.Draw($"Switched to Cmdr {loadGame.Commander}.{(isNewCommander ? " o7!" : "")}");
                        });
                        _context.SerializeState();
                    }
                    break;
                case Statistics statistics:
                    // Keep only latest value to avoid relogging from clogging up the works.
                    _context.Data.ForCommander().FileHeaderInfo.Statistics = statistics;
                    break;
                case FSDJump fsdJump:
                    ProcessNewLocation(fsdJump.StarSystem, fsdJump.SystemAddress, fsdJump.TimestampDateTime, fsdJump.Json);
                    break;
                case Location location:
                    ProcessNewLocation(location.StarSystem, location.SystemAddress, location.TimestampDateTime, location.Json);
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
                    if (_context.Data.ForCommander()?.CurrentSystem != null && !_context.Core.CurrentLogMonitorState.HasFlag(LogMonitorState.PreRead))
                    {
                        _context.Data.ForCommander().CurrentSystem.AddSystemJournalJson(journal.Json, journal.TimestampDateTime);
                        _context.FlushIfDirty();
                    }

                    _context.Core.ExecuteOnUIThread(() =>
                    {
                        _context.UI.PopulateLatestRecord();
                        _context.UI.SetMessage($"Captured journal event of type: {journal.Event}.{Environment.NewLine}{_context.Data.ForCommander().CurrentSystem.SystemJournalEntries.Count} entries captured so far...");
                    });
                    break;
                case Shutdown shutdown:
                    _context.FlushIfDirty();
                    break;
            }
        }

        private void ProcessNewLocation(string newSystemName, ulong newSystemAddress, DateTime timestamp, string json)
        {
            if (!_context.Core.CurrentLogMonitorState.HasFlag(LogMonitorState.PreRead)
                && _context.Data.ForCommander()?.CurrentSystem != null
                && newSystemName != _context.Data.ForCommander()?.CurrentSystem.SystemName)
            {
                _context.FlushIfDirty(/* force= */ true);
            }

            if (_context.Data.ForCommander()?.CurrentSystem == null
                || newSystemName != _context.Data.ForCommander()?.CurrentSystem.SystemName)
            {
                // Initialize the current system during pre-read in case we find/do more stuff or haven't flushed it yet.
                // Duplicate entries should be filtered out by AddSystemJournalJason.
                _context.Data.ForCommander().CurrentSystem = _context.Manager.LoadOrInitSystemInfo(
                    _context.Data.ForCommander().FileHeaderInfo, newSystemName, newSystemAddress, timestamp);
                _context.SerializeState();
            }

            // Don't add extraneous jumps.
            if (_context.Data.ForCommander()?.CurrentSystem.SystemJournalEntries.Count == 0)
                _context.Data.ForCommander().CurrentSystem.AddSystemJournalJson(json, timestamp);
            else
                MaybeShareSystemData();

            _context.Core.ExecuteOnUIThread(() =>
            {
                _context.UI.PopulateCurrentSystem();
                _context.UI.SetMessage($"New system detected.");
            });
        }

        private void MaybeShareSystemData()
        {
            // Send existing system data via inter-plugin message bus, in case anyone can use it.
            if (!_context.Core.IsLogMonitorBatchReading && settings.ShareSystemData)
            {
                // Send:
                // - Commander
                // - SystemName
                // - VisitCount
                // - SystemJournalEntries
                // ??? Premable?
                (string Commander, string SystemName, int VisitCount, List<string> SystemJournalEntries) msgValue =
                (
                    _context.Data.ForCommander().CurrentSystem.Commander,
                    _context.Data.ForCommander().CurrentSystem.SystemName,
                    _context.Data.ForCommander().CurrentSystem.VisitCount,
                    _context.Data.ForCommander().CurrentSystem.SystemJournalEntries
                );

                Tuple<string, object> msg = new("archivist_known_system_data", msgValue);

                _context.Core.SendPluginMessage(this, msg);
                _context.Core.ExecuteOnUIThread(() =>
                {
                    _context.UI.SetMessage($"Found {_context.Data.ForCommander().CurrentSystem.SystemJournalEntries.Count} records from a previous visit; shared via inter-plugin message.");
                });
            }
        }
    }
}