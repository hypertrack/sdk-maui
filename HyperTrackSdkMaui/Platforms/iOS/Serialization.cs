// ReSharper disable once CheckNamespace
namespace HyperTrack;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

// We don't use private modifier because it's way easier to navigate this file when you have only alphabetical order,
// and we don't have the hassle to maintain the visibility consistency for all functions
internal static class Serialization
{
    internal static bool DeserializeAllowMockLocation(Dictionary<string, object?> result)
    {
        return DeserializeSimpleValueBoolean(result);
    }

    internal static string DeserializeDeviceId(Dictionary<string, object?> result)
    {
        return DeserializeSimpleValueString(result);
    }

    internal static string DeserializeDynamicPublishableKey(Dictionary<string, object?> result)
    {
        return DeserializeSimpleValueString(result);
    }

    internal static HyperTrack.Error DeserializeError(Dictionary<string, object?> serialized)
    {
        if (serialized[KeyValue] is not string value)
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
            "noExemptionFromBackgroundStartRestrictions" =>
                new HyperTrack.Error.NoExemptionFromBackgroundStartRestrictions(),
            "permissions.location.denied" => new HyperTrack.Error.Permissions.Location.Denied(),
            "permissions.location.insufficientForBackground" =>
                new HyperTrack.Error.Permissions.Location.InsufficientForBackground(),
            "permissions.location.notDetermined" => new HyperTrack.Error.Permissions.Location.NotDetermined(),
            "permissions.location.provisional" => new HyperTrack.Error.Permissions.Location.Provisional(),
            "permissions.location.reducedAccuracy" => new HyperTrack.Error.Permissions.Location.ReducedAccuracy(),
            "permissions.location.restricted" => new HyperTrack.Error.Permissions.Location.Restricted(),
            "permissions.notifications.denied" => new HyperTrack.Error.Permissions.Notifications.Denied(),
            _ => throw new ArgumentException("Unknown error value: " + value)
        };
    }

    [SuppressMessage("ReSharper", "UseCollectionExpression")]
    internal static HashSet<HyperTrack.Error> DeserializeErrors(Dictionary<string, object?> serialized)
    {
        if (serialized[KeyType] as string != "errors")
        {
            throw new ArgumentException("Invalid errors type: " + serialized);
        }

        var errors = serialized[KeyValue] as List<object?>;
        if (errors == null)
        {
            throw new ArgumentException("Invalid errors value: " + serialized);
        }

        return new HashSet<HyperTrack.Error>(
            errors.Select(error =>
            {
                if (error is not Dictionary<string, object?> errorMap)
                {
                    throw new ArgumentException("Invalid error value: " + error);
                }
                return DeserializeError(errorMap);
            })
        );
    }

    internal static bool DeserializeIsAvailable(Dictionary<string, object?> result)
    {
        return DeserializeSimpleValueBoolean(result);
    }

    internal static bool DeserializeIsTracking(Dictionary<string, object?> result)
    {
        return DeserializeSimpleValueBoolean(result);
    }

    internal static HyperTrack.Result<HyperTrack.Location, HashSet<HyperTrack.Error>> DeserializeLocateResult(
        Dictionary<string, object?> serialized)
    {
        var type = serialized[KeyType] as string;
        var value = serialized[KeyValue] as Dictionary<string, object?>;

        if (value == null)
        {
            throw new ArgumentException("Invalid LocateResult value: " + serialized);
        }

        if (type == TypeResultSuccess)
        {
            return HyperTrack.Result<HyperTrack.Location, HashSet<HyperTrack.Error>>.Ok(DeserializeLocation(value));
        }
        else if (type == TypeResultFailure)
        {
            return HyperTrack.Result<HyperTrack.Location, HashSet<HyperTrack.Error>>.Error(DeserializeErrors(value));
        }

        throw new ArgumentException("Invalid LocateResult type: " + type);
    }

    internal static HyperTrack.Location DeserializeLocation(Dictionary<string, object?> serialized)
    {
        var latitude = (double)serialized[KeyLatitude]!;
        var longitude = (double)serialized[KeyLongitude]!;
        return new HyperTrack.Location(latitude, longitude);
    }

    internal static HyperTrack.LocationError DeserializeLocationError(Dictionary<string, object?> serialized)
    {
        var type = serialized[KeyType] as string;

        return type switch
        {
            TypeLocationErrorNotRunning => new HyperTrack.LocationError.NotRunning(),
            TypeLocationErrorStarting => new HyperTrack.LocationError.Starting(),
            TypeLocationErrorErrors => new HyperTrack.LocationError.Errors(
                DeserializeErrors((Dictionary<string, object?>)serialized[KeyValue]!)
            ),
            _ => throw new ArgumentException("Unknown location error type: " + type)
        };
    }

    internal static HyperTrack.Result<HyperTrack.Location, HyperTrack.LocationError> DeserializeLocationResult(
        Dictionary<string, object?> serialized)
    {
        var type = serialized[KeyType] as string;
        var value = serialized[KeyValue] as Dictionary<string, object?>;

        if (value == null)
        {
            throw new ArgumentException("Invalid LocationResult value: " + serialized);
        }

        if (type == TypeResultSuccess)
        {
            return HyperTrack.Result<HyperTrack.Location, HyperTrack.LocationError>.Ok(DeserializeLocation(value));
        }
        else if (type == TypeResultFailure)
        {
            return HyperTrack.Result<HyperTrack.Location, HyperTrack.LocationError>.Error(
                DeserializeLocationError(value));
        }

        throw new ArgumentException("Invalid LocationResult type: " + type);
    }

    internal static HyperTrack.LocationWithDeviation DeserializeLocationWithDeviation(Dictionary<string, object?> serialized)
    {
        var latitude = (double)serialized[KeyLatitude]!;
        var longitude = (double)serialized[KeyLongitude]!;
        var deviation = (double)serialized[KeyDeviation]!;

        return new HyperTrack.LocationWithDeviation(
            new HyperTrack.Location(latitude, longitude),
            deviation
        );
    }

    internal static HyperTrack.Result<HyperTrack.LocationWithDeviation, HyperTrack.LocationError> DeserializeLocationWithDeviationResult(
        Dictionary<string, object?> serialized)
    {
        var type = serialized[KeyType] as string;
        var value = serialized[KeyValue] as Dictionary<string, object?>;

        if (value == null)
        {
            throw new ArgumentException("Invalid LocationWithDeviationResult value: " + serialized);
        }

        if (type == TypeResultSuccess)
        {
            return HyperTrack.Result<HyperTrack.LocationWithDeviation, HyperTrack.LocationError>.Ok(
                DeserializeLocationWithDeviation(value));
        }
        else if (type == TypeResultFailure)
        {
            return HyperTrack.Result<HyperTrack.LocationWithDeviation, HyperTrack.LocationError>.Error(
                DeserializeLocationError(value));
        }

        throw new ArgumentException("Invalid LocationWithDeviationResult type: " + type);
    }

    internal static HyperTrack.Json.Object DeserializeMetadata(Dictionary<string, object?> serialized)
    {
        if (serialized[KeyType] as string != TypeMetadata)
        {
            throw new ArgumentException("Invalid metadata type: " + serialized);
        }
        if (serialized[KeyValue] is not Dictionary<string, object?> metadata)
        {
            throw new ArgumentException("Invalid metadata value: " + serialized);
        }
        return HyperTrack.Json.FromDictionary(metadata)!;
    }

    internal static string DeserializeName(Dictionary<string, object?> result)
    {
        return DeserializeSimpleValueString(result);
    }

    internal static Dictionary<string, HyperTrack.Order> DeserializeOrders(
    Dictionary<string, object?> result,
    Func<string, HyperTrack.Result<bool, HyperTrack.LocationError>> isInsideGeofenceFunc
)
    {
        if (result[KeyType] as string != TypeOrders)
        {
            throw new ArgumentException("Invalid orders type: " + result);
        }

        if (result[KeyValue] is not List<object?> orders)
        {
            throw new ArgumentException("Invalid orders value: " + result[KeyValue]);
        }

        return orders
            .Select((serialized, index) =>
            {
                if (serialized is not Dictionary<string, object?> orderMap)
                {
                    throw new ArgumentException("Invalid order value: " + serialized);
                }
                string orderHandle = (string)(orderMap[KeyOrderHandle])!;
                return new HyperTrack.Order(
                    orderHandle,
                    () => isInsideGeofenceFunc(orderHandle)
                );
            })
            .ToDictionary(order => order.OrderHandle);
    }

    internal static bool DeserializeSimpleValueBoolean(Dictionary<string, object?> serialized)
    {
        return (bool)serialized[KeyValue]!;
    }

    internal static double DeserializeSimpleValueFloat(Dictionary<string, object?> serialized)
    {
        return (double)serialized[KeyValue]!;
    }

    internal static string DeserializeSimpleValueString(Dictionary<string, object?> serialized)
    {
        return (string)serialized[KeyValue]!;
    }

    internal static string DeserializeWorkerHandle(Dictionary<string, object?> result)
    {
        return DeserializeSimpleValueString(result);
    }

    internal static HyperTrack.Result<bool, HyperTrack.LocationError> DeserializeIsInsideGeofence(
        Dictionary<string, object?> serialized)
    {
        var type = serialized[KeyType] as string;
        var value = serialized[KeyValue];

        if (type == TypeResultSuccess)
        {
            if (value is not bool boolValue)
            {
                throw new ArgumentException("Invalid IsInsideGeofence success value: " + serialized);
            }
            return HyperTrack.Result<bool, HyperTrack.LocationError>.Ok(boolValue);
        }
        else if (type == TypeResultFailure)
        {
            if (value is not Dictionary<string, object?> errorValue)
            {
                throw new ArgumentException("Invalid IsInsideGeofence error value: " + serialized);
            }
            return HyperTrack.Result<bool, HyperTrack.LocationError>.Error(
                DeserializeLocationError(errorValue));
        }

        throw new ArgumentException("Invalid IsInsideGeofence type: " + type);
    }

    internal static Dictionary<string, object?> SerializeAllowMockLocation(bool value)
    {
        return SerializeSimpleValue(value);
    }

    internal static Dictionary<string, object?> SerializeDynamicPublishableKey(string value)
    {
        return SerializeSimpleValue(value);
    }

    internal static Dictionary<string, object?> SerializeGeotagData(
        HyperTrack.Json.Object metadata,
        string orderHandle,
        HyperTrack.OrderStatus orderStatus,
        HyperTrack.Location? expectedLocation
    )
    {
        var result = new Dictionary<string, object?>
        {
            { KeyMetadata, metadata.ToDictionary() },
            { KeyOrderHandle, orderHandle },
            { KeyOrderStatus, SerializeOrderStatus(orderStatus) }
        };
        if (expectedLocation != null)
        {
            result.Add(KeyExpectedLocation, SerializeLocation(expectedLocation));
        }
        return result;
    }

    internal static Dictionary<string, object?> SerializeIsAvailable(bool isAvailable)
    {
        return SerializeSimpleValue(isAvailable);
    }

    internal static Dictionary<string, object?> SerializeIsTracking(bool isTracking)
    {
        return SerializeSimpleValue(isTracking);
    }

    internal static Dictionary<string, object?> SerializeLocation(HyperTrack.Location location)
    {
        return new Dictionary<string, object?>
        {
            { KeyLatitude, location.Latitude },
            { KeyLongitude, location.Longitude }
        };
    }

    internal static Dictionary<string, object?> SerializeMetadata(HyperTrack.Json.Object metadata)
    {
        return new Dictionary<string, object?>
        {
            { KeyType, TypeMetadata },
            { KeyValue, metadata.ToDictionary() }
        };
    }

    internal static Dictionary<string, object?> SerializeName(string name)
    {
        return SerializeSimpleValue(name);
    }

    internal static Dictionary<string, object?> SerializeOrderStatus(HyperTrack.OrderStatus orderStatus)
    {
        return orderStatus switch
        {
            HyperTrack.OrderStatus.ClockIn => new Dictionary<string, object?>
            {
                { KeyType, TypeGeotagOrderStatusClockIn }
            },
            HyperTrack.OrderStatus.ClockOut => new Dictionary<string, object?>
            {
                { KeyType, TypeGeotagOrderStatusClockOut }
            },
            HyperTrack.OrderStatus.Custom custom => new Dictionary<string, object?>
            {
                { KeyType, TypeGeotagOrderStatusCustom },
                { KeyValue, custom.Value }
            },
            _ => throw new ArgumentException("Unknown order status type")
        };
    }

    internal static Dictionary<string, object?> SerializeSimpleValue(string value)
    {
        return new Dictionary<string, object?>
        {
            { KeyValue, value }
        };
    }

    internal static Dictionary<string, object?> SerializeSimpleValue(double value)
    {
        return new Dictionary<string, object?>
        {
            { KeyValue, value }
        };
    }

    internal static Dictionary<string, object?> SerializeSimpleValue(bool value)
    {
        return new Dictionary<string, object?>
        {
            { KeyValue, value }
        };
    }

    internal static Dictionary<string, object?> SerializeWorkerHandle(string workerHandle)
    {
        return SerializeSimpleValue(workerHandle);
    }

    private const string KeyType = "type";
    private const string KeyValue = "value";
    private const string KeyLatitude = "latitude";
    private const string KeyLongitude = "longitude";
    private const string KeyOrderHandle = "orderHandle";
    private const string KeyDeviation = "deviation";
    private const string KeyMetadata = "metadata";
    private const string KeyExpectedLocation = "expectedLocation";
    private const string KeyOrderStatus = "orderStatus";

    private const string TypeOrders = "orders";
    private const string TypeMetadata = "metadata";

    private const string TypeGeotagOrderStatusClockIn = "orderStatusClockIn";
    private const string TypeGeotagOrderStatusClockOut = "orderStatusClockOut";
    private const string TypeGeotagOrderStatusCustom = "orderStatusCustom";

    private const string TypeLocationErrorNotRunning = "notRunning";
    private const string TypeLocationErrorStarting = "starting";
    private const string TypeLocationErrorErrors = "errors";

    private const string TypeResultSuccess = "success";
    private const string TypeResultFailure = "failure";
}
