using Application.Activities.Commands;
using Application.Activities.Queries;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

// Web Socket Controller. The client connects to this endpoint from the browser which will maintain the connection to this Hub
// for real time connections. 
// Need to inject IMediator to use Handlers for getting list of comments and adding a comment.
// When a new comment is added to an actviity, the backend sends a real time message to all connections
// in the group for that activity.
public class CommentHub(IMediator mediator) : Hub
{
    // When the client is connected to the SignalR Hub, the client can use the Hub to create a comment.
    // SignalR deserialises the payload from the client { body: "Hello!", activityId: "123" } into 
    // an instance of AddComment.Command. AddComment.Command command is just to specify the parameter matches AddComment.Command object.
    public async Task SendComment(AddComment.Command command)
    {
        // SendComment method sends the command to the Mediator.
        // The Mediator will return CommentDto from the AddComment handler.
        var comment = await mediator.Send(command);

        // Send the "ReceiveComment" message with new commentDto to every client connection in the Activity Group (based on ActivityId).
        await Clients.Group(command.ActivityId).SendAsync("ReceiveComment", comment.Value);
    }

    // OnConnectedAsync is for specifying actions that will be executed when a client connects to the SignalR Hub.
    // When a client connects to this hub, it passes an activityId as a query parameter.
    public override async Task OnConnectedAsync()
    {
        // Get query parameters through Context.GetHttpContext().
        var httpContext = Context.GetHttpContext();

        // check if activityId exists in http context. Put ? in case it doesnt exist.
        var activityId = httpContext?.Request.Query["activityId"];

        // If activityId is null or empty, throw an Exception.
        if (string.IsNullOrEmpty(activityId)) throw new HubException("No Activity with this Id");

        // ConnectionId represents a connection in a browser tabs.
        // Groups.AddToGroupAsync adds a specific connection (using ConnectionId) to a group named (activityId).
        // so each activity has its own SignalR group, named by its activityId.
        // As a user connects to the Hub for a specific activity, all users viewing the same activity
        // will be in the same SignalR group.
        // Put ! for telling the compiler that activityId will be available when we use it.
        await Groups.AddToGroupAsync(Context.ConnectionId, activityId!);

        // pass the activityId from query parameter to the Query in GetComments class. Then Mediator will run the pipeline.
        var result = await mediator.Send(new GetComments.Query { ActivityId = activityId! });

        // send list of comments to the client just connected to the Hub and triggered OnConnectedAsync with the current connection Id, 
        // not to all clients or group.
        // send a message "LoadComments" with list of comments (result.value) to clients that just connected only.
        await Clients.Caller.SendAsync("LoadComments", result.Value);
    }
}
