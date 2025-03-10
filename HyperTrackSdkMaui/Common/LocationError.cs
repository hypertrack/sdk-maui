// ReSharper disable CheckNamespace

namespace HyperTrack
{

    public static partial class HyperTrack
    {

        public abstract class LocationError
        {
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
            }

            public override string ToString() => "LocationError";
        }
    }
}
