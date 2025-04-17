// ReSharper disable CheckNamespace

namespace HyperTrack
{
    static partial class HyperTrack
    {

        public abstract class OrderStatus
        {
            public class ClockIn : OrderStatus { }
            public class ClockOut : OrderStatus { }
            public class Custom : OrderStatus
            {
                public string Value { get; }

                public Custom(string status)
                {
                    Value = status;
                }
            }
        }
    }
}

