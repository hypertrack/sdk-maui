namespace HyperTrack;

#if ANDROID
using HyperTrackAndroid = Com.Hypertrack.Sdk.Android.HyperTrack;
using OrderStatusAndroid = Com.Hypertrack.Sdk.Android.HyperTrack.OrderStatus;
using ClockInAndroid = Com.Hypertrack.Sdk.Android.HyperTrack.OrderStatus.ClockIn;
using JsonAndroid = Com.Hypertrack.Sdk.Android.Json;
using ResultAndroid = Com.Hypertrack.Sdk.Android.Result;
#endif

public static class Mapping
{
    internal static JsonAndroid FromJsonSharp(HyperTrack.Json jsonSharp)
    {
        switch (jsonSharp)
        {
            case HyperTrack.Json.Null:
                return JsonAndroid.Null.Instance;
            case HyperTrack.Json.Bool b:
                return new JsonAndroid.Bool(b.Value);
            case HyperTrack.Json.Number n:
                return new JsonAndroid.Number(n.Value);
            case HyperTrack.Json.String s:
                return new JsonAndroid.String(s.Value);
            case HyperTrack.Json.Array a:
                JsonAndroid[] arr = a.Items.Select(FromJsonSharp).ToArray();
                Java.Util.ArrayList list = new Java.Util.ArrayList(arr);
                return new JsonAndroid.Array(list);
            case HyperTrack.Json.Object o:
                return new JsonAndroid.Object(o.Fields.ToDictionary(
                    kvp => kvp.Key,
                    kvp => FromJsonSharp(kvp.Value)
                ));
            default:
                throw new InvalidOperationException("Invalid Json value");
        }
    }

    internal static HyperTrack.Result<T, E> FromResultAndroid<T, E>(ResultAndroid resultAndroid)
        where T : class
        where E : class
    {
        switch (resultAndroid)
        {
            case ResultAndroid.Success s:
                return HyperTrack.Result<T, E>.Ok(s.GetSuccess() as T);
            case ResultAndroid.Failure f:
                return HyperTrack.Result<T, E>.Error(f.GetFailure() as E);
            default:
                throw new InvalidOperationException("Invalid Result value");
        }
    }

    internal static HyperTrack.Location FromLocationAndroid(HyperTrackAndroid.Location locationAndroid)
    {
        return new HyperTrack.Location(locationAndroid.Latitude, locationAndroid.Longitude);
    }

    internal static HyperTrack.LocationError FromLocationErrorAndroid(HyperTrackAndroid.LocationError locationErrorAndroid)
    {
        return locationErrorAndroid switch
        {
            HyperTrackAndroid.LocationError.NotRunning _ => new HyperTrack.LocationError.NotRunning(),
            HyperTrackAndroid.LocationError.Starting _ => new HyperTrack.LocationError.Starting(),
            HyperTrackAndroid.LocationError.Errors errors => new HyperTrack.LocationError.Errors(
                new HashSet<HyperTrack.Error>(errors.GetErrors().Select(FromErrorAndroid))
            ),
            _ => throw new InvalidOperationException("Invalid LocationError value")
        };
    }

    private static HyperTrack.Error FromErrorAndroid(HyperTrackAndroid.Error errorAndroid)
    {
        return errorAndroid switch
        {
            HyperTrackAndroid.Error.BlockedFromRunning _ => new HyperTrack.Error.BlockedFromRunning(),
            HyperTrackAndroid.Error.InvalidPublishableKey _ => new HyperTrack.Error.InvalidPublishableKey(),
            HyperTrackAndroid.Error.Location.Mocked _ => new HyperTrack.Error.Location.Mocked(),
            HyperTrackAndroid.Error.Location.ServicesDisabled _ => new HyperTrack.Error.Location.ServicesDisabled(),
            HyperTrackAndroid.Error.Location.ServicesUnavailable _ => new HyperTrack.Error.Location.ServicesUnavailable(),
            HyperTrackAndroid.Error.Location.SignalLost _ => new HyperTrack.Error.Location.SignalLost(),
            HyperTrackAndroid.Error.NoExemptionFromBackgroundStartRestrictions _ => new HyperTrack.Error.NoExemptionFromBackgroundStartRestrictions(),
            HyperTrackAndroid.Error.Permissions.Location.Denied _ => new HyperTrack.Error.Permissions.Location.Denied(),
            HyperTrackAndroid.Error.Permissions.Location.InsufficientForBackground _ => new HyperTrack.Error.Permissions.Location.InsufficientForBackground(),
            HyperTrackAndroid.Error.Permissions.Location.ReducedAccuracy _ => new HyperTrack.Error.Permissions.Location.ReducedAccuracy(),
            _ => throw new InvalidOperationException("Invalid Error value")
        };
    }
}
