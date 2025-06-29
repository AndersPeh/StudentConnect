using System;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Commands;

// For Mediator to know this is the class to go for the Attend endpoint.
public class UpdateAttendance
{
    // Tells Mediator this is the Command the takes the request Id.
    // Unit means not returning anything. Result provides 2 states, Success or Failure.
    // Because .Success/ Failure sends a response, there is no need to return anything.
    public class Command : IRequest<Result<Unit>>
    {
        public required string Id { get; set; }
    }

    // Handle the request from the Mediator pipeline. IUserAccessor is for interacting with current logged in user data. DI container injects 2 things,
    // AppDbContext is for interacting with the Activity database. Handler inherits from IRequestHandler that takes the Command and returns Nothing (Unit).
    public class Handler(IUserAccessor userAccessor, AppDbContext context) : IRequestHandler<Command, Result<Unit>>
    {
        // Handle method returns Result<Unit>, which means either Success or Failure response with no data (Unit).
        // Task refers to the result after the async operation is completed.
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            // From the Activity entities, find the Activity Id that matches with the request Activity Id,
            // then eagerly loads its related ActivityAttendee data and its User navigation property to know Users attending the activity.
            var activity = await context.Activities
                .Include(x => x.Attendees)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            // returns 404 error if Activity can't be found.
            if (activity == null) return Result<Unit>.Failure("Activity not found", 404);

            // Get the current logged in User.
            var user = await userAccessor.GetUserAsync();

            // Find the logged in User from the Users attending the Activity (using logged in User Id == First User Id that matches).
            var attendance = activity.Attendees.FirstOrDefault(x => x.UserId == user.Id);

            // Any returns True if the logged in User exists in Users attending the Activity and the User has IsHost == True.
            var isHost = activity.Attendees.Any(x => x.IsHost && x.UserId == user.Id);

            // If found the User from the Users attending the Activity, if the User is the Host, reverses the current cancellation status.
            // If the User is not the Host but attending the Activity, change the status to not attending by removing the User.
            if (attendance != null)
            {
                if (isHost) activity.IsCancelled = !activity.IsCancelled;
                else activity.Attendees.Remove(attendance);
            }
            // If User not found in the Users attending the Activity and the User is not the Host, Add the User as an Attendee.
            else
            {
                activity.Attendees.Add(new ActivityAttendee
                {
                    UserId = user.Id,
                    ActivityId = activity.Id,
                    IsHost = false,
                });
            }

            // If there is at least 1 changes, save it.
            var result = await context.SaveChangesAsync(cancellationToken) > 0;

            // Result returns True if savechanges operation was executed, then send Success response.
            return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Problem updating the DB", 400);
        }
    }
}
