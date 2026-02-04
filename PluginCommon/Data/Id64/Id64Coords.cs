using Observatory.Framework.Files.ParameterTypes;
using System.Diagnostics;

namespace com.github.fredjk_gh.PluginCommon.Data.Id64
{
    public class Id64Coords(UInt64 id64, int x, int y, int z, int errLy = 0)
    {
        public UInt64 Id64 { get; private set; } = id64;
        public int X { get; private set; } = x;
        public int Y { get; private set; } = y;
        public int Z { get; private set; } = z;
        public int ErrLy { get; private set; } = errLy;

        public override string ToString()
        {
            if (ErrLy > 0)
            {
                return $"({X}, {Y}, {Z}) +/- {ErrLy}";
            }
            else
            {
                return $"({X}, {Y}, {Z})";
            }
        }

        public bool ValidateStarPosition(StarPosition p)
        {
            // Coordinate-wise check that each is with +/- ErrLy of estimate.
            Debug.Assert(ErrLy > 0);

            if (p.x >= (X - ErrLy) && p.x <= (X + ErrLy)
                && p.y >= (Y - ErrLy) && p.y <= (X + ErrLy)
                && p.z >= (Y - ErrLy) && p.z <= (Z + ErrLy))
            {
                return true;
            }
            return false;
        }
    }
}
