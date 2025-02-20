import Foundation
import HyperTrack

@objc(HyperTrackMauiWrapper)
public final class HyperTrackMauiWrapper: NSObject {

    @objc public static func addGeotag(_ geotag: String) -> String {
        let result = sdk_maui_objc_wrapper.addGeotag(toJSON(geotag)!.toDictionary())
        switch result {
            case let .success(.dict(serialized)):
                return toJSON(serialized)!.toJSONString()
            default:
                let message = "Unexpected result: \(result)"
                print(message)
                preconditionFailure(message)
        }
    }
    
    @objc public static func getDeviceId() -> String {
        let result = sdk_maui_objc_wrapper.getDeviceID()
        switch result {
            case let .success(.dict(serialized)):
                return serialized["value"] as! String
            default:
                let message = "Unexpected result: \(result)"
                print(message)
                preconditionFailure(message)
        }
    }
    
    @objc public static func getOrders() -> String {
        let result = sdk_maui_objc_wrapper.getOrders()
        switch result {
            case let .success(.dict(serialized)):
                return toJSON(serialized)!.toJSONString()
            default:
                let message = "Unexpected result: \(result)"
                print(message)
                preconditionFailure(message)
        }
    }
    
    @objc public static func getWorkerHandle() -> String {
        let result = sdk_maui_objc_wrapper.getWorkerHandle()
        switch result {
            case let .success(.dict(serialized)):
                return toJSON(serialized)!.toJSONString()
            default:
                let message = "Unexpected result: \(result)"
                print(message)
                preconditionFailure(message)
        }
    }
    
    @objc public static func setWorkerHandle(_ workerHandle: String) {
        let result = sdk_maui_objc_wrapper.setWorkerHandle(toJSON(workerHandle)!.toDictionary())
        guard case .success(.void) = result else {
            let message = "Unexpected result: \(result)"
            print(message)
            preconditionFailure(message)
        }
    }
}
