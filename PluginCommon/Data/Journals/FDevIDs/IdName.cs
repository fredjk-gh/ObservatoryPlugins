using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs.CSVListBuilder;

namespace com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs
{
    public class IdName
    {
        public string Id { get; init; }
        public string Name { get; init; }


        #region CSV Parsing
        private const string IDNAME_HEADERS = "id,name";

        internal static DictBuilderOptions<string, IdName> BodiesByIdOptions =
            new(CSV.CSVBodies, IDNAME_HEADERS, KeyFactory, ItemFactory);

        internal static DictBuilderOptions<string, IdName> EconomyByIdOptions = 
            new(CSV.CSVEconomy, IDNAME_HEADERS, KeyFactory, ItemFactory);

        internal static DictBuilderOptions<string, IdName> FactionsByIdOptions = 
            new(CSV.CSVFactionIds, IDNAME_HEADERS, KeyFactory, ItemFactory);

        internal static DictBuilderOptions<string, IdName> FactionStatesByIdOptions = 
            new(CSV.CSVFactionState, IDNAME_HEADERS, KeyFactory, ItemFactory);

        internal static DictBuilderOptions<string, IdName> GovernmentByIdOptions = 
            new(CSV.CSVGovernment, IDNAME_HEADERS, KeyFactory, ItemFactory);

        internal static DictBuilderOptions<string, IdName> HappinessByIdOptions =
            new(CSV.CSVHappiness, IDNAME_HEADERS, KeyFactory, ItemFactory);

        // Atypical header names.
        internal static DictBuilderOptions<string, IdName> PassengersByIdOptions = 
            new(CSV.CSVPassengers,"fdname,name", KeyFactory, ItemFactory);

        internal static DictBuilderOptions<string, IdName> RingsByIdOptions =
            new(CSV.CSVRings, IDNAME_HEADERS, KeyFactory, ItemFactory);

        internal static DictBuilderOptions<string, IdName> SAASignalsByIdOptions =
            new(CSV.CSVSaaSignals, IDNAME_HEADERS, KeyFactory, ItemFactory);

        internal static DictBuilderOptions<string, IdName> SecurityByIdOptions = 
            new(CSV.CSVSecurity, IDNAME_HEADERS, KeyFactory, ItemFactory);

        // Atypical header names.
        internal static DictBuilderOptions<string, IdName> SkuByIdOptions =
            new(CSV.CSVSku, "sku,requirement", KeyFactory, ItemFactory);

        // Atypical header names.
        internal static DictBuilderOptions<string, IdName> SystemAllegianceByIdOptions =
            new(CSV.CSVSystemAllegiance,"System Allegiance,Name", KeyFactory, ItemFactory);

        internal static DictBuilderOptions<string, IdName> TerraformingStateByIdOptions = 
            new(CSV.CSVTerraformingState, IDNAME_HEADERS, KeyFactory, ItemFactory);

        private static string KeyFactory(string[] parts)
        {
            return parts[0].Trim();
        }

        private static IdName ItemFactory(string[] parts) {
            return new ()
            {
                Id = parts[0].Trim(),
                Name = parts[1].Trim(),
            };
        }

        #endregion
    }
}
