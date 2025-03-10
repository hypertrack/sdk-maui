// ReSharper disable CheckNamespace

namespace HyperTrack
{

    public static partial class HyperTrack
    {

        public abstract class LocationError : IEquatable<LocationError>
        {
            public virtual bool Equals(LocationError? other) => other?.GetType() == GetType();
            public override bool Equals(object? obj) => Equals(obj as LocationError);
            public override int GetHashCode() => GetType().GetHashCode();
            public static bool operator ==(LocationError? left, LocationError? right) => left?.Equals(right) ?? right is null;
            public static bool operator !=(LocationError? left, LocationError? right) => !(left == right);

            public class NotRunning : LocationError
            {
                public override string ToString() => "LocationError.NotRunning";
            }
            public class Starting : LocationError
            {
                public override string ToString() => "LocationError.Starting";
            }
            public class Errors : LocationError
            {
                public HashSet<Error> ErrorSet { get; }

                public Errors(HashSet<Error> errors)
                {
                    ErrorSet = errors;
                }

                public override string ToString() => $"LocationError.Errors({string.Join(", ", ErrorSet)})";

                public override bool Equals(LocationError? other)
                {
                    if (other is not Errors errors) return false;
                    return ErrorSet.SetEquals(errors.ErrorSet);
                }

                public override int GetHashCode() =>
                    ErrorSet.Aggregate(0, (hash, error) => hash ^ error.GetHashCode());
            }

            public override string ToString() => "LocationError";
        }
    }
}
