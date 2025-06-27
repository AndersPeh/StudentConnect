using System;
using System.Security.Cryptography.X509Certificates;
using Application.Activities.DTOs;
using Application.Profiles.DTOs;
using AutoMapper;
using Domain;

namespace Application.Core;

// inherits from Profile class of AutoMapper, defines mapping configurations.
public class MappingProfiles : Profile
{
    // IMapper is the service that will execute mappings below.
    // It saves the step of assining property from ActivityDto to new Activity object before saving. 
    public MappingProfiles()
    {
        // control what the client needs to provide to the server, omit unnecessary data from the client like Id and isCancelled.
        // tell IMapper when the sourceObject is Activity, map it to Activity destinationObject.
        CreateMap<Activity, Activity>();
        // tell IMapper when the source object is CreateActivityDto, map it to Activity destinationObject.
        CreateMap<CreateActivityDto, Activity>();
        CreateMap<EditActivityDto, Activity>();

        // control what is sent from the server to the client, simplify complex data like join table.
        // mapping from the database entities to simple DTOs for sending out from API to the frontend.
        CreateMap<Activity, ActivityDto>()
            // Because GetActivityDetails has already run followings:
            // .Include(x => x.Attendees)
            // .ThenInclude(x => x.User)
            // Finds the first recorsd or return null.
            // .FirstOrDefaultAsync(x => request.Id == x.Id, cancellationToken);
            // Actvity object with Attendee and User populated is provided, 
            // just need to use LINQ query to extract data needed from join table.

            // The HostDisplayName and HostId on the ActivityDto destination are not provided by the Activity entity from the database.
            // so use options provided by AutoMapper to access MapFrom, then MapFrom ActivitySource, get its Attendee where IsHost is true.
            // For the attendee hosting, access to its User property, extract DisplayName from it. 
            // So HostDisplayName is extracted through LINQ query.
            .ForMember(ActivityDtoDestination => ActivityDtoDestination.HostDisplayName, options => options.MapFrom(ActivitySource =>
            // C# compiler thinks FirstOrDefault(attendee => attendee.IsHost) will return null, so need to include ! to tell the compiler that it wont be null.
                ActivitySource.Attendees.FirstOrDefault(attendee => attendee.IsHost)!.User.DisplayName))
            .ForMember(d => d.HostId, o => o.MapFrom(s =>
                s.Attendees.FirstOrDefault(x => x.IsHost)!.User.Id));

        // Because ActivityAttendee is too complicated, simplify it into UserProfile when AutoMapper maps from Activity to ActivityDto.
        // For every property in UserProfile, access options provided by AutoMapper to use MapFrom,
        // then get DisplayName from User of ActivityAttendeeSource.
        CreateMap<ActivityAttendee, UserProfile>()
            .ForMember(UserProfileDestination => UserProfileDestination.DisplayName, o =>
                o.MapFrom(ActivityAttendeeSource => ActivityAttendeeSource.User.DisplayName))
            .ForMember(d => d.Bio, o => o.MapFrom(s => s.User.Bio))
            .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.User.ImageUrl))
            .ForMember(d => d.Id, o => o.MapFrom(s => s.User.Id));

    }
}
