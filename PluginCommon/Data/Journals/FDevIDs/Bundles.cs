using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs.CSVListBuilder;

namespace com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs
{
    public class Bundles
    {
        public long Id { get; init; }
        public string Name { get; init; }
        public string Sku { get; init; }


        #region CSV Parsing
        internal static DictBuilderOptions<long, Bundles> ByIdOptions = new(
            CSV.CSVBundles,
            "id,name,sku",
            (string[] parts) => Convert.ToInt64(parts[1]),
            (string[] parts) => {
                return new()
                {
                    Id = Convert.ToInt64(parts[0]),
                    Name = parts[1].Trim(),
                    Sku = parts[2].Trim(),
                };
            });
        #endregion
    }
}
