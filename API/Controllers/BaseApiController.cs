using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        // Only BaseApiController can access to _mediator.
        // _mediator is nullable.
        private IMediator? _mediator;

        // protected means Mediator will be available to this class and any class that inherits it.
        // When a class inherits from BaseApiController,it will have access to IMediator service.

        // Before requesting IMediator service in the beginning, _mediator is null. 
        // ??= will run GetService<IMediator> to request an IMediator instance from DI, DI checks IMediator registration in Program.cs,
        // then DI injects IMediator to _mediator, then _mediator will be assigned to Mediator.
        // If HttpContext() returns null, throw Invalid message.
        // After _mediator is assigned with IMediator, this method won't run anymore because ??= is not fulfilled.
        protected IMediator Mediator =>
            _mediator ??= HttpContext.RequestServices.GetService<IMediator>()
                ?? throw new InvalidOperationException("IMediator service is unavailable.");

        protected ActionResult HandleResult<T>(Result<T> result)
        {
            // Based on Result.cs, Failure will return an object with false IsSuccess and error code (like 404 not found for Get request case).
            if (!result.IsSuccess && result.Code == 404) return NotFound();

            // Success will return a Ok response with an object with true IsSuccess and activity result as value.
            if (result.IsSuccess && result.Value != null) return Ok(result.Value);

            return BadRequest(result.Error);
        }

    }
}
