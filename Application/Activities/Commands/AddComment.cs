using Application.Activities.DTOs;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Commands;

public class AddComment
{
    // receive Body and ActivityId from the HTTP Request of Mediator when the client add a new comment,
    // then tells Mediator that it will return Result<CommentDto> for the client to display the comment.
    public class Command : IRequest<Result<CommentDto>>
    {
        public required string Body { get; set; }

        public required string ActivityId { get; set; }

    }

    // Handler takes HTTP Request's Body and ActivityId from Command, then tell Mediator that it will return Result<CommentDto> .
    public class Handler(AppDbContext context, IMapper mapper, IUserAccessor userAccessor) : IRequestHandler<Command, Result<CommentDto>>
    {
        public async Task<Result<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            // Find the activity that has new comment from HTTP Request, then loads its navigation property Comment,
            // then load each Comment's User in memory.
            // This is for checking if the activity exists before adding a comment to it.
            var activity = await context.Activities
                .Include(activity => activity.Comments)
                .ThenInclude(activity => activity.User)
                .FirstOrDefaultAsync(activity => activity.Id == request.ActivityId, cancellationToken);

            if (activity == null) return Result<CommentDto>.Failure("Could not find activity", 404);

            var user = await userAccessor.GetUserAsync();

            // Creat new Comment object, assign Body from the HTTP request, current logged in user and activity found from the database to it.
            var comment = new Comment
            {
                Body = request.Body,
                UserId = user.Id,
                ActivityId = activity.Id,

            };

            // Because activity entity eagerly loaded Comments navigation property, it requires to be updated 
            // so EF Core adds the new Comment to the tracked Comments collection, so the new Comment
            // is associated with the correct Activity and it will updated its tracked User navigation property.

            activity.Comments.Add(comment);

            // Insert new comment to the Comment Entity and update the Comments navigation property of Activity Entity.
            var result = await context.SaveChangesAsync(cancellationToken) > 0;

            // As activity.Comments.Add(comment) updates each Comment's user, 
            // mapping from comment.User.DisplayName and ImageUrl will not require reloading from the database.
            return result
                ? Result<CommentDto>.Success(mapper.Map<CommentDto>(comment))
                : Result<CommentDto>.Failure("Failure to add comment", 400);

        }
    }
}
