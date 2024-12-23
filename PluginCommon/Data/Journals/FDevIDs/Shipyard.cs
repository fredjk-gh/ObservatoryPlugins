using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs.CSVListBuilder;

namespace com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs
{
    internal class Shipyard
    {
        public long Id { get; init; }
        public string Symbol { get; init; }
        public string Name { get; init; }
        public string Entitlement { get; init; }


        #region CSV Parsing
        internal static DictBuilderOptions<string, Shipyard> BySymbolOptions = new(
            CSV.CSVShipyard,
            "id,symbol,name,entitlement",
            (string[] parts) => parts[1].Trim(),
            (string[] parts) =>
            {
                return new()
                {
                    Id = Convert.ToInt64(parts[0]),
                    Symbol = parts[1].Trim(),
                    Name = parts[2].Trim(),
                    Entitlement = parts[3].Trim(),
                };
            });
        #endregion
    }
}
