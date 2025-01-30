alias gi := generate-ios

generate-ios:
    sharpie bind -sdk iphoneos18.2 -framework ./HyperTrack.xcframework/ios-arm64/HyperTrack.framework -output ios/generated -namespace HyperTrack

print-plist:
    /usr/libexec/PlistBuddy -x -c "Print" HyperTrack.xcframework/ios-arm64/HyperTrack.framework/Info.plist

archive-ios:
    xcodebuild archive \
 -scheme maui-test2 \
 -archivePath SwiftMaui-ios.xcarchive \
 -sdk iphoneos \
 SKIP_INSTALL=NO

xcodebuild archive \
 -scheme maui-test2 \
 -archivePath SwiftMaui-sim.xcarchive \
 -sdk iphonesimulator \
 SKIP_INSTALL=NO

xcodebuild -create-xcframework \
 -framework ../XCFrameworks/SwiftMaui-sim.xcarchive/Products/Library/Frameworks/SwiftUI_MAUI_Framework.framework \
 -framework ../XCFrameworks/SwiftMaui-ios.xcarchive/Products/Library/Frameworks/SwiftUI_MAUI_Framework.framework \
 -output ../XCFrameworks/SwiftUI_MAUI_Framework.xcframework
