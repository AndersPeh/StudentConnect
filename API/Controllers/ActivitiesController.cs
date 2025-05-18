using System;
using Application.Activities.Queries;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers;

// ActivitiesController inherits from BaseApiController for basic API controller features.
// When it is created, it needs AppDbContext instance, DI uses configuration from Program.cs (builder.Services.AddDbContext<AppDbContext>)
// to create an AppDbContext instance. Then AppDbContext instance is injected to create an instance of ActivitiesController.
// which means two new instances are created for every new HTTP request because they have scoped lifetime.
// Decoupling: ActivitiesController doesn't need to knwo how to create AppDbContext. If there is any changes in AppDbContext, only need to modify Program.cs.
// Manageability: DI container manages the lifetime of AppDbContext, remove the burden of creating/ disposing it in the Controller.
public class ActivitiesController(AppDbContext context, IMediator mediator) : BaseApiController
{

    // API endpoint: GET /api/activities
    [HttpGet]

    // GetActivities method handles HTTP Get request /api/activities.
    public async Task<ActionResult<List<Activity>>> GetActivities()

    {
        // instantiates request object Query to Mediator. 
        // Mediator knows which handler to use because it is registered in Program.cs <GetActivityList.Handler>.
        // After receiving result from Application layer, returns ActionResult which wraps a List of Activity (match Domain.Activity)
        // into JSON body of OkObjectResult containing Status Code: 200 OK.
        return await mediator.Send(new GetActivityList.Query());
    }

    // *******************************************************************************************************
    [HttpGet("{id}")]

    // GetActivityDetail uses a route template parameter {id}, so it expects a value in the URL path.
    // It will return HTTP Not Found Response if null.
    public async Task<ActionResult<Activity>> GetActivityDetail(string id)

    {
        // EF executes queries in Persistence layer, maps the activity result to instance of Activity class from Domain layer,
        // returns Activity object from Persistence layer to API layer.
        var activity = await context.Activities.FindAsync(id);

        if (activity == null) return NotFound();

        return activity;

    }
}
