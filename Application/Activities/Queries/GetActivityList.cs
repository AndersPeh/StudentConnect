using System.Diagnostics;
using Application.Activities.DTOs;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Queries;

public class GetActivityList
{
    // Query inherits from IRequest and returns Result<PagedList<ActivityDto, DateTime?>>.
    // Because PagedList<T, TCursor> takes 2 parameters, it returns ActivityDto as items and sets NextCursor as DateTime.
    // PagedList<ActivityDto, DateTime?> sets DateTime as nullable because there wont be any cursor in the end when all activities have been loaded.
    public class Query : IRequest<Result<PagedList<ActivityDto, DateTime?>>>
    {
        public required ActivityParams Params { get; set; }
    }

    // <Query, Result<PagedList<ActivityDto, DateTime?>>> means this Handler handles requests of type GetActivityList.Query 
    // and returns Result<PagedList<ActivityDto, DateTime?>>. 
    // Mediator instantiates the Handler to process the query, so DI refers to AppDbContext registered in API layer,
    public class Handler(AppDbContext context, IMapper mapper, IUserAccessor userAccessor) : IRequestHandler<Query, Result<PagedList<ActivityDto, DateTime?>>>
    {
        readonly string loggedInUserId = userAccessor.GetUserId();

        // Handle method uses EF Core in Persistence layer to query all rows from the database (ToListAsync), map result into ActivityDto objects from Domain layer,
        // Mediator will retun result to the ActivitiesController mediator.Send(new GetActivityList.Query()).
        public async Task<Result<PagedList<ActivityDto, DateTime?>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = context.Activities
            // Order by Cursor type, which is Activity's Date.
                        .OrderByDescending(activity => activity.Date)
                        // Index the Date property of Activity table for faster SQL query.
                        // Database query optimiser determines that Date index seek is faster than full table scan,
                        // so the database will use the index to run the query.
                        .Where(activity => activity.Date >= (request.Params.Cursor ?? request.Params.StartDate))
                        // Indicates the query has not been executed yet, building an expression tree.
                        .AsQueryable();

            if (!string.IsNullOrEmpty(request.Params.Filter))
            {
                query = request.Params.Filter switch
                {
                    "isGoing" => query.Where(activity =>
                        activity.Attendees.Any(eachAttendee => eachAttendee.UserId == loggedInUserId)),
                    "isHost" => query.Where(activity =>
                        activity.Attendees.Any(eachAttendee => eachAttendee.UserId == loggedInUserId && eachAttendee.IsHost)),
                    _ => query
                };
            }

            var projectedActivities = query.ProjectTo<ActivityDto>(mapper.ConfigurationProvider, new { currentUserId = loggedInUserId });

            var activities = await projectedActivities
            // Always take extra 1 activity for getting the Date of the last activity and Client can use it as a cursor.
                .Take(request.Params.PageSize + 1)
                // Materialisation method to call EF Core to translate the LINQ express tree to SQL query, then execute it.
                .ToListAsync(cancellationToken);

            DateTime? nextCursor = null;

            // If there are more activities returned than the pagesize of the request (do it intentionally),
            // the next cursor will be the date of the extra activity, so next batch will start from the last extra activity.
            if (activities.Count > request.Params.PageSize)
            {
                nextCursor = activities.Last().Date;
                // Remove the last extra activity to match the page size of the request.
                activities.RemoveAt(activities.Count - 1);
            }

            return Result<PagedList<ActivityDto, DateTime?>>.Success(
                new PagedList<ActivityDto, DateTime?>
                {
                    Items = activities,
                    NextCursor = nextCursor
                }
            );

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
