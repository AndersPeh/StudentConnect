using System;
using Application.Activities.Commands;
using Application.Activities.DTOs;
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
    // It returns an Result type of Activity object indicating success or failure to HandleResult.
    // HandleResult inherited from BaseApiController will return responses accordingly.
    public async Task<ActionResult<Activity>> GetActivityDetail(string id)

    {
        // this will trigger HandleException in ExceptionMiddleware.
        // throw new Exception("Server test error");

        // use object initialiser to pass { Id = id }.
        return HandleResult(await Mediator.Send(new GetActivityDetails.Query { Id = id }));

    }

    [HttpPost]

    // When HTTP Post request arrives at /api/activities endpoint, .Net mode binding deserialises request body containing JSON
    // into CreateActivityDto object which is passed to ActivityDto later.
    // It returns string Id from the database in ActionResult. CreateActivity method takes CreateActivityDto object as its parameter, omitting unnecessary data from user.
    public async Task<ActionResult<string>> CreateActivity(CreateActivityDto activityDto)
    {
        // instantiates ActivityDto with activityDto from the HTTP request body, then instantiates CreateActivity.Command class. 
        // send fully constructed CreateActivity.Command to Mediator.
        // When Mediator takes this Command object, it starts the pipeline (behavior first, then handler).
        return HandleResult(await Mediator.Send(new CreateActivity.Command { ActivityDto = activityDto }));
    }

    [HttpPut]

    public async Task<ActionResult> EditActivity(EditActivityDto activityDto)
    {
        return HandleResult(await Mediator.Send(new EditActivity.Command { ActivityDto = activityDto }));
    }

    // when data is needed in the path, must specify in the HTTP method.
    [HttpDelete("{id}")]

    // return Status 200 Ok if delete successfully.
    public async Task<ActionResult> DeleteActivity(string id)
    {
        return HandleResult(await Mediator.Send(new DeleteActivity.Command { Id = id }));
    }
}
