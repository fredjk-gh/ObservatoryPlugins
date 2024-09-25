using com.github.fredjk_gh.ObservatoryAggregator.UI;
using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryAggregator
{
    internal class NotificationData
    {
        private TrackedData _allData;
        private AggregatorGrid _gridItem = null;
        private VisitedState _visitedState = VisitedState.MarkForVisit;
        private string _suppressedTitle = "";

        public NotificationData(TrackedData allData, string systemName, NotificationArgs args)
            : this(allData, systemName, args.Sender, args.Title, args.Detail, args.ExtendedDetails, args.CoalescingId ?? Constants.DEFAULT_COALESCING_ID)
        { }

        public NotificationData(TrackedData allData, string system, string sender, string title, string detail, string extDetails, int coalescingId)
        {
            _allData = allData;
            Timestamp = DateTime.UtcNow; // Largely for as-arrived ordering, if desired.
            System = system;
            Sender = sender;
            Title = title;
            Detail = detail;
            ExtendedDetails = extDetails;
            CoalescingID = coalescingId;
        }

        public DateTime Timestamp { get; }
        public string System { get; }
        public string Sender { get; }
        public string Title { get; }
        public string Detail { get; }
        public string ExtendedDetails { get; }
        public int CoalescingID { get; }
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
                _gridItem = new AggregatorGrid(_allData, this, suppressedTitle);
            }
            return _gridItem;
        }
    }
}
