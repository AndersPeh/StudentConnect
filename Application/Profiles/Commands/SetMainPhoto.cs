using System;
using Application.Core;
using Application.Interfaces;
using MediatR;
using Persistence;

namespace Application.Profiles.Commands;

public class SetMainPhoto
{
    // Just need to know the PhotoId that the user wishes to set as main photo.
    public class Command : IRequest<Result<Unit>>
    {
        public required string PhotoId { get; set; }
    }


    public class Handler(AppDbContext context, IUserAccessor userAccessor) :
        IRequestHandler<Command, Result<Unit>>

    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userAccessor.GetUserWithPhotosAsync();

            var photo = user.Photos.FirstOrDefault(photo => photo.Id == request.PhotoId);

            if (photo == null) return Result<Unit>.Failure("Cannot find photo", 400);

            // If the user sets same photo as current main photo, it will provide same photo.Url for ImageUrl, which means there is no changes.
            // so SaveChangesAsync wont be > 0, resulting in 400 Bad Request error.
            user.ImageUrl = photo.Url;

            var result = await context.SaveChangesAsync(cancellationToken) > 0;

            return result ?
                Result<Unit>.Success(Unit.Value) :
                Result<Unit>.Failure("Problem changing main photo", 400);
        }
    }
}
