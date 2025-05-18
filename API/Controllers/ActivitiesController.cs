using System;
using Application.Activities.Commands;
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
    // It returns a List of Activity objects in ActionResult.
    public async Task<ActionResult<List<Activity>>> GetActivities()

    {
        // instantiates request object Query to Mediator. 
        // Mediator knows which handler to use because a handler in the Application layer is specified to handle GetActivityList.Query .
        // After receiving result from Application layer, returns ActionResult which wraps a List of Activity (match Domain.Activity)
        // into JSON body of ActionResult containing Status Code: 200 OK.
        return await Mediator.Send(new GetActivityList.Query());
    }

    // *******************************************************************************************************
    [HttpGet("{id}")]

    // GetActivityDetail uses a route template parameter {id}, so it expects a value in the URL path.
    // It returns an Activity object in ActionResult.
    public async Task<ActionResult<Activity>> GetActivityDetail(string id)

    {
        // use object initialiser to pass { Id = id }.
        return await Mediator.Send(new GetActivityDetails.Query { Id = id });

    }

    [HttpPost]

    // It returns string Id from the database in ActionResult. It takes activity object as its parameter.
    public async Task<ActionResult<string>> CreateActivity(Activity activity)
    {
        // pass activity to Activity in CreateActivity.Command.
        return await Mediator.Send(new CreateActivity.Command { Activity = activity });
    }

    [HttpPut]

    // Editing activity doesn't require any return, returns NoContent if successful.

    public async Task<ActionResult> EditActivity(Activity activity)
    {
        await Mediator.Send(new EditActivity.Command { Activity = activity });
        return NoContent();
    }

    // when data is needed in the path, must specify in the HTTP method.
    [HttpDelete("{id}")]

    // return Status 200 Ok if delete successfully.
    public async Task<ActionResult> DeleteActivity(string id)
    {
        await Mediator.Send(new DeleteActivity.Command { Id = id });
        return Ok();
    }
}
