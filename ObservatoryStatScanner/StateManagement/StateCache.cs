using Observatory.Framework.Files.Journal;
using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.ObservatoryStatScanner.StateManagement
{
    internal class StateCache
    {
        internal const string NO_VERSION = "0.0.0.0";
        internal static readonly int[] NO_VERSION_PARSED = [0, 0, 0, 0];
        private readonly string _assemblyVersion = typeof(StatScanner).Assembly.GetName().Version.ToString();

        private Dictionary<string, CommanderCache> _commanderCache = [];
        private string _lastSeenCommanderName = string.Empty;
        private bool _readAllRequired = true;
        private string _lastUsedVersion = "";
        private bool _isDirty = false;

        [JsonIgnore]
        public bool IsDirty { get => _isDirty; }

        public string LastUsedVersion {
            get => _lastUsedVersion;
            set { _lastUsedVersion = value; }
        }

        public bool ReadAllRequired
        {
            get => _readAllRequired;
            set => _readAllRequired = value;
        }

        public string ReadAllReason { get; set; }
        public string LastSeenCommanderName {
            get => _lastSeenCommanderName;
            set
            {
                _lastSeenCommanderName = value;
                _isDirty = true;
            }
        }

        [JsonIgnore]
        public string LastSeenCommanderFID
        {
            get
            {
                if (KnownCommanders.Count == 0 || !IsCommanderKnown(LastSeenCommanderName))
                    return "(Unknown commander)";
                return KnownCommanders[LastSeenCommanderName].FID;
            }
        }

        public Dictionary<string, CommanderCache> KnownCommanders
        {
            get => _commanderCache;
            set => _commanderCache = value;
        }

        public bool IsCommanderKnown(string commanderName = null)
        {
            return _commanderCache.ContainsKey(commanderName ?? LastSeenCommanderName);
        }

        [JsonIgnore]
        public CommanderCache? CurrentCommander
        {
            get => KnownCommanders.GetValueOrDefault(LastSeenCommanderName, null);
        }

        public void AddCommander(string name, bool isOdyssey, bool hasReadAll, string fid = "")
        {
            var newCmdr = new CommanderCache()
            {
                Name = name,
                CurrentSystem = "(unknown location)",
                IsOdyssey = isOdyssey,
                ReadAllSinceFirstSeen = hasReadAll,
                FID = fid,
            };

            _commanderCache[name] = newCmdr;
            // Only dirty if we're also flipping the read-all flag.
            if (!hasReadAll) SetReadAllRequired("New commander detected");
        }

        public void UpdateCommanderInfo(LoadGame loadGame, bool isOdyssey = false, bool hasReadAll = false)
        {
            if (!IsCommanderKnown(loadGame.Commander)) {
                AddCommander(loadGame.Commander, isOdyssey, hasReadAll);
            }
            var cached = _commanderCache[loadGame.Commander];
            cached.Name = loadGame.Commander;
            cached.LastLoadGame = loadGame;
            cached.IsOdyssey = isOdyssey;
            LastSeenCommanderName = loadGame.Commander;
        }

        public void UpdateCommanderStats(Statistics stats)
        {
            if (IsCommanderKnown(LastSeenCommanderName))
            {
                KnownCommanders[LastSeenCommanderName].LastStatistics = stats;
            }
        }

        public void UpdateIsOdyssey(bool isOdyssey)
        {
            if (IsCommanderKnown(LastSeenCommanderName))
            {
                KnownCommanders[LastSeenCommanderName].IsOdyssey = isOdyssey;
            }
        }

        public void UpdateCommanderLocation(string systemName)
        {
            if (IsCommanderKnown(LastSeenCommanderName) && KnownCommanders[LastSeenCommanderName].CurrentSystem != systemName)
            {
                KnownCommanders[LastSeenCommanderName].CurrentSystem = systemName;
                _isDirty = true;
            }
        }

        public void ResetBeforeReadAll()
        {
            ClearReadAllRequired();
            _commanderCache.Clear();
            LastSeenCommanderName = null;

            _isDirty = true;
        }

        public void ClearReadAllRequired()
        {
            ReadAllRequired = false;
            ReadAllReason = string.Empty;
            foreach (var cmdr in _commanderCache)
            {
                cmdr.Value.ReadAllSinceFirstSeen = true;
            }

            _isDirty = true;
        }

        public void SetReadAllRequired(string reason)
        {
            if (!ReadAllRequired)
            {
                ReadAllRequired = true;
                _isDirty = true;
            }
            if (String.IsNullOrWhiteSpace(ReadAllReason) || !ReadAllReason.Contains(reason))
            {
                ReadAllReason += reason + Environment.NewLine;
                _isDirty = true;
            }
        }

        public void CheckForNewAssemblyVersion()
        {
#if !DEBUG
            // This is check is never false in DEBUG mode because the debug version is ~always newer than last read-all.
            // And in theory, I know what I'm doing. So skip the check in Debug.
            if (IsAssemblyVersionNewerThanLastUsed(_lastUsedVersion))
            {
                _lastUsedVersion = _assemblyVersion;
                SetReadAllRequired("New plugin version");
                _isDirty = true;
            }
#endif
        }

        public bool IsAssemblyVersionNewerThanLastUsed(string lastUsedVersion)
        {
            int[] lastUsedVersionParsed = ParseVersion(lastUsedVersion);
            int[] assemblyVersionParsed = ParseVersion(_assemblyVersion);

            return (Compare(lastUsedVersionParsed, assemblyVersionParsed) < 0);
        }

        // TODO: Migrate to PluginCommon's PluginVersions.
        // Re-used from PluginVersion (in FredJKsPluginAutoUpdater).
        private static int[] ParseVersion(string version)
        {
            if (string.IsNullOrEmpty(version) || version == NO_VERSION) return NO_VERSION_PARSED;

            string[] parts = version.Split('.');
            int[] numericParts = new int[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                numericParts[i] = int.Parse(parts[i]);
            }
            return numericParts;
        }

        private static int Compare(int[] left, int[] right)
        {

            int compareResult = 0; // equal.
            for (int i = 0; i < left.Length && i < right.Length; i++)
            {
                if (left[i] > right[i])
                {
                    return 1;
                }
                else if (left[i] < right[i])
                {
                    return -1; // All previous were equal, thus we can't be newer.
                }
            }

            return compareResult;
        }
    }
}
