using System;
using Application.Activities.DTOs;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Queries;

public class GetActivityDetails
{
    // Query inherits from IRequest, making it a request that Mediator processes and it returns a Result object with data payload of ActivityDto object.
    public class Query : IRequest<Result<ActivityDto>>
    {
        public required string Id { get; set; }
    }

    // This Handler handles requests of type GetActivityDetails.Query.
    // IRequestHandler<in TRequest, TResponse>, it takes query as request and returns ActivityDto type result.
    public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Query, Result<ActivityDto>>
    {
        // must match Task<Result<Activity>> with IRequest<Result<Activity>>, because both specify what will be returned.
        public async Task<Result<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            // Loading before join table.
            // FindAsync finds row with request.Id and returns details of the specific activity.
            // var activity = await context.Activities
            //     .FindAsync([request.Id], cancellationToken);

            // Eager loading: loads too much unnecessary data.
            // var activity = await context.Activities
            // For the Activity object where its Id matches the request id, load its ActivityAttendee objects,
            // Then for each ActivityAttendee object, load its User object. (this includes PasswordHash, SecurityStamp, TwoFactorEnabled etc.)
            // need to use Projection method instead of eager loading to prevent loading unnecessary data.
            // .Include(x => x.Attendees)
            // .ThenInclude(x => x.User)
            // Finds the first recorsd or return null.
            // .FirstOrDefaultAsync(x => request.Id == x.Id, cancellationToken);

            // Projection method to load necessary data only. SQL query is much cleaner as Projection only selects data necessary to map Activity to ActivityDto.
            // context.Activities represents all Activity entities in the database, so AutoMapper knows the source type is Activity entity.
            var activity = await context.Activities
                // .ProjectTo<ActivityDto> tells AutoMapper to build a database query that starts with Activity entities and ends with ActivityDto objects.
                // AutoMapper looks at mapping configuration in MappingProfiles.cs, it sees CreateMap<Activity, ActivityDto>() rules.
                // It uses the mapping rules to build a LINQ.Select() expression for EF Core to translate it into SQL Query.
                // The Select() expression will only Select columns from Activities, ActivityAttendee and Users tables necessary to populate the ActivityDto.
                .ProjectTo<ActivityDto>(mapper.ConfigurationProvider)
                // condition in the LINQ expression for EF Core to include in the SQL query.
                .FirstOrDefaultAsync(activity => request.Id == activity.Id, cancellationToken);

            // Based on Result.cs, Result Failure object requires error message and code. return it to ActivitiesController.
            if (activity == null) return Result<ActivityDto>.Failure("Activity Not Found", 404);

            // Result Success object requires activity as value. return it to ActivitiesController.
            return Result<ActivityDto>.Success(activity);
        }
    }
}
