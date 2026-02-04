using static com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs.CSVListBuilder;

namespace com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs
{
    public class Region
    {
        public string Id { get; init; }
        public string Name { get; init; }
        public int Number { get; init; }

        #region CSV Parsing
        private const string IDNAME_HEADERS = "id,name,number";

        internal static DictBuilderOptions<string, Region> ByIdOptions =
            new(CSV.CSVRegions, IDNAME_HEADERS, KeyFactory, ItemFactory);

        private static string KeyFactory(string[] parts)
        {
            return parts[0].Trim();
        }

        private static Region ItemFactory(string[] parts)
        {
            return new()
            {
                Id = parts[0].Trim(),
                Name = parts[1].Trim(),
                Number = Convert.ToInt32(parts[2]),
            };
        }
        #endregion
    }
}
