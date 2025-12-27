using Application.Profiles.DTOs;

namespace Application.Activities.DTOs;

// Without ActivityDto, if we return Activity directly, it will result in a loop where Activity returns ActivityAttendee,
// then ActivityAttendee returns User and Activity, repeat...
public class ActivityDto
{

    public required string Id { get; set; }

    public required string Title { get; set; }

    public DateTime Date { get; set; }

    public required string Description { get; set; }

    public required string Category { get; set; }

    public bool IsCancelled { get; set; }

    // Because HostDisplayName and HostId dont exist in Activity entity, when mapping,
    // Automapper will use formember to retrieve them.
    // This maps HostDisplayName from navigation property of Activity, which is ActivityAttendee.
    // .ForMember(ActivityDtoDestination => ActivityDtoDestination.HostDisplayName, options => options.MapFrom(ActivitySource =>
    // ActivitySource.Attendees.FirstOrDefault(attendee => attendee.IsHost)!.User.DisplayName))
    public required string HostDisplayName { get; set; }

    // Below is mapped from navigation property of Activity, which is ActivityAttendee.
    // .ForMember(d => d.HostId, o => o.MapFrom(s =>
    // s.Attendees.FirstOrDefault(x => x.IsHost)!.User.Id));
    public required string HostId { get; set; }

    public required string City { get; set; }

    public required string Venue { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public ICollection<UserProfile> Attendees { get; set; } = [];
}
