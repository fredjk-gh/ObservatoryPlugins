using static com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs.CSVListBuilder;

namespace com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs
{
    public class CommodityResource
    {
        public long Id { get; init; }
        public string Symbol { get; init; }
        public string Category { get; init; }
        public string Name { get; init; }
        public long MarketID { get; init; }


        #region CSV Parsing
        internal static DictBuilderOptions<string, CommodityResource> CommoditiesBySymbolOptions =
            new(CSV.CSVCommodity, "id,symbol,category,name", KeyFactory, ItemFactory4);

        // NOTE: Atypical field header.
        internal static DictBuilderOptions<string, CommodityResource> RareCommoditiesBySymbolOptions =
            new(CSV.CSVRareCommodity, "id,symbol,market_id,category,name", KeyFactory, ItemFactory5);

        // NOTE: Atypical field header.
        internal static DictBuilderOptions<string, CommodityResource> MicroResourcesBySymbolOptions =
            new(CSV.CSVMicroResources, "id,symbol,category,English name", KeyFactory, ItemFactory4);

        private static string KeyFactory(string[] parts)
        {
            return parts[1].Trim().ToLower();
        }

        private static CommodityResource ItemFactory4(string[] parts)
        {
            return new()
            {
                Id = Convert.ToInt64(parts[0]),
                Symbol = parts[1].Trim().ToLower(),
                Category = parts[2].Trim(),
                Name = parts[3].Trim(),
                MarketID = -1,
            };
        }
        private static CommodityResource ItemFactory5(string[] parts)
        {
            return new()
            {
                Id = Convert.ToInt64(parts[0]),
                Symbol = parts[1].Trim().ToLower(),
                MarketID = Convert.ToInt64(parts[2]),
                Category = parts[3].Trim(),
                Name = parts[4].Trim(),
            };
        }
        #endregion
    }
}
