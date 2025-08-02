using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Application.Core;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ValidationException = FluentValidation.ValidationException;

namespace API.Middleware;
// IMiddleware tells .Net that ExceptionMiddleware is a middleware component that should be activated via Dependency Injection.

// This middleware is placed first in Program.cs, so any exception in the pipeline can progagate upwards to reach it in the end.
// When an exception occurs within Mediator handler and handler doesnt catch it, it will propagate upwards to Mediator pipeline:
// validation behavior followed by handler. 
// If no service can capture the exception, it will propagate out of the Mediator pipeline to controller action (Controller sends Command using Mediator.send),
// If the exception is still not captured, it will propagate to middleware pipeline, so ExceptionMiddleware.

// DI container injects ILogger<ExceptionMiddleware> to handle generic error.
// and IHostEnvironment (to tell if the app is being run in development or production).
// ILogger<ExceptionMiddleware> logger specifies that this logger is configured for logging messages from ExceptionMiddleware.
public class ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IHostEnvironment env) : IMiddleware
{
    // HttpContext = HTTP request and response, next = next in the middleware pipeline to process HTTP context.
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            // process to next in the pipeline with HttpContext if no exception.
            // In mediator pipeline, Mediator passes the request to each step, no need to pass anything.
            // However, middleware pipeline is generic and stateless, it doesnt know which HTTP context is being processed.
            // Each middleware must explicitly pass it to the next middleware.
            await next(context);
        }
        catch (ValidationException ex)
        {
            // if FluentValidation.ValidationException is caught, call HandleValidationException to process it.
            await HandleValidationException(context, ex);
        }
        // for catching generic error other than validation error.
        catch (Exception ex)
        {

            await HandleException(context, ex);
        }
    }

    private async Task HandleException(HttpContext context, Exception ex)
    {
        logger.LogError(ex, ex.Message);

        // Send back error response in json so it's easier to work with in client side code.
        context.Response.ContentType = "application/json";

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        // send back stacktrace only if it is in development mode. ex.Message refers to error message specified in ActivitesController.
        var response = env.IsDevelopment()
            ? new AppException(context.Response.StatusCode, ex.Message, ex.StackTrace)
            : new AppException(context.Response.StatusCode, ex.Message, null);

        // return as serialised JSON in CamelCase format (standard format).
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        // serialise response in options format. 
        var json = JsonSerializer.Serialize(response, options);

        // if use WriteAsJsonAsync, JsonSerializerOptions and JsonSerializer steps can be removed.
        await context.Response.WriteAsync(json);
    }

    // HTTP response will show errors in Error response body containing dictionary like:
    // "errors" : {
    //  "ActivityDto.Title : [
    //      "Title is required"
    //   ]
    // }
    // Make it private encapsulates this logic within the class, only the class can use it.
    // Make it static because it operates purely on its inputs only.
    // HttpContext for forming Error response.
    private static async Task HandleValidationException(HttpContext context, ValidationException ex)
    {
        // Dictionary that stores string key (error.PropertyName) and string [] value (existingErrors).
        var validationErrors = new Dictionary<string, string[]>();

        // content of dictionary, PropertyName and ErrorMessage follow CreateActivityValidator. 
        // like RuleFor(x => x.ActivityDto.Title).NotEmpty().WithMessage("Title is required"); 
        if (ex.Errors is not null)
        {
            foreach (var error in ex.Errors)
            {
                // error example: Each error has a PropertyName (e.g., "ActivityDto.Title") and an ErrorMessage (e.g., "Title is required").
                // check if validationErrors contains the current error (error.PropertyName),
                // if it already contains, it means the error.PropertyName already has an array of existingErrors. 
                // So the array of existingErrors will be appended with more ErrorMessage (error.ErrorMessage).
                // Else, existingErrors will be null, so set error.ErrorMessage as the existingErrors.
                // Needs to check first because if we do validationErrors[error.PropertyName] = [error.ErrorMessage] everytime,
                // the previous error.ErrorMessage in existingErrors will be overridden.
                if (validationErrors.TryGetValue(error.PropertyName, out var existingErrors))
                {
                    // validationErrors is the dict, with key error.PropertyName has value existingErrors. 
                    // existingErrors is appended with new value through error.ErrorMessage.
                    // Instead of writing existingErrors.Append(error.ErrorMessage).ToArray(), write collection expression
                    // [.. existingErrors, error.ErrorMessage], the .. means append, the [] means array, append error.ErrorMessage to existingErrors.
                    validationErrors[error.PropertyName] = [.. existingErrors, error.ErrorMessage];
                }
                else
                {
                    validationErrors[error.PropertyName] = [error.ErrorMessage];

                }
                // Example Final response looks like this:
                // {
                //   "errors": {
                //     "ActivityDto.Title": [
                //       "Title is required",
                //       "Title must be at least 3 characters"
                //     ],
                //     "ActivityDto.Date": [
                //       "Date must be in the future"]}}
            }
        }
        // Error Response Header.
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        // Error response body which will be displayed with validation errors dictionary.
        var validationProblemDetails = new ValidationProblemDetails(validationErrors)
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "ValidationFailure",
            Title = "Validation error",
            Detail = "One or more validation errors has occured."
        };
        // serialise validationProblemDetails into a JSON string which will become response body.
        await context.Response.WriteAsJsonAsync(validationProblemDetails);
    }

}
