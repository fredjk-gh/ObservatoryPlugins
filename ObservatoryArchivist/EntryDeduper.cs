using System.Diagnostics;
using System.Text.Json;

namespace com.github.fredjk_gh.ObservatoryArchivist
{
    internal class EntryDeduper
    {
        // This class is scoped to a particular system -- we don't have to worry about collisions between body IDs.
        private HashSet<int> _saaSignalsFoundBodies = new();
        private HashSet<int> _scannedBodyIDs = new();
        private HashSet<int> _mappedBodyIDs = new();
        private HashSet<long> _codexEntries = new();
        private HashSet<string> _singletons = new();

        public EntryDeduper(List<string> systemJournalEntries = null)
        {
            if (systemJournalEntries != null && systemJournalEntries.Count > 0)
                Initialize(systemJournalEntries);
        }

        public bool IsThisADuplicate(string jsonStr)
        {
            return !CheckIsUnique(jsonStr);
        }

        private bool CheckIsUnique(string jsonStr)
        {
            using var json = JsonDocument.Parse(jsonStr);
            var entry = json.RootElement;
            string eventType = entry.GetProperty("event").GetString();
            bool isUnique = true; // ie. Assume unique unless one of the hash sets already has it.

            switch (eventType)
            {
                // Per value items.
                case "CodexEntry":
                    isUnique = _codexEntries.Add(entry.GetProperty("EntryID").GetInt64());
                    break;

                case "SAASignalsFound":
                    isUnique = _saaSignalsFoundBodies.Add(entry.GetProperty("BodyID").GetInt32());
                    break;
                case "Scan":
                case "ScanBaryCentre":
                    isUnique = _scannedBodyIDs.Add(entry.GetProperty("BodyID").GetInt32());
                    break;
                case "SAAScanComplete":
                    isUnique = _mappedBodyIDs.Add(entry.GetProperty("BodyID").GetInt32());
                    break;

                // One-of singletons.
                case "FSDJump":
                case "CarrierJump":
                case "Location":
                    // only allow one of these.
                    isUnique = !_singletons.Contains("FSDJump") && !_singletons.Contains("CarrierJump") && !_singletons.Contains("Location");
                    _singletons.Add("FSDJump");
                    _singletons.Add("CarrierJump");
                    _singletons.Add("Location");
                    break;
                case "FSSDiscoveryScan":
                case "NavBeaconScan":
                    isUnique = !_singletons.Contains("FSSDiscoveryScan") && !_singletons.Contains("NavBeaconScan");
                    _singletons.Add("FSSDiscoveryScan");
                    _singletons.Add("NavBeaconScan");
                    break;

                // Singletons -- this may actually be implicitly unique.
                case "FSSAllBodiesFound":
                    isUnique = _singletons.Add(eventType);
                    break;
            }
            return isUnique;
        }

        private void Initialize(List<string> systemJournalEntries)
        {
            foreach (string jsonStr in systemJournalEntries)
            {
                // Just re-run it through CheckIsUnique which, ignoring the result, which will re-hydrate the indices
                // as a side-effect. This avoids duplicating the logic above.
                if (!CheckIsUnique(jsonStr))
                    Debug.WriteLine($"EntryDeduper.Initialize: found a stored duplicate: {jsonStr}");
            }
        }
    }
}