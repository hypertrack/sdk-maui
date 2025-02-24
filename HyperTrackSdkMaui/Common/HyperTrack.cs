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
            return HyperTrackIos.DeviceId;
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
                    var isInsideGeofenceFunc = () => Mapping.FromResultAndroid<Java.Lang.Boolean, HyperTrackAndroid.LocationError>(orderAndroid.IsInsideGeofence())
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
            var resultString = HyperTrackIos.Orders;
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
            var resultString = HyperTrackIos.WorkerHandle;
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
                        var isInsideGeofenceFunc = () => Mapping.FromResultAndroid<Java.Lang.Boolean, HyperTrackAndroid.LocationError>(orderAndroid.IsInsideGeofence())
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
        var cancellable = HyperTrackIos.SubscribeToOrders((NSString resultString) => {
            var result = HyperTrack.Json.FromString(resultString)!.ToDictionary();
            var orders = Serialization.DeserializeOrders(result);
            callback(orders);
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
#endif
