using System;

namespace Domain;

public class Photo
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public required string Url { get; set; }

    public required string PublicId { get; set; }

    // navigation property to User entity so Cascade Delete will be enabled. 
    // When a User is deleted, associated photos will be deleted (onDelete: ReferentialAction.Cascade in the migration).
    public required string UserId { get; set; }

    public User User { get; set; } = null!;

}
