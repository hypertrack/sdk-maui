namespace HyperTrack 
{
public partial class HyperTrack
{
    public class Location
    {
        double Latitude { get; }
        double Longitude { get; }

        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}

}
