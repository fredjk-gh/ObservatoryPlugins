using System;
using System.Collections.ObjectModel;
using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;

namespace ObservatoryAggregator
{
    public class Aggregator : IObservatoryNotifier, IObservatoryWorker
    {
        private PluginUI pluginUI;
        private IObservatoryCore Core;
        ObservableCollection<object> GridCollection = new();
        private AggregatorSettings settings = new()
        {
            ShowCurrentSystemOnly = true,
        };
        private string CurrentSystem;
        private string CurrentCommander;

        public string Name => "Observatory Aggregator";

        public string ShortName => "Aggregator";

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
            AggregatorNotificationGrid uiObject = new();

            GridCollection = new();
            GridCollection.Add(uiObject);
            pluginUI = new PluginUI(GridCollection);
        }

        public void JournalEvent<TJournal>(TJournal journal) where TJournal : JournalBase
        {
            switch(journal)
            {
                case FSDJump fsdJump: // and CarrierJump
                    CarrierJump carrierJump = fsdJump as CarrierJump;
                    if (carrierJump != null)
                    {
                        if (carrierJump.Docked) MaybeChangeSystem(carrierJump.StarSystem);
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
                    CurrentCommander = loadGame.Commander;
                    break;
            }
        }

        public void OnNotificationEvent(NotificationArgs args)
        {
            var gridItem = new AggregatorNotificationGrid()
            {
                Timestamp = DateTime.Now.ToString(),
                System = CurrentSystem,
                Title = args.Title,
                Detail = args.Detail,
#if EXTENDED_EVENT_ARGS
                Sender = args.Sender != null ? args.Sender.ShortName : "",
                ExtendedDetails = args.ExtendedDetails,
#endif
            };
            Core.AddGridItem(this, gridItem);
        }

        private void MaybeChangeSystem(string newSystem)
        {
            if (CurrentSystem != newSystem)
            {
                if (settings.ShowCurrentSystemOnly)
                {
                    Core.ClearGrid(this, new AggregatorNotificationGrid());
                }
                Core.AddGridItem(this, new AggregatorNotificationGrid()
                {
                    Timestamp = DateTime.Now.ToString(),
                    System = newSystem,
                    Detail = CurrentCommander,
#if EXTENDED_EVENT_ARGS
                    Sender = this.GetType().Name,
                    ExtendedDetails = $"{this.ShortName} version: v{this.Version}",
#endif
                });
            }
            CurrentSystem = newSystem;
        }
    }

    public class AggregatorNotificationGrid
    {
        public string Timestamp { get; set; }
        public string System { get; set; }
        public string Sender { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string ExtendedDetails { get; set; }
    }
}
