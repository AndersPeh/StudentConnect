using System;
using Microsoft.AspNetCore.Identity;

namespace Domain;

// inherits from IdentityUser which uses a string as primary key. IdentityUser generates salted and hashed representation of the password.
public class User : IdentityUser
{
    // make these properties optional.
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }

    public string? ImageUrl { get; set; }

    // navigation properties (use conventional way to create a joint table for establishing many to many relationship with Activity table)
    // however, this method doesn't allow us to customise the name or properties of the joint table.
    // public ICollection<Activity> Activities { get; set; } = [];

    // Navigation property Activities for establishing a one to many relationship from User to ActivityAttendee.
    // A User can have many ActivityAttendee. This can be used to load attendees associated to the User.
    // ICollection provides functionality like Add, Remove, Count for EF Core..
    public ICollection<ActivityAttendee> Activities { get; set; } = [];

    // This sets up One User to Many Photo relationship.
    public ICollection<Photo> Photos { get; set; } = [];

    public ICollection<UserFollowing> Followings { get; set; } = [];

    public ICollection<UserFollowing> Followers { get; set; } = [];
}
