using System;
using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities.Queries;

public class GetActivityDetails
{
    // Query inherits from IRequest, making it a request that Mediator processes and it returns a Result object with data payload of Activity object.
    public class Query : IRequest<Result<Activity>>
    {
        public required string Id { get; set; }
    }

    // This Handler handles requests of type GetActivityDetails.Query.
    // IRequestHandler defines types involved: Query and Result<Activity>.
    public class Handler(AppDbContext context) : IRequestHandler<Query, Result<Activity>>
    {
        // must match Task<Result<Activity>> with IRequest<Result<Activity>>, because both specify what will be returned.
        public async Task<Result<Activity>> Handle(Query request, CancellationToken cancellationToken)
        {
            // FindAsync finds row with request.Id and returns details of the specific activity.
            var activity = await context.Activities
                .FindAsync([request.Id], cancellationToken);

            // Based on Result.cs, Result Failure object requires error message and code. return it to ActivitiesController.
            if (activity == null) return Result<Activity>.Failure("Activity Not Found", 404);

            // Result Success object requires activity as value. return it to ActivitiesController.
            return Result<Activity>.Success(activity);
        }
    }
}
