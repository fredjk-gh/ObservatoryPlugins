using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs.CSVListBuilder;

namespace com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs
{
    public class Outfitting
    {
        public long Id { get; init; }
        public string Symbol { get; init; }
        public string Category { get; init; }
        public string Name { get; init; }
        public string Mount { get; init; }
        public string Guidance { get; init; }
        public string Ship { get; init; }
        public UInt16 Class { get; init; }
        public char Rating{ get; init; }
        public string Entitlement { get; init; }


        #region CSV Parsing
        internal static DictBuilderOptions<string, Outfitting> BySymbolOptions = new(
            CSV.CSVOutfitting,
            "id,symbol,category,name,mount,guidance,ship,class,rating,entitlement",
            (string[] parts) => parts[1].Trim(),
            (string[] parts) => {
                return new()
                {
                    Id = Convert.ToInt64(parts[0]),
                    Symbol = parts[1].Trim(),
                    Category = parts[2].Trim(),
                    Name = parts[3].Trim(),
                    Mount = parts[4].Trim(),
                    Guidance = parts[5].Trim(),
                    Ship = parts[6].Trim(),
                    Class = Convert.ToUInt16(parts[7]),
                    Rating = Convert.ToChar(parts[8]),
                    Entitlement = parts[9].Trim(),
                };
            });
        #endregion
    }
}
