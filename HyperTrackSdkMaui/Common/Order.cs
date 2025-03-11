// ReSharper disable CheckNamespace

namespace HyperTrack
{
    static partial class HyperTrack
    {
        public class Order : IEquatable<Order>, IComparable, IComparable<Order>
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

            public bool Equals(Order? other)
            {
                if (other is null) return false;
                return OrderHandle == other.OrderHandle;
            }

            public override bool Equals(object? obj) => Equals(obj as Order);
            public override int GetHashCode() => OrderHandle.GetHashCode();
            public static bool operator ==(Order? left, Order? right) => left?.Equals(right) ?? right is null;
            public static bool operator !=(Order? left, Order? right) => !(left == right);

            public int CompareTo(Order? other)
            {
                if (other is null) return 1;
                return string.Compare(OrderHandle, other.OrderHandle, StringComparison.Ordinal);
            }

            public int CompareTo(object? obj)
            {
                if (obj is null) return 1;
                if (obj is Order other) return CompareTo(other);
                throw new ArgumentException("Object is not an Order");
            }
        }
    }
}
