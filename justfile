alias ai := archive-ios
alias bi := build-ios
alias gi := generate-ios

build-ios:
    #!/usr/bin/env sh
    set -euo pipefail

    cd ios/binding-ios
    dotnet build

generate-ios:
    #!/usr/bin/env sh
    set -euo pipefail

    sharpie bind -sdk iphoneos18.2 -framework ./ios/archives/SdkMauiObjcWrapper.xcframework/ios-arm64/sdk_maui_objc_wrapper.framework \
        -output ios/generated 

archive-ios:
    #!/usr/bin/env sh
    set -euo pipefail

    cd ios/sdk-maui-objc-wrapper

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
