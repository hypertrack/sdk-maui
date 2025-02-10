namespace HyperTrack
{
    partial class HyperTrack
    {

        public abstract class OrderStatus
        {
            public class ClockIn : OrderStatus { }
            public class ClockOut : OrderStatus { }
            public class Custom : OrderStatus
            {
                public string Status { get; }

                public Custom(string status)
                {
                    Status = status;
                }
            }
        }
    }
}

