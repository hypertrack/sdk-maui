// ReSharper disable CheckNamespace

namespace HyperTrack
{
    public static partial class HyperTrack
    {
        public class Location
        {
            public double Latitude { get; }
            public double Longitude { get; }

            public Location(double latitude, double longitude)
            {
                Latitude = latitude;
                Longitude = longitude;
            }

            public override string ToString()
            {
                return $"Location({Latitude}, {Longitude})";
            }
        }
    }

}
