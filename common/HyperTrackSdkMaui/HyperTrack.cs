namespace HyperTrack;

#if ANDROID
using HyperTrackAndroid = Com.Hypertrack.Sdk.Android.HyperTrack;
using OrderStatusAndroid = Com.Hypertrack.Sdk.Android.HyperTrack.OrderStatus;
using ClockInAndroid = Com.Hypertrack.Sdk.Android.HyperTrack.OrderStatus.ClockIn;
using JsonAndroid = Com.Hypertrack.Sdk.Android.Json;
using ResultAndroid = Com.Hypertrack.Sdk.Android.Result;
#endif

#if IOS
using HyperTrackIos = binding_ios.HyperTrackMauiWrapper;
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

    public static Result<HyperTrack.Location, HyperTrack.LocationError> AddGeotag(
        string orderHandle,
        OrderStatus orderStatus,
        Json.Object metadata)
    {
#if ANDROID
// public unsafe Object (global::System.Collections.Generic.IDictionary<string, global::Com.Hypertrack.Sdk.Android.Json> fields)
// public unsafe Array (global::Java.Lang.IIterable items)
//
        OrderStatusAndroid orderStatusAndroid = ClockInAndroid.Instance;
        JsonAndroid.Object metadataAndroid = FromJsonSharp(metadata) as JsonAndroid.Object;
        ResultAndroid result = HyperTrackAndroid.AddGeotag(orderHandle, orderStatusAndroid, metadataAndroid);
        return FromResultAndroid<HyperTrack.Location, HyperTrack.LocationError>(result);
#endif
#if IOS
        return Result<Location, LocationError>.Ok(new Location(0.0, 0.0));
#endif
    }

    private static JsonAndroid FromJsonSharp(Json jsonSharp)
    {
        switch (jsonSharp)
        {
            case Json.Null:
                return JsonAndroid.Null.Instance;
            case Json.Bool b:
                return new JsonAndroid.Bool(b.Value);
            case Json.Number n:
                return new JsonAndroid.Number(n.Value);
            case Json.String s:
                return new JsonAndroid.String(s.Value);
            case Json.Array a:
                JsonAndroid[] arr = a.Items.Select(FromJsonSharp).ToArray();
                Java.Util.ArrayList list = new Java.Util.ArrayList(arr);
                return new JsonAndroid.Array(list);
            case Json.Object o:
                return new JsonAndroid.Object(o.Fields.ToDictionary(
                    kvp => kvp.Key,
                    kvp => FromJsonSharp(kvp.Value)
                ));
            default:
                throw new InvalidOperationException("Invalid Json value");
        }
    }

    private static Result<T, E> FromResultAndroid<T, E>(ResultAndroid resultAndroid)
        where T : class
        where E : class
    {
        switch (resultAndroid)
        {
            case ResultAndroid.Success s:
                return Result<T, E>.Ok(s.GetSuccess() as T);
            case ResultAndroid.Failure f:
                return Result<T, E>.Error(f.GetFailure() as E);
            default:
                throw new InvalidOperationException("Invalid Result value");
        }
    }

}
