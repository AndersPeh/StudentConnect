using System;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities.Queries;

public class GetActivityDetails
{
    // Query inherits from IRequest, making it a request that Mediator processes and it returns an Activity object.
    public class Query : IRequest<Activity>
    {
        public required string Id { get; set; }
    }

    // This Handler handles requests of type GetActivityDetails.Query.
    public class Handler(AppDbContext context) : IRequestHandler<Query, Activity>
    {
        public async Task<Activity> Handle(Query request, CancellationToken cancellationToken)
        {
            // FindAsync finds row with request.Id and returns details of the specific activity.
            // if activity is null (??), throw Exception.
            var activity = await context.Activities
                .FindAsync([request.Id], cancellationToken)
                    ?? throw new Exception("Activity not found");

            return activity;
        }
    }
}
