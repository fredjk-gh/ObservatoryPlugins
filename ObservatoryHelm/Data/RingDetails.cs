namespace com.github.fredjk_gh.ObservatoryHelm.Data
{
    internal class RingDetails
    {
        public string ShortName;
        public string RingType;
        public string Commodities;
        public string Density;

        public override string ToString()
        {
            return $"{ShortName}: {RingType} {Commodities}, {Density}";
        }
    }
}
