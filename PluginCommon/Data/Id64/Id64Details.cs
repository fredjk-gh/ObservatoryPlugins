using System.Collections.Specialized;
using System.Text;

namespace com.github.fredjk_gh.PluginCommon.Data.Id64
{
    public class Id64Details
    {
        public static Id64Details FromId64(UInt64 id64)
        {
            Id64XYZ boxXYZ = Id64XYZ.ForBoxel(id64);
            Id64XYZ sectorXYZ = Id64XYZ.ForSector(id64);

            int masscode = (int)id64 & 7;
            int bitshift = (7 - masscode);

            var details = new Id64Details()
            {
                Id64 = id64,
                Masscode = masscode,
                BoxSize = 10 * (1 << masscode),
                BoxelCoords = boxXYZ,
                SectorCoords = sectorXYZ,
                // TODO: Add Region id/name.

                SystemNum = (int)((id64 & 0x7FFFFFFFFFFFFF) >> (23 + 3 * bitshift)),
                BodyId = (int)(id64 >>> 55),
            };

            return details;
        }

        internal Id64Details()
        {

        }

        public ulong Id64 { get; init; }
        public int SystemNum { get; init; }
        public int Masscode { get; init; }

        public int BoxSize { get; init; }
        public Id64XYZ BoxelCoords { get; init; }

        public Id64XYZ SectorCoords { get; init; }

        public int BodyId { get; init; }

        public char MassCodeIndicator
        {
            get => (char)(((int)'a') + Masscode);
        }

        // Logic from https://web.archive.org/web/20240303181408/http://disc.thargoid.space/Sector_Naming
        public string SectorName
        {
            get
            {
                //string name;
                //if (ProcGenConstants.SECTOR_NAMES_BY_ID.TryGetValue(sectorId, out name))
                //{
                //    return name;
                //}
                //return null;
                return Id64SectorName.GetSectorName(this);
            }
        }

        public string SystemIdentifier
        {
            get
            {
                var n1 = BoxelCoords.Id;

                char A = (char)((n1 % 26) + 'A');
                n1 /= 26;
                char B = (char)((n1 % 26) + 'A');
                n1 /= 26;
                char C = (char)((n1 % 26) + 'A');
                n1 /= 26;

                return $"{A}{B}-{C} {MassCodeIndicator}{(n1 > 0 ? $"{n1}-" : "")}{SystemNum}";
            }
        }

        public string ProcGenSystemName
        {
            get => $"{SectorName} {SystemIdentifier}";
        }

        public NameValueCollection Values()
        {
            var system = Id64CoordHelper.EstimatedCoords(Id64);
            NameValueCollection v = new()
            {
                { "Id64", $"{Id64}" },
                { "Sector Id", $"{SectorCoords.Id}" },
                { "Sector Coords", $"{SectorCoords.ToString()}" },
                { "Sector Name", $"{SectorName}" },
                { "Boxel Id", $"{BoxelCoords.Id}" },
                { "Boxel Coords", $"{BoxelCoords.ToString()}" },
                { "Boxel Size", $"{BoxSize} Ly" },
                { "Masscode", $"{MassCodeIndicator}" },
                { "System Index", $"{SystemNum}" },
                { "Estimated System Coords", $"{system}" },
                { "System Identifier", $"{SystemIdentifier}" },
                { "Full ProcGen System Name", $"{ProcGenSystemName}" }
            };

            if (BodyId > 0)
                v.Add("Body ID", $"{BodyId}");

            return v;
        }

        public override string ToString()
        {
            StringBuilder sb = new();

            var allValues = Values();
            foreach (var v in allValues.AllKeys)
            {
                sb.AppendLine($"{v}: {allValues[v]}");
            }
            return sb.ToString();
        }
    }
}
