import Foundation
import HyperTrack

@objc public final class HyperTrackMauiWrapper: NSObject {
    
    @objc public static func getDeviceId() -> String {
            sdk_maui_objc_wrapper.getDeviceID()
            return "kek"
        }
    
//  @objc public static func didRegisterForRemoteNotifications(deviceToken: Data) {
//    HyperTrack.didRegisterForRemoteNotificationsWithDeviceToken(deviceToken)
//  }
}
