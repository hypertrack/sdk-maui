using Foundation;

[Static]
[Verify (ConstantsInterfaceAssociation)]
partial interface Constants
{
	// extern double sdk_maui_objc_wrapperVersionNumber;
	[Field ("sdk_maui_objc_wrapperVersionNumber", "__Internal")]
	double sdk_maui_objc_wrapperVersionNumber { get; }

	// extern const unsigned char[] sdk_maui_objc_wrapperVersionString;
	[Field ("sdk_maui_objc_wrapperVersionString", "__Internal")]
	byte[] sdk_maui_objc_wrapperVersionString { get; }
}

// @interface HyperTrackMauiWrapper : NSObject
[BaseType (typeof(NSObject), Name = "_TtC21sdk_maui_objc_wrapper21HyperTrackMauiWrapper")]
interface HyperTrackMauiWrapper
{
	// +(NSString * _Nonnull)getDeviceId __attribute__((warn_unused_result("")));
	[Static]
	[Export ("getDeviceId")]
	[Verify (MethodToProperty)]
	string DeviceId { get; }
}
