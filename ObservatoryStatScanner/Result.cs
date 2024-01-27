using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryStatScanner
{
    enum NotificationClass
    {
        None,
        PossibleNewGalacticRecord,
        MatchedGalacticRecord,
        VisitedGalacticRecord,
        NearGalacticRecord,
        PersonalBest,
        NewCodex,
        Tally,
    }

    internal class Result
    {
        public Result(NotificationClass notificationClass, StatScannerGrid item, int coalescingId)
        {
            NotificationClass = notificationClass;
            ResultItem = item;
            CoalescingID = coalescingId;
        }

        public NotificationClass NotificationClass { get; }
        public int CoalescingID { get; }
        public StatScannerGrid ResultItem { get; }

    }
}
