// ReSharper disable CheckNamespace

namespace HyperTrack
{
    partial class HyperTrack
    {
        public class GeotagData(
            Dictionary<string, object> data,
            Location? expectedLocation,
            string? orderHandle,
            HyperTrack.OrderStatus? orderStatus)
        {
            public Dictionary<string, object> Data { get; } = data;
            public Location? ExpectedLocation { get; } = expectedLocation;
            public string? OrderHandle { get; } = orderHandle;
            public HyperTrack.OrderStatus? OrderStatus { get; } = orderStatus;
        }
    }
}
