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
	[BaseType (typeof(NSObject))]
	interface HyperTrackMauiWrapper
	{
		// +(NSString * _Nonnull)addGeotag:(NSString * _Nonnull)geotag __attribute__((warn_unused_result("")));
		[Static]
		[Export ("addGeotag:")]
		string AddGeotag (string geotag);

		// +(NSString * _Nonnull)getDeviceId __attribute__((warn_unused_result("")));
		[Static]
		[Export ("getDeviceId")]
		string DeviceId { get; }

		// +(NSString * _Nonnull)getOrders __attribute__((warn_unused_result("")));
		[Static]
		[Export ("getOrders")]
		string Orders { get; }

		// +(NSString * _Nonnull)getWorkerHandle __attribute__((warn_unused_result("")));
		[Static]
		[Export ("getWorkerHandle")]
		string WorkerHandle { get; }

		// +(void)setWorkerHandle:(NSString * _Nonnull)workerHandle;
		[Static]
		[Export ("setWorkerHandle:")]
		void SetWorkerHandle (string workerHandle);

		// +(HyperTrackCancellable *)subscribeToOrders:(void (^)(NSString *))callback;
		[Static]
		[Export("subscribeToOrders:")]
		HyperTrackCancellable SubscribeToOrders(Action<NSString> callback);
	}
}
