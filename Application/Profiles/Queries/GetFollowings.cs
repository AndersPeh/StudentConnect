using Application.Core;
using Application.Interfaces;
using Application.Profiles.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles.Queries;

public class GetFollowings
{
    public class Query : IRequest<Result<List<UserProfile>>>
    {
        // For selecting followers or followings.
        public string Predicate { get; set; } = "followers";

        // To retrieve any user's followings or followers instead of just the current user.
        public required string UserId { get; set; }
    }

    public class Handler(AppDbContext context, IMapper mapper, IUserAccessor userAccessor) : IRequestHandler<Query, Result<List<UserProfile>>>
    {
        public async Task<Result<List<UserProfile>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var profiles = new List<UserProfile>();

            switch (request.Predicate)
            {
                case "followers":
                    // Because context.UserFollowings, Where, Select and ProjectTo are just making query blueprint, they dont execute database query,
                    // they dont need Async.
                    profiles = await context.UserFollowings
                        // Filter to records Where the User selected is Target.
                        .Where(userFollowingRecord => userFollowingRecord.TargetId == request.UserId)
                        // From filtered records, only select the related Observer.
                        .Select(userFollowingRecord => userFollowingRecord.Observer)
                        // From the selected Observer navigation properties from Users table, only select specific columns from User table necessary to populate UserProfile Dto.
                        // Pass current user id to the Mapper to find out if the logged in user is following the selected user.
                        .ProjectTo<UserProfile>(mapper.ConfigurationProvider, new { currentUserId = userAccessor.GetUserId() })
                        // ToList needs Async as it translates Linq queries to Sql command, then wait for the database to execute the query,
                        // it then takes the data returned from the database and materialises it into  List<UserProfile>.
                        .ToListAsync(cancellationToken);
                    break;

                case "followings":
                    profiles = await context.UserFollowings
                        .Where(userFollowingRecord => userFollowingRecord.ObserverId == request.UserId)
                        .Select(userFollowingRecord => userFollowingRecord.Target)
                        .ProjectTo<UserProfile>(mapper.ConfigurationProvider, new { currentUserId = userAccessor.GetUserId() })
                        .ToListAsync(cancellationToken);
                    break;
            }

            return Result<List<UserProfile>>.Success(profiles);
        }
    }

}