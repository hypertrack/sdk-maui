// ReSharper disable CheckNamespace

namespace HyperTrack
{
    public static partial class HyperTrack
    {
        public class Location : IEquatable<Location>
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

            public bool Equals(Location? other)
            {
                if (other is null) return false;
                return Latitude == other.Latitude && Longitude == other.Longitude;
            }

            public override bool Equals(object? obj) => Equals(obj as Location);
            public override int GetHashCode() => HashCode.Combine(Latitude, Longitude);
            public static bool operator ==(Location? left, Location? right) => left?.Equals(right) ?? right is null;
            public static bool operator !=(Location? left, Location? right) => !(left == right);
        }
    }

}
