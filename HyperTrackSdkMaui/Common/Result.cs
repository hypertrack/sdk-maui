// ReSharper disable CheckNamespace

namespace HyperTrack;

static partial class HyperTrack
{
    public class Result<T, TE> : IEquatable<Result<T, TE>>
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
            string ValueToString<TValue>(TValue? value)
            {
                if (value == null) return "null";
                if (value is IEnumerable<object> collection && value is not string)
                {
                    return $"[{string.Join(", ", collection)}]";
                }
                return value.ToString() ?? "null";
            }

            return IsSuccess
                ? $"Success({ValueToString(Success)})"
                : $"Failure({ValueToString(Failure)})";
        }

        public bool Equals(Result<T, TE>? other)
        {
            if (other is null) return false;
            if (IsSuccess != other.IsSuccess) return false;
            return IsSuccess
                ? EqualityComparer<T>.Default.Equals(Success, other.Success)
                : EqualityComparer<TE>.Default.Equals(Failure, other.Failure);
        }

        public override bool Equals(object? obj) => Equals(obj as Result<T, TE>);
        public override int GetHashCode() => IsSuccess ? Success?.GetHashCode() ?? 0 : Failure?.GetHashCode() ?? 0;
        public static bool operator ==(Result<T, TE>? left, Result<T, TE>? right) => left?.Equals(right) ?? right is null;
        public static bool operator !=(Result<T, TE>? left, Result<T, TE>? right) => !(left == right);
    }
}
