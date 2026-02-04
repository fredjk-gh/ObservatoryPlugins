using static com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs.CSVListBuilder;

namespace com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs
{
    public class Rank
    {
        public int Number { get; init; }
        public string Name { get; init; }

        #region CSV Parsing
        internal static DictBuilderOptions<int, Rank> CQCRankByNumberOptions =
            new(CSV.CSVCQCRank, RANK_HEADERS, KeyFactory, ItemFactory);

        internal static DictBuilderOptions<int, Rank> EmpireRankByNumberOptions =
            new(CSV.CSVEmpireRank, RANK_HEADERS, KeyFactory, ItemFactory);

        internal static DictBuilderOptions<int, Rank> ExplorationRankByNumberOptions =
            new(CSV.CSVExplorationRank, RANK_HEADERS, KeyFactory, ItemFactory);

        internal static DictBuilderOptions<int, Rank> FederationRankByNumberOptions =
            new(CSV.CSVFederationRank, RANK_HEADERS, KeyFactory, ItemFactory);

        internal static DictBuilderOptions<int, Rank> TradeRankByNumberOptions =
            new(CSV.CSVTradeRank, RANK_HEADERS, KeyFactory, ItemFactory);

        internal static DictBuilderOptions<int, Rank> CombatRankByNumberOptions =
            new(CSV.CSVCombatRank, RANK_HEADERS, KeyFactory, ItemFactory);

        private const string RANK_HEADERS = "number,name";

        private static int KeyFactory(string[] parts)
        {
            return Convert.ToInt32(parts[0]);
        }

        private static Rank ItemFactory(string[] parts)
        {
            return new()
            {
                Number = Convert.ToInt32(parts[0]),
                Name = parts[1].Trim(),
            };
        }
        #endregion
    }
}
