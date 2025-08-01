using System;
using Application.Activities.Commands;
using Application.Activities.DTOs;
using Application.Activities.Queries;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// ActivitiesController inherits from BaseApiController for features like Route("api/activities") and IMediator.
// As .NET automatically removes Controller keyword, changes Activities to lowercase,
// so the base route becomes api/activities.

// Below is provided in Program.cs to set up the Mediator pipeline. Whenever ActivitiesController sends a Query or Command to the Mediator,
// It will go to ValidationBehavior to use Validator for validating the request before it sending to the Handler.

// builder.Services.AddMediatR(x =>
// {
//     x.AddOpenBehavior(typeof(ValidationBehavior<,>));

// After any Handler has processed the request, it will return response up the pipeline to ValidationBehavior.
// Then ValidationBehavior will return response to the Mediator and Mediator will return response to the Controller.

//     x.RegisterServicesFromAssemblyContaining<GetActivityList.Handler>();
// });


// Step	                        File/Class	                    What happens?
// Controller sends request	    ActivitiesController	        Calls Mediator.Send(command)
// Pipeline starts	            MediatR	                        Calls ValidationBehavior.Handle(...)
// Validation passes	        ValidationBehavior	            Calls await next()
// Handler runs	                CreateActivity.Handler	        Returns Result<string>.Success(activity.Id)
// Response returns	            ValidationBehavior	            Returns handler's response up the pipeline
// Response returns	            MediatR	                        Returns response to controller
// Controller receives result	ActivitiesController	        Handles result (e.g., via HandleResult)

public class ActivitiesController : BaseApiController
{
    // By segregating Command and Query Responbility, every class is kept simple to handle either read or write operations only.
    // For Queries, they typically return DTOs.
    // API endpoint: GET /api/activities
    [HttpGet]

    // GetActivities method handles HTTP Get request /api/activities.
    // It returns a List of ActivityDto objects in ActionResult.
    // Task <TResult> means the asynchronous operation returns a value of type TResult upon completion.
    public async Task<ActionResult<List<ActivityDto>>> GetActivities()

    {
        // instantiates request object Query to Mediator. 
        // Mediator knows which handler to use because a handler in the Application layer is specified to handle GetActivityList.Query .
        // After receiving result from Application layer, returns ActionResult which wraps a List of ActivityDto (match Domain.Activity)
        // into JSON body of ActionResult containing Status Code: 200 OK.
        return await Mediator.Send(new GetActivityList.Query());
    }

    // *******************************************************************************************************

    [HttpGet("{id}")]

    // GetActivityDetail uses a route template parameter {id}, so it expects a value in the URL path.
    // It returns a Result type of ActivityDto object indicating success or failure to HandleResult.
    // HandleResult inherited from BaseApiController will return responses accordingly.
    public async Task<ActionResult<ActivityDto>> GetActivityDetail(string id)

    {

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

    // when data is needed in the path, must specify in the HTTP method.
    [HttpDelete("{id}")]
    [Authorize(Policy = "IsActivityHost")]

    // return Status 200 Ok if delete successfully.
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
