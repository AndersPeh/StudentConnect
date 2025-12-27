using Application.Activities.DTOs;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Queries;

public class GetComments
{
    // When Mediator passes HTTP Request containing ActivityId to here, tell it to expect Result<List<CommentDto>>.
    public class Query : IRequest<Result<List<CommentDto>>>
    {
        public required string ActivityId { get; set; }
    }

    // It takes Query for ActivityId and tells Mediator that it will return Result<List<CommentDto>>.
    public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Query, Result<List<CommentDto>>>
    {
        // Find comments of the specific activity, order starts from latest comment, 
        // map to CommentDto, convert to List for returning.
        public async Task<Result<List<CommentDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var comments = await context.Comments
                .Where(comment => comment.ActivityId == request.ActivityId)
                .OrderByDescending(comment => comment.CreatedAt)
                .ProjectTo<CommentDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result<List<CommentDto>>.Success(comments);
        }
    }
}
