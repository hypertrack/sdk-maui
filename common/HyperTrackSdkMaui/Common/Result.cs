namespace HyperTrack;

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
   
    public static Result<T, E> Ok(T success)
    {
        return new Result<T, E>(success);
    }

    public static Result<T, E> Error(E error)
    {
        return new Result<T, E>(error);
    }

    public Result<U, E> Map<U>(Func<T, U> f)
    {
        return IsSuccess ? Result<U, E>.Ok(f(Success!)) : Result<U, E>.Error(Failure!);
    }

    public Result<T, F> MapFailure<F>(Func<E, F> f)
    {
        return IsSuccess ? Result<T, F>.Ok(Success!) : Result<T, F>.Error(f(Failure!));
    }
}
