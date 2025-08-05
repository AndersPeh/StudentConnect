using System;
using Application.Core;
using Application.Interfaces;
using MediatR;
using Persistence;

namespace Application.Profiles.Commands;

public class DeletePhoto
{
    // Just need to know the PhotoId that the user wishes to delete.
    public class Command : IRequest<Result<Unit>>
    {
        public required string PhotoId { get; set; }
    }

    public class Handler(IUserAccessor userAccessor, AppDbContext context, IPhotoService photoService) :
        IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userAccessor.GetUserWithPhotosAsync();

            // no need to use FirstOrDefaultAsync because we are not querying from the database, just get it from the user object.
            var photo = user.Photos.FirstOrDefault(eachPhoto => eachPhoto.Id == request.PhotoId);

            if (photo == null) return Result<Unit>.Failure("Cannot find photo", 400);

            // Main photo stored as ImageUrl in User entity shouldnt be deleted.
            if (photo.Url == user.ImageUrl) return Result<Unit>.Failure("Cannot delete main photo", 400);

            // Cloudinary only requires PublicId of photo to perform deletion. Any error in DeletePhoto will throw an exception
            // in the implementation class of IPhotoService.
            await photoService.DeletePhoto(photo.PublicId);

            // After deleting from Cloud, remove from the database as well.
            user.Photos.Remove(photo);

            // SaveChanges of removing the photo from User entity.
            var result = await context.SaveChangesAsync(cancellationToken) > 0;

            // Unit.Value means no data return.
            return result ? Result<Unit>.Success(Unit.Value)
                : Result<Unit>.Failure("Problem deleting photo", 400);
        }
    }
}
