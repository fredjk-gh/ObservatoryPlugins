namespace com.github.fredjk_gh.ObservatoryAggregator.UI
{
    public enum VisitedState
    {
        None,
        MarkForVisit,
        Unvisited,
        Visited,
    }

    public static class VisitedStates // Extensions
    {
        public static VisitedState NextState(this VisitedState state)
        {
            return state switch
            {
                VisitedState.MarkForVisit => VisitedState.Unvisited,
                VisitedState.Unvisited => VisitedState.Visited,
                VisitedState.Visited => VisitedState.MarkForVisit,
                _ => VisitedState.None,
            };
        }
    }
}
