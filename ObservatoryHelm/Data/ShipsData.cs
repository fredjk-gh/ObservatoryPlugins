using System.Text.Json.Serialization;
using com.github.fredjk_gh.PluginCommon.Data.Journals;
using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;

namespace com.github.fredjk_gh.ObservatoryHelm.Data
{
    public class ShipsData
    {
        private StoredShips _storedShips;
        private Dictionary<ulong, ShipData> _shipsByID = [];
        private Dictionary<string, int> _cargo = [];

        public ulong? CurrentShipID { get; set; } // Setter is for deserialization.

        [JsonIgnore]
        public ShipData CurrentShip
        {
            get => GetShip(CurrentShipID);
        }

        public Dictionary<string, int> Cargo
        {
            get => _cargo;
            set => _cargo = value; // For deserialization
        }

        // For cache; don't use.
        public StoredShips InternalStoredShips { get => _storedShips; set => _storedShips = value; }
        // For cache; don't use.
        public Dictionary<ulong, ShipData> InternalShips { get => _shipsByID; set => _shipsByID = value ?? []; }

        public ShipData GetShip(ulong? id)
        {
            if (id == null) { return null; }

            return _shipsByID.GetValueOrDefault(id.Value);
        }

        public bool IsKnown(ulong id)
        {
            return _shipsByID.ContainsKey(id);
        }

        public void UpdateStoredShips(StoredShips ships)
        {
            _storedShips = ships;

            foreach (StoredShip ship in ships.ShipsHere.Union(ships.ShipsRemote))
            {
                if (ship.ShipID >= 4293000000) continue;
                if (!_shipsByID.TryGetValue(ship.ShipID, out ShipData shipData))
                {
                    shipData = new(ship);
                    _shipsByID.Add(ship.ShipID, shipData);
                }
                else
                    shipData.StoredShip = ship;

                shipData.UpdateLocalMarket(ships.MarketID, ships.StarSystem, ships.StationName);
            }
        }

        public void UpdateLoadout(Loadout loadout)
        {
            if (loadout.ShipID >= 4293000000) return;
            if (!_shipsByID.ContainsKey(loadout.ShipID))
            {
                _shipsByID.Add(loadout.ShipID, new(loadout));
            }
            else
            {
                GetShip(loadout.ShipID).Loadout = loadout;
            }
            CurrentShipID = loadout.ShipID;
        }


        public void UpdateCargo(CargoFile cargo)
        {
            UpdateCargoInventory(cargo.Inventory);
        }

        public void UpdateCargo(Cargo cargo)
        {
            if (cargo.Inventory is null)
            {
                if (cargo.Count == 0)
                    _cargo.Clear();
                return;
            }
            UpdateCargoInventory(cargo.Inventory);
        }

        private void UpdateCargoInventory(IEnumerable<CargoType> inventory)
        {
            _cargo.Clear();
            foreach (var item in inventory)
            {
                string key = CargoHelper.GetCargoKey(item.Name, item.Name_Localised);
                _cargo.Add(key, item.Count);
            }
        }
        
        public void InitializeShip(LoadGame loadGame)
        {
            // Yeah -- if you can believe it, FDev shtuffs suit info into Ship properties if you load the game while on-foot.
            if (loadGame.ShipID >= 4293000000) return;
            if (loadGame.Ship == "TestBuggy") return;
            if (!_shipsByID.ContainsKey(loadGame.ShipID))
            {
                _shipsByID.Add(loadGame.ShipID, new(loadGame));
            }
            else
            {
                ShipData shipData = GetShip(loadGame.ShipID);

                shipData.FuelCapacity = loadGame.FuelCapacity;
                shipData.FuelRemaining = loadGame.FuelLevel;
            }
            CurrentShipID = loadGame.ShipID;
        }
    }
}
