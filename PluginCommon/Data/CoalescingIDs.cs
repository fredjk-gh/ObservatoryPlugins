namespace com.github.fredjk_gh.PluginCommon.Data
{
    public static class CoalescingIDs
    {
        public const int ALERT_COALESCING_ID = Int32.MinValue; // preamble / alerts
        public const int HEADER_COALESCING_ID = Int32.MinValue; // preamble / alerts
        public const int SUMMARY_COALESCING_ID = -11;
        public const int STATS_COALESCING_ID = -10;
        public const int REGION_COALESCING_ID = -2;
        public const int SYSTEM_COALESCING_ID = -1;
        public const int MIN_BODY_COALESCING_ID = 0;
        public const int MAX_BODY_COALESCING_ID = 1000;
        public const int DEFAULT_COALESCING_ID = 1001; // After bodies.

        public static bool IsBodyCoalescingId(int cId)
        {
            return cId >= MIN_BODY_COALESCING_ID && cId <= MAX_BODY_COALESCING_ID;
        }
    }
}
