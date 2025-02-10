namespace HyperTrack;

#if ANDROID
using Com.Hypertrack.Sdk.Android;
#endif

#if IOS
using binding_ios;
#endif

public partial class HyperTrack
{

    // static var to get device id string
    public static string DeviceId
    {
        get
        {
#if ANDROID
            return Com.Hypertrack.Sdk.Android.HyperTrack.DeviceID + "android";
#endif
#if IOS
            return HyperTrackMauiWrapper.DeviceId + "ios";
#endif
        }
    }

    public static Result<HyperTrack.Location, HyperTrack.LocationError> AddGeotag(
        string orderHandle,
        OrderStatus orderStatus,
        Json.Object metadata)
    {
        return Result<Location, LocationError>.Ok(new Location(0.0, 0.0));
    }

}
