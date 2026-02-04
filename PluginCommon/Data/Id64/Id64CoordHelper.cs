using Observatory.Framework.Files.ParameterTypes;

namespace com.github.fredjk_gh.PluginCommon.Data.Id64
{
    /// <summary>
    /// ID64 coordinate helper functions.
    /// 
    /// Adapted from https://github.com/Orvidius/edastro/blob/main/EDSM.pm
    /// Required Reading:
    /// - https://web.archive.org/web/20220618134655/http://disc.thargoid.space/ID64
    /// - https://web.archive.org/web/20240303181408/http://disc.thargoid.space/Sector_Naming:
    ///     -- Note: Hash function used to determine if class 1 or class 2 is used is the Jenkins function:
    ///        https://en.wikipedia.org/wiki/Jenkins_hash_function
    ///     -- Example, see: https://bitbucket.org/Esvandiary/edts/src/5ff04315/edtslib/util.py#lines-231
    /// </summary>
    public class Id64CoordHelper
    {
        public const int GAL_CENTER_x = -65;
        public const int GAL_CENTER_y = -25;
        public const int GAL_CENTER_z = 25815;
        public const int SECTOR_OFFSET_z = -1065;

        public static double Distance(StarPosition pos1, StarPosition pos2)
        {
            double dist = Math.Sqrt(Math.Pow(pos1.x - pos2.x, 2) + Math.Pow(pos1.y - pos2.y, 2) + Math.Pow(pos1.z - pos2.z, 2));
            return Math.Round(dist, 2);
        }

        public static double Distance(UInt64 id64_1, UInt64 id64_2)
        {
            Id64Coords s1 = EstimatedCoords(id64_1);
            Id64Coords s2 = EstimatedCoords(id64_2);

            double dist = Math.Sqrt(Math.Pow(s1.X - s2.X, 2) + Math.Pow(s1.Y - s2.Y, 2) + Math.Pow(s1.Z - s2.Z, 2));
            return Math.Round(dist, 2);
        }

        public static Id64Coords EstimatedCoords(UInt64 id64)
        {

            Id64Details sectorDetails = Id64Details.FromId64(id64);

            int size = 10 * (1 << sectorDetails.Masscode);

            int x = sectorDetails.BoxelCoords.X * size;
            int y = sectorDetails.BoxelCoords.Y * size;
            int z = sectorDetails.BoxelCoords.Z * size;

            int sx = sectorDetails.SectorCoords.X - 39;
            int sy = sectorDetails.SectorCoords.Y - 32;
            int sz = sectorDetails.SectorCoords.Z - 18;

            int error = size / 2;

            var estcoords = new Id64Coords(
                id64,
                sx * 1280 + x + error + GAL_CENTER_x,
                sy * 1280 + y + error + GAL_CENTER_y,
                sz * 1280 + z + error + SECTOR_OFFSET_z,
                error);
            return estcoords;
        }

        // A javascript version similar to the above:
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
