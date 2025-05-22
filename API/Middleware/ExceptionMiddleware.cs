using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ValidationException = FluentValidation.ValidationException;

namespace API.Middleware;

// This middleware is placed first in Program.cs, so any exception in the pipeline can progagate upwards to reach it in the end.
// When an exception occurs within Mediator handler and handler doesnt catch it, it will propagate upwards to Mediator pipeline (behavior top, handler bottom). 
// If no service can capture the exception, it will propagate out ot the Mediator pipeline to controller action (Controller sends Command using Mediator.send),
// If the exception is still not captured, it will propagate to middleware pipeline, so ExceptionMiddleware. 
public class ExceptionMiddleware : IMiddleware
{
    // HttpContext = HTTP request and response, next = next in the middleware pipeline.
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

            Console.WriteLine(ex);
        }
    }

    // HTTP response will show errors containing dictionary like:
    // "errors" : {
    //  "ActivityDto.Title : [
    //      "Title is required"
    //   ]
    // }
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

        // Error response body.
        var validationProblemDetails = new ValidationProblemDetails(validationErrors)
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "ValidationFailure",
            Title = "Validation error",
            Detail = "One or more validation errors has occured."
        };

        await context.Response.WriteAsJsonAsync(validationProblemDetails);
    }

}
