using System.Diagnostics.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace HyperTrack;

using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;

// We don't use private modifier because it's way easier to navigate this file when you have only alphabetical order,
// and we don't have the hassle to maintain the visibility consistency for all functions
internal static class Serialization
{
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
            "permissions.location.reducedAccuracy" => new HyperTrack.Error.Permissions.Location.ReducedAccuracy(),
            "permissions.notifications.denied" => new HyperTrack.Error.Permissions.Notifications.Denied(),
            _ => throw new ArgumentException("Unknown error value: " + value)
        };
    }

    [SuppressMessage("ReSharper", "UseCollectionExpression")]
    internal static HashSet<HyperTrack.Error> DeserializeErrors(List<Dictionary<string, object?>> errors)
    {
        return new HashSet<HyperTrack.Error>(
            errors.Select(DeserializeError)
        );
    }

    internal static HyperTrack.Location DeserializeLocation(Dictionary<string, object?> serialized)
    {
        if (serialized[KeyType] as string != TypeLocation)
        {
            throw new ArgumentException("Invalid Location type: " + serialized);
        }

        if (serialized[KeyValue] is not Dictionary<string, object?> value)
        {
            throw new ArgumentException("Invalid Location value: " + serialized);
        }

        var latitude = (double)value[KeyLatitude]!;
        var longitude = (double)value[KeyLongitude]!;

        return new HyperTrack.Location(latitude, longitude);
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

    internal static HyperTrack.LocationError DeserializeLocationError(Dictionary<string, object?> serialized)
    {
        var type = serialized[KeyType] as string;

        return type switch
        {
            TypeLocationErrorNotRunning => new HyperTrack.LocationError.NotRunning(),
            TypeLocationErrorStarting => new HyperTrack.LocationError.Starting(),
            TypeLocationErrorErrors => new HyperTrack.LocationError.Errors(
                DeserializeErrors((serialized[KeyValue] as List<Dictionary<string, object?>>)!)
            ),
            _ => throw new ArgumentException("Unknown location error type: " + type)
        };
    }

    public static Dictionary<string, HyperTrack.Order> DeserializeOrders(Dictionary<string, object?> result)
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
                return new HyperTrack.Order(
                    (string)(orderMap[KeyOrderHandle])!,
                    () => HyperTrack.Result<bool, HyperTrack.LocationError>.Ok(false)
                );
            })
            .ToDictionary(order => order.OrderHandle);
    }

    public static string DeserializeWorkerHandle(Dictionary<string, object?> result)
    {
        if (result[KeyType] as string != TypeWorkerHandle)
        {
            throw new ArgumentException("Invalid worker handle type: " + result);
        }
        
        return (string)result[KeyValue]!;
    }
    
    internal static Dictionary<string, object?> SerializeGeotagData(
        HyperTrack.Json.Object metadata,
        string orderHandle,
        HyperTrack.OrderStatus orderStatus
    )
    {
        return new Dictionary<string, object?>
        {
            {
                "data", new Dictionary<string, object?>
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
            { KeyType, TypeLocation },
            {
                KeyValue, new Dictionary<string, object?>
                {
                    { KeyLatitude, location.Latitude },
                    { KeyLongitude, location.Longitude }
                }
            }
        };
    }

    internal static Dictionary<string, object?> SerializeOrderHandle(string orderHandle)
    {
        return new Dictionary<string, object?>
        {
            { KeyType, TypeOrderHandle },
            { KeyValue, orderHandle }
        };
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
    
    internal static Dictionary<string, object?> SerializeWorkerHandle(string workerHandle)
    {
        return new Dictionary<string, object?>
        {
            { KeyType, TypeWorkerHandle },
            { KeyValue, workerHandle }
        };
    }

    private const string KeyType = "type";
    private const string KeyValue = "value";

    private const string KeyLatitude = "latitude";
    private const string KeyLongitude = "longitude";
    private const string KeyOrderHandle = "orderHandle";

    private const string TypeAllowMockLocation = "allowMockLocation";
    private const string TypeDeviceId = "deviceID";
    private const string TypeDynamicPublishableKey = "dynamicPublishableKey";
    private const string TypeError = "error";
    private const string TypeIsAvailable = "isAvailable";
    private const string TypeLocation = "location";
    private const string TypeOrders = "orders";
    private const string TypeOrderHandle = "orderHandle";
    private const string TypeWorkerHandle = "workerHandle";

    private const string TypeGeotagOrderStatusClockIn = "orderStatusClockIn";
    private const string TypeGeotagOrderStatusClockOut = "orderStatusClockOut";
    private const string TypeGeotagOrderStatusCustom = "orderStatusCustom";
    
    private const string TypeLocationErrorNotRunning = "notRunning";
    private const string TypeLocationErrorStarting = "starting";
    private const string TypeLocationErrorErrors = "errors";
    
    private const string TypeResultSuccess = "success";
    private const string TypeResultFailure = "failure";
}
