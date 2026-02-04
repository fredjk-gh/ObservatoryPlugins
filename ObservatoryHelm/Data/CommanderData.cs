using System.Diagnostics;
using System.Text.Json.Serialization;
using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;

namespace com.github.fredjk_gh.ObservatoryHelm.Data
{
    internal class CommanderData
    {
        private DateTime? _lastActive = null;
        private NavRouteFile _lastNavRoute = null;
        // TODO: Upgrade to OrderedDictionary when moving to .NET 9+
        private Dictionary<UInt64, SystemData> _recentSystems = [];
        private List<UInt64> _recentSystemIds = [];
        private UInt64 _currentSystemAddr;

        // For deserialization
        public CommanderData()
        { }

        public CommanderData(CommanderKey key)
        {
            Key = key;
            Ships = new();
        }
        public CommanderKey Key { get; init; }
        [JsonIgnore]
        public string Name { get => Key.Name; }
        [JsonIgnore]
        public string FID { get => Key.FID; }
        public LoadGame LastLoadGame { get; set; }
        public Statistics LastStatistics { get; set; }
        public DateTime LastActive 
        {
            get => _lastActive
                ?? LastLoadGame?.TimestampDateTime
                ?? LastStatistics?.TimestampDateTime
                ?? DateTime.MinValue;
            set
            {
                if (!_lastActive.HasValue || value > _lastActive.Value)
                {
                    _lastActive = value;
                }
            }
        }
        [JsonIgnore]
        public string CurrentSystemName { get => CurrentSystemData?.SystemName; }
        public UInt64 CurrentSystemAddress
        { 
            get => _currentSystemAddr;
            set => _currentSystemAddr = value; // for deserialization
        }
        public SystemData CurrentSystemData
        {
            get
            {
                if (CurrentSystemAddress == 0) return null;
                return _recentSystems.GetValueOrDefault(CurrentSystemAddress, null);
            }
            set
            {
                if (value is not null)
                    _recentSystems[value.SystemId64] = value;
            }
        }
        [JsonIgnore]
        public Dictionary<UInt64, SystemData> RecentSystems { get => _recentSystems; set => _recentSystems = value; } // setter is for deserialization
        [JsonIgnore]
        public List<UInt64> RecentSystemIds { get => _recentSystemIds; set => _recentSystemIds = value; } // setter is for deserialization
        public double DistanceTravelled { get; set; }
        public long JumpsCompleted { get; set; }
        public long DockedCarrierJumpsCompleted { get; set; }
        public Status LastStatus { get; set; }
        public StartJump LastStartJumpEvent { get; set; }
        [JsonIgnore] // Until missing Faction serializer is fixed.
        public FSDJump LastJumpEvent { get; set; }
        public NavRouteFile LastNavRoute
        {
            get => _lastNavRoute;
            set
            {
                _lastNavRoute = value;
                JumpsRemainingInRoute = 0;
                if (_lastNavRoute != null)
                {
                    Destination = value.Route[^1].StarSystem;
                    JumpsRemainingInRoute = value.Route.Count - 1; // Route includes origin system.
                    DerivedRoute = DerivedRouteData.FromNavRouteFile(_lastNavRoute);
                }
            }
        }
        public DerivedRouteData DerivedRoute { get; set; }
        // TODO: Look up destination system from Archivist's cache? (This would not be needed if the whole route is cached or the whole destination route node is cached.)
        public string Destination { get; set; }
        public int JumpsRemainingInRoute { get; set; }
        public string NeutronPrimarySystem { get; set; }
        public Scan ScoopableSecondaryCandidateScan { get; set; }
        public string NeutronPrimarySystemNotified { get; set; }
        public string FuelWarningNotifiedSystem { get; set; }
        public bool IsDockedOnCarrier { get; set; }
        public bool AllBodiesFound { get; set; }
        public bool UndiscoveredSystem { get; set; }
        public ShipsData Ships { get; set; /* for deserialization */ }
        public SystemBasicData ReferenceSystem { get; set; }

        public void SystemInit()
        {
            if (CurrentSystemData is null) return;

            _currentSystemAddr = CurrentSystemData.SystemId64;
            _recentSystemIds.Add(CurrentSystemData.SystemId64);
            _recentSystems[CurrentSystemData.SystemId64] = CurrentSystemData;
        }

        public void SystemReset(UInt64 address, string name, StarPosition starPos)
        {
            if (!_recentSystems.ContainsKey(address))
            {
                SystemData sys = new(address, name, starPos);
                _recentSystems.Add(sys.SystemId64, sys);
                _recentSystemIds.Add(sys.SystemId64);

                // Cap the recent lists at 25 items.
                while (_recentSystemIds.Count > 25)
                {
                    var id = _recentSystemIds[0];
                    _recentSystems.Remove(id);
                    _recentSystemIds.RemoveAt(0);
                }
                Debug.Assert(_recentSystems.Count == _recentSystemIds.Count, "Recent system lists are desynced!");
            }
            _currentSystemAddr = address;
            AllBodiesFound = false;
            UndiscoveredSystem = false;
            IsDockedOnCarrier = false;
        }

        public void SystemReset(UInt64 address, string name, StarPosition starPos, double fuelLevel, double jumpDist)
        {
            SystemReset(address, name, starPos);

            if (!HelmContext.Instance.UIMgr.ReplayMode)
                Ships.CurrentShip.FuelRemaining = fuelLevel;

            if (!HelmContext.Instance.Core.IsLogMonitorBatchReading)
            {
                DistanceTravelled += jumpDist;
                JumpsCompleted++;
            }
        }

        public void SystemResetDockedOnCarrier(UInt64 address, string name, StarPosition starPos, double distanceTravelled = 0)
        {
            SystemReset(address, name, starPos);
            DockedCarrierJumpsCompleted++;
            DistanceTravelled += distanceTravelled;
            IsDockedOnCarrier = true;
        }

        public void SessionReset(LoadGame loadGame)
        {
            Ships.InitializeShip(loadGame);
            JumpsRemainingInRoute = 0;
            DistanceTravelled = 0;
            JumpsCompleted = 0;
            DockedCarrierJumpsCompleted = 0;
            LastNavRoute = null;
            LastStatus = null;
        }
    }
}
