using System;

namespace Application.Activities.DTOs;

// Although creating an activity doesnt need Id, Editing activity requires Id to find the Id to update it.
public class EditActivityDto : BaseActivityDto
{
    public string Id { get; set; } = "";
}
