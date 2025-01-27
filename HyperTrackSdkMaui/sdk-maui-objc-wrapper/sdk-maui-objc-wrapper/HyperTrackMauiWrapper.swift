import Foundation
import HyperTrack

@objc(HyperTrackMauiWrapper)
public final class HyperTrackMauiWrapper: NSObject {
    
    @objc public static func addGeotag(_ geotagJson: String) -> String {
        let geotagDict = toJSON(geotagJson)!.toDictionary()
        
        guard case let .success(geotagData) = deserializeGeotagData(geotagDict) else {
            preconditionFailure("Failed to parse geotag data")
        }
        guard let orderHandle = geotagData.orderHandle else {
            preconditionFailure("orderHandle must be provided")
        }
        guard let orderStatus = geotagData.orderStatus else {
            preconditionFailure("orderStatus must be provided")
        }
        guard let metadata = toJSON(geotagData.data) else {
            preconditionFailure("failed to parse metadata")
        }
        
        if let expectedLocation = geotagData.expectedLocation {
            let result = HyperTrack.addGeotag(
                orderHandle: orderHandle,
                orderStatus: orderStatus,
                metadata: metadata,
                expectedLocation: expectedLocation
            )
            return toJSON(serializeLocationWithDeviationResult(result))!.toJSONString()
        } else {
            let result = HyperTrack.addGeotag(
                orderHandle: orderHandle,
                orderStatus: orderStatus,
                metadata: metadata
            )
            return toJSON(serializeLocationResult(result))!.toJSONString()
        }
    }
    
    @objc public static func getAllowMockLocation() -> String {
        return toJSON(serializeSimpleValue(HyperTrack.allowMockLocation))!.toJSONString()
    }
    
    @objc public static func getDeviceId() -> String {
        let deviceId = HyperTrack.deviceID
        return toJSON(serializeSimpleValue(deviceId))!.toJSONString()
    }
    
    @objc public static func getDynamicPublishableKey() -> String {
        return toJSON(serializeSimpleValue(HyperTrack.dynamicPublishableKey))!.toJSONString()
    }
    
    @objc public static func getErrors() -> String {
        let errors = HyperTrack.errors
        return toJSON(serializeErrors(errors))!.toJSONString()
    }
    
    @objc public static func getIsAvailable() -> String {
        return toJSON(serializeSimpleValue(HyperTrack.isAvailable))!.toJSONString()
    }
    
    @objc public static func getIsTracking() -> String {
        return toJSON(serializeSimpleValue(HyperTrack.isTracking))!.toJSONString()
    }
    
    @objc public static func getLocation() -> String {
        let location = HyperTrack.location
        return toJSON(serializeLocationResult(location))!.toJSONString()
    }
    
    @objc public static func getMetadata() -> String {
        let metadata = HyperTrack.metadata
        return toJSON(serializeMetadata(metadata))!.toJSONString()
    }
    
    @objc public static func getName() -> String {
        return toJSON(serializeSimpleValue(HyperTrack.name))!.toJSONString()
    }
    
    @objc public static func getOrders() -> String {
        let orders = Array(HyperTrack.orders)
        return toJSON(serializeOrders(orders))!.toJSONString()
    }
    
    @objc public static func getWorkerHandle() -> String {
        let handle = HyperTrack.workerHandle
        return toJSON(serializeSimpleValue(handle))!.toJSONString()
    }
    
    @objc public static func locate(_ callback: @escaping (String) -> Void) -> HyperTrackCancellable {
        let cancellable = HyperTrack.locate { result in
            let serialized = toJSON(serializeLocateResult(result))!.toJSONString()
            callback(serialized)
        }
        return HyperTrackCancellable(cancellable)
    }
    
    @objc public static func orderIsInsideGeofence(_ orderHandle: String) -> String {
        let order = HyperTrack.orders.first { $0.orderHandle == orderHandle }
        return toJSON(serializeIsInsideGeofence(order?.isInsideGeofence ?? .success(false)))!.toJSONString()
    }
    
    @objc public static func setAllowMockLocation(_ valueJson: String) {
        let dict = toJSON(valueJson)!.toDictionary()
        guard case let .success(value) = deserializeSimpleValueBoolean(dict) else {
            preconditionFailure("Failed to parse boolean value")
        }
        HyperTrack.allowMockLocation = value
    }
    
    @objc public static func setDynamicPublishableKey(_ valueJson: String) {
        let dict = toJSON(valueJson)!.toDictionary()
        guard case let .success(value) = deserializeSimpleValueString(dict) else {
            preconditionFailure("Failed to parse string value")
        }
        HyperTrack.dynamicPublishableKey = value
    }
    
    @objc public static func setIsAvailable(_ valueJson: String) {
        let dict = toJSON(valueJson)!.toDictionary()
        guard case let .success(value) = deserializeSimpleValueBoolean(dict) else {
            preconditionFailure("Failed to parse boolean value")
        }
        HyperTrack.isAvailable = value
    }
    
    @objc public static func setIsTracking(_ valueJson: String) {
        let dict = toJSON(valueJson)!.toDictionary()
        guard case let .success(value) = deserializeSimpleValueBoolean(dict) else {
            preconditionFailure("Failed to parse boolean value")
        }
        HyperTrack.isTracking = value
    }
    
    @objc public static func setMetadata(_ metadataJson: String) {
        let dict = toJSON(metadataJson)!.toDictionary()
        HyperTrack.metadata = try! deserializeMetadata(dict).get()
    }
    
    @objc public static func setName(_ valueJson: String) {
        let dict = toJSON(valueJson)!.toDictionary()
        guard case let .success(value) = deserializeSimpleValueString(dict) else {
            preconditionFailure("Failed to parse string value")
        }
        HyperTrack.name = value
    }
    
    @objc public static func setWorkerHandle(_ workerHandleJson: String) {
        let dict = toJSON(workerHandleJson)!.toDictionary()
        let handle = dict["value"] as! String
        HyperTrack.workerHandle = handle
    }
    
    @objc public static func subscribeToErrors(_ callback: @escaping (String) -> Void) -> HyperTrackCancellable {
        let cancellable = HyperTrack.subscribeToErrors { errors in
            let serialized = toJSON(serializeErrors(errors))!.toJSONString()
            callback(serialized)
        }
        return HyperTrackCancellable(cancellable)
    }
    
    @objc public static func subscribeToIsAvailable(_ callback: @escaping (String) -> Void) -> HyperTrackCancellable {
        let cancellable = HyperTrack.subscribeToIsAvailable { value in
            let serialized = toJSON(serializeSimpleValue(value))!.toJSONString()
            callback(serialized)
        }
        return HyperTrackCancellable(cancellable)
    }
    
    @objc public static func subscribeToIsTracking(_ callback: @escaping (String) -> Void) -> HyperTrackCancellable {
        let cancellable = HyperTrack.subscribeToIsTracking { value in
            let serialized = toJSON(serializeSimpleValue(value))!.toJSONString()
            callback(serialized)
        }
        return HyperTrackCancellable(cancellable)
    }
    
    @objc public static func subscribeToLocation(_ callback: @escaping (String) -> Void) -> HyperTrackCancellable {
        let cancellable = HyperTrack.subscribeToLocation { result in
            let serialized = toJSON(serializeLocationResult(result))!.toJSONString()
            callback(serialized)
        }
        return HyperTrackCancellable(cancellable)
    }
    
    @objc public static func subscribeToOrders(_ callback: @escaping (String) -> Void) -> HyperTrackCancellable {
        let cancellable = HyperTrack.subscribeToOrders { orders in
            let serialized = toJSON(serializeOrders(Array(orders)))!.toJSONString()
            callback(serialized)
        }
        
        return HyperTrackCancellable(cancellable)
    }
}

@objc(HyperTrackCancellable)
public class HyperTrackCancellable: NSObject {
    private let cancellable: HyperTrack.Cancellable
    
    init(_ cancellable: HyperTrack.Cancellable) {
        self.cancellable = cancellable
    }
    
    @objc public func cancel() {
        cancellable.cancel()
    }
}
