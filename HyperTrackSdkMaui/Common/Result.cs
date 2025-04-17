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

        public static Result<T, TE> Ok(T success)
        {
            return new Result<T, TE>(success);
        }

        public static Result<T, TE> Error(TE error)
        {
            return new Result<T, TE>(error);
        }

        // ReSharper disable once UnusedMember.Global
        public Result<TU, TE> Map<TU>(Func<T, TU> f)
        {
            return IsSuccess ? Result<TU, TE>.Ok(f(Success!)) : HyperTrack.Result<TU, TE>.Error(Failure!);
        }

        // ReSharper disable once UnusedMember.Global
        public Result<T, TF> MapFailure<TF>(Func<TE, TF> f)
        {
            return IsSuccess ? HyperTrack.Result<T, TF>.Ok(Success!) : Result<T, TF>.Error(f(Failure!));
        }

        public override string ToString()
        {
            return IsSuccess
                ? $"Success({ValueToString(Success)})"
                : $"Failure({ValueToString(Failure)})";

            string ValueToString<TValue>(TValue? value)
            {
                if (value == null) return "null";
                if (value is IEnumerable<object> collection && value is not string)
                {
                    return $"[{string.Join(", ", collection)}]";
                }
                return value.ToString() ?? "null";
            }
        }

        private static bool ValueEquals<TValue>(TValue? left, TValue? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left == null || right == null) return false;

            // Handle collections (except strings)
            if (left is IEnumerable<object> leftCollection && right is IEnumerable<object> rightCollection
                && left is not string && right is not string)
            {
                return leftCollection.OrderBy(t => t).SequenceEqual(rightCollection.OrderBy(t => t));
            }

            return EqualityComparer<TValue>.Default.Equals(left, right);
        }

        public bool Equals(Result<T, TE>? other)
        {
            if (other is null) return false;
            if (IsSuccess != other.IsSuccess) return false;
            return IsSuccess
                ? ValueEquals(Success, other.Success)
                : ValueEquals(Failure, other.Failure);
        }

        public override bool Equals(object? obj) => Equals(obj as Result<T, TE>);
        public override int GetHashCode() => IsSuccess ? Success?.GetHashCode() ?? 0 : Failure?.GetHashCode() ?? 0;
        public static bool operator ==(Result<T, TE>? left, Result<T, TE>? right) => left?.Equals(right) ?? right is null;
        public static bool operator !=(Result<T, TE>? left, Result<T, TE>? right) => !(left == right);
    }
}
