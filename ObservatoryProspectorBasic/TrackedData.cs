using Microsoft.VisualBasic;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryProspectorBasic
{
    internal class TrackedData
    {
        public class BodyMaterialContent
        {
            public BodyMaterialContent(string bodyName, int bodyID, string mat, float percent)
            {
                BodyName = bodyName;
                BodyID = bodyID;
                Material = mat;
                Percent = percent;
            }

            public string BodyName { get; }
            public int BodyID { get; }
            public string Material { get; }
            public float Percent { get; }
        }

        private readonly Dictionary<string, int> _rawMatInventory = new();
        private readonly Dictionary<string, List<BodyMaterialContent>> _matContentByMat = new();
        private readonly HashSet<string> _alreadyReportedScansSaaSignals = new();
        private readonly Dictionary<string, int> _cargo = new();
        private Loadout _lastLoadout = null;

        public string SystemName { get; private set; }
        public string LocationName { get; private set; }
        public bool CurrentLocationShown { get; set; }
        public int? CargoMax { get; set; }
        public int? CargoCur { get; set; }
        public bool AllBodiesFound { get; set; }

        public HashSet<string> AlreadyReportedScansSaaSignals { get => _alreadyReportedScansSaaSignals; }
        public Dictionary<string, int> Cargo { get => _cargo; }
        public Dictionary<string, List<BodyMaterialContent>> MatsInSystem { get => _matContentByMat; }

        public bool SystemChanged(string newSystemName)
        {
            if (string.IsNullOrEmpty(newSystemName) || newSystemName == SystemName) return false;

            SystemName = newSystemName;
            LocationName = null;
            _matContentByMat.Clear();
            CurrentLocationShown = false;
            _alreadyReportedScansSaaSignals.Clear();
            AllBodiesFound = false;

            return true;
        }
        
        public bool LocationChanged(string newLocation)
        {
            if (string.IsNullOrEmpty(newLocation) || newLocation == LocationName) return false;
            LocationName = newLocation;
            CurrentLocationShown = false;

            return true;
        }

        public void RawMatInventoryUpdate(Materials mats)
        {
            _rawMatInventory.Clear();
            foreach(var mat in mats.Raw)
            {
                _rawMatInventory[mat.Name.ToLower()] = mat.Count;
            }
        }

        public int RawMatInventoryAdd(string name, int count)
        {
            var nameLower = name.ToLower();
            if (!_rawMatInventory.ContainsKey(nameLower))
                _rawMatInventory[nameLower] = 0;

            _rawMatInventory[nameLower] += count;

            var maxCapacity = MaterialData.MaterialCapacity(nameLower);
            if (_rawMatInventory[nameLower] > maxCapacity)
                _rawMatInventory[nameLower] = maxCapacity;

            return _rawMatInventory[nameLower];
        }

        public int RawMatInventorySubtract(string name, int count)
        {
            var nameLower = name.ToLower();
            if (!_rawMatInventory.ContainsKey(nameLower))
                _rawMatInventory[nameLower] = 0;

            _rawMatInventory[nameLower] -= count;
            if (_rawMatInventory[nameLower] < 0) _rawMatInventory[nameLower] = 0;

            return _rawMatInventory[nameLower];
        }

        public int RawMaterialGet(string name)
        {
            return _rawMatInventory[name.ToLower()];
        }

        public void AddScanRawMats(Scan scan)
        {
            if (!scan.Landable) return;

            var bodyShortName = GetShortBodyName(scan.BodyName);
            foreach (var mat in scan.Materials)
            {
                var matLower = mat.Name.ToLower();
                if (!_matContentByMat.ContainsKey(matLower))
                    _matContentByMat[matLower] = new();

                _matContentByMat[matLower].Add(new BodyMaterialContent(bodyShortName, scan.BodyID, matLower, mat.Percent));
            }
        }

        public int CargoAdd(string name, int qty)
        {
            if (!_cargo.ContainsKey(name))
                _cargo[name] = 0;

            if (CargoMax.HasValue)
            {
                int cargoTotal = _cargo.Values.Sum();
                if (cargoTotal + qty > CargoMax.Value)
                {
                    Debug.WriteLine($"CargoMax would be exceeded by adding {qty} items of type {name}: {cargoTotal} + {qty} > {CargoMax.Value}! Adding only {CargoMax.Value - cargoTotal}!");
                    _cargo[name] += (CargoMax.Value - cargoTotal);
                }
                else
                {
                    _cargo[name] += qty;
                }
            }
            else
            {
                // No max to enforce; best effort.
                _cargo[name] += qty;
            }
            return _cargo[name];
        }

        public int CargoRemove(string name, int qty)
        {
            if (!_cargo.ContainsKey(name)) return 0;

            int remainder = (_cargo[name] -= Math.Min(qty, _cargo[name]));
            if (remainder <= 0)
            {
                _cargo.Remove(name);
                remainder = 0;
            }
            return remainder;
        }

        public int CargoGet(string name)
        {
            if (!_cargo.ContainsKey(name)) return 0;
            return _cargo[name];
        }

        // TODO: Extract these to shared library or move into IObservatoryCore?
        public string GetShortBodyName(string bodyName, string baseName = "")
        {
            return string.IsNullOrEmpty(baseName) ? bodyName.Replace(SystemName, "").Trim() : bodyName.Replace(baseName, "").Trim();
        }

        // TODO Handle Barycenters?
        public string GetBodyTitle(string bodyName)
        {
            if (bodyName.Length == 0)
            {
                return "Primary Star";
            }
            return $"Body {bodyName}";
        }

        public void LoadoutChanged(Loadout loadout)
        {
            CargoMax = loadout.CargoCapacity;
            _lastLoadout = loadout;
        }

        public int? MiningModuleCount()
        {
            if (_lastLoadout == null) return null;

            int count = 0;
            foreach (var mod in _lastLoadout.Modules)
            {
                if (MINING_MODULES.Contains(mod.Item.ToLower()))
                {
                    count++;
                }
            }
            return count;
        }

        private readonly HashSet<string> MINING_MODULES = new()
        {
            "hpt_mininglaser_turret_small",
            "hpt_mininglaser_turret_medium",
            "hpt_mininglaser_fixed_small_advanced",
            "hpt_mining_subsurfdispmisle_fixed_small",
            "hpt_mining_subsurfdispmisle_turret_small",
            "hpt_mining_subsurfdispmisle_fixed_medium",
            "hpt_mining_subsurfdispmisle_turret_medium",
            "hpt_mining_abrblstr_fixed_small",
            "hpt_mining_abrblstr_turret_small",
            "hpt_mining_seismchrgwarhd_fixed_medium",
            "hpt_mining_seismchrgwarhd_turret_medium",
            "int_multidronecontrol_mining_size3_class1",
            "int_multidronecontrol_mining_size3_class3",
            "int_refinery_size1_class1",
            "int_refinery_size2_class1",
            "int_refinery_size3_class1",
            "int_refinery_size4_class1",
            "int_refinery_size1_class2",
            "int_refinery_size2_class2",
            "int_refinery_size3_class2",
            "int_refinery_size4_class2",
            "int_refinery_size1_class3",
            "int_refinery_size2_class3",
            "int_refinery_size3_class3",
            "int_refinery_size4_class3",
            "int_refinery_size1_class4",
            "int_refinery_size2_class4",
            "int_refinery_size3_class4",
            "int_refinery_size4_class4",
            "int_refinery_size1_class5",
            "int_refinery_size2_class5",
            "int_refinery_size3_class5",
            "int_refinery_size4_class5",
            "int_dronecontrol_prospector_size1_class1",
            "int_dronecontrol_prospector_size1_class2",
            "int_dronecontrol_prospector_size1_class3",
            "int_dronecontrol_prospector_size1_class4",
            "int_dronecontrol_prospector_size1_class5",
            "int_dronecontrol_prospector_size3_class1",
            "int_dronecontrol_prospector_size3_class2",
            "int_dronecontrol_prospector_size3_class3",
            "int_dronecontrol_prospector_size3_class4",
            "int_dronecontrol_prospector_size3_class5",
            "int_dronecontrol_prospector_size5_class1",
            "int_dronecontrol_prospector_size5_class2",
            "int_dronecontrol_prospector_size5_class3",
            "int_dronecontrol_prospector_size5_class4",
            "int_dronecontrol_prospector_size5_class5",
            "int_dronecontrol_prospector_size7_class1",
            "int_dronecontrol_prospector_size7_class2",
            "int_dronecontrol_prospector_size7_class3",
            "int_dronecontrol_prospector_size7_class4",
            "int_dronecontrol_prospector_size7_class5",
        };
    }
}
