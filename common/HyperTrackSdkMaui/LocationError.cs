
namespace HyperTrack
{

    public partial class HyperTrack
    {

        public abstract class LocationError
        {
            public class NotRunning : LocationError { }
            public class Starting : LocationError { }
            public class Errors : LocationError
            {
                public HashSet<Error> ErrorSet { get; }

                public Errors(HashSet<Error> errors)
                {
                    ErrorSet = errors;
                }
            }
        }
    }
}
