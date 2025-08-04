using System;
using Application.Interfaces;
using Application.Profiles.DTOs;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Photos;

// PhotoService is an implementation class of the interface IPhotoService in Application layer.
public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;

    // Inject CloudinarySettings through IOptions. Because .NET uses IOptions to inject strongly-typed configurtion objects.
    public PhotoService(IOptions<CloudinarySettings> config)
    {
        var account = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );

        // Setup Cloudinary credentials in the constructor.
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> DeletePhoto(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);

        var result = await _cloudinary.DestroyAsync(deleteParams);

        if (result.Error != null)
        {
            throw new Exception(result.Error.Message);
        }

        return result.Result;
    }

    // IFormFile refers to the uploaded file from an HTTP request.
    // Optional Task<PhotoUploadResult?> because there may be an empty file which leads to return null.
    public async Task<PhotoUploadResult?> UploadPhoto(IFormFile file)
    {
        // file.Length returns the size of the uploaded file to ensure the file is not empty before processing it.
        if (file.Length > 0)
        {
            // file.OpenReadStream() opens a read only stream to access the file's content in memory. 
            // "using" disposes stream after finish using it.
            await using var stream = file.OpenReadStream();

            // ImageUploadParams defines parameters to upload an image to Cloudinary such as file, folder, transformation, etc.
            var uploadParams = new ImageUploadParams
            {
                // FileDescription represents the file to upload, need to include FileName and the stream containing file data.
                File = new FileDescription(file.FileName, stream),

                // Transform the image to this height and width. If add .Crop("fill"), it will transform the image to fill.
                // Transformation = new Transformation().Height(500).Width(500).Crop("fill")

                Folder = "StudentConnect"
            };

            // Upload the file to Cloudinary account.
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                throw new Exception(uploadResult.Error.Message);
            }

            return new PhotoUploadResult
            {
                PublicId = uploadResult.PublicId,

                // Uri is a class like a web address containing Scheme, Host, Path and other properties and methods.
                // Url is just a string representation of a web address.
                Url = uploadResult.SecureUrl.AbsoluteUri,

            };
        }
        // if file.Length is not 0,
        return null;
    }
}
