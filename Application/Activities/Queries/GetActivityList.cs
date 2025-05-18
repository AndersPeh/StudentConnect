using System;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Queries;

public class GetActivityList
{
    // Query receives a request that wants a response in List<Activity>.
    public class Query : IRequest<List<Activity>> { }

    // Handler provides response to the Query request.
    // Mediator instantiates the Handler to process the query, so DI refers AppDbContext registered in API layer,
    // then DI instantiates AppDbContext from Persistence layer and use constructor injection to inject to the Handler.

    public class Handler(AppDbContext context) : IRequestHandler<Query, List<Activity>>
    {
        // Handle method uses EF in Persistence layer to query the database, map result into Activity objects from Domain layer,
        // Mediator will retun result to the ActivitiesController mediator.Send(new GetActivityList.Query()).
        public async Task<List<Activity>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await context.Activities.ToListAsync(cancellationToken);
        }
    }
}
