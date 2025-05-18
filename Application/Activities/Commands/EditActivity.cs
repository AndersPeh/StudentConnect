using System;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities.Commands;

public class EditActivity
{
    public class Command : IRequest
    {
        public required Activity Activity { get; set; }
    }

    // This Handler handles requests of type EditActivity.Command.
    // Mediator asks DI container to creaye an instance of EditActivity.Handler, 
    // DI container knows how to provide AppDbContext and IMapper because they're registered in Program.cs.
    // DI gets configuration from Program.cs to instantiates IMapper with mapping rules from MappingProfiles.cs.
    // DI inject IMapper to do automatic mapping instead of typing activity property one by one (like activity.Title = request.Activity.Title;).
    public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var activity = await context.Activities
                .FindAsync([request.Activity.Id], cancellationToken)
                    ?? throw new Exception("Cannot find activity.");

            // automatically map everything in Activity to activity.
            mapper.Map(request.Activity, activity);

            // save changes to database after auto mapping.
            await context.SaveChangesAsync(cancellationToken);

        }
    }
}
