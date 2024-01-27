namespace com.github.fredjk_gh.ObservatoryProspectorBasic
{
    internal class TrackedStats
    {
        public int GoodRocks = 0;
        public int ProspectorsEngaged = 0;
        public int LimpetsAbandoned = 0;
        public int LimpetsUsed = 0;
        public int LimpetsSynthed = 0;

        public void Reset()
        {
            GoodRocks = 0;
            ProspectorsEngaged = 0;
            LimpetsAbandoned = 0;
            LimpetsUsed = 0;
            LimpetsSynthed = 0;
        }
    }
}