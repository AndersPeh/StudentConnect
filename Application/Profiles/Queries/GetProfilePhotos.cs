using System;
using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles.Queries;

public class GetProfilePhotos
{
    public class Query : IRequest<Result<List<Photo>>>
    {
        // Not the current user's ID, users will use other user's Id to request to view photos of other user.
        // It will be derived from api endpoint parameter.
        public required string UserId { get; set; }
    }

    // IRequestHandler tells the Mediator that it processes GetProfilePhotos.Query and returns Result<List<Photo>>.
    public class Handler(AppDbContext context) : IRequestHandler<Query, Result<List<Photo>>>
    {
        public async Task<Result<List<Photo>>> Handle(Query request, CancellationToken cancellationToken)
        {
            // Find the user matching the Http Request UserId, select all photos of the User, convert the result into List.
            var photos = await context.Users
                .Where(user => user.Id == request.UserId)
                .SelectMany(user => user.Photos)
                .ToListAsync(cancellationToken);

            // This Result has to match both IRequest and IRequestHandler expected result.
            // Then it will be processed by HandleResult inherited from BaseApiController in the end to be Ok(result.Value).
            return Result<List<Photo>>.Success(photos);
        }
    }
}
