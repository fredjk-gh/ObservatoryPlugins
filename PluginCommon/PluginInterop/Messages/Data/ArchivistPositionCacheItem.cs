using Observatory.Framework.Files.ParameterTypes;

namespace com.github.fredjk_gh.PluginCommon.PluginInterop.Messages.Data
{
    public class ArchivistPositionCacheItem
    {
        public static ArchivistPositionCacheItem New(ArchivistPositionCacheRequestItem req, double x, double y, double z)
        {
            return new()
            {
                SystemName = req.SystemName,
                SystemId64 = req.SystemId64,
                X = x,
                Y = y,
                Z = z,
            };
        }

        public static ArchivistPositionCacheItem New(string sysName, ulong id64, StarPosition pos)
        {
            return new()
            {
                SystemName = sysName,
                SystemId64 = id64,
                X = pos.x,
                Y = pos.y,
                Z = pos.z,
            };
        }

        public StarPosition ToStarPosition()
        {
            return new()
            {
                x = X,
                y = Y,
                z = Z,
            };
        }

        public string SystemName { get; set; }
        public ulong SystemId64 { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}
