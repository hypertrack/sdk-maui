// ReSharper disable CheckNamespace

namespace HyperTrack;

partial class HyperTrack
{
    public class LocationWithDeviation(Location location, double deviation)
    {
        public Location Location { get; } = location;
        public double Deviation { get; } = deviation;
    }
}