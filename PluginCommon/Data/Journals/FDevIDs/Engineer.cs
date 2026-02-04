using static com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs.CSVListBuilder;

namespace com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs
{
    public class Engineer
    {
        public long Id { get; init; }
        public ulong SystemAddress { get; init; }
        public ulong MarketId { get; init; }
        public string Name { get; init; }


        #region CSV Parsing
        internal static DictBuilderOptions<long, Engineer> BySymbolOptions = new(
            CSV.CSVEngineers,
            "id,system_address,market_id,name",
            parts => Convert.ToInt64(parts[0]),
            parts =>
            {
                return new()
                {
                    Id = Convert.ToInt64(parts[0]),
                    SystemAddress = Convert.ToUInt64(parts[1]),
                    MarketId = Convert.ToUInt64(parts[2]),
                    Name = parts[3].Trim(),
                };
            });
        #endregion
    }
}
