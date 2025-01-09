using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Observatory.Framework.Files.ParameterTypes;

namespace com.github.fredjk_gh.ObservatoryHelm.Id64
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
    internal class Id64CoordHelper
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

        public static double Distance(Int64 id64_1, Int64 id64_2)
        {
            Id64Coords s1 = EstimatedCoords(id64_1);
            Id64Coords s2 = EstimatedCoords(id64_2);

            double dist = Math.Sqrt(Math.Pow(s1.x - s2.x, 2) + Math.Pow(s1.y - s2.y, 2) + Math.Pow(s1.z - s2.z, 2));
            return Math.Round(dist, 2);
        }


        public static Id64Coords EstimatedCoords(Int64 id64)
        {

            Id64SectorDetails sectorDetails = SectorDetails(id64);

            int size = 1 << sectorDetails.masscode;

            int x = 10 * sectorDetails.boxX * size;
            int y = 10 * sectorDetails.boxY * size;
            int z = 10 * sectorDetails.boxZ * size;

            int sx = sectorDetails.sectorX - 39;
            int sy = sectorDetails.sectorY - 32;
            int sz = sectorDetails.sectorZ - 18;

            int error = size * 5;

            var estcoords = new Id64Coords(
                id64,
                sx * 1280 + x + error + GAL_CENTER_x,
                sy * 1280 + y + error + GAL_CENTER_y,
                sz * 1280 + z + error + SECTOR_OFFSET_z,
                error);
            return estcoords;
        }

        public static Id64SectorDetails SectorDetails(Int64 id64)
        {
            var details = new Id64SectorDetails();

            details.masscode = (int) id64 & 7;
            int boxbits = 7 - details.masscode;
            int boxmask = (1 << boxbits) - 1;

            details.systemNum = (int) ((id64 & 0x7FFFFFFFFFFFFF) >> (23 + 3*boxbits));
            details.boxX = (int)((id64 >> (boxbits * 2 + 16)) & boxmask);
            details.boxY = (int)((id64 >> (boxbits + 10)) & boxmask);
            details.boxZ = (int)((id64 >> 3) & boxmask);
            details.sectorX = (int)(127 & (id64 >> (16 + boxbits * 3)));
            details.sectorY = (int)(63 & (id64 >> (10 + boxbits * 2)));
            details.sectorZ = (int)(127 & (id64 >> (3 + boxbits)));
            details.sectorId = details.sectorX << 13 | details.sectorY << 7 | details.sectorZ;

            return details;
        }
    }
}
