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

        public string Name => "Observatory Aggregator";

        public string ShortName => Constants.PLUGIN_SHORT_NAME;

        public string Version => typeof(Aggregator).Assembly.GetName().Version.ToString();

        public PluginUI PluginUI => _pluginUI;

        public IObservatoryComparer ColumnSorter => new NoOpColumnSorter();

        public object Settings
        {
            get => settings;
            set {
                settings = (AggregatorSettings)value;
                settings.OpenAggregatorWiki = OpenWikiUrl;

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
                            MaybeChangeSystem(carrierJump.StarSystem);
                        }
                    }
                    else
                    {
                        MaybeChangeSystem(fsdJump.StarSystem);
                    }
                    break;
                case Location location:
                    MaybeChangeSystem(location.StarSystem);
                    break;
                case LoadGame loadGame:
                    _data.CurrentCommander = loadGame.Commander;
                    _data.CurrentShip = loadGame.ShipName;
                    break;
                case Loadout loadout:
                    _data.CurrentShip = loadout.ShipName;
                    break;
                case FSSDiscoveryScan dScan:
                    _data.CurrentSystem.DiscoveryScan = dScan;
                    RedrawGrid();
                    break;
                case FSSAllBodiesFound allFound:
                    _data.CurrentSystem.AllBodiesFound = allFound;
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
                    RedrawGrid();
                    break;
            }
        }

        public void OnNotificationEvent(NotificationArgs args)
        {
            List<NotificationData> nList = MaybeSplitMultilineArgs(args);

            foreach (var n in nList)
            {
                _data.AddNotification(n);
            }
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

        private List<NotificationData> MaybeSplitMultilineArgs(NotificationArgs args)
        {
            List<NotificationData> notifications = new();

            if (args.Detail.Trim().Contains(Environment.NewLine))
            {
                string[] details = args.Detail.Trim().Split(Environment.NewLine);
                string[] extDetails = args.ExtendedDetails?.Trim().Split(Environment.NewLine) ?? new string[0];

                for (int i = 0; i < details.Length; i++)
                {
                    if (string.IsNullOrEmpty(details[i].Trim()) && (i >= extDetails.Length || string.IsNullOrEmpty(extDetails[i].Trim()))) continue;

                    // Clone the core bits we care about but split the detail/extdetails.
                    NotificationData split = new(
                        _data.CurrentSystem.Name, args.Sender, args.Title, details[i], (i < extDetails.Length ? extDetails[i] : ""), args.CoalescingId ?? Constants.DEFAULT_COALESCING_ID);

                    if (shouldShow(split))
                        notifications.Add(split);
                }
            }
            else
            {
                NotificationData nData = new(_data.CurrentSystem.Name, args);
                if (shouldShow(nData))
                    notifications.Add(nData);
            }

            return notifications;
        }

        private void RedrawGrid()
        {
            // Check for Pre-read too?
            if ((Core.CurrentLogMonitorState & LogMonitorState.Realtime) != 0)
            {
                var gridItems = _data.ToGrid(false);
                _ui.SetGridItems(gridItems);
            }
        }

        private bool shouldShow(NotificationData nData)
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

        private void MaybeChangeSystem(string newSystem)
        {
            if ((_data.CurrentSystem?.Name ?? "") != newSystem)
            {
                if ((Core.CurrentLogMonitorState & LogMonitorState.Batch) != 0)
                {
                    if (_data.CurrentSystem != null) _readAllGridItems.AddRange(_data.ToGrid(true));
                    _data.ChangeSystem(newSystem);
                }
                else
                {
                    _data.ChangeSystem(newSystem);
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
