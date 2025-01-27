alias ai := archive-ios
alias ba := build-android
alias bi := build-ios
alias c := clean
alias gi := generate-ios
alias s := setup

JAVA_SDK_DIR := "/opt/homebrew/opt/openjdk@17/libexec/openjdk.jdk/Contents/Home"

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
    # assuming JAVA_HOME="/opt/homebrew/opt/openjdk@17"
    dotnet build HyperTrackSdkMaui/HyperTrackSdkMaui.csproj -f net9.0-android -p:Configuration=Debug -p:JavaSdkDirectory="{{JAVA_SDK_DIR}}"


build-ios:
    # dotnet build -t:Run -v diag --debug -f net9.0-ios
    dotnet build HyperTrackSdkMaui/HyperTrackSdkMaui.csproj -f net9.0-ios


clean:
    dotnet clean
    rm -rf HyperTrackSdkMaui/bin
    rm -rf HyperTrackSdkMaui/obj
    rm -rf HyperTrackSdkMaui/Debug
    rm -rf HyperTrackSdkMaui.iOSBinding/bin
    rm -rf HyperTrackSdkMaui.iOSBinding/obj
    rm -rf HyperTrackSdkMaui.iOSBinding/Debug
    rm -rf HyperTrackSdkMaui.AndroidBinding/bin
    rm -rf HyperTrackSdkMaui.AndroidBinding/obj
    rm -rf HyperTrackSdkMaui.AndroidBinding/Debug


generate-ios:
    mkdir -p HyperTrackSdkMaui.iOSBinding/generated
    sharpie bind -sdk iphoneos18.2 -framework ./HyperTrackSdkMaui.iOSBinding/archives/SdkMauiObjcWrapper.xcframework/ios-arm64/sdk_maui_objc_wrapper.framework \
        -output HyperTrackSdkMaui.iOSBinding/generated 
    cp -f HyperTrackSdkMaui.iOSBinding/generated/ApiDefinitions.cs HyperTrackSdkMaui.iOSBinding/ApiDefinition.cs
    rm -rf HyperTrackSdkMaui.iOSBinding/generated


release: clean
    dotnet restore
    dotnet build HyperTrackSdkMaui/HyperTrackSdkMaui.csproj -f net9.0-android -p:Configuration=Release -p:JavaSdkDirectory="{{JAVA_SDK_DIR}}"
    dotnet build HyperTrackSdkMaui/HyperTrackSdkMaui.csproj -f net9.0-ios -p:Configuration=Release
    dotnet pack -c Release -p:JavaSdkDirectory="{{JAVA_SDK_DIR}}"
    echo "Check the release build .nupkg in ./bin/Release"
    # dotnet nuget push path/to/output/folder/YourLibrary.nupkg --api-key YOUR_NUGET_API_KEY --source https://api.nuget.org/v3/index.json


setup: archive-ios
