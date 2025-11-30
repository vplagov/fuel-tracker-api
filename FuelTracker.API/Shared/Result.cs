namespace FuelTracker.API.Shared;

public class Result
{
    protected Result(bool isSuccess, ErrorType? errorType = null, string? errorMessage = null)
    {
        if (isSuccess && !string.IsNullOrEmpty(errorMessage) ||
            !isSuccess && string.IsNullOrEmpty(errorMessage))
        {
            throw new ArgumentException("Invalid result");
        }
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        ErrorType = errorType;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? ErrorMessage { get; }
    public ErrorType? ErrorType { get; }

    public static Result Success() => new(true);
    public static Result Failure(ErrorType errorType, string errorMessage) => 
        new(false, errorType, errorMessage);
}

public class Result<T> : Result
{
    private Result(T? value, bool isSuccess, ErrorType? errorType = null, string? errorMessage = null) : 
        base(isSuccess, errorType, errorMessage)
    {
        Value = value;
    }
    
    public T? Value { get; }
    
    public static Result<T> Success(T value) => new(value, true);
    
    public new static Result<T> Failure(ErrorType errorType, string errorMessage) => 
        new(default, false, errorType, errorMessage);
}