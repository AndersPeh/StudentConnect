using System;
using AutoMapper;
using Domain;

namespace Application.Core;

// inherits from Profile class of AutoMapper.
// defines mapping configurations.
public class MappingProfiles : Profile
{
    // IMapper is the service that will execute mappings below.
    public MappingProfiles()
    {
        // tell IMapper to map Activity from sourceObject to another Activity destinationObject.
        CreateMap<Activity, Activity>();
    }
}
