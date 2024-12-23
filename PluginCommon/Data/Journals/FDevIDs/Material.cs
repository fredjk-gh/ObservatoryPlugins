using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using static com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs.CSVListBuilder;

namespace com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs
{
    public class Material
    {
        public long Id { get; init; }
        public string Symbol { get; init; }
        public UInt16 Rarity { get; init; }
        public string Type { get; init; }
        public string Category { get; init; }
        public string Name { get; init; }

        #region CSV Parsing
        internal static DictBuilderOptions<string, Material> BySymbolOptions = new(
            CSV.CSVMaterial,
            "id,symbol,rarity,type,category,name",
            (string[] parts) => parts[1].Trim(),
            (string[] parts) => {
                return new()
                {
                    Id = Convert.ToInt64(parts[0]),
                    Symbol = parts[1].Trim(),
                    Rarity = Convert.ToUInt16(parts[2]),
                    Type = parts[3].Trim(),
                    Category = parts[4].Trim(),
                    Name = parts[5].Trim(),
                };
            });
        #endregion
    }
}
