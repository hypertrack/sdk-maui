alias ai := archive-ios
alias ba := build-android
alias bi := build-ios
alias b := build
alias c := clean
alias cp := copy-to-public
alias d := docs
alias f := format
alias gd := get-dependencies
alias gi := generate-ios
alias od := open-docs
alias pt := push-tag
alias ogr := open-github-releases
alias r := release
alias s := setup
alias us := update-sdk
alias v := version

REPO_URL := "https://github.com/hypertrack/sdk-react-native"

# Source: https://semver.org/#is-there-a-suggested-regular-expression-regex-to-check-a-semver-string
# \ are escaped
SEMVER_REGEX := "(0|[1-9]\\d*)\\.(0|[1-9]\\d*)\\.(0|[1-9]\\d*)(?:-((?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\\.(?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\\+([0-9a-zA-Z-]+(?:\\.[0-9a-zA-Z-]+)*))?"

JAVA_SDK_DIR := "/opt/homebrew/opt/openjdk@17/libexec/openjdk.jdk/Contents/Home"

archive-ios:
    #!/usr/bin/env sh
    set -euo pipefail

    cd HyperTrackSdkMaui/sdk-maui-objc-wrapper

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

build: get-dependencies docs format build-android build-ios

build-android:
    dotnet build HyperTrackSdkMaui/HyperTrackSdkMaui.csproj -f net9.0-android -p:Configuration=Debug -p:JavaSdkDirectory="{{JAVA_SDK_DIR}}"

build-ios:
    # add -v diag --debug for verbosity
    # add -r ios-arm64 to build for real device
    dotnet build HyperTrackSdkMaui/HyperTrackSdkMaui.csproj -f net9.0-ios

clean:
    dotnet clean
    rm -rf HyperTrackSdkMaui/bin
    rm -rf HyperTrackSdkMaui/obj
    rm -rf HyperTrackSdkMaui/Debug

copy-to-public:
    #!/usr/bin/env sh
    set -euo pipefail

    PUBLIC_REPO_PATH=../../../../sdk-maui

    # delete everything in the public repo except .git folder
    find $PUBLIC_REPO_PATH -mindepth 1 -maxdepth 1 -not -name '.git' -exec rm -rf {} \;

    cp -rf . $PUBLIC_REPO_PATH
    
docs: format
    #!/usr/bin/env sh
    set -euo pipefail
    
    rm -rf tmp/docs
    rm -rf docs

    mkdir -p tmp/docs
    mkdir -p docs

    cd HyperTrackSdkMaui
    doxygen Doxyfile
    cd ..

    mv tmp/docs/html/* docs

get-dependencies:
    dotnet restore

format: 
    # todo

generate-ios:
    mkdir -p HyperTrackSdkMaui.iOSBinding/generated
    sharpie bind -sdk iphoneos18.2 -framework ./HyperTrackSdkMaui.iOSBinding/archives/SdkMauiObjcWrapper.xcframework/ios-arm64/sdk_maui_objc_wrapper.framework \
        -output HyperTrackSdkMaui.iOSBinding/generated 
    cp -f HyperTrackSdkMaui.iOSBinding/generated/ApiDefinitions.cs HyperTrackSdkMaui.iOSBinding/ApiDefinition.cs
    rm -rf HyperTrackSdkMaui.iOSBinding/generated

open-docs: # docs
    open docs/class_hyper_track_1_1_hyper_track.html

_open-github-release-data:
    code CHANGELOG.md
    just open-github-releases

open-github-releases:
    open "{{REPO_URL}}/releases"

push-tag:
    #!/usr/bin/env sh
    set -euo pipefail
    
    if [ $(git symbolic-ref --short HEAD) = "master" ] ; then
        VERSION=$(just version)
        git tag $VERSION
        git push origin $VERSION
        just _open-github-release-data
    else
        echo "You are not on master branch"
    fi

release publish="dry-run": clean build
    #!/usr/bin/env sh
    set -euo pipefail

    VERSION=$(just version)
    if [ {{publish}} = "publish" ]; then
        BRANCH=$(git branch --show-current)
        if [ $BRANCH != "master" ]; then
            echo "You must be on main branch to publish a new version (current branch: $BRANCH))"
            exit 1
        fi
        echo "Are you sure you want to publish version $VERSION? (y/N)"
        just -f ../../justfile-utils ask-confirm

        dotnet build HyperTrackSdkMaui/HyperTrackSdkMaui.csproj -f net9.0-android -p:Configuration=Release -p:JavaSdkDirectory="{{JAVA_SDK_DIR}}"
        dotnet build HyperTrackSdkMaui/HyperTrackSdkMaui.csproj -f net9.0-ios -p:Configuration=Release
        dotnet pack -c Release -p:JavaSdkDirectory="{{JAVA_SDK_DIR}}"
        echo "Check the release build .nupkg in ./bin/Release"
        rm -rf HyperTrackSdkMaui/HyperTrack.xcframework
        # dotnet nuget push path/to/output/folder/YourLibrary.nupkg --api-key YOUR_NUGET_API_KEY --source https://api.nuget.org/v3/index.jso

        open "https://www.nuget.org/packages/HyperTrack.SDK.MAUI/$VERSION"
    else
        dotnet build HyperTrackSdkMaui/HyperTrackSdkMaui.csproj -f net9.0-android -p:Configuration=Release -p:JavaSdkDirectory="{{JAVA_SDK_DIR}}"
        dotnet build HyperTrackSdkMaui/HyperTrackSdkMaui.csproj -f net9.0-ios -p:Configuration=Release
        dotnet pack -c Release -p:JavaSdkDirectory="{{JAVA_SDK_DIR}}"
    fi

setup: get-dependencies archive-ios
    brew install doxygen

update-sdk wrapper_version ios_version android_version commit="true" branch="true":
    #!/usr/bin/env sh
    set -euo pipefail

    if [ "{{branch}}" = "true" ] ; then
        git checkout -b update-sdk-ios-{{ios_version}}-android-{{android_version}}
    fi

    just _update-wrapper-version-file {{wrapper_version}}

    COMMIT_MESSAGE=""
    if [ -n "{{ios_version}}" ] && [ -n "{{android_version}}" ]; then
        just _update-sdk-ios-version-file {{ios_version}}
        just _update-sdk-android-version-file {{android_version}}
        just -f ../../justfile-utils update-readme-ios {{ios_version}} $(pwd)/README.md
        just -f ../../justfile-utils update-readme-android {{android_version}} $(pwd)/README.md
        ../../scripts/update_changelog.sh -w {{wrapper_version}} -i {{ios_version}} -a {{android_version}}
        COMMIT_MESSAGE="Update HyperTrack SDK iOS to {{ios_version}} and Android to {{android_version}}"
    elif [ -n "{{ios_version}}" ]; then
        just _update-sdk-ios-version-file {{ios_version}}
        just -f ../../justfile-utils update-readme-ios {{ios_version}} $(pwd)/README.md
        ../../scripts/update_changelog.sh -w {{wrapper_version}} -i {{ios_version}}
        COMMIT_MESSAGE="Update HyperTrack SDK iOS to {{ios_version}}"
    elif [ -n "{{android_version}}" ]; then
        just _update-sdk-android-version-file {{android_version}}
        just -f ../../justfile-utils update-readme-android {{android_version}} $(pwd)/README.md
        ../../scripts/update_changelog.sh -w {{wrapper_version}} -a {{android_version}}
        COMMIT_MESSAGE="Update HyperTrack SDK Android to {{android_version}}"
    fi

    if [ "{{commit}}" = "true" ] ; then
        git add .
        git commit -m "$COMMIT_MESSAGE"
    fi
    if [ "{{branch}}" = "true" ] && [ "{{commit}}" = "true" ] ; then
        just open-github-prs
    fi

_update-sdk-android-version-file android_version:
    #!/usr/bin/env sh
    set -euo pipefail

    ../../scripts/update_file.sh HyperTrackSdkMaui/HyperTrackSdkMaui.csproj '<AndroidMavenLibrary Include="com.hypertrack:activity-service" Version=".*">' '<AndroidMavenLibrary Include="com.hypertrack:activity-service" Version="{{android_version}}" />'
    ../../scripts/update_file.sh HyperTrackSdkMaui/HyperTrackSdkMaui.csproj '<AndroidMavenLibrary Include="com.hypertrack:activity-service-google" Version=".*">' '<AndroidMavenLibrary Include="com.hypertrack:activity-service-google" Version="{{android_version}}" />'
    ../../scripts/update_file.sh HyperTrackSdkMaui/HyperTrackSdkMaui.csproj '<AndroidMavenLibrary Include="com.hypertrack:location-services" Version=".*">' '<AndroidMavenLibrary Include="com.hypertrack:location-services" Version="{{android_version}}" />'
    ../../scripts/update_file.sh HyperTrackSdkMaui/HyperTrackSdkMaui.csproj '<AndroidMavenLibrary Include="com.hypertrack:location-services-google" Version=".*">' '<AndroidMavenLibrary Include="com.hypertrack:location-services-google" Version="{{android_version}}" />'
    ../../scripts/update_file.sh HyperTrackSdkMaui/HyperTrackSdkMaui.csproj '<AndroidMavenLibrary Include="com.hypertrack:push-service" Version=".*">' '<AndroidMavenLibrary Include="com.hypertrack:push-service" Version="{{android_version}}" />'
    ../../scripts/update_file.sh HyperTrackSdkMaui/HyperTrackSdkMaui.csproj '<AndroidMavenLibrary Include="com.hypertrack:push-service-firebase" Version=".*">' '<AndroidMavenLibrary Include="com.hypertrack:push-service-firebase" Version="{{android_version}}" />'
    ../../scripts/update_file.sh HyperTrackSdkMaui/HyperTrackSdkMaui.csproj '<AndroidMavenLibrary Include="com.hypertrack:sdk-android" Version=".*">' '<AndroidMavenLibrary Include="com.hypertrack:sdk-android" Version="{{android_version}}" />'
    ../../scripts/update_file.sh HyperTrackSdkMaui/HyperTrackSdkMaui.csproj '<AndroidMavenLibrary Include="com.hypertrack:sdk-android-model" Version=".*">' '<AndroidMavenLibrary Include="com.hypertrack:sdk-android-model" Version="{{android_version}}" />'

_update-sdk-ios-version-file ios_version:
    @echo "Updating iOS SDK doesn't make sense for MAUI, we include SDK binaries directly"

update-wrapper-version version: (_update-wrapper-version-file version)

_update-wrapper-version-file wrapper_version:
    #!/usr/bin/env sh
    set -euo pipefail

    ../../scripts/update_file.sh HyperTrackSdkMaui/HyperTrackSdkMaui.csproj '<Version>.*</Version>' '<Version>{{wrapper_version}}</Version>'

version:
    @cat HyperTrackSdkMaui/HyperTrackSdkMaui.csproj | grep Version | head -n 1 | grep -o -E '{{SEMVER_REGEX}}'
