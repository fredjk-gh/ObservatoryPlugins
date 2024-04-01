using Microsoft.VisualBasic;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;
using System;
using System.Collections.Generic;
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

        public string SystemName { get; private set; }
        public string LocationName { get; private set;  }
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

            return (_cargo[name] += qty);
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
    }
}
