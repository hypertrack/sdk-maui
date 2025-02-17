import Foundation
import HyperTrack

@objc(HyperTrackMauiWrapper)
public final class HyperTrackMauiWrapper: NSObject {
    
    @objc public static func getDeviceId() -> String {
        let result = sdk_maui_objc_wrapper.getDeviceID()
        switch result {
        case let .success(.dict(serialized)):
            return serialized["value"] as! String
        default:
            preconditionFailure("\(result)")
        }
    }
    
    @objc public static func addGeotag(_ geotag: String) -> String {
        let result = sdk_maui_objc_wrapper.addGeotag(toJSON(geotag)!.toDictionary())
        switch result {
            case let .success(.dict(serialized)):
                return toJSON(serialized)!.toJSONString()
            case let .failure(error):
                preconditionFailure("Wrapper API failure: \(error)")
            default:
                preconditionFailure("Unexpected result: \(result)")
        }
    }
}
