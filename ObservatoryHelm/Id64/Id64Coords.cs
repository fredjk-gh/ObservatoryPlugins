using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.ObservatoryHelm.Id64
{
    internal class Id64Coords
    {
        public Id64Coords(Int64 id64, int x, int y, int z, int errLy)
        {
            this.Id64 = id64;
            this.x = x;
            this.y = y;
            this.z = z;
            this.ErrLy = errLy;
        }

        public Int64 Id64 { get; private set; }
        public int x { get; private set; }
        public int y { get; private set; }
        public int z { get; private set; }
        public int ErrLy { get; private set; }

        public override string ToString()
        {
            return $"{Id64} = ({x}, {y}, {z}) +/- {ErrLy}";
        }
    }
}
