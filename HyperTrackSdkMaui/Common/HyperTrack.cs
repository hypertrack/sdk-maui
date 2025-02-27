// ReSharper disable CheckNamespace

namespace HyperTrack;

#if ANDROID
using HyperTrackAndroid = Com.Hypertrack.Sdk.Android.HyperTrack;
using OrderStatusAndroid = Com.Hypertrack.Sdk.Android.HyperTrack.OrderStatus;
using ClockInAndroid = Com.Hypertrack.Sdk.Android.HyperTrack.OrderStatus.ClockIn;
using JsonAndroid = Com.Hypertrack.Sdk.Android.Json;
using ResultAndroid = Com.Hypertrack.Sdk.Android.Result;
#endif

#if IOS
using Foundation;
using HyperTrackIos = binding_ios.HyperTrackMauiWrapper;
using Foundation;
#endif

public static partial class HyperTrack
{
    public static string DeviceId
    {
        get
        {
#if ANDROID
            return HyperTrackAndroid.DeviceID;
#endif
#if IOS
            var resultString = HyperTrackIos.GetDeviceId();
            var result = HyperTrack.Json.FromString(resultString)!.ToDictionary();
            return Serialization.DeserializeDeviceId(result);
#endif
        }
    }

    public static Dictionary<string, Order> Orders
    {
        get
        {
#if ANDROID
            HyperTrackAndroid.OrdersMap ordersMap = HyperTrackAndroid.Orders;
            var dict = Mapping.FromIMap<string, HyperTrackAndroid.Order>(ordersMap);

            return dict
                .Select(kvp => {
                    var orderAndroid = (HyperTrackAndroid.Order)kvp.Value;
                    var orderHandle = kvp.Key;
                    var isInsideGeofenceFunc =
 () => Mapping.FromResultAndroid<Java.Lang.Boolean, HyperTrackAndroid.LocationError>(orderAndroid.IsInsideGeofence())
                        .Map((Java.Lang.Boolean b) => b.BooleanValue())
                        .MapFailure(Mapping.FromLocationErrorAndroid);
                    return new KeyValuePair<string, Order>(
                        orderHandle,
                        new Order(orderHandle, isInsideGeofenceFunc)
                    );
                })
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
#endif
# if IOS
            var resultString = HyperTrackIos.GetOrders();
            var result = HyperTrack.Json.FromString(resultString)!.ToDictionary();
            return Serialization.DeserializeOrders(result);
#endif
        }
    }

    public static string WorkerHandle
    {
        get
        {
#if ANDROID
            return HyperTrackAndroid.WorkerHandle;
#endif
#if IOS
            var resultString = HyperTrackIos.GetWorkerHandle();
            var result = HyperTrack.Json.FromString(resultString)!.ToDictionary();
            return Serialization.DeserializeWorkerHandle(result);
#endif
        }
        set
        {
#if ANDROID
            HyperTrackAndroid.WorkerHandle = value;
#endif
#if IOS
            var serialized = Serialization.SerializeWorkerHandle(value);
            var stringParam = HyperTrack.Json.FromDictionary(serialized)!.ToString();
            HyperTrackIos.SetWorkerHandle(stringParam);
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
        var stringParam = HyperTrack.Json.FromDictionary(serialized)!.ToString();
        var resultString = HyperTrackIos.AddGeotag(stringParam);
        var result = HyperTrack.Json.FromString(resultString)!.ToDictionary();
        return Serialization.DeserializeLocationResult(result);
#endif
    }

    public interface ICancellable
    {
        void Cancel();
    }
#if ANDROID
    private class AndroidCancellable : ICancellable
    {

        private readonly Com.Hypertrack.Sdk.Android.HyperTrack.Cancellable _cancellable;

        public AndroidCancellable(Com.Hypertrack.Sdk.Android.HyperTrack.Cancellable cancellable)
        {
            _cancellable = cancellable;
        }

        public void Cancel()
        {
            _cancellable.Cancel();
        }

    }
#endif

    public static ICancellable SubscribeToOrders(Action<Dictionary<string, Order>> callback)
    {
#if ANDROID
        var androidCallback = new AndroidOrdersCallback((obj) => {
            var ordersMap = obj as Com.Hypertrack.Sdk.Android.HyperTrack.OrdersMap;
            if (ordersMap != null)
            {
                var dict = Mapping.FromIMap<string, HyperTrackAndroid.Order>(ordersMap);
                var orders = dict
                    .Select(kvp => {
                        var orderAndroid = (HyperTrackAndroid.Order)kvp.Value;
                        var orderHandle = kvp.Key;
                        var isInsideGeofenceFunc =
 () => Mapping.FromResultAndroid<Java.Lang.Boolean, HyperTrackAndroid.LocationError>(orderAndroid.IsInsideGeofence())
                            .Map((Java.Lang.Boolean b) => b.BooleanValue())
                            .MapFailure(Mapping.FromLocationErrorAndroid);
                        return new KeyValuePair<string, Order>(
                            orderHandle,
                            new Order(orderHandle, isInsideGeofenceFunc)
                        );
                    })
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                callback(orders);
            }
        });
        var cancellable = HyperTrackAndroid.SubscribeToOrders(androidCallback);
        return new AndroidCancellable(cancellable);
#endif
#if IOS
        var cancellable = HyperTrackIos.SubscribeToOrders((NSString resultString) =>
        {
            var result = HyperTrack.Json.FromString(resultString)!.ToDictionary();
            var orders = Serialization.DeserializeOrders(result);
            callback(orders);
        });
        return new IosCancellable(cancellable);
#endif
    }

    public static ICancellable SubscribeToIsTracking(Action<bool> callback)
    {
#if ANDROID
        var androidCallback = new AndroidBooleanCallback((obj) => {
            var isTracking = (bool)obj;
            callback(isTracking);
        });
        var cancellable = HyperTrackAndroid.SubscribeToTracking(androidCallback);
        return new AndroidCancellable(cancellable);
#endif
#if IOS
        var cancellable = HyperTrackIos.SubscribeToIsTracking((NSString resultString) =>
        {
            var result = HyperTrack.Json.FromString(resultString)!.ToDictionary();
            var isTracking = Serialization.DeserializeIsTracking(result);
            callback(isTracking);
        });
        return new IosCancellable(cancellable);
#endif
    }

    public static ICancellable SubscribeToIsAvailable(Action<bool> callback)
    {
#if ANDROID
        var androidCallback = new AndroidBooleanCallback((obj) => {
            var isAvailable = (bool)obj;
            callback(isAvailable);
        });
        var cancellable = HyperTrackAndroid.SubscribeToAvailability(androidCallback);
        return new AndroidCancellable(cancellable);
#endif
#if IOS
        var cancellable = HyperTrackIos.SubscribeToIsAvailable((NSString resultString) =>
        {
            var result = HyperTrack.Json.FromString(resultString)!.ToDictionary();
            var isAvailable = Serialization.DeserializeIsAvailable(result);
            callback(isAvailable);
        });
        return new IosCancellable(cancellable);
#endif
    }

    public static ICancellable SubscribeToErrors(Action<HashSet<Error>> callback)
    {
#if ANDROID
        var androidCallback = new AndroidErrorsCallback((obj) => {
            var errors = (HashSet<Error>)obj;
            callback(errors);
        });
        var cancellable = HyperTrackAndroid.SubscribeToErrors(androidCallback);
        return new AndroidCancellable(cancellable);
#endif
#if IOS
        var cancellable = HyperTrackIos.SubscribeToErrors((NSString resultString) =>
        {
            var result = HyperTrack.Json.FromString(resultString)!.ToDictionary();
            var errors = Serialization.DeserializeErrors(result);
            callback(errors);
        });
        return new IosCancellable(cancellable);
#endif
    }

    public static ICancellable SubscribeToLocation(Action<Result<Location, LocationError>> callback)
    {
#if ANDROID
        var androidCallback = new AndroidLocationCallback((obj) => {
            var result = (ResultAndroid)obj;
            var mappedResult = Mapping.FromResultAndroid<HyperTrackAndroid.Location, HyperTrackAndroid.LocationError>(result)
                .Map(Mapping.FromLocationAndroid)
                .MapFailure(Mapping.FromLocationErrorAndroid);
            callback(mappedResult);
        });
        var cancellable = HyperTrackAndroid.SubscribeToLocation(androidCallback);
        return new AndroidCancellable(cancellable);
#endif
#if IOS
        var cancellable = HyperTrackIos.SubscribeToLocation((NSString resultString) =>
        {
            var result = HyperTrack.Json.FromString(resultString)!.ToDictionary();
            var locationResult = Serialization.DeserializeLocationResult(result);
            callback(locationResult);
        });
        return new IosCancellable(cancellable);
#endif
    }

#if IOS
    private class IosCancellable : ICancellable
    {
        private readonly binding_ios.HyperTrackCancellable _cancellable;

        public IosCancellable(binding_ios.HyperTrackCancellable cancellable)
        {
            _cancellable = cancellable;
        }

        public void Cancel()
        {
            _cancellable.Cancel();
        }
    }
#endif

    public static bool AllowMockLocation
    {
        get
        {
#if ANDROID
            return HyperTrackAndroid.Instance.AllowMockLocation;
#endif
#if IOS
            var resultString = HyperTrackIos.GetAllowMockLocation();
            var result = HyperTrack.Json.FromString(resultString)!.ToDictionary();
            return Serialization.DeserializeAllowMockLocation(result);
#endif
        }
        set
        {
#if ANDROID
            HyperTrackAndroid.Instance.AllowMockLocation = value;
#endif
#if IOS
            var serialized = Serialization.SerializeAllowMockLocation(value);
            var stringParam = HyperTrack.Json.FromDictionary(serialized)!.ToString();
            HyperTrackIos.SetAllowMockLocation(stringParam);
#endif
        }
    }

    public static HashSet<Error> Errors
    {
        get
        {
#if ANDROID
            return new HashSet<Error>(HyperTrackAndroid.Errors.Select(Mapping.FromErrorAndroid));
#endif
#if IOS
            var resultString = HyperTrackIos.GetErrors();
            var result = HyperTrack.Json.FromString(resultString)!.ToDictionary();
            return Serialization.DeserializeErrors(result);
#endif
        }
    }

    public static bool IsAvailable
    {
        get
        {
#if ANDROID
            return HyperTrackAndroid.Available;
#endif
#if IOS
            var resultString = HyperTrackIos.GetIsAvailable();
            var result = HyperTrack.Json.FromString(resultString)!.ToDictionary();
            return Serialization.DeserializeIsAvailable(result);
#endif
        }
        set
        {
#if ANDROID
            HyperTrackAndroid.Available = value;
#endif
#if IOS
            var serialized = Serialization.SerializeIsAvailable(value);
            var stringParam = HyperTrack.Json.FromDictionary(serialized)!.ToString();
            HyperTrackIos.SetIsAvailable(stringParam);
#endif
        }
    }

    public static bool IsTracking
    {
        get
        {
#if ANDROID
            return HyperTrackAndroid.Tracking;
#endif
#if IOS
            var resultString = HyperTrackIos.GetIsTracking();
            var result = HyperTrack.Json.FromString(resultString)!.ToDictionary();
            return Serialization.DeserializeIsTracking(result);
#endif
        }
        set
        {
#if ANDROID
            HyperTrackAndroid.Tracking = value;
#endif
#if IOS
            var serialized = Serialization.SerializeIsTracking(value);
            var stringParam = HyperTrack.Json.FromDictionary(serialized)!.ToString();
            HyperTrackIos.SetIsTracking(stringParam);
#endif
        }
    }

    public static Result<Location, LocationError> GetLocation
    {
        get
        {
#if ANDROID
            return Mapping.FromResultAndroid<HyperTrackAndroid.Location, HyperTrackAndroid.LocationError>(HyperTrackAndroid.GetLocation())
                .Map(Mapping.FromLocationAndroid)
                .MapFailure(Mapping.FromLocationErrorAndroid);
#endif
#if IOS
            var resultString = HyperTrackIos.GetLocation();
            var result = HyperTrack.Json.FromString(resultString)!.ToDictionary();
            return Serialization.DeserializeLocationResult(result);
#endif
        }
    }

    public static Json.Object Metadata
    {
        get
        {
#if ANDROID
            return Mapping.FromJsonAndroid(HyperTrackAndroid.Metadata) as Json.Object;
#endif
#if IOS
            var resultString = HyperTrackIos.GetMetadata();
            var result = HyperTrack.Json.FromString(resultString)!.ToDictionary();
            return Serialization.DeserializeMetadata(result)!;
#endif
        }
        set
        {
#if ANDROID
            HyperTrackAndroid.Metadata = Mapping.FromJsonSharp(value) as JsonAndroid.Object;
#endif
#if IOS
            var serialized = Serialization.SerializeMetadata(value);
            var stringParam = HyperTrack.Json.FromDictionary(serialized)!.ToString();
            HyperTrackIos.SetMetadata(stringParam);
#endif
        }
    }

    public static string Name
    {
        get
        {
#if ANDROID
            return HyperTrackAndroid.Name;
#endif
#if IOS
            var resultString = HyperTrackIos.GetName();
            var result = HyperTrack.Json.FromString(resultString)!.ToDictionary();
            return Serialization.DeserializeName(result);
#endif
        }
        set
        {
#if ANDROID
            HyperTrackAndroid.Name = value;
#endif
#if IOS
            var serialized = Serialization.SerializeName(value);
            var stringParam = HyperTrack.Json.FromDictionary(serialized)!.ToString();
            HyperTrackIos.SetName(stringParam);
#endif
        }
    }
}

#if ANDROID
class AndroidOrdersCallback : Java.Lang.Object, global::Kotlin.Jvm.Functions.IFunction1
{
    private readonly Action<Java.Lang.Object> _callback;

    public AndroidOrdersCallback(Action<Java.Lang.Object> callback)
    {
        _callback = callback;
    }

    public Java.Lang.Object? Invoke(Java.Lang.Object? p0)
    {
        _callback(p0!);
        return null;
    }
}

class AndroidLocationCallback : Java.Lang.Object, global::Kotlin.Jvm.Functions.IFunction1
{
    private readonly Action<Java.Lang.Object> _callback;

    public AndroidLocationCallback(Action<Java.Lang.Object> callback)
    {
        _callback = callback;
    }

    public Java.Lang.Object? Invoke(Java.Lang.Object? p0)
    {
        _callback(p0!);
        return null;
    }
}
#endif
