using System;
using Application.Activities.Commands;
using Application.Activities.DTOs;
using Application.Activities.Queries;
using Application.Core;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// A Controller should only take HTTP request and return HTTP response, it shouldn't process any logic.
// ActivitiesController inherits from BaseApiController for features like Route("api/activities") and IMediator.
// As .NET automatically removes Controller keyword, changes Activities to lowercase,
// so the base route becomes api/activities.

// Scans the Application layer for all IRequestHandler<TRequest, TResponse> implementations (like CreateActivity.Handler).
// After any Handler has processed the request, it will return response up the pipeline to ValidationBehavior.
// Then ValidationBehavior will return response to the Mediator and Mediator will return response to the Controller.

//     x.RegisterServicesFromAssemblyContaining<GetActivityList.Handler>();
// });

// Example flow of Mediator Pipeline:
// Step	                        File/Class	                    What happens?
// Controller sends request	    ActivitiesController	        Calls Mediator.Send(command)
// Pipeline starts	            MediatR	                        Calls ValidationBehavior.Handle(...)
// Validation passes	        ValidationBehavior	            Calls await next() (delegate calls another method directly)
// Handler runs	                CreateActivity.Handler	        Returns Result<string>.Success(activity.Id)
// Response returns	            ValidationBehavior	            Returns handler's response up the pipeline
// Response returns	            MediatR	                        Returns response to controller
// Controller receives result	ActivitiesController	        Handles result (e.g., via HandleResult)

public class ActivitiesController : BaseApiController
{
    // By segregating Command and Query Responbility, every class is kept simple to handle either read or write operations only.
    // For Queries, they typically return DTOs.
    // API endpoint: GET /api/activities
    // Because cursor is an optional query string parameter, no need to specify in the route. When model binder sees query string named cursor,
    // then it will bind to  GetActivities(DateTime? cursor).
    [HttpGet]

    // GetActivities method handles HTTP Get request /api/activities.
    // It returns a PagedList of ActivityDto and DateTime nextCursor in ActionResult.
    public async Task<ActionResult<PagedList<ActivityDto, DateTime?>>> GetActivities(DateTime? cursor)

    {
        // Mediator knows which handler to use because a handler in the Application layer is specified to handle GetActivityList.Query .
        // After receiving result from Application layer, returns ActionResult which wraps a PagedList of ActivityDto (match Domain.Activity)
        // into JSON body of ActionResult containing Status Code: 200 OK.
        return HandleResult(await Mediator.Send(new GetActivityList.Query { Cursor = cursor }));
    }

    // *******************************************************************************************************

    [HttpGet("{id}")]

    // GetActivityDetail uses a route parameter {id}, so it expects a value in the URL path.
    // It returns a Result type of ActivityDto object indicating success or failure to HandleResult.
    // HandleResult inherited from BaseApiController will return responses accordingly.
    public async Task<ActionResult<ActivityDto>> GetActivityDetail(string id)

    {
        // Centralise the Success and Failure resuls processing in BaseApiController so dont need to write below in every HTTP method.
        // Just need to call HandleResult method instead of writing lines below:
        // if (!result.IsSuccess && result.Code == 404) return NotFound();
        // if (result.IsSuccess && result.Value != null) return Ok(result.Value);
        // return BadRequest(result.Error);

        // use object initialiser to pass { Id = id }.
        return HandleResult(await Mediator.Send(new GetActivityDetails.Query { Id = id }));

    }

    // *******************************************************************************************************
    // For Commands, they typically returns a simple success/ failure ActionResult or an ID, not a complex data object.

    [HttpPost]

    // When HTTP Post request arrives at /api/activities endpoint, .Net model binding deserialises request body containing JSON
    // into CreateActivityDto object which is passed to ActivityDto later.
    // It returns string Id from the database in ActionResult. CreateActivity method takes CreateActivityDto object as its parameter, omitting unnecessary data from user.
    public async Task<ActionResult<string>> CreateActivity(CreateActivityDto activityDto)
    {
        // instantiates ActivityDto with activityDto from the HTTP request body, then instantiates CreateActivity.Command class. 
        // send fully constructed CreateActivity.Command to Mediator.
        // When Mediator takes this Command object, it starts the pipeline (behavior first, then handler).
        return HandleResult(await Mediator.Send(new CreateActivity.Command { ActivityDto = activityDto }));
    }

    // *******************************************************************************************************

    [HttpPut("{id}")]

    // This applies the IsActivityHost policy from Program.cs to this API endpoint.
    // Before executing the code inside this method, the user must be authenticated and satisfy the policy named IsActivityHost.
    [Authorize(Policy = "IsActivityHost")]

    public async Task<ActionResult> EditActivity(string id, EditActivityDto activityDto)
    {
        activityDto.Id = id;
        return HandleResult(await Mediator.Send(new EditActivity.Command { ActivityDto = activityDto }));
    }

    // *******************************************************************************************************

    [HttpDelete("{id}")]
    [Authorize(Policy = "IsActivityHost")]

    public async Task<ActionResult> DeleteActivity(string id)
    {
        return HandleResult(await Mediator.Send(new DeleteActivity.Command { Id = id }));
    }

    // *******************************************************************************************************
    [HttpPost("{id}/attend")]

    // Attend takes ActivityId for updating attendance status, returns ActionResult (Success or Failure),
    public async Task<ActionResult> Attend(string id)
    {
        return HandleResult(await Mediator.Send(new UpdateAttendance.Command { Id = id }));
    }
}
