import HyperTrack

private let keyMetadata = "metadata"
private let keyType = "type"
private let keyValue = "value"
private let keyLatitude = "latitude"
private let keyLongitude = "longitude"
private let keyOrderHandle = "orderHandle"
private let keyOrderStatus = "orderStatus"
private let keyExpectedLocation = "expectedLocation"
private let keyDeviation = "deviation"

private let typeMetadata = "metadata"
private let typeSuccess = "success"
private let typeFailure = "failure"

extension String: @retroactive Error {}

func deserializeGeotagData(
    _ args: [String: Any]
) -> Result<GeotagData, String> {
    guard let data = args[keyMetadata] as? [String: Any] else {
        return .failure(getParseError(args, key: keyMetadata))
    }
    let orderHandle = args[keyOrderHandle] as! String

    let orderStatusData = args[keyOrderStatus] as! [String: Any]
    let orderStatusResult = deserializeOrderStatus(orderStatusData)
    if case let .failure(error) = orderStatusResult {
        return .failure(error)
    }
    let orderStatus: HyperTrack.OrderStatus? = if case let .success(orderStatus) = orderStatusResult {
        orderStatus
    } else {
        nil
    }

    let expectedLocationData = args[keyExpectedLocation] as? [String: Any]
    let expectedLocation: HyperTrack.Location?
    if let expectedLocationData = expectedLocationData {
        let expectedLocationResult = deserializeLocation(expectedLocationData)
        switch expectedLocationResult {
        case let .failure(error):
            return .failure(error)
        case let .success(expectedLocationValue):
            expectedLocation = expectedLocationValue
        }
    } else {
        expectedLocation = nil
    }

    return .success(GeotagData(
        data: data,
        expectedLocation: expectedLocation,
        orderHandle: orderHandle,
        orderStatus: orderStatus
    ))
}

func deserializeLocation(_ dict: [String: Any]) -> Result<HyperTrack.Location, String> {
    guard let latitude = dict[keyLatitude] as? Double else {
        return .failure(getParseError(dict, key: keyLatitude))
    }
    guard let longitude = dict[keyLongitude] as? Double else {
        return .failure(getParseError(dict, key: keyLongitude))
    }
    return .success(HyperTrack.Location(
        latitude: latitude,
        longitude: longitude
    ))
}

func deserializeMetadata(_ dict: [String: Any]) -> Result<HyperTrack.JSON.Object, String> {
    if dict[keyType] as? String != typeMetadata {
        return .failure("Invalid type value: expected '\(typeMetadata)', got '\(String(describing: dict[keyType]))'")
    }
    guard let value = dict[keyValue] as? [String: Any] else {
        return .failure(getParseError(dict, key: keyValue))
    }
    return .success(toJSON(value)!)
}

func deserializeOrderStatus(_ dict: [String: Any]) -> Result<HyperTrack.OrderStatus, String> {
    guard let type = dict[keyType] as? String else {
        return .failure(getParseError(dict, key: keyType))
    }
    switch type {
    case "orderStatusClockIn":
        return .success(.clockIn)
    case "orderStatusClockOut":
        return .success(.clockOut)
    case "orderStatusCustom":
        guard let value = dict[keyValue] as? String else {
            return .failure(getParseError(dict, key: keyValue))
        }
        return .success(.custom(value))
    default:
        return .failure("Unknown order status type: \(type)")
    }
}

func deserializeSimpleValueBoolean(_ dict: [String: Any]) -> Result<Bool, String> {
    guard let value = dict[keyValue] as? Bool else {
        return .failure(getParseError(dict, key: keyValue))
    }
    return .success(value)
}

func deserializeSimpleValueFloat(_ dict: [String: Any]) -> Result<Double, String> {
    guard let value = dict[keyValue] as? Double else {
        return .failure(getParseError(dict, key: keyValue))
    }
    return .success(value)
}

func deserializeSimpleValueString(_ dict: [String: Any]) -> Result<String, String> {
    guard let value = dict[keyValue] as? String else {
        return .failure(getParseError(dict, key: keyValue))
    }
    return .success(value)
}

func serializeError(_ error: HyperTrack.Error) -> [String: Any] {
    let value: String
    switch error {
    case .blockedFromRunning:
        value = "blockedFromRunning"
    case .invalidPublishableKey:
        value = "invalidPublishableKey"
    case let .location(location):
        switch location {
        case .mocked:
            value = "location.mocked"
        case .servicesDisabled:
            value = "location.servicesDisabled"
        case .signalLost:
            value = "location.signalLost"
        }
    case let .permissions(permissionType):
        switch permissionType {
        case let .location(permission):
            switch permission {
            case .denied:
                value = "permissions.location.denied"
            case .insufficientForBackground:
                value = "permissions.location.insufficientForBackground"
            case .notDetermined:
                value = "permissions.location.notDetermined"
            case .provisional:
                value = "permissions.location.provisional"
            case .reducedAccuracy:
                value = "permissions.location.reducedAccuracy"
            case .restricted:
                value = "permissions.location.restricted"
            }
        }
    }

    return [
        keyValue: value,
    ]
}

func serializeErrors(_ errors: Set<HyperTrack.Error>) -> [String: Any] {
    return [
        keyType: "errors",
        keyValue: errors.map { error in
            serializeError(error)
        },
    ]
}

func serializeIsInsideGeofence(_ isInsideGeofence: Result<Bool, HyperTrack.Location.Error>) -> [String: Any] {
    switch isInsideGeofence {
    case let .success(success):
        return [
            keyType: typeSuccess,
            keyValue: success,
        ]
    case let .failure(failure):
        return [
            keyType: typeFailure,
            keyValue: serializeLocationError(failure),
        ]
    }
}

func serializeLocateResult(_ result: Result<HyperTrack.Location, Set<HyperTrack.Error>>) -> [String: Any] {
    switch result {
    case let .success(success):
        return [
            keyType: typeSuccess,
            keyValue: serializeLocation(success),
        ]
    case let .failure(failure):
        return [
            keyType: typeFailure,
            keyValue: serializeErrors(Set(failure)),
        ]
    }
}

func serializeLocation(_ location: HyperTrack.Location) -> [String: Any] {
    return [
        keyLatitude: location.latitude,
        keyLongitude: location.longitude,
    ]
}

func serializeLocationError(_ error: HyperTrack.Location.Error) -> [String: Any] {
    switch error {
    case .starting:
        return [
            keyType: "starting",
        ]
    case .notRunning:
        return [
            keyType: "notRunning",
        ]
    case let .errors(errors):
        return [
            keyType: "errors",
            keyValue: serializeErrors(errors),
        ]
    }
}

func serializeLocationResult(_ result: Result<HyperTrack.Location, HyperTrack.Location.Error>) -> [String: Any] {
    switch result {
    case let .success(success):
        return [
            keyType: typeSuccess,
            keyValue: serializeLocation(success),
        ]
    case let .failure(failure):
        return [
            keyType: typeFailure,
            keyValue: serializeLocationError(failure),
        ]
    }
}

func serializeLocationWithDeviation(_ location: HyperTrack.LocationWithDeviation) -> [String: Any] {
    return [
        keyLatitude: location.location.latitude,
        keyLongitude: location.location.longitude,
        keyDeviation: location.deviation,
    ]
}

func serializeLocationWithDeviationResult(
    _ result: Result<HyperTrack.LocationWithDeviation, HyperTrack.Location.Error>
) -> [String: Any] {
    switch result {
    case let .success(success):
        return [
            keyType: typeSuccess,
            keyValue: serializeLocationWithDeviation(success),
        ]
    case let .failure(failure):
        return [
            keyType: typeFailure,
            keyValue: serializeLocationError(failure),
        ]
    }
}

func serializeMetadata(_ metadata: HyperTrack.JSON.Object) -> [String: Any] {
    return [
        keyType: typeMetadata,
        keyValue: metadata.toDictionary(),
    ]
}

func serializeOrders(_ orders: [HyperTrack.Order]) -> [String: Any] {
    return [
        keyType: "orders",
        keyValue: orders.enumerated().map { index, order in
            [
                "index": index,
                "orderHandle": order.orderHandle,
                // beware not to call order.isInsideGeofence here, it is a computed property
            ]
        },
    ]
}

func serializeSimpleValue(_ value: String) -> [String: Any] {
    return [keyValue: value]
}

func serializeSimpleValue(_ value: Double) -> [String: Any] {
    return [keyValue: value]
}

func serializeSimpleValue(_ value: Bool) -> [String: Any] {
    return [keyValue: value]
}

private func getParseError(_ data: Any, key: String) -> String {
    return "Invalid input for key \(key): \(data)"
}
