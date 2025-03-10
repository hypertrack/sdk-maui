// ReSharper disable CheckNamespace

namespace HyperTrack;

static partial class HyperTrack
{
    public class Result<T, TE>
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public T? Success { get; }
        // ReSharper disable once MemberCanBePrivate.Global
        public TE? Failure { get; }
        // ReSharper disable once MemberCanBePrivate.Global
        public bool IsSuccess { get; }

        private Result(T success)
        {
            Success = success;
            IsSuccess = true;
        }

        private Result(TE error)
        {
            Failure = error;
            IsSuccess = false;
        }

        public static HyperTrack.Result<T, TE> Ok(T success)
        {
            return new HyperTrack.Result<T, TE>(success);
        }

        public static HyperTrack.Result<T, TE> Error(TE error)
        {
            return new HyperTrack.Result<T, TE>(error);
        }

        public HyperTrack.Result<U, TE> Map<U>(Func<T, U> f)
        {
            return IsSuccess ? HyperTrack.Result<U, TE>.Ok(f(Success!)) : HyperTrack.Result<U, TE>.Error(Failure!);
        }

        public HyperTrack.Result<T, TF> MapFailure<TF>(Func<TE, TF> f)
        {
            return IsSuccess ? HyperTrack.Result<T, TF>.Ok(Success!) : HyperTrack.Result<T, TF>.Error(f(Failure!));
        }

        public override string ToString()
        {
            return IsSuccess ? $"Success({Success})" : $"Failure({Failure})";
        }
    }
}
