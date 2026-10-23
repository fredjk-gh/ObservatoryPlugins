using com.github.fredjk_gh.PluginCommon.UI.Shared;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryHelm.Data
{
    internal class ProspectorDetails
    {
        internal ProspectedAsteroid Event { get; init; }
        internal ImageSpec CommoditiesImage { get; set; }
        internal ImageSpec CoreImage { get; set; }
    }
}
