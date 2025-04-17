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

		// +(NSString * _Nonnull)getAllowMockLocation __attribute__((warn_unused_result("")));
		[Static]
		[Export("getAllowMockLocation")]
		string GetAllowMockLocation();

		// +(NSString * _Nonnull)getDeviceId __attribute__((warn_unused_result("")));
		[Static]
		[Export("getDeviceId")]
		string GetDeviceId();

		// +(NSString * _Nonnull)getDynamicPublishableKey __attribute__((warn_unused_result("")));
		[Static]
		[Export("getDynamicPublishableKey")]
		string GetDynamicPublishableKey();

		// +(NSString * _Nonnull)getErrors __attribute__((warn_unused_result("")));
		[Static]
		[Export("getErrors")]
		string GetErrors();

		// +(NSString * _Nonnull)getIsAvailable __attribute__((warn_unused_result("")));
		[Static]
		[Export("getIsAvailable")]
		string GetIsAvailable();

		// +(NSString * _Nonnull)getIsTracking __attribute__((warn_unused_result("")));
		[Static]
		[Export("getIsTracking")]
		string GetIsTracking();

		// +(NSString * _Nonnull)getLocation __attribute__((warn_unused_result("")));
		[Static]
		[Export("getLocation")]
		string GetLocation();

		// +(NSString * _Nonnull)getMetadata __attribute__((warn_unused_result("")));
		[Static]
		[Export("getMetadata")]
		string GetMetadata();

		// +(NSString * _Nonnull)getName __attribute__((warn_unused_result("")));
		[Static]
		[Export("getName")]
		string GetName();

		// +(NSString * _Nonnull)getOrders __attribute__((warn_unused_result("")));
		[Static]
		[Export("getOrders")]
		string GetOrders();

		// +(NSString * _Nonnull)getWorkerHandle __attribute__((warn_unused_result("")));
		[Static]
		[Export("getWorkerHandle")]
		string GetWorkerHandle();

		// +(HyperTrackCancellable *)locate:(void (^)(NSString *))callback;
		[Static]
		[Export("locate:")]
		HyperTrackCancellable Locate(Action<NSString> callback);

		// +(void)setAllowMockLocation:(NSString * _Nonnull)valueJson;
		[Static]
		[Export("setAllowMockLocation:")]
		void SetAllowMockLocation(string valueJson);

		// +(void)setDynamicPublishableKey:(NSString * _Nonnull)valueJson;
		[Static]
		[Export("setDynamicPublishableKey:")]
		void SetDynamicPublishableKey(string valueJson);

		// +(void)setIsAvailable:(NSString * _Nonnull)valueJson;
		[Static]
		[Export("setIsAvailable:")]
		void SetIsAvailable(string valueJson);

		// +(void)setIsTracking:(NSString * _Nonnull)valueJson;
		[Static]
		[Export("setIsTracking:")]
		void SetIsTracking(string valueJson);

		// +(void)setMetadata:(NSString * _Nonnull)metadataJson;
		[Static]
		[Export("setMetadata:")]
		void SetMetadata(string metadataJson);

		// +(void)setName:(NSString * _Nonnull)valueJson;
		[Static]
		[Export("setName:")]
		void SetName(string valueJson);

		// +(void)setWorkerHandle:(NSString * _Nonnull)workerHandleJson;
		[Static]
		[Export("setWorkerHandle:")]
		void SetWorkerHandle(string workerHandleJson);

		// +(HyperTrackCancellable *)subscribeToErrors:(void (^)(NSString *))callback;
		[Static]
		[Export("subscribeToErrors:")]
		HyperTrackCancellable SubscribeToErrors(Action<NSString> callback);

		// +(HyperTrackCancellable *)subscribeToIsAvailable:(void (^)(NSString *))callback;
		[Static]
		[Export("subscribeToIsAvailable:")]
		HyperTrackCancellable SubscribeToIsAvailable(Action<NSString> callback);

		// +(HyperTrackCancellable *)subscribeToIsTracking:(void (^)(NSString *))callback;
		[Static]
		[Export("subscribeToIsTracking:")]
		HyperTrackCancellable SubscribeToIsTracking(Action<NSString> callback);

		// +(HyperTrackCancellable *)subscribeToLocation:(void (^)(NSString *))callback;
		[Static]
		[Export("subscribeToLocation:")]
		HyperTrackCancellable SubscribeToLocation(Action<NSString> callback);

		// +(HyperTrackCancellable *)subscribeToOrders:(void (^)(NSString *))callback;
		[Static]
		[Export("subscribeToOrders:")]
		HyperTrackCancellable SubscribeToOrders(Action<NSString> callback);

		// +(NSString * _Nonnull)orderIsInsideGeofence:(NSString * _Nonnull)orderHandle __attribute__((warn_unused_result("")));
		[Static]
		[Export("orderIsInsideGeofence:")]
		string OrderIsInsideGeofence(string orderHandle);
		
	}
}
