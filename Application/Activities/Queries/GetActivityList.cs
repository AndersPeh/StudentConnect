using System;
using Application.Activities.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Queries;

public class GetActivityList
{
    // Query inherits from IRequest and returns List<Activity>. It is an empty class without method or property.
    public class Query : IRequest<List<ActivityDto>> { }

    // <Query, List<Activity>> means this Handler handles requests of type GetActivityList.Query and returns  List<Activity>. 
    // Mediator instantiates the Handler to process the query, so DI refers to AppDbContext registered in API layer,
    // then DI instantiates AppDbContext from Persistence layer and use constructor injection to inject to the Handler.

    public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Query, List<ActivityDto>>
    {
        // Handle method uses EF Core in Persistence layer to query all rows from the database (ToListAsync), map result into ActivityDto objects from Domain layer,
        // Mediator will retun result to the ActivitiesController mediator.Send(new GetActivityList.Query()).
        public async Task<List<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
        {

            // cancel this operation if cancellationToken is provided.
            return await context.Activities
                // AutoMapper knows the source type is Activity based on context.Activities (it represents all Activity entities),
                // It also knows destination type is ActivityDto from .ProjectTo<ActivityDto>.
                // ConfigurationProvider holds mapping rules from MappingProfiles.cs, CreateMap<Activity, ActivityDto>() matches the criteria.
                // It generates Select() expression that only select necessary data to transform Activity into ActivityDto, 
                // omitting unnecessary columns.
                // Then the ActivityDto results will be converted into a list.
                .ProjectTo<ActivityDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
    }
}

// Without using ProjectTo, need to eagerly load everything from the database for Attendees and User entities. It will load unnecessary data,
// increasing the network trafic (PasswordHash, Email etc).

// return await context.Activities
//  .Include( x => x.Attendees)
//  .ThenInclude( x => x.User)
//  .FirstOrDefaultAsync( x => request.Id == x.Id, cancellationToken);
//  .ToListAsync(cancellationToken);
