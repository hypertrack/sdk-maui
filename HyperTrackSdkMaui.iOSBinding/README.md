# Notes

We are using a wrapper Xcode project to build .xcfreamework for MAUI because original 
HyperTrack SDK iOS API doesn't support calling it from Objective-C.

So in the project we generate Objective-C/Swift headers and then use them to proxy calls
to the Swift code of the SDK.

Important project params for correct header generation:

`Project Settings -> Build Settings`
    - `Swift Compiler`
      - `Generated Header Name`
      - `Install Bridging Header` - Yes
      - `Precompile Bridging Header` - Yes
    - `Deployment`
      - `Skip Install` - No
