using System;

namespace Application.Core;

// primary constructor with parameters: int statusCode, string message, string? details.
// send back stack trace to the client for debugging in development mode.
public class AppException(int statusCode, string message, string? details)
{
    public int StatusCode { get; set; } = statusCode;

    public string Message { get; set; } = message;

    public string? Details { get; set; } = details;
}
