using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.RegularExpressions;
using com.github.fredjk_gh.PluginCommon.Data;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;

namespace com.github.fredjk_gh.ObservatoryHelm.Data
{
    partial class TrackedData(string dataPath)
    {
        private const string STATE_CACHE_FILENAME = "helm_state_cache_001.json";
        private CommanderKey _currentCommander = null;
        private readonly Dictionary<CommanderKey, CommanderData> _commanders = [];
        private readonly Regex _fidRegex = FidRegex();

        public bool IsOdyssey { get; set; }

        public bool HasCurrentCommander { get => _currentCommander != null; }

        public bool IsCommanderKnown(CommanderKey commanderKey)
        {
            return commanderKey is not null && _commanders.ContainsKey(commanderKey);
        }

        // And for each commander, we need to track "current" values per the game and capture a history from which
        // we can select and change the "displayed" system / body / station.
        // Always initialize to the current commander, but allow the user to switch away without yanking back (but
        // keep tracking data in case they switch back). For system/body/station, snap back to current if something
        // changes while current commander is displayed.
        public CommanderKey CurrentCommander
        {
            get => _currentCommander;
            set
            {
                if (value == null) return;

                GetOrCreateCommanderData(value);
                _currentCommander = value;
            }
        }

        public Dictionary<CommanderKey, CommanderData> Commanders { get => _commanders; }

        [AllowNull]
        public CommanderData CommanderData
        {
            get
            {
                if (CurrentCommander is null)
                    return null;

                return GetOrCreateCommanderData(CurrentCommander);
            }
        }

        public CommanderData For(CommanderKey commanderKey = null)
        {
            var key = commanderKey ?? CurrentCommander;
            return GetOrCreateCommanderData(key);
        }

        // Used for rehydration from cache.
        internal void AddCommanderData(CommanderData d)
        {
            _commanders[d.Key] = d;
        }

        public void SystemReset(UInt64 address, string name, StarPosition starPos)
        {
            CommanderData?.SystemReset(address, name, starPos);
        }

        public void SystemReset(UInt64 address, string name, StarPosition starPos, double fuelLevel, double jumpDist)
        {
            CommanderData?.SystemReset(address, name, starPos, fuelLevel, jumpDist);
        }

        public void SystemResetDockedOnCarrier(UInt64 address, string name, StarPosition starPos, double distanceTravelled = 0)
        {
            CommanderData?.SystemResetDockedOnCarrier(address, name, starPos, distanceTravelled);
        }

        public void SessionReset(LoadGame loadGame)
        {
            IsOdyssey = loadGame.Odyssey;
            CurrentCommander = CommanderKey.FromLoadGame(loadGame);
            CommanderData?.SessionReset(loadGame);
        }

        public void SaveToCache()
        {
            // Gather data
            HelmDataCache cacheObj = HelmDataCache.FromTrackedData(this);

            // Serialize to json.

            // Save in a file in dataPath.
            var jsonFile = $"{dataPath}{STATE_CACHE_FILENAME}";
            File.WriteAllText(jsonFile, JsonSerializer.Serialize(cacheObj, JsonHelper.PRETTY_PRINT_OPTIONS));
        }

        public bool RestoreFromCache()
        {
            // Open the file (if it exists).
            var jsonFileName = $"{dataPath}{STATE_CACHE_FILENAME}";
            var jsonFile = new FileInfo(jsonFileName);
            if (!jsonFile.Exists)
            {
                var result = MaybeRestoreFromDestinations(); // Attempt to migrate.

                if (result)
                {
                    SaveToCache(); // Create a new file.
                }
                return result;
            }

            var fileIsCorrupt = false;

            // Deserialize from json.
            HelmDataCache cacheObj = null;
            try
            {
                string jsonString = File.ReadAllText(jsonFileName);
                cacheObj = JsonSerializer.Deserialize<HelmDataCache>(jsonString, JsonHelper.PRETTY_PRINT_OPTIONS)!;
            }
            catch (Exception ex)
            {
                // File is corrupt. We should nuke it (after we close it) and carry on.
                fileIsCorrupt = true;
                Debug.WriteLine(ex);
            }
            if (fileIsCorrupt || cacheObj is null)
            {
                jsonFile.Delete();
                return false;
            }

            cacheObj.ToTrackedData(this);
            return true;
        }

        public bool MaybeRestoreFromDestinations()
        {
            var destinationsFile = "destinations.json";
            // Open the file (if it exists).
            var jsonFile = new FileInfo($"{dataPath}{destinationsFile}");
            if (!jsonFile.Exists) return false;

            var fileIsCorrupt = false;

            // Deserialize from json.
            Dictionary<string, string> routeDestinations = [];
            using var fs = jsonFile.OpenRead();
            using var sr = new StreamReader(fs);
            try
            {
                var jsonDoc = JsonDocument.Parse(fs, new() { AllowTrailingCommas = true });
                routeDestinations = jsonDoc.Deserialize<Dictionary<string, string>>(JsonHelper.PRETTY_PRINT_OPTIONS);
            }
            catch (JsonException)
            {
                // File is corrupt. We should nuke it (after we close it) and carry on.
                fileIsCorrupt = true;
            }

            if (fileIsCorrupt)
            {
                jsonFile.Delete();
                return false;
            }

            // This is to avoid breaking compatibility with the current "production" destinations.json file which
            // only stores the commander name. We should know all commander keys but maybe not, so until that is
            // persisted, we will actually lose that.
            //
            // Also, I had a bug which initilized the key backwards -- which added duplicate items to the
            // destinations list... *sigh*. Keep the one with the correct order and discard the other.
            // It will be slightly lossy.
            Dictionary<string, CommanderKey> nameToKeymap = [];
            foreach (var key in _commanders.Keys)
            {
                if (!_fidRegex.Match(key.FID).Success) continue;

                // If we still managed to have a duplicate, this approach will not barf if the key already exists.
                nameToKeymap[key.Name] = key;
            }

            foreach (var item in routeDestinations)
            {
                if (!CommanderKey.TryParse(item.Key, out CommanderKey cmdrKey) && !nameToKeymap.TryGetValue(item.Key, out cmdrKey))
                {
                    Debug.WriteLine($"No CommanderKey for CMDR ${item.Key}; Destination of {item.Value} may be lost!");
                    continue;
                }
                if (!_fidRegex.Match(cmdrKey.FID).Success) continue;
                var data = GetOrCreateCommanderData(cmdrKey);
                data.Destination = item.Value;
            }
            return true;
        }

        private CommanderData GetOrCreateCommanderData(CommanderKey key)
        {
            if (key is null) return null;
            if (!_commanders.TryGetValue(key, out var cmdrData))
            {
                cmdrData = new CommanderData(key);
                _commanders.Add(key, cmdrData);
            }
            return cmdrData;
        }

        [GeneratedRegex("^F[0-9]+$")]
        private static partial Regex FidRegex();
    }
}
