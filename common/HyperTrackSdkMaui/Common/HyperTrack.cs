namespace HyperTrack;
using UIKit;
using Foundation;

#if ANDROID
using HyperTrackAndroid = Com.Hypertrack.Sdk.Android.HyperTrack;
using OrderStatusAndroid = Com.Hypertrack.Sdk.Android.HyperTrack.OrderStatus;
using ClockInAndroid = Com.Hypertrack.Sdk.Android.HyperTrack.OrderStatus.ClockIn;
using JsonAndroid = Com.Hypertrack.Sdk.Android.Json;
using ResultAndroid = Com.Hypertrack.Sdk.Android.Result;
#endif

#if IOS
using HyperTrackIos = binding_ios.HyperTrackMauiWrapper;
using Foundation;
#endif

public partial class HyperTrack
{
    public static string DeviceId
    {
        get
        {
#if ANDROID
            return HyperTrackAndroid.DeviceID;
#endif
#if IOS
            return HyperTrackIos.DeviceId;
#endif
        }
    }

    public static HyperTrack.Result<HyperTrack.Location, HyperTrack.LocationError> AddGeotag(
        string orderHandle,
        OrderStatus orderStatus,
        Json.Object metadata)
    {
#if ANDROID
        OrderStatusAndroid orderStatusAndroid = ClockInAndroid.Instance;
        JsonAndroid.Object metadataAndroid = Mapping.FromJsonSharp(metadata) as JsonAndroid.Object;
        ResultAndroid result = HyperTrackAndroid.AddGeotag(orderHandle, orderStatusAndroid, metadataAndroid);
        return Mapping.FromResultAndroid<HyperTrackAndroid.Location, HyperTrackAndroid.LocationError>(result)
            .Map(Mapping.FromLocationAndroid)
            .MapFailure(Mapping.FromLocationErrorAndroid);
#endif
#if IOS
        var serialized = Serialization.SerializeGeotagData(
            metadata,
            orderHandle,
            orderStatus
        );
        var sringParam = HyperTrack.Json.FromDictionary(serialized)!.ToString();
        var resultString = HyperTrackIos.AddGeotag(sringParam);
        var result = HyperTrack.Json.FromString(resultString)!.ToDictionary();
        return Serialization.DeserializeLocationResult(result);
#endif
    }



}
