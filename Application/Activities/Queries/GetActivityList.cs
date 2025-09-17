using Application.Activities.DTOs;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Queries;

public class GetActivityList
{
    // User can only request a maximum of 50 records from the server.
    private const int MaxPageSize = 50;

    // Query inherits from IRequest and returns Result<PagedList<ActivityDto, DateTime?>>.
    // Because PagedList<T, TCursor> takes 2 parameters, it returns ActivityDto as items and sets NextCursor as DateTime.
    // PagedList<ActivityDto, DateTime?> sets DateTime as nullable because there wont be any cursor in the end when all activities have been loaded.
    public class Query : IRequest<Result<PagedList<ActivityDto, DateTime?>>>
    {
        // Use DateTime as the cursor to indicate the next starting point.
        public DateTime? Cursor { get; set; }

        // private backing field _pageSize is guarded by PageSize for preventing DOS attack.
        private int _pageSize = 3;

        // PageSize ensures the _pageSize wont exceed MaxPageSize by 
        // checking the request page size and compare with the MaxPageSize.
        public int PageSize
        {
            // It is essentially the _pageSize.
            get => _pageSize;

            // If user requests more than MaxPageSize, returns MaxPageSize to limit the user's request.
            // If user requests less than MaxPageSize, returns the amount that the user requests.
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

    }

    // <Query, Result<PagedList<ActivityDto, DateTime?>>> means this Handler handles requests of type GetActivityList.Query 
    // and returns Result<PagedList<ActivityDto, DateTime?>>. 
    // Mediator instantiates the Handler to process the query, so DI refers to AppDbContext registered in API layer,
    public class Handler(AppDbContext context, IMapper mapper, IUserAccessor userAccessor) : IRequestHandler<Query, Result<PagedList<ActivityDto, DateTime?>>>
    {
        // Handle method uses EF Core in Persistence layer to query all rows from the database (ToListAsync), map result into ActivityDto objects from Domain layer,
        // Mediator will retun result to the ActivitiesController mediator.Send(new GetActivityList.Query()).
        public async Task<Result<PagedList<ActivityDto, DateTime?>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = context.Activities
            // Order by Cursor type, which is Activity's Date.
                        .OrderByDescending(Activity => Activity.Date)
                        // Indicates the query has not been executed yet, building an expression tree.
                        .AsQueryable();

            // If Cursor has the DateTime value, then start from the Cursor value.
            // The Cursor comes from the extra activity in Take, to use as the starting point of next batch.
            if (request.Cursor.HasValue)
            {
                // Index the Date property of Activity table for faster SQL query.
                // Database query optimiser determines that Date index seek is faster than full table scan,
                // so the database will use the index to run the query.
                query = query.Where(Activity => Activity.Date <= request.Cursor.Value);
            }

            var activities = await query
            // Always take extra 1 activity for getting the Date of the last activity and Client can use it as a cursor.
                .Take(request.PageSize + 1)

                // AutoMapper knows the source type is Activity based on context.Activities (it represents all Activity entities),
                // It also knows destination type is ActivityDto from .ProjectTo<ActivityDto>.
                // ConfigurationProvider holds mapping rules from MappingProfiles.cs, CreateMap<Activity, ActivityDto>() matches the criteria.
                // It generates Select() expression that only select necessary data to transform Activity into ActivityDto, omitting unnecessary columns.
                // Then the ActivityDto results will be converted into a list.
                .ProjectTo<ActivityDto>(mapper.ConfigurationProvider, new { currentUserId = userAccessor.GetUserId() })

                // Materialisation method to call EF Core to translate the LINQ express tree to SQL query, then execute it.
                .ToListAsync(cancellationToken);

            DateTime? nextCursor = null;

            // If there are more activities returned than the pagesize of the request (do it intentionally),
            // the next cursor will be the date of the extra activity, so next batch will start from the last extra activity.
            if (activities.Count > request.PageSize)
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
