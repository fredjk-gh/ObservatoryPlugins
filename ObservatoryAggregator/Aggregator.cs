using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http.Headers;
using Observatory.Framework;
using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Sorters;
using Observatory.Framework.Interfaces;
using com.github.fredjk_gh.ObservatoryAggregator.UI;

namespace com.github.fredjk_gh.ObservatoryAggregator
{
    public class Aggregator : IObservatoryNotifier, IObservatoryWorker
    {
        private static string AGGREGATOR_WIKI_URL = "https://github.com/fredjk-gh/ObservatoryPlugins/wiki/Plugin:-Aggregator";

        private IObservatoryCore Core;
        private AggregatorSettings settings = AggregatorSettings.DEFAULT;
        private PluginUI _pluginUI;
        private TrackedData _data = new();
        private AggregatorUIPanel _ui;
        private List<AggregatorGrid> _readAllGridItems = new();
        private AboutInfo _aboutInfo = new()
        {
            FullName = $"Notification {Constants.PLUGIN_SHORT_NAME}",
            ShortName = Constants.PLUGIN_SHORT_NAME,
            Description = "The Aggregator plugin is a notification log -- collecting notifications from all other plugins into one place to reduce the number of times you need to switch between plugins.",
            AuthorName = "fredjk-gh",
            Links = new()
            {
                new AboutLink("github", "https://github.com/fredjk-gh/ObservatoryPlugins"),
                new AboutLink("github release notes", "https://github.com/fredjk-gh/ObservatoryPlugins/wiki/Plugin:-Aggregator"),
                new AboutLink("Documentation", "https://observatory.xjph.net/usage/plugins/thirdparty/aggregator"),
            }
        };

        public AboutInfo AboutInfo => _aboutInfo;
        public string Version => typeof(Aggregator).Assembly.GetName().Version.ToString();
        public PluginUI PluginUI => _pluginUI;

        public object Settings
        {
            get => settings;
            set {
                settings = (AggregatorSettings)value;

                _data.ApplySettings(settings);
            }
        }

        public void Load(IObservatoryCore observatoryCore)
        {
            Core = observatoryCore;
            _data.Load(Core, this, settings);
            _ui = new AggregatorUIPanel(_data);
            _pluginUI = new PluginUI(PluginUI.UIType.Panel, _ui);
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args)
        {
            // * -> ReadAll
            if ((args.NewState & LogMonitorState.Batch) != 0)
            {
                _readAllGridItems.Clear();
                //Core.ClearGrid(this, new AggregatorGrid());
                _ui.Clear();
            }
            // ReadAll -> *
            else if ((args.PreviousState & LogMonitorState.Batch) != 0)
            {
                _readAllGridItems.AddRange(_data.ToGrid(true));
                //Core.AddGridItems(this, _readAllGridItems);
                Core.ExecuteOnUIThread(() =>
                {
                    _ui.SetGridItems(_readAllGridItems);
                });
            }
            // PreRead -> *
            // -> Realtime
            else if ((args.PreviousState & LogMonitorState.PreRead) != 0 || (args.NewState & LogMonitorState.Realtime) != 0)
            {
                RedrawGrid();
            }
        }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            switch (journal)
            {
                case FSDJump fsdJump: // and CarrierJump
                    CarrierJump carrierJump = fsdJump as CarrierJump;
                    if (carrierJump != null)
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
                    _data.CurrentCommander = loadGame.Commander;
                    _data.ChangeShip(loadGame.ShipID, loadGame.ShipName);
                    break;
                case Loadout loadout:
                    _data.ChangeShip(loadout.ShipID, loadout.ShipName);
                    RedrawGrid();
                    break;
                case StoredShips ships:
                    foreach (var ship in ships.ShipsHere.Concat(ships.ShipsRemote))
                    {
                        _data.AddKnownShip(ship.ShipID, ship.Name);
                    }
                    break;
                case ShipyardSwap swapShip:
                    _data.ChangeShip(swapShip.ShipID);
                    RedrawGrid();
                    break;
                case FSSDiscoveryScan dScan:
                    _data.AddDiscoveryScan(dScan);
                    RedrawGrid();
                    break;
                case FSSAllBodiesFound allFound:
                    _data.AddAllBodiesFound(allFound);
                    RedrawGrid();
                    break;
                case FSSBodySignals bodySignals:
                    _data.AddBodySignals(bodySignals);
                    // No redraw -- no body info yet, typically.
                    break;
                case Scan scan:
                    if (!scan.BodyName.Contains("belt", StringComparison.InvariantCultureIgnoreCase))
                    {
                        _data.AddScan(scan);
                        if (_data.CurrentSystem != null
                            && scan.DistanceFromArrivalLS == 0
                            && scan.ScanType != "NavBeaconDetail"
                            && scan.PlanetClass != "Barycentre"
                            && !scan.WasDiscovered)
                        {
                            _data.CurrentSystem.IsUndiscovered = true;
                        }
                        // This generates a ton of flicker. Maybe this can be handled by Core (suppressing re-paints?)
                        // Make this somewhat conditional or only if data is "dirty"?
                        RedrawGrid();
                    }
                    break;
                case SAAScanComplete scanComplete:
                    _data.AddScanComplete(scanComplete);
                    _data.MarkVisited(scanComplete.BodyID);
                    RedrawGrid();
                    break;
                //case ApproachBody approachBody:
                //    _data.MarkVisited(approachBody.BodyID);
                //    RedrawGrid();
                //    break;
                case Touchdown touchdown:
                    _data.MarkVisited(touchdown.BodyID);
                    RedrawGrid();
                    break;
            }
        }

        public void StatusChange(Status status)
        {
            if (status.Destination != null)
                _data.MaybeChangeDestinationBody(status.Destination);
        }

        public void OnNotificationEvent(NotificationArgs args)
        {
            _data.AddNotification(new(_data, _data.CurrentSystem.Name, args));
            RedrawGrid();
        }

        public void HandlePluginMessage(string sourceName, string sourceVersion, object messageArgs)
        {
            switch (sourceName)
            {
                case "Observatory Archivist":
                    HandleArchivistMsg(sourceVersion, messageArgs);
                    break;

                // Observatory Evaluator?
                case "Observatory BioInsights":
                    // Maybe receive notification of interest and/or visitation?
                    break;
            }
        }

        public void HandleArchivistMsg(string version, object messageArgs)
        {
            // We get values in the form of Tuple<string, object> from Archivist.
            //
            // Known messages:
            // - archivist_known_system_journal_json
            //
            Tuple<string, object> msgTuple = messageArgs as Tuple<string, object>;

            if (msgTuple == null) return; // Unrecognized msg from Archivist!

            //switch (msgTuple.Item1)
            //{
            //    case "archivist_known_system_journal_json":
            //        // In this event, the value is an tuple of data.
            //        var value = ((string Commander, string SystemName, int VisitCount, List<string> SystemJournalEntries))msgTuple.Item2;

            //        // Deserialize and pump into data for current system.
            //        foreach (var line in value.SystemJournalEntries)
            //        {
            //            var eventType = JournalUtilities.GetEventType(line);

            //            // TODO rehydrate object from this JSON.
            //        }
            //        break;
            //}
        }

        private void RedrawGrid()
        {
            // Check for Pre-read too?
            if ((Core.CurrentLogMonitorState & LogMonitorState.Realtime) != 0)
            {
                var gridItems = _data.ToGrid();
                _ui.SetGridItems(gridItems);
            }
        }

        private bool ShouldShow(NotificationData nData)
        {
            var show = true;
            foreach (string f in settings.Filters)
            {
                if (f.Trim().Length == 0) continue;
                if (nData.Title.Contains(f, StringComparison.InvariantCultureIgnoreCase)
                    || nData.Detail.Contains(f, StringComparison.InvariantCultureIgnoreCase)
                    || (nData.Sender.Contains(f, StringComparison.InvariantCultureIgnoreCase))
                    || (nData.ExtendedDetails?.Contains(f, StringComparison.InvariantCultureIgnoreCase) ?? false))
                {
                    show = false;
                    break;
                }
            }
            return show;
        }

        private void MaybeChangeSystem(string newSystem, ulong systemAddress)
        {
            if ((_data.CurrentSystem?.Name ?? "") != newSystem)
            {
                if ((Core.CurrentLogMonitorState & LogMonitorState.Batch) != 0)
                {
                    if (_data.CurrentSystem != null) _readAllGridItems.AddRange(_data.ToGrid(true));
                    _data.ChangeSystem(newSystem, systemAddress);
                }
                else
                {
                    _data.ChangeSystem(newSystem, systemAddress);
                    RedrawGrid();
                }
            }
        }

        private void OpenWikiUrl()
        {
            OpenUrl(AGGREGATOR_WIKI_URL);
        }

        private void OpenUrl(string url)
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
}
