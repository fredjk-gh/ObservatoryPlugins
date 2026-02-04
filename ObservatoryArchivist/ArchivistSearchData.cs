using com.github.fredjk_gh.ObservatoryArchivist.DB;
using com.github.fredjk_gh.PluginCommon.Data.EdGIS;

namespace com.github.fredjk_gh.ObservatoryArchivist
{
    internal class ArchivistSearchData(ArchivistContext ctx)
    {
        private readonly ArchivistContext _c = ctx;

        public string SearchSystemName { get; private set; }
        public ulong? SearchId64 { get; private set; }
        public string CommanderName { get; private set; }

        public bool MaySearch { get => (!string.IsNullOrEmpty(SearchSystemName) || SearchId64.HasValue); }
        public bool HasResult
        {
            get
            {
                return CollectedJournals is not null
                    || AugmentedJournals is not null
                    || Notifications is not null
                    || PositionCache is not null;
            }
        }

        public VisitedSystem CollectedJournals { get; set; }

        public VisitedSystem AugmentedJournals { get; set; }

        public List<NotificationInfo> Notifications { get; set; }

        public SystemInfo PositionCache { get; set; }

        public CoordsResponse PositionCacheLookup { get; set; }

        public void SetSearchContext(string systemIdentifier, string cmdrName)
        {
            SearchSystemName = string.Empty;
            SearchId64 = null;

            if (!string.IsNullOrWhiteSpace(systemIdentifier) && UInt64.TryParse(systemIdentifier, out ulong id64))
            {
                SearchId64 = id64;
            }
            else
            {
                SearchSystemName = systemIdentifier?.Trim();
            }

            CommanderName = cmdrName?.Trim();
        }

        public bool SearchAll()
        {
            if (!MaySearch) return false;

            SearchJournals();
            SearchNotifications();
            SearchPositionCache();

            return true;
        }

        public bool SearchJournals()
        {
            if (!MaySearch) return false;

            List<VisitedSystem> results;
            if (SearchId64.HasValue)
                // Search by system ID (and/or commander)
                results = _c.Journals.GetVisitedSystems(SearchId64.Value, CommanderName);
            else
                results = _c.Journals.GetVisitedSystems(SearchSystemName, CommanderName);

            if (results.Count == 0)
            {
                _c.UI.SetMessage("No journals found");
                return false;
            }

            VisitedSystem result = results[0];
            string resultMessage;
            if (results.Count > 1)
                resultMessage = $"Results for multiple commanders found; showing result with most collected entries ({result.SystemJournalEntries.Count}) from {result.Commander}";
            else
                resultMessage = $"Found {result.SystemJournalEntries.Count} collected entries";
            _c.UI.SetMessage(resultMessage);

            VisitedSystem augmentedSystem = _c.Journals.GetExactMatchAugmentedSystem(result.SystemId64);

            if (augmentedSystem is not null)
                _c.UI.SetMessage($"Also found {result.SystemJournalEntries.Count} Spansh-augmented entries");
            else
                _c.UI.SetMessage("No augmented entries found");

            CollectedJournals = result;
            AugmentedJournals = augmentedSystem;
            return true;
        }

        public bool SearchNotifications()
        {
            if (!MaySearch) return false;

            Dictionary<string, List<NotificationInfo>> results;
            List<NotificationInfo> result;
            if (SearchId64.HasValue)
                results = _c.Notifications.FindNotifications(SearchId64.Value, CommanderName);
            else
                results = _c.Notifications.FindNotifications(SearchSystemName, CommanderName);

            if (results.Count == 0)
            {
                _c.UI.SetMessage("No notifications found");
                return false;
            }

            result = results.OrderByDescending(e => e.Value.Count).First().Value;
            string resultMessage;
            if (results.Count > 1)
                resultMessage = $"Results for multiple commanders found; showing result with most notifications ({result.Count}) from {result.First().Commander}";
            else
                resultMessage = $"Found {result.Count} notifications";
            _c.UI.SetMessage(resultMessage);

            Notifications = result;
            return true;
        }

        public bool SearchPositionCache()
        {
            if (!MaySearch) return false;

            if (SearchId64.HasValue)
                PositionCache = _c.PositionCache.GetSystem(SearchId64.Value);
            else
                PositionCache = _c.PositionCache.GetSystem(SearchSystemName);

            string resultMessage;
            if (PositionCache is not null)
                resultMessage = "Position cache entry found";
            else
                resultMessage = "No position cache entry found";
            _c.UI.SetMessage(resultMessage);

            return (PositionCache is not null);
        }

        public void Clear()
        {
            CollectedJournals = null;
            AugmentedJournals = null;
            Notifications = null;
            PositionCache = null;
            PositionCacheLookup = null;
        }
    }
}
