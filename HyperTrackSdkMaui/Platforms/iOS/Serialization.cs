namespace HyperTrack;

using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;

// We don't use private modifier because it's way easier to navigate this file when you have only alphabetical order
// and we don't have the hassle to maintain the visibility consistency for all functions
internal static class Serialization
{
    internal static HyperTrack.Location DeserializeLocation(Dictionary<string, object?> serialized)
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

    internal static HyperTrack.Result<HyperTrack.Location, HyperTrack.LocationError> DeserializeLocationResult(Dictionary<string, object?> serialized)
    {
        var type = serialized[KEY_TYPE] as string;
        var value = serialized[KEY_VALUE] as Dictionary<string, object?>;

        if (value == null)
        {
            throw new ArgumentException("Invalid LocationResult value: " + serialized);
        }

        if (type == TYPE_RESULT_SUCCESS)
        {
            return HyperTrack.Result<HyperTrack.Location, HyperTrack.LocationError>.Ok(DeserializeLocation(value));
        }
        else if (type == TYPE_RESULT_FAILURE)
        {
            return HyperTrack.Result<HyperTrack.Location, HyperTrack.LocationError>.Error(DeserializeLocationError(value));
        }

        throw new ArgumentException("Invalid LocationResult type: " + type);
    }

    internal static HyperTrack.LocationError DeserializeLocationError(Dictionary<string, object?> serialized)
    {
        var type = serialized[KEY_TYPE] as string;

        return type switch
        {
            TYPE_LOCATION_ERROR_NOT_RUNNING => new HyperTrack.LocationError.NotRunning(),
            TYPE_LOCATION_ERROR_STARTING => new HyperTrack.LocationError.Starting(),
            TYPE_LOCATION_ERROR_ERRORS => new HyperTrack.LocationError.Errors(
                DeserializeErrors((serialized[KEY_VALUE] as List<Dictionary<string, object?>>)!)
            ),
            _ => throw new ArgumentException("Unknown location error type: " + type)
        };
    }

    internal static HashSet<HyperTrack.Error> DeserializeErrors(List<Dictionary<string, object?>> errors)
    {
        return new HashSet<HyperTrack.Error>(
            errors.Select(error => DeserializeError(error))
        );
    }

    internal static HyperTrack.Error DeserializeError(Dictionary<string, object?> serialized)
    {
        var value = serialized[KEY_VALUE] as string;
        if (value == null)
        {
            throw new ArgumentException("Invalid error value: " + serialized);
        }

        return value switch
        {
            "blockedFromRunning" => new HyperTrack.Error.BlockedFromRunning(),
            "invalidPublishableKey" => new HyperTrack.Error.InvalidPublishableKey(),
            "location.mocked" => new HyperTrack.Error.Location.Mocked(),
            "location.servicesDisabled" => new HyperTrack.Error.Location.ServicesDisabled(),
            "location.servicesUnavailable" => new HyperTrack.Error.Location.ServicesUnavailable(),
            "location.signalLost" => new HyperTrack.Error.Location.SignalLost(),
            "noExemptionFromBackgroundStartRestrictions" => new HyperTrack.Error.NoExemptionFromBackgroundStartRestrictions(),
            "permissions.location.denied" => new HyperTrack.Error.Permissions.Location.Denied(),
            "permissions.location.insufficientForBackground" => new HyperTrack.Error.Permissions.Location.InsufficientForBackground(),
            "permissions.location.reducedAccuracy" => new HyperTrack.Error.Permissions.Location.ReducedAccuracy(),
            "permissions.notifications.denied" => new HyperTrack.Error.Permissions.Notifications.Denied(),
            _ => throw new ArgumentException("Unknown error value: " + value)
        };
    }

    internal static Dictionary<string, object?> SerializeGeotagData(
        HyperTrack.Json.Object metadata,
        string orderHandle,
        HyperTrack.OrderStatus orderStatus
    )
    {
        return new Dictionary<string, object?>
        {
            { "data",  new Dictionary<string, object?>
            {
                { "metadata", metadata.ToDictionary() },
                { "orderHandle", SerializeOrderHandle(orderHandle) },
                { "orderStatus", SerializeOrderStatus(orderStatus) }
            }
            }
        };
    }

    internal static Dictionary<string, object?> SerializeLocation(HyperTrack.Location location)
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

    internal static Dictionary<string, object?> SerializeOrderHandle(string orderHandle)
    {
        return new Dictionary<string, object?>
        {
            { KEY_TYPE, TYPE_ORDER_HANDLE },
            { KEY_VALUE, orderHandle }
        };
    }

    internal static Dictionary<string, object?> SerializeOrderStatus(HyperTrack.OrderStatus orderStatus)
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

    private const string TYPE_RESULT_SUCCESS = "success";
    private const string TYPE_RESULT_FAILURE = "failure";
    private const string TYPE_LOCATION_ERROR_NOT_RUNNING = "notRunning";
    private const string TYPE_LOCATION_ERROR_STARTING = "starting";
    private const string TYPE_LOCATION_ERROR_ERRORS = "errors";
}
