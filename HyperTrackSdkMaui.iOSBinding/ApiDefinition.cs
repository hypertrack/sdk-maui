using System;
using Foundation;
namespace binding_ios
{
	// @interface HyperTrackCancellable : NSObject
	[BaseType(typeof(NSObject))]
	interface HyperTrackCancellable
	{
		// -(void)cancel;
		[Export("cancel")]
		void Cancel();
	}

	// @interface HyperTrackMauiWrapper : NSObject
	[BaseType(typeof(NSObject))]
	interface HyperTrackMauiWrapper
	{
		// +(NSString * _Nonnull)addGeotag:(NSString * _Nonnull)geotagJson __attribute__((warn_unused_result("")));
		[Static]
		[Export("addGeotag:")]
		string AddGeotag(string geotagJson);

		// +(NSString * _Nonnull)getDeviceId __attribute__((warn_unused_result("")));
		[Static]
		[Export("getDeviceId")]
		string GetDeviceId();

		// +(NSString * _Nonnull)getOrders __attribute__((warn_unused_result("")));
		[Static]
		[Export("getOrders")]
		string GetOrders();

		// +(NSString * _Nonnull)getWorkerHandle __attribute__((warn_unused_result("")));
		[Static]
		[Export("getWorkerHandle")]
		string GetWorkerHandle();

		// +(void)setWorkerHandle:(NSString * _Nonnull)workerHandleJson;
		[Static]
		[Export("setWorkerHandle:")]
		void SetWorkerHandle(string workerHandleJson);

		// +(HyperTrackCancellable *)subscribeToOrders:(void (^)(NSString *))callback;
		[Static]
		[Export("subscribeToOrders:")]
		HyperTrackCancellable SubscribeToOrders(Action<NSString> callback);
	}
}
