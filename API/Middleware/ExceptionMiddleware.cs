using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Application.Core;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ValidationException = FluentValidation.ValidationException;

namespace API.Middleware;

// This middleware is placed first in Program.cs, so any exception in the pipeline can progagate upwards to reach it in the end.
// When an exception occurs within Mediator handler and handler doesnt catch it, it will propagate upwards to Mediator pipeline (behavior top, handler bottom). 
// If no service can capture the exception, it will propagate out ot the Mediator pipeline to controller action (Controller sends Command using Mediator.send),
// If the exception is still not captured, it will propagate to middleware pipeline, so ExceptionMiddleware.

// DI container injects ILogger<ExceptionMiddleware> and IHostEnvironment (to tell if the app is being run in development or production).
public class ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IHostEnvironment env) : IMiddleware
{
    // HttpContext = HTTP request and response, next = next in the middleware pipeline to process HTTP context.
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            // process to next in the pipeline with HttpContext if no exception.
            await next(context);
        }
        catch (ValidationException ex)
        {
            // if ValidationException is caught, call HandleValidationException to process it.
            await HandleValidationException(context, ex);
        }

        catch (Exception ex)
        {

            await HandleException(context, ex);
        }
    }

    private async Task HandleException(HttpContext context, Exception ex)
    {
        logger.LogError(ex, ex.Message);

        // Send back error response in json so it's easier to work with in client cide code.
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
                if (validationErrors.TryGetValue(error.PropertyName, out var existingErrors))
                {
                    validationErrors[error.PropertyName] = [.. existingErrors, error.ErrorMessage];
                }
                else
                {
                    validationErrors[error.PropertyName] = [error.ErrorMessage];

                }
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
