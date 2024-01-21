using System.Text.Json;

namespace com.github.fredjk_gh.ObservatoryArchivist
{
    internal class EntryDeduper
    {
        private HashSet<string> _saaSignalsFoundBodies = new();
        private HashSet<long> _codexEntries = new();
        private HashSet<string> _singletons = new();

        public EntryDeduper(List<string> systemJournalEntries = null)
        {
            if (systemJournalEntries != null && systemJournalEntries.Count > 0)
                ExtractIndexData(systemJournalEntries);
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
            bool isUnique = true; // ie. Assume unique unless one of the hash set already has it.

            switch (eventType)
            {
                // Per value items.
                case "SAASignalsFound":
                    string bodyName = entry.GetProperty("BodyName").GetString();
                    isUnique = _saaSignalsFoundBodies.Add(bodyName);
                    break;
                case "CodexEntry":
                    isUnique = _codexEntries.Add(entry.GetProperty("EntryID").GetInt64());
                    break;

                // Singletons
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
                    isUnique = _singletons.Add("eventType");
                    break;
            }
            return isUnique;
        }

        private void ExtractIndexData(List<string> systemJournalEntries)
        {
            foreach (string jsonStr in systemJournalEntries)
            {
                using var json = JsonDocument.Parse(jsonStr);
                var entry = json.RootElement;
                string eventType = entry.GetProperty("event").GetString();

                switch (eventType)
                {
                    // Per value items.
                    case "SAASignalsFound":
                        string bodyName = entry.GetProperty("BodyName").GetString();
                        _saaSignalsFoundBodies.Add(bodyName);
                        break;
                    case "CodexEntry":
                        _codexEntries.Add(entry.GetProperty("EntryID").GetInt64());
                        break;

                    // Singletons
                    case "FSDJump":
                    case "CarrierJump":
                        _singletons.Add("FSDJump");
                        _singletons.Add("CarrierJump");
                        break;
                    case "FSSDiscoveryScan":
                        _singletons.Add("eventType");
                        break;
                }
            }
        }
    }
}