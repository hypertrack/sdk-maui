// ReSharper disable CheckNamespace

namespace HyperTrack;

partial class HyperTrack
{
    public class LocationWithDeviation : IEquatable<LocationWithDeviation>
    {
        public Location Location { get; }
        public double Deviation { get; }

        public LocationWithDeviation(Location location, double deviation)
        {
            Location = location;
            Deviation = deviation;
        }

        public override string ToString()
        {
            return $"LocationWithDeviation({Location}, deviation: {Deviation})";
        }

        public bool Equals(LocationWithDeviation? other)
        {
            if (other is null) return false;
            return Location.Equals(other.Location) && Deviation.Equals(other.Deviation);
        }

        public override bool Equals(object? obj) => Equals(obj as LocationWithDeviation);
        public override int GetHashCode() => HashCode.Combine(Location, Deviation);
        public static bool operator ==(LocationWithDeviation? left, LocationWithDeviation? right) => left?.Equals(right) ?? right is null;
        public static bool operator !=(LocationWithDeviation? left, LocationWithDeviation? right) => !(left == right);
    }
}
