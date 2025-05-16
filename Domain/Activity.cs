using System;

namespace Domain;

public class Activity
{
    // this property has to be public for Entity Framework to access to it.
    // GUID generates a new Globally Unique Identifier suitable for primary keys
    // across different system.
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

}
