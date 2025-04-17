# if ANDROID

namespace HyperTrack;

using HyperTrackAndroid = Com.Hypertrack.Sdk.Android.HyperTrack;
using JsonAndroid = Com.Hypertrack.Sdk.Android.Json;
using ResultAndroid = Com.Hypertrack.Sdk.Android.Result;

using System.Collections.Generic;
using Java.Util;
using Java.IO;

/// Using only internal access modifier to make it easier to change and maintain sorting
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
                // ReSharper disable once SuggestVarOrType_Elsewhere
                JsonAndroid[] arr = a.Items.Select(FromJsonSharp).ToArray();
                // ReSharper disable once RedundantNameQualifier
                // ReSharper disable once SuggestVarOrType_SimpleTypes
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
    
    internal static HyperTrack.Json FromJsonAndroid(JsonAndroid jsonAndroid)
    {
        switch (jsonAndroid)
        {
            case JsonAndroid.Null _:
                return new HyperTrack.Json.Null();
            case JsonAndroid.Bool b:
                return new HyperTrack.Json.Bool(b.Value);
            case JsonAndroid.Number n:
                return new HyperTrack.Json.Number(n.Value);
            case JsonAndroid.String s:
                return new HyperTrack.Json.String(s.Value);
            case JsonAndroid.Array a:
                var list = new List<HyperTrack.Json>();
                var iterator = a.Items.Iterator();
                while (iterator.HasNext)
                {
                    list.Add(FromJsonAndroid((iterator.Next() as JsonAndroid)!));
                }
                return new HyperTrack.Json.Array(list);
            case JsonAndroid.Object o:
                IDictionary<string, Com.Hypertrack.Sdk.Android.Json> fields = o.Fields;
                return new HyperTrack.Json.Object(fields.ToDictionary(
                    kvp => kvp.Key,
                    kvp => FromJsonAndroid(kvp.Value)
                ));
            default:
                throw new InvalidOperationException("Invalid Json value");
        }
    }

    internal static HyperTrack.Result<T, TE> FromResultAndroid<T, TE>(ResultAndroid resultAndroid)
        where T : class
        where TE : class
    {
        switch (resultAndroid)
        {
            case ResultAndroid.Success s:
                return HyperTrack.Result<T, TE>.Ok((s.GetSuccess() as T)!);
            case ResultAndroid.Failure f:
                return HyperTrack.Result<T, TE>.Error((f.GetFailure() as TE)!);
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
    
    internal static HyperTrackAndroid.Location FromLocationSharp(HyperTrack.Location locationSharp)
    {
        return new HyperTrackAndroid.Location(locationSharp.Latitude, locationSharp.Longitude);
    }
    
    internal static HyperTrack.LocationWithDeviation FromLocationWithDeviationAndroid(HyperTrackAndroid.LocationWithDeviation locationWithDeviationAndroid)
    {
        return new HyperTrack.LocationWithDeviation(
            new HyperTrack.Location(locationWithDeviationAndroid.Location.Latitude, locationWithDeviationAndroid.Location.Longitude),
            locationWithDeviationAndroid.Deviation
        );
    }

    internal static HashSet<HyperTrack.Error> FromErrorsAndroid(object? errorsAndroid)
    {
        switch (errorsAndroid)
        {
            case Android.Runtime.JavaSet errors:
                var result = new HashSet<HyperTrack.Error>();
                var enumerator = errors.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    // ReSharper disable once SuggestVarOrType_SimpleTypes
                    HyperTrackAndroid.Error current = (enumerator.Current as HyperTrackAndroid.Error)!;
                    result.Add(FromErrorAndroid(current));
                }
                (enumerator as IDisposable)?.Dispose();
                return result;
            default:
                throw new InvalidClassException(errorsAndroid?.GetType().ToString());
        }
    }

    internal static HyperTrack.Error FromErrorAndroid(HyperTrackAndroid.Error errorAndroid)
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

    internal static Dictionary<TK, TV> FromIMap<TK, TV>(IMap map)
        where TK : class
        where TV : class
    {
        var dictionary = new Dictionary<TK, TV>();
        foreach (var key in map.KeySet())
        {
            var javaKey = new Java.Lang.String(key!.ToString()!.ToCharArray());
            var value = map.Get(javaKey)!;
            dictionary.Add((TK)key, (value as TV)!);
        }
        return dictionary;
    }   
    
    internal static HyperTrackAndroid.OrderStatus FromOrderStatusSharp(HyperTrack.OrderStatus orderStatusSharp)
    {
        return orderStatusSharp switch
        {
            HyperTrack.OrderStatus.ClockIn _ => HyperTrackAndroid.OrderStatus.ClockIn.Instance,
            HyperTrack.OrderStatus.ClockOut _ => HyperTrackAndroid.OrderStatus.ClockOut.Instance,
            HyperTrack.OrderStatus.Custom custom => new HyperTrackAndroid.OrderStatus.Custom(custom.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(orderStatusSharp), orderStatusSharp, null)
        };
    }


}

# endif
