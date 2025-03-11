// ReSharper disable CheckNamespace

namespace HyperTrack
{
    static partial class HyperTrack
    {
        public abstract class Error : IEquatable<Error>, IComparable, IComparable<Error>
        {
            public virtual bool Equals(Error? other) => other?.GetType() == GetType();
            public override bool Equals(object? obj) => Equals(obj as Error);
            public override int GetHashCode() => GetType().GetHashCode();
            public static bool operator ==(Error? left, Error? right) => left?.Equals(right) ?? right is null;
            public static bool operator !=(Error? left, Error? right) => !(left == right);

            public override string ToString() => GetType().Name;

            public int CompareTo(Error? other)
            {
                if (other is null) return 1;
                return string.Compare(ToString(), other.ToString(), StringComparison.Ordinal);
            }

            public int CompareTo(object? obj)
            {
                if (obj is null) return 1;
                if (obj is Error other) return CompareTo(other);
                throw new ArgumentException("Object is not an Error");
            }

            public class BlockedFromRunning : Error { }

            public class InvalidPublishableKey : Error { }

            public abstract class Location : Error
            {
                public class Mocked : Location { }

                public class ServicesDisabled : Location { }

                public class ServicesUnavailable : Location { }

                public class SignalLost : Location { }
            }

            public class NoExemptionFromBackgroundStartRestrictions : Error { }

            public abstract class Permissions : Error
            {
                public abstract class Location : Permissions
                {
                    public class Denied : Location { }

                    public class InsufficientForBackground : Location { }

                    public class ReducedAccuracy : Location { }

                    public class NotDetermined : Location { }

                    public class Provisional : Location { }

                    public class Restricted : Location { }
                }

                [Obsolete("Notifications permission is not required anymore")]
                public abstract class Notifications : Permissions
                {
                    [Obsolete("Notifications permission is not required anymore")]
                    public class Denied : Notifications { }
                }
            }
        }
    }
}
