using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Activities.DTOs;

// Data Transfer Object defines structure of the data client sends to create an activity.
// .Net model binding system automatically try to deserialises JSON from the client into an instance of DTO.
// Validation rules applied to the properties of DTO verify user inputs, because validation (logic) should be handled in Application layer.
// DTO allows omitting unnecessary data like Id and booleans, so Domain layer can be changed without breaking API contract.
public class CreateActivityDto
{
    // set empty string as initial value to stop null error. CreateActivityValidator will ensure user inputs won't be null.
    public string Title { get; set; } = "";

    public DateTime Date { get; set; }

    public string Description { get; set; } = "";

    public string Category { get; set; } = "";

    public string City { get; set; } = "";

    public string Venue { get; set; } = "";

    public double Latitude { get; set; }

    public double Longitude { get; set; }
}
