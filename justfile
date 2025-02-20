alias ai := archive-ios
alias ba := build-android
alias bi := build-ios
alias gi := generate-ios
alias s := setup

archive-ios:
    #!/usr/bin/env sh
    set -euo pipefail

    cd HyperTrackSdkMaui.iOSBinding/sdk-maui-objc-wrapper

    rm -rf ../archives
    mkdir -p ../archives

    xcodebuild archive \
        -scheme sdk-maui-objc-wrapper \
        -archivePath ../archives/sdk-maui-objc-wrapper-ios.xcarchive \
        -sdk iphoneos \
        SKIP_INSTALL=NO

    xcodebuild archive \
        -scheme sdk-maui-objc-wrapper \
        -archivePath ../archives/sdk-maui-objc-wrapper-sim.xcarchive \
        -sdk iphonesimulator \
        SKIP_INSTALL=NO

    xcodebuild -create-xcframework \
        -framework ../archives/sdk-maui-objc-wrapper-ios.xcarchive/Products/Library/Frameworks/sdk_maui_objc_wrapper.framework \
        -framework ../archives/sdk-maui-objc-wrapper-sim.xcarchive/Products/Library/Frameworks/sdk_maui_objc_wrapper.framework \
        -output ../archives/SdkMauiObjcWrapper.xcframework


build-android:
    #!/usr/bin/env sh
    set -euo pipefail
    
    # assuming JAVA_HOME="/opt/homebrew/opt/openjdk@17"
    dotnet build -f net9.0-android -p:Configuration=Debug -p:JavaSdkDirectory="$JAVA_HOME/libexec/openjdk.jdk/Contents/Home"

build-ios:
    # dotnet build -t:Run -v diag --debug -f net9.0-ios
    dotnet build -f net9.0-ios

clean:
    dotnet clean

generate-ios:
    #!/usr/bin/env sh
    set -euo pipefail

    sharpie bind -sdk iphoneos18.2 -framework ./ios/archives/SdkMauiObjcWrapper.xcframework/ios-arm64/sdk_maui_objc_wrapper.framework \
        -output ios/generated 

setup: archive-ios
