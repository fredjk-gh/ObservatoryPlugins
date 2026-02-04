using com.github.fredjk_gh.ObservatoryAggregator.UI;
using com.github.fredjk_gh.PluginCommon.AutoUpdates;
using com.github.fredjk_gh.PluginCommon.Data;
using com.github.fredjk_gh.PluginCommon.PluginInterop.Messages;
using Observatory.Framework;
using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;

namespace com.github.fredjk_gh.ObservatoryAggregator
{
    public class Aggregator : IObservatoryNotifier, IObservatoryWorker
    {
        private static readonly Guid PLUGIN_GUID = new("b4654977-434b-424f-a9fd-b2ac5be9915a");
        private static readonly AboutLink GH_LINK = new("github", "https://github.com/fredjk-gh/ObservatoryPlugins");
        private static readonly AboutLink GH_RELEASE_NOTES_LINK = new("github release notes", "https://github.com/fredjk-gh/ObservatoryPlugins/wiki/Plugin:-Aggregator");
        private static readonly AboutLink DOC_LINK = new("Documentation", "https://observatory.xjph.net/usage/plugins/thirdparty/fredjk-gh/aggregator");
        private static readonly AboutInfo ABOUT_INFO = new()
        {
            FullName = $"Notification {Constants.PLUGIN_SHORT_NAME}",
            ShortName = Constants.PLUGIN_SHORT_NAME,
            Description = "The Aggregator plugin is a notification log -- collecting notifications from all other plugins into one place to reduce the number of times you need to switch between plugins.",
            AuthorName = "fredjk-gh",
            Links =
            [
                GH_LINK,
                GH_RELEASE_NOTES_LINK,
                DOC_LINK,
            ]
        };

        private readonly AggregatorContext _c = new();
        private AggregatorSettings settings = new();
        private PluginUI _pluginUI;
        private AggregatorUIPanel _ui;
        private bool _replayMode = false;

        public static Guid Guid => PLUGIN_GUID;
        public AboutInfo AboutInfo => ABOUT_INFO;
        public string Version => typeof(Aggregator).Assembly.GetName().Version.ToString();
        public PluginUI PluginUI => _pluginUI;
        public bool OverrideAcceptNotificationsDuringBatch { get => true; }

        internal bool ReplayMode
        {
            get => _replayMode;
            set
            {
                _replayMode = value;
            }
        }

        public bool SuppressUIUpdates
        {
            get => ReplayMode || _c.Core.IsLogMonitorBatchReading;
        }

        public object Settings
        {
            get => settings;
            set
            {
                settings = (AggregatorSettings)value;

                MaybeFixUnsetSettings();
                _c.ApplySettings(settings);
            }
        }

        private void MaybeFixUnsetSettings()
        {
            if (string.IsNullOrEmpty(settings.GridSizingModeString))
            {
                settings.GridSizingModeEnum = AggregatorSettings.GridSizingMode.AutoFit;
            }
        }

        public void Load(IObservatoryCore observatoryCore)
        {
            _c.Load(observatoryCore, this, settings);
            _ui = new(_c);
            _pluginUI = new(PluginUI.UIType.Panel, _ui);

            _c.Dispatcher.RegisterHandler<GenericPluginReadyMessage>(HandleReadyMessage);
            _c.Dispatcher.RegisterHandler<ArchivistNotificationsMessage>(HandleArchivistNotifications);
            _c.Dispatcher.RegisterHandler<ArchivistJournalsMessage>(HandleArchivistScansMessage);
        }

        public PluginUpdateInfo CheckForPluginUpdate()
        {
            AutoUpdateHelper.Init(_c.Core);
            return AutoUpdateHelper.CheckForPluginUpdate(
                this, GH_RELEASE_NOTES_LINK.Url, settings.EnableAutoUpdates, settings.EnableBetaUpdates);
        }

        public void ObservatoryReady()
        {
            _c.Dispatcher.SendMessage(GenericPluginReadyMessage.New());
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args)
        {
            // * -> ReadAll
            if ((args.NewState & LogMonitorState.Batch) != 0)
            {
                _ui.Clear();
            }
            // ReadAll -> *
            else if ((args.PreviousState & LogMonitorState.Batch) != 0)
            {
                RedrawGrid(true);
            }
            // PreRead -> *
            // -> Realtime
            else if ((args.PreviousState & LogMonitorState.PreRead) != 0 || (args.NewState & LogMonitorState.Realtime) != 0)
            {
                if (_c.LoadTimeNotifications.Count > 0)
                {
                    _c.LoadTimeNotifications.ForEach(n => _c.AddNotification(n));
                    _c.LoadTimeNotifications.Clear();
                }
                RedrawGrid();
            }
        }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            switch (journal)
            {
                case FSDJump fsdJump: // and CarrierJump
                    if (fsdJump is CarrierJump carrierJump)
                    {
                        if (carrierJump.Docked || carrierJump.OnFoot)
                        {
                            MaybeChangeSystem(carrierJump.StarSystem, carrierJump.SystemAddress);
                        }
                    }
                    else
                    {
                        MaybeChangeSystem(fsdJump.StarSystem, fsdJump.SystemAddress);
                    }
                    break;
                case Location location:
                    MaybeChangeSystem(location.StarSystem, location.SystemAddress);
                    break;
                case LoadGame loadGame:
                    _c.CurrentCommander = loadGame.Commander;
                    _c.ChangeShip(loadGame.ShipID, loadGame.ShipName);
                    break;
                case Loadout loadout:
                    _c.ChangeShip(loadout.ShipID, loadout.ShipName);
                    RedrawGrid();
                    break;
                case StoredShips ships:
                    foreach (var ship in ships.ShipsHere.Concat(ships.ShipsRemote))
                    {
                        _c.AddKnownShip(ship.ShipID, ship.Name);
                    }
                    break;
                case ShipyardSwap swapShip:
                    _c.ChangeShip(swapShip.ShipID);
                    RedrawGrid();
                    break;
                case SetUserShipName shipNameChange:
                    _c.UpdateShip(shipNameChange.ShipID, shipNameChange.UserShipName);
                    RedrawGrid();
                    break;
                case FSSDiscoveryScan dScan:
                    _c.AddDiscoveryScan(dScan);
                    RedrawGrid();
                    break;
                case FSSAllBodiesFound allFound:
                    _c.AddAllBodiesFound(allFound);
                    RedrawGrid();
                    break;
                case FSSBodySignals bodySignals:
                    _c.AddBodySignals(bodySignals);
                    // No redraw -- no body info yet, typically.
                    break;
                case Scan scan:
                    if (!scan.BodyName.Contains("belt", StringComparison.OrdinalIgnoreCase))
                    {
                        _c.AddScan(scan);
                        if (_c.CurrentSystem != null
                            && scan.DistanceFromArrivalLS == 0
                            && scan.ScanType != "NavBeaconDetail"
                            && scan.PlanetClass != "Barycentre"
                            && !scan.WasDiscovered)
                        {
                            _c.CurrentSystem.IsUndiscovered = true;
                        }

                        // This generates a ton of flicker. Maybe this can be handled by Core (suppressing re-paints?)
                        // Make this somewhat conditional or only if data is "dirty"?
                        RedrawGrid();
                    }
                    break;
                case ScanBaryCentre scanBarycentre:
                    _c.AddScanBarycentre(scanBarycentre);
                    RedrawGrid();
                    break;
                case SAAScanComplete scanComplete:
                    _c.AddScanComplete(scanComplete);
                    _c.MarkVisited(scanComplete.BodyID);
                    RedrawGrid();
                    break;
                case SAASignalsFound scanSignals:
                    _c.AddScanSignals(scanSignals);
                    _c.MarkVisited(scanSignals.BodyID);
                    RedrawGrid();
                    break;
                //case ApproachBody approachBody:
                //    _data.MarkVisited(approachBody.BodyID);
                //    RedrawGrid();
                //    break;
                case Touchdown touchdown:
                    _c.MarkVisited(touchdown.BodyID);
                    RedrawGrid();
                    break;
            }
        }


        public void StatusChange(Status status)
        {
            if (status.Destination != null)
                _c.MaybeChangeDestinationBody(status.Destination);
        }

        public void OnNotificationEvent(NotificationArgs args)
        {
            NotificationData nd = new(_c, _c.CurrentSystem.Name, args);
            if (ShouldShow(nd))
            {
                _c.AddNotification(nd);
                RedrawGrid();
            }
        }

        public void HandlePluginMessage(MessageSender sender, PluginMessage messageArgs)
        {
            _c.Dispatcher.MaybeHandlePluginMessage(sender, messageArgs, out _);
        }

        private void HandleReadyMessage(GenericPluginReadyMessage readyMsg)
        {
            _c.Dispatcher.HandleReadyMessage(readyMsg);

            string displayName = readyMsg.Sender.FullName;
            NotificationData n = new(_c, "", readyMsg.Sender.ShortName, readyMsg.Type,
                $"{displayName} v{readyMsg.Sender.Version} reported ready!", "(via v2 message)", CoalescingIDs.DEFAULT_COALESCING_ID);
            _c.LoadTimeNotifications.Add(n);
        }

        public void HandleArchivistScansMessage(ArchivistJournalsMessage m)
        {
            // Make sure metadata matches the current situation.
            // TODO: Reconsider non-matching commander names?
            if (_c.CurrentSystem?.SystemAddress != m.Id64
                || (!m.IsGeneratedFromSpansh && _c.CurrentCommander != m.SourceCommanderName)) return;

            ReplayMode = true;

            try
            {
                foreach (var entry in m.SystemJournalEntries)
                {
                    JournalEvent(entry);
                }

                if (_c.CurrentCommander == m.SourceCommanderName && m.CommanderVisitCount > 1)
                {
                    NotificationArgs args = new()
                    {
                        CoalescingId = CoalescingIDs.SYSTEM_COALESCING_ID,
                        Guid = new(),
                        Title = "Visit count",
                        Detail = $"You have visited this system {m.CommanderVisitCount} times",
                        Rendering = NotificationRendering.PluginNotifier,
                        Sender = ABOUT_INFO.ShortName,
                    };
                    OnNotificationEvent(args);
                }
            }
            finally
            {
                ReplayMode = false;
                RedrawGrid();
            }
        }

        private void HandleArchivistNotifications(ArchivistNotificationsMessage notifications)
        {
            if (notifications == null || notifications.Notifications.Count == 0 || _c.Core.IsLogMonitorBatchReading) return;

            if (notifications.Id64 != _c.CurrentSystem.SystemAddress
                || notifications.SourceCommanderName != _c.CurrentCommander) return;

            ReplayMode = true;
            foreach(var n in notifications.Notifications)
            {
                OnNotificationEvent(n);
            }
            ReplayMode = false;
            RedrawGrid();
        }

        private void RedrawGrid(bool force = false)
        {
            if (SuppressUIUpdates) return;

            var gridItems = _c.ToGrid();
            _ui.SetGridItems(gridItems, force);
        }

        private bool ShouldShow(NotificationData nData)
        {
            var show = true;
            foreach (string f in settings.Filters)
            {
                if (f.Trim().Length == 0) continue;
                if (nData.Title.Contains(f, StringComparison.OrdinalIgnoreCase)
                    || nData.Detail.Contains(f, StringComparison.OrdinalIgnoreCase)
                    || (nData.Sender.Contains(f, StringComparison.OrdinalIgnoreCase))
                    || (nData.ExtendedDetails?.Contains(f, StringComparison.OrdinalIgnoreCase) ?? false))
                {
                    show = false;
                    break;
                }
            }
            return show;
        }

        private void MaybeChangeSystem(string newSystem, ulong systemAddress)
        {
            if ((_c.CurrentSystem?.Name ?? "") != newSystem)
            {
                _c.ChangeSystem(newSystem, systemAddress);
                if ((_c.Core.CurrentLogMonitorState & LogMonitorState.Batch) == 0)
                {
                    RedrawGrid();
                }
            }
        }
    }
}
