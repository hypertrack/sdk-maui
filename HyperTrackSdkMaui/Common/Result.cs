namespace HyperTrack;

static partial class HyperTrack
{
    public class Result<T, E>
    {
        public T? Success { get; }
        public E? Failure { get; }
        public bool IsSuccess { get; }

        private Result(T success)
        {
            Success = success;
            IsSuccess = true;
        }

        private Result(E error)
        {
            Failure = error;
            IsSuccess = false;
        }

        public static HyperTrack.Result<T, E> Ok(T success)
        {
            return new HyperTrack.Result<T, E>(success);
        }

        public static HyperTrack.Result<T, E> Error(E error)
        {
            return new HyperTrack.Result<T, E>(error);
        }

        public HyperTrack.Result<U, E> Map<U>(Func<T, U> f)
        {
            return IsSuccess ? HyperTrack.Result<U, E>.Ok(f(Success!)) : HyperTrack.Result<U, E>.Error(Failure!);
        }

        public HyperTrack.Result<T, F> MapFailure<F>(Func<E, F> f)
        {
            return IsSuccess ? HyperTrack.Result<T, F>.Ok(Success!) : HyperTrack.Result<T, F>.Error(f(Failure!));
        }
    }
}
