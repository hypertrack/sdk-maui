using Foundation;

namespace binding_ios
{

	// @interface HyperTrackMauiWrapper : NSObject
	[BaseType(typeof(NSObject))]
	interface HyperTrackMauiWrapper
	{
		// +(NSString * _Nonnull)getDeviceId __attribute__((warn_unused_result("")));
		[Static]
		[Export("getDeviceId")]
		string DeviceId { get; }

		// +(NSString * _Nonnull)addGeotag:(NSString * _Nonnull)geotag __attribute__((warn_unused_result("")));
		[Static]
		[Export("addGeotag:")]
		string AddGeotag(string geotag);
	}
}
