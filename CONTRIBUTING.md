# Contributing

## File structure

We have main C# library and binding libraries merged to have only one dependency artifact (usually binding libs are separate projects).

### Main library

- `Common/`
- `Platforms/`

### Android Binding files

- `Additions/`
  - The code that is compiled along with generated code (used for adding workarounds if needed)
- `Transforms/Metadata.xml`
  - Config to modify the generated code (e.g. rename the methods)

### iOS Binding files

- `Platforms/iOS/ApiDefinition.cs`
  - C# headers that are corresponding to Objective-C API in `sdk-maui-obj-wrapper/sdk-maui-obj-wrapper/HyperTrackMauiWrapper.swift`
- `sdk-maui-obj-wrapper/`

## sdk-maui-obj-wrapper

We are using a wrapper Xcode project to build .xcframework for MAUI because original 
HyperTrack SDK iOS API doesn't support calling it from Objective-C.

So in the project we generate Objective-C/Swift headers and then use them to proxy calls
to the Swift code of the SDK.

Important project params for correct header generation:

`Project Settings -> Build Settings`
    - `Swift Compiler`
      - `Generated Header Name`
      - `Install Bridging Header` - Yes
      - `Precompile Bridging Header` - Yes

### Serialization

We are using serializing to JSON string to make transferring data between Objective-C and C# easy and reliable as we already have JSON string (de)serialization in the SDK API ouf of the box.

## FAQ

### How to update the HyperTrack SDK version and make a release?

1. This process is automated with just recipes, use `just update-sdk` command
2. Push the tag `maui/sdk/<version>` to the repo
3. Check the release in the repo and get the `.nupkg` file
  - alternatively you can use `just release` and generate the file locally
4. Open `https://www.nuget.org` and press `Upload`
  - All the release data will be filled in automatically
5. Create a [public Github repo](https://github.com/hypertrack/sdk-maui) release
  - Release title should be the current version tag

### How to change build config

#### Android

- `minSdkVersion`
  -`HyperTrackSdkMaui/HyperTrackSdkMaui.csproj`
    - `SupportedOSPlatformVersion`

