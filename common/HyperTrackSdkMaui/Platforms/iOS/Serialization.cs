namespace HyperTrack;

using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;

internal static class Serialization
{
    internal static Dictionary<string, object?> SerializeGeotagData(
        HyperTrack.Json.Object metadata,
        string orderHandle,
        HyperTrack.OrderStatus orderStatus
    )
    {
        return new Dictionary<string, object?>
        {
            { "metadata", metadata.ToDictionary() },
            { "orderHandle", SerializeOrderHandle(orderHandle) },
            { "orderStatus", SerializeOrderStatus(orderStatus) }
        };
    }

    public static Dictionary<string, object?> SerializeLocation(HyperTrack.Location location)
    {
        return new()
            {
                { KEY_TYPE, TYPE_LOCATION },
                { KEY_VALUE, new Dictionary<string, object?>
                    {
                        { KEY_LATITUDE, location.Latitude },
                        { KEY_LONGITUDE, location.Longitude }
                    }
                }
            };
    }

    private static HyperTrack.Location DeserializeLocation(Dictionary<string, object?> serialized)
    {
        if (serialized[KEY_TYPE] as string != TYPE_LOCATION)
        {
            throw new ArgumentException("Invalid Location type: " + serialized);
        }

        var value = serialized[KEY_VALUE] as Dictionary<string, object?>;
        if (value == null)
        {
            throw new ArgumentException("Invalid Location value: " + serialized);
        }

        double latitude = (double)value[KEY_LATITUDE]!;
        double longitude = (double)value[KEY_LONGITUDE]!;

        return new HyperTrack.Location(latitude, longitude);
    }
        

    private static Dictionary<string, object?> SerializeOrderHandle(string orderHandle)
    {
        return new Dictionary<string, object?>
        {
            { KEY_TYPE, TYPE_ORDER_HANDLE },
            { KEY_VALUE, orderHandle }
        };
    }

    private static Dictionary<string, object?> SerializeOrderStatus(HyperTrack.OrderStatus orderStatus)
    {
        return orderStatus switch
        {
            HyperTrack.OrderStatus.ClockIn => new Dictionary<string, object?>
                {
                    { KEY_TYPE, TYPE_GEOTAG_ORDER_STATUS_CLOCK_IN }
                },
            HyperTrack.OrderStatus.ClockOut => new Dictionary<string, object?>
                {
                    { KEY_TYPE, TYPE_GEOTAG_ORDER_STATUS_CLOCK_OUT }
                },
            HyperTrack.OrderStatus.Custom custom => new Dictionary<string, object?>
                {
                    { KEY_TYPE, TYPE_GEOTAG_ORDER_STATUS_CUSTOM },
                    { KEY_VALUE, custom.Value }
                },
            _ => throw new ArgumentException("Unknown order status type")
        };
    }

    private const string KEY_TYPE = "type";
    private const string KEY_VALUE = "value";

    private const string TYPE_ALLOW_MOCK_LOCATION = "allowMockLocation";
    private const string TYPE_DEVICE_ID = "deviceID";
    private const string TYPE_DYNAMIC_PUBLISHABLE_KEY = "dynamicPublishableKey";
    private const string TYPE_ERROR = "error";
    private const string TYPE_IS_AVAILABLE = "isAvailable";
    private const string TYPE_LOCATION = "location";

    private const string TYPE_ORDER_HANDLE = "orderHandle";
    private const string KEY_LATITUDE = "latitude";
    private const string KEY_LONGITUDE = "longitude";

    private const string TYPE_GEOTAG_ORDER_STATUS_CLOCK_IN = "orderStatusClockIn";
    private const string TYPE_GEOTAG_ORDER_STATUS_CLOCK_OUT = "orderStatusClockOut";
    private const string TYPE_GEOTAG_ORDER_STATUS_CUSTOM = "orderStatusCustom";
}
