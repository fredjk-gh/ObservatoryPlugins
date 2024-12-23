using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs.CSVListBuilder;

namespace com.github.fredjk_gh.PluginCommon.Data.Journals.FDevIDs
{
    public class CSVIdLists
    {
        public string Id { get; init; }

        #region CSV Parsing
        internal static ListBuilderOptions<string> CrimeOptions = new(
            CSV.CSVCrimes,
            "id",
            (string line) => line.Trim());

        internal static ListBuilderOptions<string> DockingDeniedReasonsOptions = new(
            CSV.CSVDockingDeniedReasons,
            "id",
            (string line) => line.Trim());
        #endregion
    }
}
