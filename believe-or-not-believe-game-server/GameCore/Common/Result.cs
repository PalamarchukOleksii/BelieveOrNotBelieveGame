using GameCore.Enums;

namespace GameCore.Common;

public class Result
{
    public bool IsSuccess { get; }
    public Error? Error { get; }

    protected Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new Result(true, null);

    public static Result Failure(string code, string message, ErrorType type) =>
        new Result(false, new Error(code, message, type));
}

public class Result<T> : Result where T : notnull
{
    public T Value { get; }

    protected Result(bool isSuccess, T value, Error? error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new Result<T>(true, value, null);

    public static new Result<T> Failure(string code, string message, ErrorType type) =>
        new Result<T>(false, default!, new Error(code, message, type));
}