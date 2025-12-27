using Application.Core;

namespace Application.Activities.Queries;

// Use DateTime as the cursor to indicate a generic next starting point.
public class ActivityParams : PaginationParams<DateTime?>
{
    public string? Filter { get; set; }

    public DateTime StartDate { get; set; } = DateTime.UtcNow;
}
