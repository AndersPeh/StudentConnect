using System;
using Application.Core;
using MediatR;
using Persistence;

namespace Application.Activities.Commands;

public class DeleteActivity
{
    // This Command return Result object with data payload of void.
    public class Command : IRequest<Result<Unit>>
    {
        public required string Id { get; set; }
    }

    // Handler that handles requests of type DeleteActivity.Command.
    public class Handler(AppDbContext context) : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var activity = await context.Activities
                .FindAsync([request.Id], cancellationToken);

            // If failure, returns error message and code to Controller.
            if (activity == null) return Result<Unit>.Failure("Activity Not Found", 404);

            // It only tracks activity in the memory, need to save later.
            context.Remove(activity);

            // if user triggers an event in the browser to cancel the event, 
            // cancellation token is sent to API Controller then to Mediator Handler to cancel that request from continuing.
            // when SaveChangesAsync is successful, it will return a number representing number of entries written to the database, > 0 means it was successfully written.
            var result = await context.SaveChangesAsync(cancellationToken) > 0;

            // if nothing was written to the database, result will be false.
            if (!result) return Result<Unit>.Failure("Failed to delete the activity.", 400);

            // return Result object with success to Controller.result.Value can't null when success, pass Unit.value.
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
