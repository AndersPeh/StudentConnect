using System;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Persistence;

namespace Application.Profiles.Commands;

public class AddPhoto
{
    // Return Photo so client will know Url of the uploaded photo.
    public class Command : IRequest<Result<Photo>>
    {
        public required IFormFile File { get; set; }
    }

    public class Handler(IUserAccessor userAccessor, AppDbContext context, IPhotoService photoService) :
        IRequestHandler<Command, Result<Photo>>
    {
        public async Task<Result<Photo>> Handle(Command request, CancellationToken cancellationToken)
        {
            // use UploadPhoto method of photoService to upload the file from Http Request to Cloud.
            var uploadResult = await photoService.UploadPhoto(request.File);

            if (uploadResult == null) return Result<Photo>.Failure("Failed to upload photo", 400);

            var user = await userAccessor.GetUserAsync();

            // uploadResult = public class PhotoUploadResult
            // {public required string PublicId { get; set; }
            //     public required string Url { get; set; }}

            // use uploadResult properties to create new Photo entity.
            var photo = new Photo
            {
                Url = uploadResult.Url,
                PublicId = uploadResult.PublicId,
                UserId = user.Id
            };

            // If user.ImageUrl is null (means first photo of the user), assign photo.Url to it.
            user.ImageUrl ??= photo.Url;

            // After saving the photo to Cloud, save the Photo entity to the database.
            context.Photos.Add(photo);

            var result = await context.SaveChangesAsync(cancellationToken) > 0;

            return result ? Result<Photo>.Success(photo)
                : Result<Photo>.Failure("Problem saving photo to database", 400);
        }
    }
}
