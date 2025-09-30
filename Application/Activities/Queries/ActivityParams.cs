using System;
using Application.Core;

namespace Application.Activities.Queries;

// Use DateTime as the cursor to indicate a generic next starting point.
public class ActivityParams : PaginationParams<DateTime?>
{

}
