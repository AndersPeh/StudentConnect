using System;

namespace Application.Core;

// put a generic type for anything that uses handler.
public class Result<T>
{
    public bool IsSuccess { get; set; }

    public T? Value { get; set; }

    public string? Error { get; set; }

    // status code.
    public int Code { get; set; }

    // If handler is successful, must pass activity to here.
    public static Result<T> Success(T value) => new()
    {
        IsSuccess = true,
        Value = value
    };

    // If handler fails, must pass error message and error code.
    public static Result<T> Failure(string error, int code) => new()
    {
        IsSuccess = false,
        Error = error,
        Code = code,
    };
}
