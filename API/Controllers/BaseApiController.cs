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
    }
}
