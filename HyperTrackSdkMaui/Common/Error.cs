// ReSharper disable CheckNamespace

namespace HyperTrack
{
    static partial class HyperTrack
    {
        public abstract class Error
        {
            public class BlockedFromRunning : Error
            {
            }

            public class InvalidPublishableKey : Error
            {
            }

            public abstract class Location : Error
            {
                public class Mocked : Location
                {
                }

                public class ServicesDisabled : Location
                {
                }

                public class ServicesUnavailable : Location
                {
                }

                public class SignalLost : Location
                {
                }
            }

            public class NoExemptionFromBackgroundStartRestrictions : Error
            {
            }

            public abstract class Permissions : Error
            {
                public abstract class  Location : Permissions
                {
                    public class Denied : Location
                    {
                    }

                    public class InsufficientForBackground : Location
                    {
                    }

                    public class ReducedAccuracy : Location
                    {
                    }
                    
                    public class NotDetermined : Location
                    {
                    }

                    public class Provisional : Location
                    {
                    }

                    public class Restricted : Location
                    {
                    }
                }

                [Obsolete("Notifications permission is not required anymore")]
                public abstract class Notifications : Permissions
                {
                    [Obsolete("Notifications permission is not required anymore")]
                    public class Denied : Notifications
                    {
                    }
                }
            }
        }
    }
}
