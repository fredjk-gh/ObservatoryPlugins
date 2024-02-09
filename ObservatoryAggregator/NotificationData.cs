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
        public NotificationData(string systemName, NotificationArgs args)
            : this(systemName, args.Sender, args.Title, args.Detail, args.ExtendedDetails, args.CoalescingId ?? Constants.DEFAULT_COALESCING_ID)
        { }

        public NotificationData(string system, string sender, string title, string detail, string extDetails, int coalescingId)
        { 
            Timestamp = DateTime.UtcNow; // Largely for as-arrived ordering, if desired.
            System = system;
            Sender = sender;
            Title = title;
            Detail = detail;
            ExtendedDetails = extDetails;
            CoalescingID = coalescingId;
            IsInteresting = false;
            IsVisited = false;
        }

        public DateTime Timestamp { get; }
        public string System { get; }
        public string Sender { get; }
        public string Title { get; }
        public string Detail { get; }
        public string ExtendedDetails { get; }
        public int CoalescingID { get; }
        public bool IsInteresting { get; set; }
        public bool IsVisited { get; set; }

        public AggregatorGrid ToGridItem(string suppressTitle = "")
        {
            return new AggregatorGrid()
            {
                Flags = GetFlagsString(),
                Title = !string.IsNullOrWhiteSpace(suppressTitle) && suppressTitle.Equals(Title, StringComparison.CurrentCultureIgnoreCase) ? "" : Title,
                Detail = Detail,
                ExtendedDetails = ExtendedDetails,
                Sender = Sender,
            };
        }

        public string GetFlagsString()
        {
            var result = "";
            if (IsInteresting) result = "🔎"; // 🔭 as alt?
            if (IsVisited) result += (IsInteresting ? $"{Constants.DETAIL_SEP}✔" : "✔");
            return result;
        }
    }
}
