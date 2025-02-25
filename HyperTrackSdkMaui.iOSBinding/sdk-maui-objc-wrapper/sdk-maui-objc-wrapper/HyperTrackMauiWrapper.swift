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
    
    @objc public static func getDeviceId() -> String {
        let deviceId = HyperTrack.deviceID
        return toJSON(serializeSimpleValue(deviceId))!.toJSONString()
    }
    
    @objc public static func getOrders() -> String {
        let orders = Array(HyperTrack.orders)
        return toJSON(serializeOrders(orders))!.toJSONString()
    }
    
    @objc public static func getWorkerHandle() -> String {
        let handle = HyperTrack.workerHandle
        return toJSON(serializeSimpleValue(handle))!.toJSONString()
    }
    
    @objc public static func setWorkerHandle(_ workerHandleJson: String) {
        let dict = toJSON(workerHandleJson)!.toDictionary()
        let handle = dict["value"] as! String
        HyperTrack.workerHandle = handle
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
