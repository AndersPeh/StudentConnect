using System;
using System.Security.Claims;
using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security;

// Because UserAccessor has to access to ASP.NET (IHttpContextAccessor) to extract user info from cookies/ claims,
// it is dependent on external service. For logic, we usually store in Application layer which is not dependent
// on external service like API layer, so we cant store UserAccessor in Application layer.
// As such, created a layer named Infrastructure that is dependent on external services.

// Application layer should not be dependent on external services so any changes in external services wont affect
// its logic. Application layer is loosely coupled to external services, so interface of UserAccessor is provided
// in it. Interface just defines the contract, any changes in external services wont affect it.

// UserAccessor is required to implement GetUserAsync and GetUserId methods.
// httpContextAccessor is for accessing user info in the cookie.
// use Dependency Injection to inject IHttpContextAccessor and AppDbContext (interact with database).

public class UserAccessor(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext) : IUserAccessor
{
    public async Task<User> GetUserAsync()
    {
        // It uses AppDbContext to find the user in the database using the ID from GetUserId() method.
        // This method returns a User object but it doesnt eagerly load navigation properties like Photos,
        // either eagerly load it using .Include(u => u.Photos).FirstOrDefaultAsync(u => u.Id == userId) because FindAsync doesnt work with eager loading.
        // or create another method to do so.
        return await dbContext.Users.FindAsync(GetUserId())
            ?? throw new UnauthorizedAccessException("No user is logged in");
    }

    public string GetUserId()
    {
        // It uses IHttpContextAccessor to get the user's ID from the claims of HTTP request.
        // HttpContext contains information about the request including user's claims from the cookie, it is scoped to a single request.
        // That's why AddScoped is used in Program.cs builder.Services.AddScoped<IUserAccessor, UserAccessor>();
        // if return null, throw error message.
        return httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new Exception("No user found");

        // NameIdentifier = User ID. Can find it by adding Breakpoint at line var user = await userAccessor.GetUserAsync(); in CreateActivity.cs
        // then run debugger (.Net Core Attach, select API.exe).
        // send a POST request in Postman, another POST reqeust to create activity.
        // HttpContext -> User -> Identity -> Claims -> nameidentifier.
    }

    public async Task<User> GetUserWithPhotosAsync()
    {
        var userId = GetUserId();

        return await dbContext.Users
            .Include(user => user.Photos)
            .FirstOrDefaultAsync(user => user.Id == userId)
                ?? throw new UnauthorizedAccessException("No user is logged in");
    }
}
