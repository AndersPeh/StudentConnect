using System;
using FluentValidation;
using MediatR;

namespace Application.Core;

// TRequest and TResponse are generic placeholders that will be replaced by actual values when called, 
// generic so handlers with different types of Request/Response can call it.
// TRequest and TResponse are the expected types for request and response.
// Inject IValidator that processes IRequest (Command like an CreateActivityDto object), 
// it is nullable because not every request needs a validator.
public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    // IPipelineBehavior 
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    // Handle method takes in request, cancellationToken and next means next behavior in the pipeline (request handler).
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // process to handler when there is no validator for this request (validator files specify requests they take like AbstractValidator<CreateActivity.Command>).
        if (validator == null) return await next();

        // validator.ValidateAsync takes request, calls the instance of specific Validator injected (CreateActivityValidator) to execute validation rules againt the request object.
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // process to handler after validation.
        return await next();
    }
}
