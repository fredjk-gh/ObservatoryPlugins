using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using com.github.fredjk_gh.ObservatoryHelm.Data;
using com.github.fredjk_gh.PluginCommon.Data.Journals;
using static com.github.fredjk_gh.ObservatoryHelm.UI.UIStateManager;

namespace com.github.fredjk_gh.ObservatoryHelm.UI
{
    internal class UIStateData : INotifyPropertyChanged
    {
        private readonly UIMode _forMode;
        private CommanderKey _commanderKey = null;
        private TransitModeType _transitMode;
        private SystemNamePosition _refSystem;
        private UInt64? _id64 = null;
        private int _bodyCount = 0;
        private int _bodyId = -1;
        private SurfacePosition _surfacePos = null;
        private UInt64? _shipId = null;
        private double _fuelRemaining = 0;
        private double _jetConeBoost = 0;
        private int _shipCargo = 0;
        private int _srvCargo = 0;
        private int _jumpsRemaining = 0;
        private bool _allFound = false;
        private readonly ObservableCollection<ProspectorDetails> _prospectorEvents = [];

        internal UIStateData(UIMode forMode)
        {
            _forMode = forMode;

            if (_forMode == UIMode.Realtime) // Only forward events for realtime for Prospector.
                _prospectorEvents.CollectionChanged += ProspectorEvents_CollectionChanged;
        }

        internal UIMode ForMode
        {
            get => _forMode;
        }

        public CommanderKey CommanderKey
        {
            get => _commanderKey;
            set
            {
                SwitchCommander(value);
                if (value is null) return;

                OnPropertyChanged();
            }
        }

        public TransitModeType TransitMode
        {
            get => _transitMode;
            set
            {
                _transitMode = value;
                OnPropertyChanged();
            }
        }

        public SystemNamePosition RefSystem
        {
            get => _refSystem;
            set
            {
                _refSystem = value;
                OnPropertyChanged();
            }
        }

        public UInt64? SystemId64
        {
            get => _id64;
            set
            {
                if (_id64 != value)
                {
                    SwitchSystem(value);
                    OnPropertyChanged();
                }
            }
        }

        public int BodyCount
        {
            get => _bodyCount;
            set
            {
                if (_bodyCount != value)
                {
                    _bodyCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public int BodyId
        {
            get => _bodyId;
            set
            {
                bool changed = _bodyId != value;
                _bodyId = value;
                if (changed && _bodyId >= 0)
                    OnPropertyChanged();
            }
        }

        public SurfacePosition SurfacePosition
        {
            get => _surfacePos;
            set
            {
                _surfacePos = value;
                OnPropertyChanged();
            }
        }

        public UInt64? ShipId
        {
            get => _shipId;
            set
            {
                _shipId = value;
                OnPropertyChanged();

                // Already triggers cargo.
            }
        }

        public double FuelRemaining
        {
            get => _fuelRemaining;
            set
            {
                _fuelRemaining = value;
                OnPropertyChanged();
            }
        }

        public double JetConeBoost
        {
            get => _jetConeBoost;
            set
            {
                if (value != _jetConeBoost)
                {
                    _jetConeBoost = Math.Clamp(value, 1, 100);
                    OnPropertyChanged();
                }
            }
        }

        public int ShipCargo
        {
            get => _shipCargo;
            set
            {
                _shipCargo = value;
                OnPropertyChanged();
            }
        }
        public int SRVCargo
        {
            get => _srvCargo;
            set
            {
                _srvCargo = value;
                OnPropertyChanged();
            }
        }

        public int JumpsRemaining
        {
            get => _jumpsRemaining;
            set
            {
                _jumpsRemaining = value;
                OnPropertyChanged();
            }
        }

        public bool AllBodiesFound
        {
            get => _allFound;
            set
            {
                _allFound = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ProspectorDetails> ProspectedEvents
        {
            get => _prospectorEvents;
        }

        internal void Clear()
        {
            _commanderKey = null;
            _transitMode = TransitModeType.Unknown;
            _refSystem = null;
            _id64 = 0;
            _bodyId = -1;
            _surfacePos = null;
            _shipId = 0;
            _fuelRemaining = 0;
            _jetConeBoost = 1;
            _shipCargo = 0;
            _srvCargo = 0;
            _jumpsRemaining = 0;
            _allFound = false;
            _prospectorEvents.Clear(); // don't lose the existing event wiring by creating a new one.
        }

        internal UIStateData CopyFrom(UIStateData other)
        {
            // Use backing variables here to avoid notification spam.
            _commanderKey = other.CommanderKey;
            _transitMode = other.TransitMode;
            _refSystem = other.RefSystem;
            _id64 = other.SystemId64;
            _bodyId = other.BodyId;
            _surfacePos = other.SurfacePosition;
            _shipId = other.ShipId;
            _fuelRemaining = other.FuelRemaining;
            _jetConeBoost = other.JetConeBoost;
            _shipCargo = other.ShipCargo;
            _srvCargo = other.SRVCargo;
            _jumpsRemaining = other.JumpsRemaining;
            _allFound = other.AllBodiesFound;
            _prospectorEvents.Clear(); // Don't copy the prospector info.

            return this;
        }

        internal void SwitchCommander(CommanderKey key)
        {
            Clear();
            if (key is null) return;

            _commanderKey = key;

            var cmdrData = HelmContext.Instance.Data.For(key);
            Debug.Assert(cmdrData != null);

            _transitMode = cmdrData.ActiveTransitMode;
            _id64 = cmdrData.CurrentSystemAddress;
            _bodyId = cmdrData.CurrentSystemData?.Planets.Keys.Order().FirstOrDefault() ?? -1;
            _allFound = cmdrData.CurrentSystemData?.IsFullyDiscovered ?? false;

            if (cmdrData.Ships.CurrentShipID.HasValue)
            {
                _shipId = cmdrData.Ships.CurrentShipID;
                _fuelRemaining = cmdrData.Ships.CurrentShip?.FuelRemaining ?? 0;
                _jetConeBoost = cmdrData.Ships.CurrentShip?.JetConeBoostFactor ?? 1;
            }
            else
            {
                _shipId = 0;
            }
            _shipCargo = cmdrData.Ships.Cargo?.Select(c => c.Value).Sum() ?? 0;
            _srvCargo = cmdrData.SRV.Cargo?.Select(c => c.Value).Sum() ?? 0;
            _refSystem = cmdrData.ReferenceSystem;
            _jumpsRemaining = cmdrData.JumpsRemainingInRoute;
            _surfacePos = null;
            // ProspectedEvents was already Clear()ed by Clear()

            OnPropertyChanged(PROP_COMMANDERKEY);
        }

        internal void SwitchSystem(UInt64? newId64)
        {
            _bodyId = -1;
            _id64 = newId64;
            _allFound = false;
            _surfacePos = null;
            _prospectorEvents.Clear();

            OnPropertyChanged(PROP_SYSTEMID64);
        }

        private void ProspectorEvents_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(UIStateManager.PROP_PROSPECTEDEVENTS);
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
