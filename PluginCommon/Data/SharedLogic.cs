using com.github.fredjk_gh.PluginCommon.Data.Id64;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Files.ParameterTypes;
using System.Collections.Immutable;

namespace com.github.fredjk_gh.PluginCommon.Data
{
    public static class SharedLogic
    {

        public const string PRIMARY_STAR = "Primary star";
        public const double BUBBLE_RADIUS_LY = 700;
        public static readonly StarPosition EARTH_POSITION = new() { x = 0, y = 0, z = 0 };

        public static bool IsWellKnownCoreSys(
            StarPosition pos,
            bool hasNavBeacon,
            ImmutableList<string> powers,
            double bubbleRadius = BUBBLE_RADIUS_LY)
        {
            return hasNavBeacon
                || (Id64CoordHelper.Distance(EARTH_POSITION, pos) <= bubbleRadius
                && (powers?.Count ?? 0) > 0);
        }

        public static string GetBodyShortName(string bodyName, string baseName)
        {
            return bodyName.Replace(baseName, "").Trim();
        }

        //public static string GetBodyShortNameExt(string bodyName, string systemName = "", bool isBarycentre = false)
        //{
        //    string shortName = bodyName;
        //    if (isBarycentre && BarycentreChildren.Count >= 1)
        //    {
        //        var childIds = BarycentreChildren.Select(bc => bc.BodyID).ToHashSet();
        //        shortName = $"({string.Join("-", _allData.BodyData.Where(e => childIds.Contains(e.Key)).Select(e => e.Value.BodyShortName))})";
        //    }
        //    else if (!isBarycentre)
        //    {
        //        var sysName = systemName;
        //        if (!string.IsNullOrWhiteSpace(sysName)) shortName = bodyName.Replace(sysName, "").Trim();
        //    }
        //    return (string.IsNullOrEmpty(shortName) ? PRIMARY_STAR : shortName);
        //}

        public static string GetBodyDisplayName(string shortBodyName)
        {
            if (string.IsNullOrWhiteSpace(shortBodyName))
            {
                return PRIMARY_STAR;
            }
            return $"Body {shortBodyName}";
        }

        //public static string GetBodyDisplayNameExt(string bodyName, Scan scan, bool isBarycentre = false)
        //{
        //    string bodyShortName = GetShortBodyNameExt(bodyName, scan.StarSystem, isBarycentre);

        //    if (scan?.PlanetClass != null)
        //        return $"Body {bodyShortName}";
        //    else if (scan?.StarType != null)
        //        return $"{(bodyShortName == PRIMARY_STAR ? "" : "Body ")}{bodyShortName}";
        //    else if (isBarycentre)
        //    {
        //        if (scan.BodyID == 0)
        //            return "System barycentre";
        //        else
        //            return $"Barycentre {bodyShortName}";
        //    }
        //    return bodyShortName;
        //}
    }
}
