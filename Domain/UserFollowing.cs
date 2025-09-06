using System;

namespace Domain;

// Because a user can follow and be followed by many users, creating a many to many relationship.
// UserFollowing breaks it down into 1 User to Many UserFollowing x2 (for Observer and Target).
// Essentially, 1 Observer User is linked to Many UserFollowing records and 1 Target User is linked to Many UserFollowing records.
public class UserFollowing
{
    // The User following another User is the observer. ObserverId and TargetId form the composite primary key.
    public required string ObserverId { get; set; }

    // The User getting followed is the target.
    public required string TargetId { get; set; }

    // Navigation properties for EF Core to load Observer and Target from Users Table using ObserverId and TargetId.
    // Use null forgiving operator because EF Core will handle loading them, they shouldnt be required to be initialised when creating the UserFollowing entity.
    public User Observer { get; set; } = null!;

    public User Target { get; set; } = null!;
}
