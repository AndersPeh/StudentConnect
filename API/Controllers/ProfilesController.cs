using System;
using Application.Profiles.Commands;
using Application.Profiles.DTOs;
using Application.Profiles.Queries;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// Any Url that contains /api/profiles go to here.
public class ProfilesController : BaseApiController
{
    // This endpoint takes Url that ends with /profiles/add-photo.
    [HttpPost("add-photo")]

    // This endpoints takes IFormFile from the Http Request and returns Task<ActionResult<Photo>>.
    // It will return Ok(result.Value) from BaseApiController.cs which is ActionResult<Photo> for success case, because
    // value refers to value which is Photo from Result.cs: public static Result<T> Success(T value) => new()
    // {IsSuccess = true,
    //     Value = value}; 
    public async Task<ActionResult<Photo>> AddPhoto(IFormFile file)
    {
        return HandleResult(await Mediator.Send(new AddPhoto.Command { File = file }));
    }

    // This endpoints handles Url that ends with /profiles/{userId}/photos.
    [HttpGet("{userId}/photos")]
    // This endpoints takes userId from the route parameter.
    public async Task<ActionResult<List<Photo>>> GetPhotosForUser(string userId)
    {
        return HandleResult(await Mediator.Send(new GetProfilePhotos.Query { UserId = userId }));
    }

    // This endpoints handles Url that ends with /profiles/{photoId}/photos.
    [HttpDelete("{photoId}/photos")]
    public async Task<ActionResult> DeletePhoto(string photoId)
    {
        return HandleResult(await Mediator.Send(new DeletePhoto.Command { PhotoId = photoId }));
    }

    // This endpoints handles Url that ends with /profiles/{photoId}/setMain.
    [HttpPut("{photoId}/setMain")]

    public async Task<ActionResult> SetMainPhoto(string photoId)
    {
        return HandleResult(await Mediator.Send(new SetMainPhoto.Command { PhotoId = photoId }));
    }

    // This endpoints handles Url that ends with /profiles/{userId}.
    [HttpGet("{userId}")]
    // This endpoints takes userId from the route parameter.
    public async Task<ActionResult<UserProfile>> GetProfile(string userId)
    {
        return HandleResult(await Mediator.Send(new GetProfile.Query { UserId = userId }));
    }
}
