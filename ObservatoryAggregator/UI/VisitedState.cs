using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            switch (state)
            {
                case VisitedState.MarkForVisit:
                    return VisitedState.Unvisited;
                case VisitedState.Unvisited:
                    return VisitedState.Visited;
                case VisitedState.Visited:
                    return VisitedState.MarkForVisit;
            }
            return VisitedState.None;
        }

        public static EmojiSpec ToEmojiSpec(this VisitedState state)
        {
            switch (state)
            {
                case VisitedState.MarkForVisit:
                    return new("🔍");
                case VisitedState.Unvisited:
                    return new("🔲");
                case VisitedState.Visited:
                    return new("✅");
            }
            return new("");
        }

        public static VisitedState FromString(string str)
        {
            switch (str)
            {
                case "🔍": return VisitedState.MarkForVisit;
                case "🔲": return VisitedState.Unvisited;
                case "✅": return VisitedState.Visited;
            }
            return VisitedState.None;
        }
    }
}
