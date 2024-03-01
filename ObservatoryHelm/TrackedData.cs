using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryHelm
{
    class TrackedData
    {
        private const string SAVED_DESTINATIONS_FILENAME = "destinations.json";

        private string _currentCommander = null;
        private Dictionary<string, CommanderData> _commanders = new();

        public bool IsOdyssey { get; set; }

        public bool IsCommanderKnown { get => !string.IsNullOrWhiteSpace(_currentCommander); }

        public string CurrentCommander
        {
            get => _currentCommander;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) return;

                var data = GetOrCreateCommanderData(value);
                _currentCommander = value;
            }
        }

        [AllowNull]
        public CommanderData CommanderData {
            get
            {
                if (string.IsNullOrWhiteSpace(CurrentCommander))
                    return null;

                return GetOrCreateCommanderData(CurrentCommander);
            }
        }

        public void SystemReset(string systemName, double fuelLevel, double jumpDist)
        {
            CommanderData?.SystemReset(systemName, fuelLevel, jumpDist);
        }

        public void SystemResetDockedOnCarrier(string systemName, double distanceTravelled = 0)
        {
            CommanderData?.SystemResetDockedOnCarrier(systemName, distanceTravelled);
        }

        public void SessionReset(bool isOdyssey, string commander, double fuelCapacity, double fuelLevel)
        {
            IsOdyssey = isOdyssey;
            CurrentCommander = commander;
            CommanderData.SessionReset(fuelCapacity, fuelLevel);
        }

        public void SaveRouteDestinations(string dataPath)
        {
            // Gather data
            Dictionary<string, string> routeDestinations = new();
            foreach (var data in _commanders.Values)
            {
                if (!string.IsNullOrWhiteSpace(data.Destination))
                {
                    routeDestinations[data.Commander] = data.Destination;
                }
            }

            // Serialize to json.
            var serializerOptions = new JsonSerializerOptions() { AllowTrailingCommas = true };

            // Save in a file in dataPath.
            FileInfo jsonFile = new FileInfo($"{dataPath}{SAVED_DESTINATIONS_FILENAME}");

            using (var fs = jsonFile.Open(FileMode.Truncate))
            using (var sw = new StreamWriter(fs))
            {
                sw.Write(JsonSerializer.Serialize<Dictionary<string, string>>(routeDestinations, serializerOptions));
                sw.Flush();
            }
        }

        public bool RestoreRouteDestinations(string dataPath)
        {
            // Open the file (if it exists).
            FileInfo jsonFile = new FileInfo($"{dataPath}{SAVED_DESTINATIONS_FILENAME}");
            if (!jsonFile.Exists) return false;

            bool fileIsCorrupt = false;

            // Deserialize from json.
            var serializerOptions = new JsonSerializerOptions() { AllowTrailingCommas = true };
            Dictionary<string, string> routeDestinations = new();
            using (var fs = jsonFile.OpenRead())
            using (var sr = new StreamReader(fs))
            {
                try
                {
                    var jsonDoc = JsonDocument.Parse(fs, new() { AllowTrailingCommas = true });
                    routeDestinations = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonDoc, serializerOptions);
                }
                catch (JsonException ex)
                {
                    // File is corrupt. We should nuke it (after we close it) and carry on.
                }
            }
            if (fileIsCorrupt)
            {
                jsonFile.Delete();
                return false;
            }

            foreach (var item in routeDestinations)
            {
                var data = GetOrCreateCommanderData(item.Key);
                data.Destination = item.Value;
            }
            return true;
        }

        private CommanderData GetOrCreateCommanderData(string commander)
        {
            if (!_commanders.ContainsKey(commander))
            {
                _commanders.Add(commander, new CommanderData(commander));
            }
            return _commanders[commander];
        }
    }
}
