using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security;

// This class is a requirement to be used in an Authorization Policy. 
// It sets the requirement where the current user must be the host of the activity being requested.
public class IsHostRequirement : IAuthorizationRequirement
{

}

// This Handler checks if the Useer meets the <IsHostRequirement> (must be the host of the activity).
// DI container injects AppDbContext for accessing ActivityAttendee entities. IHttpContextAccessor for accessing current URL route.

public class IsHostRequirementHandler(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : AuthorizationHandler<IsHostRequirement>
{
    // AuthorizationHandlerContext for accessing current logged in User data.
    // IsHostRequirement for determining is the requirement is satisfied.
    // Overriding HandleRequirementAsync method of AuthorizationHandler with custom logic to check if a User is the host of an Activity.
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
    {
        // Get the current user's ID from the cookie.
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        // If user's ID doesn't exist in the cookie (not authenticated), exit the method, it will automatically return 403 error. 
        if (userId == null) return;

        // accesses details of the current HTTP request.
        var httpContext = httpContextAccessor.HttpContext;

        // Get the activityId from the URL's route parameter (api/activities/{id}).
        // if route value id exists and is a string, assign it to activityId.
        // if id doesn't exist or is not a string, return nothing. It will lead to 403 error.
        if (httpContext?.GetRouteValue("id") is not string activityId) return;

        // Access the User and Activity navigation properties of ActivityAttendee to find User and Activity in the database that match the
        // currently logged in User and activityId from the route parameter.
        var attendee = await dbContext.ActivityAttendees
            // If .AsNoTracking() is not provided, any Command or Query operation after this will continue tracking the ActivityAttendees. 

            // For example, EditActivity Command needs to get Succeed message from IsHostRequirement first, then only proceed to EditActivity logic.
            // If AsNoTracking is not provided, after finishing IsHostRequirement, the application will bring the ActivityAttendees to EditActivity logic in Api layer.
            // After editing the activity, when saving changes, EF Core will see ActivityAttendees from IsHostRequirement as part of the Activity from the DB, 
            // but the Edit Activity request has no ActivityAttendees, EF Core will misunderstand that ActivityAttendees of the Activity should be removed,
            // causing ActivityAttendees to be removed by error. Actually, when the request doesnt come with ActivityAttendees, it just means ActivityAttendees is not edited,
            // it shouldnt be removed.
            // IsHostRequirement is a read only operation and does not call SaveChangesAsync.
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.UserId == userId && x.ActivityId == activityId);

        // If can't find the User and Activity from the database that match the currently logged in User and activityId of the route parameter,
        // automatically generates 403 error response. Because it means the User is not attending the Activity (can't be host).
        if (attendee == null) return;

        // If the found User has IsHost == True, the User must be the host of the Activity. AuthorizationHandlerContext.Succed tells
        // the authorisation middleware that this requirement has been met, so the user is granted access.
        if (attendee.IsHost) context.Succeed(requirement);
    }
}
