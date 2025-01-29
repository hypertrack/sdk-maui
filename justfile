alias gi := generate-ios

generate-ios:
    sharpie bind -sdk iphoneos18.2 -framework ./HyperTrack.xcframework/ios-arm64/HyperTrack.framework -output ios/generated -namespace HyperTrack

print-plist:
    /usr/libexec/PlistBuddy -x -c "Print" HyperTrack.xcframework/ios-arm64/HyperTrack.framework/Info.plist
