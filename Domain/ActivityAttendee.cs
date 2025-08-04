using System;

namespace Domain;

// Each ActivityAttendee belongs to 1 Activity and 1 User.
// Each Activity or User can have many ActivityAttendee (one attendee record for each Activity a User Attends AND one attendee record for each User in an Activity) 
public class ActivityAttendee
{
    // standard properties of joint table.
    // UserId and ActivityId are foreign keys from User and Activity tables.
    // Because EF Core will only populate them at runtime, so they will be null when ActivityAttendee is initialised. Need to set them as nullable.
    // But they are also set as primary keys, so they won't be null in the ActivityAttendee table.
    public string? UserId { get; set; }
    public string? ActivityId { get; set; }

    // User and Activity are for establishing many to many relationships between User and Activity tables through ActivityAttendee join table.
    // User and Activity are navigation properties that allow us to set User and Activity objects for this attendee record. 
    // So we can navigate properties like Activity.title or User.DisplayName.
    // null! tells the compiler that User and Activity won't be null at runtime although they are currently.
    // Because there is no constructor to initialise them when created, EF Core creates instance of ActivityAttendee class,
    // then populate its properties with data from the database.
    public User User { get; set; } = null!;
    public Activity Activity { get; set; } = null!;

    // custom properties of joint table.
    public bool IsHost { get; set; }

    // default is current date and time.
    public DateTime DateJoined { get; set; } = DateTime.UtcNow;
}
