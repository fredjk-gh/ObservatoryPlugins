using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryAggregator
{
    internal class SystemSummary(string systemName, ulong systemAddress)
    {
        public string Name { get; } = systemName;
        public ulong SystemAddress { get; } = systemAddress;
        public bool IsUndiscovered { get; set; } = false;
        public FSSDiscoveryScan DiscoveryScan { get; set; }
        public FSSAllBodiesFound AllBodiesFound { get; set; }

        public bool HasScoopableStar(AggregatorContext data)
        {
            return data.BodyData.Values.Any(b => b.IsScoopableStar);
        }

        public string GetDetailString()
        {
            if (DiscoveryScan != null)
                return $"{DiscoveryScan.BodyCount} bodies";

            return string.Empty;
        }
    }
}
