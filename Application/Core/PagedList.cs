using System;

namespace Application.Core;

// Use PagedList for any type and use any type of Cursor.
public class PagedList<T, TCursor>
{
    public List<T> Items { get; set; } = [];

    // Any type of cursor can be used to indicate the next item in the list that we want to start from.
    public TCursor? NextCursor { get; set; }

}
