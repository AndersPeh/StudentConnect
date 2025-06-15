using System;
using Application.Activities.DTOs;
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
        // tell IMapper when the sourceObject is Activity, map it to Activity destinationObject.
        CreateMap<Activity, Activity>();
        // tell IMapper when the source object is CreateActivityDto, map it to Activity destinationObject.
        CreateMap<CreateActivityDto, Activity>();
        CreateMap<EditActivityDto, Activity>();

    }
}
