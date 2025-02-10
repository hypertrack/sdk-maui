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
}
