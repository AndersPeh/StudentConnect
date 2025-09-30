using System;

namespace Application.Core;

public class PaginationParams<TCursor>
{
    // User can only request a maximum of 50 records from the server.
    private const int MaxPageSize = 50;

    // Use generic as the cursor to indicate a generic next starting point.
    public TCursor? Cursor { get; set; }

    // private backing field _pageSize is guarded by PageSize for preventing DOS attack.
    private int _pageSize = 3;

    // PageSize ensures the _pageSize wont exceed MaxPageSize by 
    // checking the request page size and compare with the MaxPageSize.
    public int PageSize
    {
        // It is essentially the _pageSize.
        get => _pageSize;

        // If user requests more than MaxPageSize, returns MaxPageSize to limit the user's request.
        // If user requests less than MaxPageSize, returns the amount that the user requests.
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
}
