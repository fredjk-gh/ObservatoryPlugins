using static com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs.CSVListBuilder;

namespace com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs
{
    public class Shipyard
    {
        public long Id { get; init; }
        public string Symbol { get; init; }
        public string Name { get; init; }
        public string Entitlement { get; init; }


        #region CSV Parsing
        internal static DictBuilderOptions<string, Shipyard> BySymbolOptions = new(
            CSV.CSVShipyard,
            "id,symbol,name,entitlement",
            parts => parts[1].Trim().ToLower(),
            parts =>
            {
                return new()
                {
                    Id = Convert.ToInt64(parts[0]),
                    Symbol = parts[1].Trim().ToLower(),
                    Name = parts[2].Trim(),
                    Entitlement = parts[3].Trim(),
                };
            });
        #endregion
    }
}
