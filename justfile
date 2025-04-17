alias ba := build-android
alias bi := build-ios
alias b := build
alias c := clean
alias d := docs
alias f := format
alias gi := generate-ios-binding
alias gen := generate
alias gd := get-dependencies
alias od := open-docs
alias pt := push-tag
alias ogr := open-github-releases
alias r := release
alias s := setup
alias us := update-sdk
alias v := version

DOTNET_PATH := "tmp/dotnet"
IOS_GENERATED_BINDIND_API_PATH := "HyperTrackSdkMaui/generated"
JUSTFILE_UTILS := "../../justfile-utils"
MAUI_SDK := "HyperTrackSdkMaui"
MOBILE_ROOT := "../../.."
PROJECT_FILE := "HyperTrackSdkMaui/HyperTrackSdkMaui.csproj"
REPO_URL := "https://github.com/hypertrack/sdk-maui"
SCRIPTS_PATH := "../../scripts"

# Source: https://semver.org/#is-there-a-suggested-regular-expression-regex-to-check-a-semver-string
# \ are escaped
SEMVER_REGEX := "(0|[1-9]\\d*)\\.(0|[1-9]\\d*)\\.(0|[1-9]\\d*)(?:-((?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\\.(?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\\+([0-9a-zA-Z-]+(?:\\.[0-9a-zA-Z-]+)*))?"

JAVA_SDK_DIR := "/opt/homebrew/opt/openjdk@17/libexec/openjdk.jdk/Contents/Home"
ANDROID_SDK_PATH := '/opt/homebrew/share/android-commandlinetools'

build: get-dependencies docs format build-android build-ios 

build-android:
    {{DOTNET_PATH}}/dotnet build {{PROJECT_FILE}} -f net9.0-android -p:Configuration=Debug -p:JavaSdkDirectory="{{JAVA_SDK_DIR}}" -p:AndroidSdkDirectory="{{ANDROID_SDK_PATH}}"

build-ios:
    # add -v diag --debug for verbosity
    # add -r ios-arm64 to build for real device
    {{DOTNET_PATH}}/dotnet build {{PROJECT_FILE}} -f net9.0-ios

clean:
    {{DOTNET_PATH}}/dotnet clean
    rm -rf {{MAUI_SDK}}/bin
    rm -rf {{MAUI_SDK}}/obj
    rm -rf {{MAUI_SDK}}/Debug
    rm -rf ../archives
    
docs: format
    #!/usr/bin/env sh
    set -euo pipefail
    
    rm -rf tmp/docs
    mkdir -p tmp/docs
    
    rm -rf docs
    mkdir -p docs

    cd HyperTrackSdkMaui
    doxygen Doxyfile
    cd ..

    mv tmp/docs/html/* docs

format: 
    # todo

get-dependencies:
    # required if you run Quickstart with local repo before to not get missing repo error
    mkdir -p {{MAUI_SDK}}/bin/Release 
    {{DOTNET_PATH}}/dotnet restore -p:JavaSdkDirectory="{{JAVA_SDK_DIR}}" -p:AndroidSdkDirectory="{{ANDROID_SDK_PATH}}"

generate profile="release":
    #!/usr/bin/env sh
    set -euo pipefail

    cd {{MAUI_SDK}}/sdk-maui-objc-wrapper

    rm -rf ../archives
    mkdir -p ../archives

    IOS_SDK_XCF_TARGET_PATH="../archives"

    IOS_SDK_XCF_SOURCE_PATH="../../../../../ios/sdk/tmp"
    if [ "{{profile}}" = "debug" ]; then
        IOS_SDK_XCF_SOURCE_PATH="$IOS_SDK_PATH/Debug/HyperTrack.xcframework"
    else
        IOS_SDK_XCF_SOURCE_PATH="$IOS_SDK_PATH/Release/HyperTrack.xcframework"
    fi

    if [ ! -d $IOS_SDK_XCF_SOURCE_PATH ]; then
        echo "iOS SDK not found at $IOS_SDK_XCF_SOURCE_PATH"
        exit 1
    fi

    rm -rf $IOS_SDK_XCF_TARGET_PATH/HyperTrack.xcframework
    cp -r $IOS_SDK_XCF_SOURCE_PATH $IOS_SDK_XCF_TARGET_PATH

    xcodebuild archive \
        -scheme sdk-maui-objc-wrapper \
        -archivePath ../archives/sdk-maui-objc-wrapper-ios.xcarchive \
        -sdk iphoneos \
        SKIP_INSTALL=NO

    xcodebuild archive \
        -scheme sdk-maui-objc-wrapper \
        -archivePath ../archives/sdk-maui-objc-wrapper-sim.xcarchive \
        -sdk iphonesimulator \

    xcodebuild -create-xcframework \
        -framework ../archives/sdk-maui-objc-wrapper-ios.xcarchive/Products/Library/Frameworks/sdk_maui_objc_wrapper.framework \
        -framework ../archives/sdk-maui-objc-wrapper-sim.xcarchive/Products/Library/Frameworks/sdk_maui_objc_wrapper.framework \
        -output ../archives/SdkMauiObjcWrapper.xcframework

generate-ios-binding:
    # used to generate the binding C# headers from Swift code 
    mkdir -p {{IOS_GENERATED_BINDIND_API_PATH}}
    sharpie bind -sdk iphoneos18.2 -framework ./{{MAUI_SDK}}/archives/SdkMauiObjcWrapper.xcframework/ios-arm64/sdk_maui_objc_wrapper.framework \
        -output {{IOS_GENERATED_BINDIND_API_PATH}} 
    cp -f {{IOS_GENERATED_BINDIND_API_PATH}}/ApiDefinitions.cs {{MAUI_SDK}}/Platforms/iOS/ApiDefinition.cs
    rm -rf {{IOS_GENERATED_BINDIND_API_PATH}}

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
        just -f {{JUSTFILE_UTILS}} ask-confirm

        {{DOTNET_PATH}}/dotnet build {{PROJECT_FILE}} -f net9.0-android -p:Configuration=Release -p:JavaSdkDirectory="{{JAVA_SDK_DIR}}" -p:AndroidSdkDirectory="{{ANDROID_SDK_PATH}}"
        {{DOTNET_PATH}}/dotnet build {{PROJECT_FILE}} -f net9.0-ios -p:Configuration=Release
        {{DOTNET_PATH}}/dotnet pack -c Release -p:JavaSdkDirectory="{{JAVA_SDK_DIR}}"
        echo "Check the release build .nupkg in ./bin/Release"
        rm -rf {{MAUI_SDK}}/HyperTrack.xcframework
        # {{DOTNET_PATH}}/dotnet nuget push path/to/output/folder/YourLibrary.nupkg --api-key YOUR_NUGET_API_KEY --source https://api.nuget.org/v3/index.jso

        open "https://www.nuget.org/packages/HyperTrack.SDK.MAUI/$VERSION"
    else
        {{DOTNET_PATH}}/dotnet build {{PROJECT_FILE}} -f net9.0-android -p:Configuration=Release -p:JavaSdkDirectory="{{JAVA_SDK_DIR}}" -p:AndroidSdkDirectory="{{ANDROID_SDK_PATH}}"
        {{DOTNET_PATH}}/dotnet build {{PROJECT_FILE}} -f net9.0-ios -p:Configuration=Release
        {{DOTNET_PATH}}/dotnet pack -c Release -p:JavaSdkDirectory="{{JAVA_SDK_DIR}}" -p:AndroidSdkDirectory="{{ANDROID_SDK_PATH}}"
    fi

setup:
    #!/usr/bin/env sh
    set -euo pipefail

    brew install wget
    # download to tmp folder
    wget -P tmp https://dot.net/v1/dotnet-install.sh
    chmod +x tmp/dotnet-install.sh
    ./tmp/dotnet-install.sh --version 9.0.202 --install-dir $(pwd)/{{DOTNET_PATH}}

    {{DOTNET_PATH}}/dotnet workload install maui
    brew install doxygen
    just get-dependencies
    # accept Android SDK license agreements
    echo y | {{ANDROID_SDK_PATH}}/cmdline-tools/latest/bin/sdkmanager --licenses || true
    echo y | {{ANDROID_SDK_PATH}}/cmdline-tools/latest/bin/sdkmanager --licenses || true
    echo y | {{ANDROID_SDK_PATH}}/cmdline-tools/latest/bin/sdkmanager --licenses || true
    echo y | {{ANDROID_SDK_PATH}}/cmdline-tools/latest/bin/sdkmanager --licenses || true
    {{DOTNET_PATH}}/dotnet build -t:InstallAndroidDependencies -f net9.0-android "-p:AndroidSdkDirectory={{ANDROID_SDK_PATH}}" "-p:JavaSdkDirectory={{JAVA_SDK_DIR}}" "-p:AcceptAndroidSDKLicenses=True"

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
        just -f {{JUSTFILE_UTILS}} update-readme-ios {{ios_version}} $(pwd)/README.md
        just -f {{JUSTFILE_UTILS}} update-readme-android {{android_version}} $(pwd)/README.md
        {{SCRIPTS_PATH}}/changelog_add_sdk_versions_bump.sh -r {{REPO_URL}} -w {{wrapper_version}} -i {{ios_version}} -a {{android_version}}
        COMMIT_MESSAGE="Update HyperTrack SDK iOS to {{ios_version}} and Android to {{android_version}}"
    elif [ -n "{{ios_version}}" ]; then
        just _update-sdk-ios-version-file {{ios_version}}
        just -f {{JUSTFILE_UTILS}} update-readme-ios {{ios_version}} $(pwd)/README.md
        {{SCRIPTS_PATH}}/changelog_add_sdk_versions_bump.sh -r {{REPO_URL}} -w {{wrapper_version}} -i {{ios_version}}
        COMMIT_MESSAGE="Update HyperTrack SDK iOS to {{ios_version}}"
    elif [ -n "{{android_version}}" ]; then
        just _update-sdk-android-version-file {{android_version}}
        just -f {{JUSTFILE_UTILS}} update-readme-android {{android_version}} $(pwd)/README.md
        {{SCRIPTS_PATH}}/changelog_add_sdk_versions_bump.sh -r {{REPO_URL}} -w {{wrapper_version}} -a {{android_version}}
        COMMIT_MESSAGE="Update HyperTrack SDK Android to {{android_version}}"
    fi

    if [ "{{commit}}" = "true" ] ; then
        git add -A
        git commit -m "$COMMIT_MESSAGE"
    fi
    if [ "{{branch}}" = "true" ] && [ "{{commit}}" = "true" ] ; then
        just open-github-prs
    fi

_update-sdk-android-version-file android_version:
    #!/usr/bin/env sh
    set -euo pipefail

    {{SCRIPTS_PATH}}/update_file.sh {{PROJECT_FILE}} '<AndroidMavenLibrary Include="com.hypertrack:activity-service" Version=".*">' '<AndroidMavenLibrary Include="com.hypertrack:activity-service" Version="{{android_version}}" />'
    {{SCRIPTS_PATH}}/update_file.sh {{PROJECT_FILE}} '<AndroidMavenLibrary Include="com.hypertrack:activity-service-google" Version=".*">' '<AndroidMavenLibrary Include="com.hypertrack:activity-service-google" Version="{{android_version}}" />'
    {{SCRIPTS_PATH}}/update_file.sh {{PROJECT_FILE}} '<AndroidMavenLibrary Include="com.hypertrack:location-services" Version=".*">' '<AndroidMavenLibrary Include="com.hypertrack:location-services" Version="{{android_version}}" />'
    {{SCRIPTS_PATH}}/update_file.sh {{PROJECT_FILE}} '<AndroidMavenLibrary Include="com.hypertrack:location-services-google" Version=".*">' '<AndroidMavenLibrary Include="com.hypertrack:location-services-google" Version="{{android_version}}" />'
    {{SCRIPTS_PATH}}/update_file.sh {{PROJECT_FILE}} '<AndroidMavenLibrary Include="com.hypertrack:push-service" Version=".*">' '<AndroidMavenLibrary Include="com.hypertrack:push-service" Version="{{android_version}}" />'
    {{SCRIPTS_PATH}}/update_file.sh {{PROJECT_FILE}} '<AndroidMavenLibrary Include="com.hypertrack:push-service-firebase" Version=".*">' '<AndroidMavenLibrary Include="com.hypertrack:push-service-firebase" Version="{{android_version}}" />'
    {{SCRIPTS_PATH}}/update_file.sh {{PROJECT_FILE}} '<AndroidMavenLibrary Include="com.hypertrack:sdk-android" Version=".*">' '<AndroidMavenLibrary Include="com.hypertrack:sdk-android" Version="{{android_version}}" />'
    {{SCRIPTS_PATH}}/update_file.sh {{PROJECT_FILE}} '<AndroidMavenLibrary Include="com.hypertrack:sdk-android-model" Version=".*">' '<AndroidMavenLibrary Include="com.hypertrack:sdk-android-model" Version="{{android_version}}" />'

_update-sdk-ios-version-file ios_version:
    @echo "Updating iOS SDK doesn't make sense for MAUI, we include SDK binaries directly"

update-wrapper-version version: (_update-wrapper-version-file version)

_update-wrapper-version-file wrapper_version:
    #!/usr/bin/env sh
    set -euo pipefail

    {{SCRIPTS_PATH}}/update_file.sh {{PROJECT_FILE}} '<Version>.*</Version>' '<Version>{{wrapper_version}}</Version>'

version:
    @cat {{PROJECT_FILE}} | grep Version | head -n 1 | grep -o -E '{{SEMVER_REGEX}}'
