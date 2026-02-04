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

    internal class Result(NotificationClass notificationClass, StatScannerGrid item, int coalescingId)
    {
        public NotificationClass NotificationClass { get; } = notificationClass;
        public int CoalescingID { get; } = coalescingId;
        public StatScannerGrid ResultItem { get; } = item;

    }
}
