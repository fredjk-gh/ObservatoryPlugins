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
            line => line.Trim());

        internal static ListBuilderOptions<string> DockingDeniedReasonsOptions = new(
            CSV.CSVDockingDeniedReasons,
            "id",
            line => line.Trim());
        #endregion
    }
}
