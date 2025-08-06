using System;
using Application.Core;
using Application.Profiles.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles.Queries;

public class GetProfile
{
    public class Query : IRequest<Result<UserProfile>>
    {
        // Not the current user's ID, users will use other user's Id to request to view profile of other user.
        // It will be derived from api endpoint parameter.
        public required string UserId { get; set; }
    }

    public class Handler(AppDbContext context, IMapper mapper) :
        IRequestHandler<Query, Result<UserProfile>>
    {
        public async Task<Result<UserProfile>> Handle(Query request, CancellationToken cancellationToken)
        {
            // Project Users to UserProfile Dto. ProjectTo select required columns only, avoid loading
            // sensitive fields like password hashes and navigation properties into memory, reducing risk and memory usage.
            // It translates mapper.ConfigurationProvider which is CreateMap<User, UserProfile>() into SQL Select Statements for necessary columns only.
            var profile = await context.Users
                .ProjectTo<UserProfile>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(user => user.Id == request.UserId, cancellationToken);

            return profile == null ?
                Result<UserProfile>.Failure("Profile Not Found.", 404) :
                Result<UserProfile>.Success(profile);
        }
    }
}
