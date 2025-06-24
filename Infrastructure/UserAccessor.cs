using System;
using System.Security.Claims;
using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Http;
using Persistence;

namespace Infrastructure;

// UserAccessor is required to implement GetUserAsync and GetUserId methods.
// httpContextAccessor is for accessing user info in the cookie.
// use Dependency Injection to inject IHttpContextAccessor and AppDbContext (interact with database).

public class UserAccessor(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext) : IUserAccessor
{
    public async Task<User> GetUserAsync()
    {
        // It uses AppDbContext to find the user in the database using the ID from GetUserId() method.
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
    }
}
