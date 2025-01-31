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
    
//  @objc public static func didRegisterForRemoteNotifications(deviceToken: Data) {
//    HyperTrack.didRegisterForRemoteNotificationsWithDeviceToken(deviceToken)
//  }
}
