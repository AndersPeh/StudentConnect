using System;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Persistence;

namespace Application.Profiles.Commands;

public class FollowToggle
{
    public class Command : IRequest<Result<Unit>>
    {
        public required string TargetUserId { get; set; }
    }

    public class Handler(IUserAccessor userAccessor, AppDbContext context) : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            // Get the current user as the observer.
            var observer = await userAccessor.GetUserAsync();

            // Get the target user id from the Http Request to find the target user. FindAsync is designed to handle both single and composite primary key,
            // so the key must be placed in an array [].
            var target = await context.Users.FindAsync([request.TargetUserId], cancellationToken);

            if (target == null) return Result<Unit>.Failure("Target user not found", 400);

            var following = await context.UserFollowings.FindAsync([observer.Id, target.Id], cancellationToken);

            // As Add and Remove are change tracking methods that carries out in-memory operation of the database received from FindAsync earlier, it doesnt need async for a non I/O operation.
            if (following == null) context.UserFollowings.Add(new UserFollowing { ObserverId = observer.Id, TargetId = target.Id });

            else context.UserFollowings.Remove(following);

            // SaveChangesAsync generates INSERT or DELETE SQL commands for Add/ Remove, sends it to the database. Needs await as it takes time to execute.
            return await context.SaveChangesAsync(cancellationToken) > 0 ?
                Result<Unit>.Success(Unit.Value) :
                Result<Unit>.Failure("Error updating following", 400);
        }
    }
}
