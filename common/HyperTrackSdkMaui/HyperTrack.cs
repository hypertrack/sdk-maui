namespace HyperTrack;

#if ANDROID
using Com.Hypertrack.Sdk.Android;
#endif

#if IOS
using binding_ios;
#endif

public class HyperTrack
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

}
