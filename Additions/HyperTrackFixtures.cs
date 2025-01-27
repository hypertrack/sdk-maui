namespace Com.Hypertrack.Sdk.Android
{
    partial class HyperTrack
    {
        public partial class OrdersMap
        {

            public Java.Lang.Object Get(Java.Lang.Object? p0)
            {
                return GetJava((string?)p0);
            }

            public void PutAll(System.Collections.IDictionary p0)
            {
                PutAllJava((IDictionary<string, Order>?)p0);
            }

            public Java.Lang.Object Put(Java.Lang.Object key, Java.Lang.Object value)
            {
                return PutJava((string?)key, (Order?)value);
            }

            public Java.Lang.Object Remove(Java.Lang.Object key)
            {
                return RemoveJava((string?)key);
            }
            public System.Collections.ICollection EntrySet()
            {
                return (System.Collections.ICollection)EntrySetJava();
            }

            public System.Collections.ICollection KeySet()
            {
                return (System.Collections.ICollection)KeySetJava();
            }

            System.Collections.ICollection Java.Util.IMap.Values()
            {
                return (System.Collections.ICollection)ValuesJava();
            }

            // public System.Collections.ICollection IMap.Values()
            // {
            //     return (System.Collections.ICollection)ValuesJava();
            // }

            // public unsafe global::System.Collections.Generic.ICollection<global::Com.Hypertrack.Sdk.Android.HyperTrack.Order> Values {
			// 	// // Metadata.xml XPath method reference: path="/api/package[@name='com.hypertrack.sdk.android']/class[@name='HyperTrack.OrdersMap']/method[@name='getValues' and count(parameter)=0]"
			// 	// [Register ("getValues", "()Ljava/util/Collection;", "")]
			// 	get {
			// 		const string __id = "getValues.()Ljava/util/Collection;";
			// 		try {
			// 			var __rm = _members.InstanceMethods.InvokeAbstractObjectMethod (__id, this, null);
			// 			return global::Android.Runtime.JavaCollection<global::Com.Hypertrack.Sdk.Android.HyperTrack.Order>.FromJniHandle (__rm.Handle, JniHandleOwnership.TransferLocalRef)!;
			// 		} finally {
			// 		}
			// 	}
			// }
        }
    }
}
