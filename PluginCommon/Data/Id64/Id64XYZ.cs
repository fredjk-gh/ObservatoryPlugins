namespace com.github.fredjk_gh.PluginCommon.Data.Id64
{
    public class Id64XYZ
    {
        private enum CoordsFor
        {
            Boxel,
            Sector,
        }

        private CoordsFor _coordsFor;

        public static Id64XYZ ForBoxel(UInt64 id64)
        {
            int masscode = (int)id64 & 7;
            int bitshift = (7 - masscode);
            ulong boxmask = (1UL << bitshift) - 1;

            Id64XYZ xyz = new()
            {
                _coordsFor = CoordsFor.Boxel,
                X = (int)((id64 >> (bitshift * 2 + 16)) & boxmask),
                Y = (int)((id64 >> (bitshift + 10)) & boxmask),
                Z = (int)((id64 >> 3) & boxmask),
            };
            return xyz;
        }

        public static Id64XYZ ForSector(UInt64 id64)
        {
            int masscode = (int)id64 & 7;
            int bitshift = (7 - masscode);

            Id64XYZ xyz = new()
            {
                _coordsFor = CoordsFor.Sector,
                X = (int)(127 & (id64 >> (16 + bitshift * 3))),
                Y = (int)(63 & (id64 >> (10 + bitshift * 2))),
                Z = (int)(127 & (id64 >> (3 + bitshift))),
            };
            return xyz;
        }

        public int X { get; init; }
        public int Y { get; init; }
        public int Z { get; init; }

        public int Id
        {
            get
            {
                return _coordsFor switch
                {
                    CoordsFor.Boxel => X + (Y << 7) + (Z << 14),
                    CoordsFor.Sector => X << 13 | Y << 7 | Z,
                    _ => -1,
                };
            }
        }


        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }
}
