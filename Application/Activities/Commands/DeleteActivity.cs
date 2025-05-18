using System;
using MediatR;
using Persistence;

namespace Application.Activities.Commands;

public class DeleteActivity
{
    public class Command : IRequest
    {
        public required string Id { get; set; }
    }

    // Handler that handles requests of type DeleteActivity.Command.
    public class Handler(AppDbContext context) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var activity = await context.Activities
                .FindAsync([request.Id], cancellationToken)
                 ?? throw new Exception("Cannot find activity.");

            // It only tracks activity in the memory, need to save later.
            context.Remove(activity);

            // if user triggers an event in the browser to cancel the event, 
            // cancellation token is sent to API Controller then to Mediator Handler to cancel that request from continuing.
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
