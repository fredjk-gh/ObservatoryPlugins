using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs.CSVListBuilder;

namespace com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs
{
    public class Genus
    {
        public string Id { get; init; }
        public string Name { get; init; }
        public int ColonyRange { get; init; }

        #region CSV Parsing
        private const string IDNAME_HEADERS = "id,name,colony_range";
        
        internal static DictBuilderOptions<string, Genus> ByIdOptions =
            new(CSV.CSVGenuses, IDNAME_HEADERS, KeyFactory, ItemFactory);

        private static string KeyFactory(string[] parts)
        {
            return parts[0].Trim();
        }

        private static Genus ItemFactory(string[] parts)
        {
            return new()
            {
                Id = parts[0].Trim(),
                Name = parts[1].Trim(),
                ColonyRange = Convert.ToInt32(parts[2]),
            };
        }
        #endregion
    }
}
