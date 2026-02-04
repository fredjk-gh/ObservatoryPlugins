using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using com.github.fredjk_gh.ObservatoryHelm.Data;

namespace com.github.fredjk_gh.ObservatoryHelm.UI
{
    internal class UIStateManager : INotifyPropertyChanged
    {
        public const string PROP_COMMANDERKEY = "CommanderKey";
        public const string PROP_REFSYSTEM = "RefSystem";
        public const string PROP_SYSTEMID64 = "SystemId64";
        public const string PROP_BODYID = "BodyId";
        public const string PROP_SURFACEPOSITION = "SurfacePosition";
        public const string PROP_SHIPID = "ShipId";
        public const string PROP_FUELREMAINING = "FuelRemaining";
        public const string PROP_JETCONEBOOST = "JetConeBoost";
        public const string PROP_CARGO = "Cargo";
        public const string PROP_JUMPSREMAINING = "JumpsRemaining";
        public const string PROP_ALLBODIESFOUND = "AllBodiesFound";
        public const string PROP_PROSPECTEDEVENTS = "ProspectedEvents";
        public const string PROP_MESSAGES = "Messages";
        public const string PROP_MODE = "Mode";

        internal enum UIMode
        {
            Realtime,
            Detached,
        }

        private readonly Dictionary<UIMode, UIStateData> _d = [];
        private bool _suppressEvents = false;
        private UIMode _mode = UIMode.Realtime;
        private readonly ObservableCollection<HelmMessage> _messages = [];

        internal UIStateManager()
        {
            foreach (UIMode mode in Enum.GetValues<UIMode>())
            {
                UIStateData modeData = new(mode);
                modeData.PropertyChanged += ModeData_PropertyChanged;
                
                _d.Add(mode, modeData);
            }
            _messages.CollectionChanged += MessagesChanged;
        }

        public void Clear()
        {
            _suppressEvents = true; // Prevent _prospectorEvents from firing.

            foreach (var modeData in _d.Values)
            {
                modeData.Clear();
            }
            _messages.Clear();

            _suppressEvents = false;
        }

        internal UIStateData ForMode(UIMode? mode = null)
        {
            return _d[mode ?? Mode];
        }

        internal UIStateData Realtime { get => _d[UIMode.Realtime]; }
        internal UIStateData Detatched { get => _d[UIMode.Detached]; }
        internal UIMode Mode 
        { 
            get => _mode;
            set
            {
                if (_mode != value) 
                {
                    _mode = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ReplayMode { get => _suppressEvents; set => _suppressEvents = value; }

        public ObservableCollection<HelmMessage> Messages
        {
            get => _messages;
        }

        public void AddMessage(string msg, DateTime timestamp, string sender)
        {
            _messages.Add(new(timestamp, msg, sender));
        }

        public void MessagesChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(PROP_MESSAGES);
        }

        private void ModeData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UIStateData data = sender as UIStateData;

            if (data.ForMode == Mode)
            {
                OnPropertyChanged(e.PropertyName);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            if (HelmContext.Instance.Core.IsLogMonitorBatchReading || _suppressEvents) return;

            HelmContext.Instance.Core.ExecuteOnUIThread(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
