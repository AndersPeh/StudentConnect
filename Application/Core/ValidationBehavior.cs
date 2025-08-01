using System;
using FluentValidation;
using MediatR;

namespace Application.Core;

// TRequest and TResponse are generic placeholders that will be replaced by actual values when called, 
// because handlers have different types of Request/Response.
// TRequest refers to Command or Query created in controller (like CreateActivity.Command).
// DI container injects IValidator that processes this specific TRequest (Command).
// It is nullable because not every request needs a validator.
// As ValidationBehavior returns to the Mediator, so it needs to take TResponse to know exactly what
// type to return (ie. ActionResult<ActivityDto>) so it wont return wrong data type causing Controller to have an error.
public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    // IPipelineBehavior puts this ValidationBehavior first in the pipeline before handler.
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    // Mediator.Send passes these from the controller: TRequest (Command), cancellationToken (if user cancels) and next = next behavior or handler in the pipeline.
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // process to handler (next in the pipeline) when there is no validator for this request (validator files specify requests they take like AbstractValidator<CreateActivity.Command>).
        if (validator == null) return await next();

        // validator.ValidateAsync takes request, calls the instance of specific Validator injected (like CreateActivityValidator) to execute validation rules againt the request object.
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            // This Exception will propagate upwards to ExceptionMiddleware.
            throw new ValidationException(validationResult.Errors);
        }

        // process to handler (next in the pipeline) after validation. Handler will return to ValidationBehavior 
        // and ValidationBehavior has to return to the Mediator, then Mediator will return the response to the Controller.
        return await next();
    }
}
