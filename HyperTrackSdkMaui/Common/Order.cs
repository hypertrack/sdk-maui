// ReSharper disable CheckNamespace

namespace HyperTrack
{
    static partial class HyperTrack
    {
        public class Order
        {
            public string OrderHandle { get; }
            private readonly Func<HyperTrack.Result<bool, HyperTrack.LocationError>> _isInsideGeofenceFunc;

            public Order(string orderHandle, Func<HyperTrack.Result<bool, HyperTrack.LocationError>> isInsideGeofenceFunc)
            {
                OrderHandle = orderHandle;
                _isInsideGeofenceFunc = isInsideGeofenceFunc;
            }

            public HyperTrack.Result<bool, HyperTrack.LocationError> IsInsideGeofence
            {
                get { return _isInsideGeofenceFunc(); }
            }

            public override string ToString()
            {
                return $"Order({OrderHandle})";
            }
        }
    }
}
