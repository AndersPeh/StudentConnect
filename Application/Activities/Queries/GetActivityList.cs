using System;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Queries;

public class GetActivityList
{
    // Query inherits from IRequest and returns List<Activity>. It is an empty class without method or property.
    public class Query : IRequest<List<Activity>> { }

    // <Query, List<Activity>> means this Handler handles requests of type GetActivityList.Query and returns  List<Activity>. 
    // Mediator instantiates the Handler to process the query, so DI refers AppDbContext registered in API layer,
    // then DI instantiates AppDbContext from Persistence layer and use constructor injection to inject to the Handler.

    public class Handler(AppDbContext context) : IRequestHandler<Query, List<Activity>>
    {
        // Handle method uses EF in Persistence layer to query all rows from the database (ToListAsync), map result into Activity objects from Domain layer,
        // Mediator will retun result to the ActivitiesController mediator.Send(new GetActivityList.Query()).
        public async Task<List<Activity>> Handle(Query request, CancellationToken cancellationToken)
        {

            // cancel this operation if cancellationToken is provided.
            return await context.Activities.ToListAsync(cancellationToken);
        }
    }
}
