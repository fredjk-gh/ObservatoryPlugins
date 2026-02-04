using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;

namespace com.github.fredjk_gh.ObservatoryHelm.Data
{
    public class ShipData
    {
        private Loadout _loadout = null;
        private StoredShip _storedShip = null;
        private ulong _shipId;
        private string _shipIdent = string.Empty;
        private string _shipName = string.Empty;
        private string _shipType = string.Empty;
        private ulong? _localMarketId = null;
        private string _localStarSystem = null;
        private string _localStationName = null;
        private double _fuelCapacity = Constants.MAX_FUEL;
        private double _fuelRemaining = 0;
        private double _jetConeBoostFactor = 1.0;
        private double? _maxFuelPerJump = null;

        public ShipData() // For cache deserialization; don't use.
        { }

        public ShipData(StoredShip ship)
        {
            _shipId = ship.ShipID;
            _storedShip = ship;
        }

        public ShipData(Loadout loadout)
        {
            _shipId = loadout.ShipID;
            Loadout = loadout;
        }

        public ShipData(LoadGame loadGame)
        {
            _shipId = loadGame.ShipID;
            _shipIdent = loadGame.ShipIdent;
            _shipName = loadGame.ShipName;
            _shipType = loadGame.Ship;
            _fuelCapacity = loadGame.FuelCapacity;
            _fuelRemaining = loadGame.FuelLevel;
        }

        public StoredShip StoredShip
        {
            get => _storedShip;
            set => _storedShip = value; // Anything else to do?
        }

        public Loadout Loadout
        {
            get => _loadout;
            set
            {
                _loadout = value;

                if (_loadout is null) return;

                var fsdModule = findFSD(Loadout.Modules);
                if (fsdModule is not null)
                    Constants.MaxFuelPerJumpByFSDSizeClass.TryGetValue(fsdModule.Item, out var MaxFuelPerJump);

                // If FuelCapacity is null, it's probably an old journal that had a different format: "FuelCapacity":2.000000
                // Ignore and use a default value as that historical data doesn't really matter here.
                _fuelCapacity = (Loadout.FuelCapacity is not null ? Loadout.FuelCapacity.Main : Constants.MAX_FUEL);
            }
        }

        public ulong ShipID
        {
            get => _shipId;
            set => _shipId = value; // For deserialization.
        }

        public string ShipType
        {
            get
            {
                if (Loadout != null) return Loadout.Ship;
                if (StoredShip != null)
                {
                    return !string.IsNullOrEmpty(StoredShip.ShipType_Localised)
                        ? StoredShip.ShipType_Localised
                        : StoredShip.ShipType;
                }
                return _shipType;
            }
            set => _shipType = value; // For deserialization.
        }

        public string ShipName
        {
            get
            {
                if (Loadout != null) return (!string.IsNullOrWhiteSpace(Loadout.ShipName) ? Loadout.ShipName : ShipType);
                if (StoredShip != null) return (!string.IsNullOrWhiteSpace(StoredShip.Name) ? StoredShip.Name : ShipType);
                if (!string.IsNullOrWhiteSpace(_shipName)) return _shipName;

                return $"Ship {ShipID}";
            }
            set => _shipName = value; // For deserialization.
        }

        // Source StoredShip
        [JsonIgnore]
        public long? Value
        {
            get
            {
                if (StoredShip == null) return null;
                return StoredShip.Value;
            }
        }

        [JsonIgnore]
        public bool? Hot
        {
            get
            {
                if (StoredShip == null) return null;
                return StoredShip.Hot;
            }
        }

        [JsonIgnore]
        public bool? InTransit
        {
            get
            {
                if (StoredShip == null) return null;
                return StoredShip.InTransit;
            }
        }

        [JsonIgnore]
        public long? TransferPrice
        {
            get
            {
                if (StoredShip == null) return null;
                return StoredShip.TransferPrice;
            }
        }

        [JsonIgnore]
        public long? TransferTime
        {
            get
            {
                if (StoredShip == null) return null;
                return StoredShip.TransferTime;
            }
        }

        public ulong? ShipMarketId
        {
            get
            {
                return _localMarketId ?? StoredShip?.ShipMarketID;
            }
            set => _localMarketId = value; // For deserialization.
        }

        public string StarSystem
        {
            get => !string.IsNullOrWhiteSpace(_localStarSystem) ? _localStarSystem : (StoredShip?.StarSystem ?? "");
            set => _localStarSystem = value; // For deserialization.
        }

        public string StationName
        {
            get => _localStationName; // no station name available from StoredShip.
            set => _localStationName = value; // For deserialization.
        }

        public void UpdateLocalMarket(ulong? localMarketId, string starSystem, string stationName)
        {
            _localMarketId = localMarketId;
            _localStarSystem = starSystem;
            _localStationName = stationName;
        }

        // Source Loadout
        public string ShipIdent
        {
            get => Loadout?.ShipIdent ?? _shipIdent;
            set => _shipIdent = value; // For deserialization.
        }

        [JsonIgnore]
        public int? CargoCapacity
        {
            get => Loadout?.CargoCapacity;
        }

        public double FuelCapacity
        {
            get => _fuelCapacity;
            set => _fuelCapacity = value;
        }

        public double FuelRemaining
        {
            get => _fuelRemaining;
            set => _fuelRemaining = FuelCapacity > 0 ? Math.Min(value, FuelCapacity) : value;
        }

        public double JetConeBoostFactor
        {
            get => _jetConeBoostFactor;
            set => Math.Clamp(_jetConeBoostFactor = value, 1, 100);
        }

        public double? MaxFuelPerJump
        {
            get => _maxFuelPerJump;
            set => _maxFuelPerJump = value; // For deserialization.
        }

        [JsonIgnore]
        public double? MaxJumpRange
        {
            get
            {
                if (Loadout == null) return null;
                return Loadout.MaxJumpRange * _jetConeBoostFactor;
            }
        }

        private static Modules? findFSD(ImmutableList<Modules> modules)
        {
            foreach (var m in modules)
            {
                if (m.Slot.Equals("FrameShiftDrive", StringComparison.OrdinalIgnoreCase))
                {
                    return m;
                }
            }
            return null;
        }
    }
}
