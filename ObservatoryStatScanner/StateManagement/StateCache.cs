using Observatory.Framework.Files.Journal;
using System.Reflection;
using System.Text.Json.Serialization;

namespace com.github.fredjk_gh.ObservatoryStatScanner.StateManagement
{
    internal class StateCache
    {
        internal const string NO_VERSION = "0.0.0.0";
        internal static readonly int[] NO_VERSION_PARSED = new int[] { 0, 0, 0, 0 };

        private Dictionary<string, CommanderCache> _commanderCache = new();
        private string _lastSeenFid = string.Empty;
        private bool _readAllRequired = true;
        private string _lastUsedVersion = "";
        private string _assemblyVersion = typeof(StatScanner).Assembly.GetName().Version.ToString();
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
        public string LastSeenCommanderFID {
            get => _lastSeenFid;
            set
            {
                _lastSeenFid = value;
                _isDirty = true;
            }
        }

        [JsonIgnore]
        public string LastSeenCommanderName
        {
            get
            {
                if (KnownCommanders.Count == 0 || !IsCommanderKnown(LastSeenCommanderFID))
                    return "(Unknown commander)";
                return KnownCommanders[LastSeenCommanderFID].Name;
            }
        }

        public Dictionary<string, CommanderCache> KnownCommanders
        {
            get => _commanderCache;
            set => _commanderCache = value;
        }

        public bool IsCommanderKnown(string commanderFID = null)
        {
            return _commanderCache.ContainsKey(commanderFID ?? LastSeenCommanderFID);
        }

        [JsonIgnore]
        public CommanderCache? CurrentCommander
        {
            get
            {
                if (KnownCommanders.ContainsKey(LastSeenCommanderFID))
                {
                    return KnownCommanders[LastSeenCommanderFID];
                }
                return null;
            }
        }

        public void AddCommander(string fid, bool isOdyssey, bool hasReadAll)
        {
            var newCmdr = new CommanderCache()
            {
                FID = fid,
                Name = fid,
                CurrentSystem = "(unknown location)",
                IsOdyssey = isOdyssey,
                ReadAllSinceFirstSeen = hasReadAll,
            };

            _commanderCache[fid] = newCmdr;
            // Only dirty if we're also flipping the read-all flag.
            if (!hasReadAll) SetReadAllRequired("New commander detected");
        }

        public void UpdateCommanderInfo(LoadGame loadGame, bool isOdyssey = false, bool hasReadAll = false)
        {
            if (!IsCommanderKnown(loadGame.FID)) {
                AddCommander(loadGame.FID, isOdyssey, hasReadAll);
            }
            var cached = _commanderCache[loadGame.FID];
            cached.Name = loadGame.Commander;
            cached.LastLoadGame = loadGame;
            cached.IsOdyssey = isOdyssey;
            LastSeenCommanderFID = loadGame.FID;
        }

        public void UpdateCommanderStats(Statistics stats)
        {
            if (IsCommanderKnown(LastSeenCommanderFID))
            {
                KnownCommanders[LastSeenCommanderFID].LastStatistics = stats;
            }
        }

        public void UpdateIsOdyssey(bool isOdyssey)
        {
            if (IsCommanderKnown(LastSeenCommanderFID))
            {
                KnownCommanders[LastSeenCommanderFID].IsOdyssey = isOdyssey;
            }
        }

        public void UpdateCommanderLocation(string systemName)
        {
            if (IsCommanderKnown(LastSeenCommanderFID) && KnownCommanders[LastSeenCommanderFID].CurrentSystem != systemName)
            {
                KnownCommanders[LastSeenCommanderFID].CurrentSystem = systemName;
                _isDirty = true;
            }
        }

        public void ResetBeforeReadAll()
        {
            ClearReadAllRequired();
            _commanderCache.Clear();
            LastSeenCommanderFID = null;

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
