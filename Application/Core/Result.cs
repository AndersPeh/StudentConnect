using System;

namespace Application.Core;

// put a generic type for anything that uses handler.
// In handlers, we always return using static method of this class like return Result<string>.Success(activity.Id).
public class Result<T>
{
    public bool IsSuccess { get; set; }

    public T? Value { get; set; }

    public string? Error { get; set; }

    // status code.
    public int Code { get; set; }

    // If handler is successful, must pass activity to here for value. Success return object Result of generic type. value refers to the <T> of Result.
    // For example, when GetActivityDetails Handler returns Result<Activity>.Success(activity), value will becomes activity.
    // Success and Failure static methods return an object with different messages to the Controller.
    public static Result<T> Success(T value) => new()
    {
        IsSuccess = true,
        Value = value
    };

    // If handler fails, must pass error message and error code.
    // For example, return Result<string>.Failure("Failed to create the activity", 400);
    public static Result<T> Failure(string error, int code) => new()
    {
        IsSuccess = false,
        Error = error,
        Code = code,
    };
}
