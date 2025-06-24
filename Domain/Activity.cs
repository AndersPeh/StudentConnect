using System;

namespace Domain;

// blueprint of the Activities table in AppDbContext (Persistence layer).
public class Activity
{
    // this property has to be public for Entity Framework to access to it.
    // GUID generates a new Globally Unique Identifier suitable for primary keys across different layers.
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Because Id is initialised, it doesn't need "required", it won't be null.
    // For other strings and objects, "required" or "?" is needed to specify if they are nullable.
    public required string Title { get; set; }

    public DateTime Date { get; set; }

    public required string Description { get; set; }

    public required string Category { get; set; }

    public bool IsCancelled { get; set; }

    // location props
    public required string City { get; set; }

    public required string Venue { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    // navigation properties (use conventional way to create a joint table for establishing many to many relationship with User table)
    // however, this method doesn't allow us to customise the name or properties of the joint table.
    // public ICollection<User> Attendees { get; set; } = [];

    // Navigation property Attendees for establishing a one to many relationship from Activity to ActivityAttendee.
    // An Activity can have many ActivityAttendee. This can be used to load attendees associated to the activity.
    // ICollection provides functionality like Add, Remove, Count for EF Core..
    public ICollection<ActivityAttendee> Attendees { get; set; } = [];
}
