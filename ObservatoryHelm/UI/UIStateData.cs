using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using com.github.fredjk_gh.ObservatoryHelm.Data;
using Observatory.Framework.Files.Journal;
using static com.github.fredjk_gh.ObservatoryHelm.UI.UIStateManager;

namespace com.github.fredjk_gh.ObservatoryHelm.UI
{
    internal class UIStateData : INotifyPropertyChanged
    {
        private readonly UIMode _forMode;
        private CommanderKey _commanderKey = null;
        private SystemBasicData _refSystem;
        private UInt64? _id64 = null;
        private int _bodyId = -1;
        private SurfacePosition _surfacePos = null;
        private UInt64? _shipId = null;
        private double _fuelRemaining = 0;
        private double _jetConeBoost = 0;
        private int _cargo = 0;
        private int _jumpsRemaining = 0;
        private bool _allFound = false;
        private readonly ObservableCollection<ProspectedAsteroid> _prospectorEvents = [];

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

        public SystemBasicData RefSystem
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

        public int Cargo
        {
            get => _cargo;
            set
            {
                _cargo = value;
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

        public ObservableCollection<ProspectedAsteroid> ProspectedEvents
        {
            get => _prospectorEvents;
        }

        internal void Clear()
        {
            _commanderKey = null;
            _refSystem = null;
            _id64 = 0;
            _bodyId = -1;
            _surfacePos = null;
            _shipId = 0;
            _fuelRemaining = 0;
            _jetConeBoost = 1;
            _cargo = 0;
            _jumpsRemaining = 0;
            _allFound = false;
            _prospectorEvents.Clear(); // don't lose the existing event wiring by creating a new one.
        }

        internal UIStateData CopyFrom(UIStateData other)
        {
            
            // Use backing variables here to avoid notification spam.
            _commanderKey = other.CommanderKey;
            _refSystem = other.RefSystem;
            _id64 = other.SystemId64;
            _bodyId = other.BodyId;
            _surfacePos = other.SurfacePosition;
            _shipId = other.ShipId;
            _fuelRemaining = other.FuelRemaining;
            _jetConeBoost = other.JetConeBoost;
            _cargo = other.Cargo;
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

            _id64 = cmdrData.CurrentSystemAddress;
            _bodyId = cmdrData.CurrentSystemData?.Planets.Keys.Order().FirstOrDefault() ?? -1;
            _allFound = cmdrData.AllBodiesFound;

            if (cmdrData.Ships.CurrentShipID.HasValue)
            {
                _shipId = cmdrData.Ships.CurrentShipID;
                _fuelRemaining = cmdrData.Ships.CurrentShip?.FuelRemaining ?? 0;
                _jetConeBoost = cmdrData.Ships.CurrentShip?.JetConeBoostFactor ?? 1;
            }
            _cargo = cmdrData.Ships.Cargo?.Select(c => c.Value).Sum() ?? 0;
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
            OnPropertyChanged("ProspectorEvents");
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
