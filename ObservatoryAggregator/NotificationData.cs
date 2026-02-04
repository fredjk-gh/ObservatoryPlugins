using com.github.fredjk_gh.ObservatoryAggregator.UI;
using com.github.fredjk_gh.PluginCommon.Data;
using Observatory.Framework;

namespace com.github.fredjk_gh.ObservatoryAggregator
{
    internal class NotificationData(AggregatorContext allData, string system, string sender, string title, string detail, string extDetails, int coalescingId)
    {
        private AggregatorGrid _gridItem = null;
        private VisitedState _visitedState = VisitedState.MarkForVisit;
        private string _suppressedTitle = "";

        public NotificationData(AggregatorContext allData, string systemName, NotificationArgs args)
            : this(allData, systemName, args.Sender, args.Title, args.Detail, args.ExtendedDetails, args.CoalescingId ?? CoalescingIDs.DEFAULT_COALESCING_ID)
        { }

        public DateTime Timestamp { get; } = DateTime.UtcNow; // Largely for as-arrived ordering, if desired.
        public string System { get; } = system ?? string.Empty;
        public string Sender { get; } = sender ?? string.Empty;
        public string Title { get; } = title ?? string.Empty;
        public string Detail { get; } = detail?.TrimEnd() ?? string.Empty;
        public string ExtendedDetails { get; } = extDetails?.TrimEnd() ?? string.Empty;
        public int CoalescingID { get; } = coalescingId;
        public VisitedState VisitedState
        {
            get => _visitedState;
            set
            {
                _visitedState = value;
                if (_gridItem != null)
                {
                    _gridItem.State = value; // Updates the cell display too.
                }
            }
        }

        public string GetTitleDisplayString(string suppressTitle = "")
        {
            return !string.IsNullOrWhiteSpace(suppressTitle) && suppressTitle.Equals(Title, StringComparison.CurrentCultureIgnoreCase) ? "" : Title;
        }

        public AggregatorGrid ToGrid(string suppressedTitle = "")
        {
            if (_gridItem == null || _suppressedTitle != suppressedTitle)
            {
                _suppressedTitle = suppressedTitle;
                _gridItem = new AggregatorGrid(allData, this, suppressedTitle);
            }
            return _gridItem;
        }

        public override bool Equals(object obj)
        {
            if (obj is not NotificationData otherN) return false;
            if (ReferenceEquals(this, otherN)) return true;

            // Don't include transient values: VisitedState 
            if (otherN.CoalescingID == CoalescingID
                && otherN.Detail == Detail
                && otherN.ExtendedDetails == ExtendedDetails
                && otherN.Sender == Sender
                && otherN.System == System
                && otherN.Title == Title)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            // Don't include transient values: VisitedState 
            return $"{CoalescingID}|{System}|{Title}|{Detail}|{ExtendedDetails}|{Sender}";
        }
    }
}
