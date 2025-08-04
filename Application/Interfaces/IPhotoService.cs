using System;
using Application.Profiles.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

// Interface for PhotoService class in the Infrastructure layer.
public interface IPhotoService
{
    Task<PhotoUploadResult?> UploadPhoto(IFormFile file);

    Task<string> DeletePhoto(string publicId);
}
