using Observatory.Framework.Files;

namespace com.github.fredjk_gh.ObservatoryHelm.Data
{
    internal class SurfacePosition
    {
        internal static SurfacePosition FromStatus(Status status)
        {
            return new(status.BodyName, status.Latitude, status.Longitude, status.Altitude);
        }

        internal SurfacePosition(string bodyName, double lat, double lng, float alt)
        {
            BodyName = bodyName;
            Latitude = lat;
            Longitude = lng;
            Altitude = alt;
        }

        public string BodyName { get; internal init; }
        public double Latitude { get; internal init; }
        public double Longitude { get; internal init; }
        public double Altitude { get; internal init; }
    }
}
