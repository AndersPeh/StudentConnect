using System;
using Application.Activities.Queries;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// ActivitiesController inherits from BaseApiController for features like Route("api/[controller]") and IMediator.
public class ActivitiesController : BaseApiController
{

    // API endpoint: GET /api/activities
    [HttpGet]

    // GetActivities method handles HTTP Get request /api/activities.
    public async Task<ActionResult<List<Activity>>> GetActivities()

    {
        // instantiates request object Query to Mediator. 
        // Mediator knows which handler to use because mediator is registered in API layer and all handlers in assembly of Application layer are scanned by mediator .
        // After receiving result from Application layer, returns ActionResult which wraps a List of Activity (match Domain.Activity)
        // into JSON body of OkObjectResult containing Status Code: 200 OK.
        return await Mediator.Send(new GetActivityList.Query());
    }

    // *******************************************************************************************************
    [HttpGet("{id}")]

    // GetActivityDetail uses a route template parameter {id}, so it expects a value in the URL path.
    public async Task<ActionResult<Activity>> GetActivityDetail(string id)

    {
        // use object initialiser to pass { Id = id }.
        return await Mediator.Send(new GetActivityDetails.Query { Id = id });

    }
}
