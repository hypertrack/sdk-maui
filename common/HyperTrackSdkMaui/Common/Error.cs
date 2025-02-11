namespace HyperTrack
{
    partial class HyperTrack
    {
        public abstract class Error
        {
            public class BlockedFromRunning : Error { }
            public class InvalidPublishableKey : Error { }

            public abstract class Location : Error
            {
                public class Mocked : Location { }
                public class ServicesDisabled : Location { }
                public class ServicesUnavailable : Location { }
                public class SignalLost : Location { }
            }

            public class NoExemptionFromBackgroundStartRestrictions : Error
            {
            }

            public abstract class Permissions : Error
            {

                public abstract class Location : Permissions
                {
                    public class Denied : Location { }
                    public class InsufficientForBackground : Location { }
                    public class ReducedAccuracy : Location { }
                }
            }
        }
    }
}
