using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Observatory.Framework;
using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;

namespace com.github.fredjk_gh.ObservatoryAggregator
{
    public class Aggregator : IObservatoryNotifier, IObservatoryWorker
    {
        private PluginUI pluginUI;
        private IObservatoryCore Core;
        ObservableCollection<object> GridCollection = new();
        private AggregatorSettings settings = new()
        {
            ShowAllBodySummaries = false,
            FilterSpec = "",
        };
        internal TrackedData data = new();
        private List<AggregatorGrid> _readAllGridItems = new();

        public string Name => "Observatory Aggregator";

        public string ShortName => Constants.PLUGIN_SHORT_NAME;

        public string Version => typeof(Aggregator).Assembly.GetName().Version.ToString();

        public PluginUI PluginUI => pluginUI;

        public object Settings
        {
            get => settings;
            set => settings = (AggregatorSettings)value;
        }

        public void Load(IObservatoryCore observatoryCore)
        {
            Core = observatoryCore;
            AggregatorGrid uiObject = new();
            GridCollection = new() { uiObject };
            pluginUI = new PluginUI(GridCollection);
            data.Settings = (AggregatorSettings)Settings; 
        }

        public void LogMonitorStateChanged(LogMonitorStateChangedEventArgs args)
        {
            // * -> ReadAll
            if (args.NewState.HasFlag(LogMonitorState.Batch))
            {
                _readAllGridItems.Clear();
                Core.ClearGrid(this, new AggregatorGrid());
            }
            // ReadAll -> *
            else if (args.PreviousState.HasFlag(LogMonitorState.Batch))
            {
                _readAllGridItems.AddRange(data.ToGrid(true));
                Core.AddGridItems(this, _readAllGridItems);
            }
        }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            switch(journal)
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
                    data.CurrentCommander = loadGame.Commander;
                    data.CurrentShip = loadGame.ShipName;
                    break;
                case Loadout loadout:
                    data.CurrentShip = loadout.ShipName;
                    break;
                case FSSDiscoveryScan dScan:
                    data.CurrentSystem.DiscoveryScan = dScan;
                    RedrawGrid();
                    break;
                case FSSAllBodiesFound allFound:
                    data.CurrentSystem.AllBodiesFound = allFound;
                    RedrawGrid();
                    break;
                case FSSBodySignals bodySignals:
                    data.AddBodySignals(bodySignals);
                    // No redraw -- no body info yet, typically.
                    break;
                case Scan scan:
                    if (!scan.BodyName.Contains("belt", StringComparison.InvariantCultureIgnoreCase))
                    {
                        data.AddScan(scan);
                        if (data.CurrentSystem != null
                            && scan.DistanceFromArrivalLS == 0
                            && scan.ScanType != "NavBeaconDetail"
                            && scan.PlanetClass != "Barycentre"
                            && !scan.WasDiscovered)
                        {
                            data.CurrentSystem.IsUndiscovered = true;
                        }
                        RedrawGrid();
                    }
                    break;
                case SAAScanComplete scanComplete:
                    data.AddScanComplete(scanComplete);
                    RedrawGrid();
                    break;
            }
        }

        public void OnNotificationEvent(NotificationArgs args)
        {
            List<NotificationData> nList = MaybeSplitMultilineArgs(args);

            foreach (var n in nList)
            {
                data.AddNotification(n);
            }
            RedrawGrid();
        }

        private List<NotificationData> MaybeSplitMultilineArgs(NotificationArgs args)
        {
            List<NotificationData> notifications = new();

            if (args.Detail.Trim().Contains(Environment.NewLine))
            {
                string[] details = args.Detail.Trim().Split(Environment.NewLine);
                string[] extDetails = args.ExtendedDetails.Trim().Split(Environment.NewLine);

                for (int i = 0; i < details.Length; i++)
                {
                    if (string.IsNullOrEmpty(details[i].Trim()) && (i >= extDetails.Length || string.IsNullOrEmpty(extDetails[i].Trim()))) continue;

                    // Clone the core bits we care about but split the detail/extdetails.
                    NotificationData split = new(
                        data.CurrentSystem.Name, args.Sender, args.Title, details[i], (i < extDetails.Length ? extDetails[i] : ""), args.CoalescingId);

                    if (shouldShow(split))
                        notifications.Add(split);
                }
            }
            else
            {
                NotificationData nData = new(data.CurrentSystem.Name, args);
                if (shouldShow(nData))
                    notifications.Add(nData);
            }

            return notifications;
        }

        public void HandlePluginMessage(string sourceName, string sourceVersion, object messageArgs)
        {
            switch (sourceName)
            {
                case "Observatory Archivist":
                    // Maybe receive previous scan records on FSDJump event?
                    List<string> existingData = messageArgs as List<string>;

                    if (existingData != null)
                    {
                        // Deserialize and pump into data for current system.
                        foreach (var line in existingData)
                        {
                            var eventType = JournalUtilities.GetEventType(line);

                        }
                    }
                    break;

                case "Observatory BioInsights": // or Observatory Evaluator?
                    // Maybe receive notification of interest and/or visitation?
                    break;
            }
        }

        private void RedrawGrid()
        {
            // Check for Pre-read too?
            if (Core.CurrentLogMonitorState.HasFlag(LogMonitorState.Realtime))
            {
                Core.ClearGrid(this, new AggregatorGrid());
                Core.AddGridItems(this, data.ToGrid(false)) ;
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
            if ((data.CurrentSystem?.Name ?? "") != newSystem)
            {
                if (Core.CurrentLogMonitorState.HasFlag(LogMonitorState.Batch))
                {
                    if (data.CurrentSystem != null) _readAllGridItems.AddRange(data.ToGrid(true));
                    data.ChangeSystem(newSystem);
                }
                else
                {
                    data.ChangeSystem(newSystem);
                    RedrawGrid();
                }
            }
        }
    }
}
