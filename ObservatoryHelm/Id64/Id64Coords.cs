using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Observatory.Framework.Files.ParameterTypes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

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

        // TODO: Implement a parse method based on this (JavaScript):
        // Source EDCD discord: https://discord.com/channels/164411426939600896/164411426939600896/1230593245705273394
        //function parseId64(id64)
        //{
        //    const i = BigInt(id64); // Convert input to BigInt for accurate bit manipulation

        //    function unpackAndShift(value, bits)
        //    {
        //        return [(value >> bits), value & (2n * *bits - 1n)];
        //    };

        //    let lenUsed = 0n;
        //    const [i1, mc] = unpackAndShift(i, 3n); lenUsed += 3n; // mc = 0-7 for a-h
        //    const [i2, boxelZ] = unpackAndShift(i1, 7n - mc); lenUsed += 7n - mc;
        //    const [i3, sectorZ] = unpackAndShift(i2, 7n); lenUsed += 7n;
        //    const [i4, boxelY] = unpackAndShift(i3, 7n - mc); lenUsed += 7n - mc;
        //    const [i5, sectorY] = unpackAndShift(i4, 6n); lenUsed += 6n;
        //    const [i6, boxelX] = unpackAndShift(i5, 7n - mc); lenUsed += 7n - mc;
        //    const [i7, sectorX] = unpackAndShift(i6, 7n); lenUsed += 7n;
        //    const [i9, n2] = unpackAndShift(i7, 55n - lenUsed);
        //    const [i8, bodyId] = unpackAndShift(i9, 9n);

        //    const boxelSize = 10n * (2n** mc);

        //    // Return the parsed data as a JSON string
        //    return JSON.stringify({
        //        mc: Number(mc),
        //        boxel_size: Number(boxelSize),
        //        sector_x: Number(sectorX),
        //        sector_y: Number(sectorY),
        //        sector_z: Number(sectorZ),
        //        boxel_x: Number(boxelX),
        //        boxel_y: Number(boxelY),
        //        boxel_z: Number(boxelZ),
        //        n2: Number(n2),
        //        body_id: Number(bodyId),
        //    });
        //};
    }
}
