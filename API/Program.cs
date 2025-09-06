using API.Middleware;
using API.SignalR;
using Application.Activities.Queries;
using Application.Activities.Validators;
using Application.Core;
using Application.Interfaces;
using Domain;
using FluentValidation;
using Infrastructure;
using Infrastructure.Photos;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Persistence;

// AddScoped: Service is created once per HTTP request.
// AddTransient: Service is created every time it is requested (could be multiple times per request).
// AddSingleton: Service is created once for the entire application lifetime.

var builder = WebApplication.CreateBuilder(args);

// Add services to the Dependency Injection container.
// Register controllers as services automatically with DI container and for service provider to request an instance later.
builder.Services.AddControllers(opt =>
{
    // AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build() creates AuthorizationPolicy that requires user to be authenticated.
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    // opt.Filters.Add adds AuthorizationPolicy to every API endpoint in every controller. So user must be authenticated with cookie to make any request.
    opt.Filters.Add(new AuthorizeFilter(policy));
});

// provides DI container the database configuration to AppDbContext (which is options object).
// <AppDbContext> specifies that the DbContext being configured is AppDbContext class from Persistence Layer.
// AddDbContext registers AppDbContext with a scoped lifetime in dependency injection container.
// When a HTTP request is finished, this AppDbContext instance is disposed of.
// AddDbContext<AppDbContext> tells DI container that when any part needs AppDbContext, it knows how to make one.
builder.Services.AddDbContext<AppDbContext>(opt =>

{
    // use Sqlite with this connection string.
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Registers CORS service which uses HTTP headers to tell Browsers to give the application 
// running at localhost:3000 (frontend) access to resources from localhost:5001 (backend)
builder.Services.AddCors();

// To use SignalR Hub.
builder.Services.AddSignalR();

// Registers Mediator service with DI container. Mediator constructs a pipeline, 
// behaviors will be placed first in the pipeline followed by Handler.
builder.Services.AddMediatR(x =>
{
    // Registers a generic type behavior <,> (TRequest, TResponse). 
    // For every command or query processed by the Mediator, Mediator will check if it can instantiate Validation behavior, 
    // if the request has a corresponding validator, it will be validated before Mediator sends it to handlers. So Validation logic will be kept in Application layer.
    x.AddOpenBehavior(typeof(ValidationBehavior<,>));

    // Registered Handler with DI container, Mediator will scan the Application layer assembly to access all Handlers.
    x.RegisterServicesFromAssemblyContaining<GetActivityList.Handler>();

});

// tells DI container to map the IUserAccessor interface to the UserAccessor class.
// When any class requires injection of IUserAccessor, DI container injects UserAccessor because it is a concrete class that implements the interface.
// AddScoped means a new instance is created once per HTTP request, the DI container will use the same instance for every class that uses
// DI to inject it until the HTTP request is finished.
builder.Services.AddScoped<IUserAccessor, UserAccessor>();

// DI Container knows PhotoService is the implementation class of IPhotoService. DI Container generates an instance per HTTP Request so it is reused
// until it is discarded after the Http Request is done.
builder.Services.AddScoped<IPhotoService, PhotoService>();

// Registers AddAutoMapper with DI container.
// AutoMapper will look for typeof(MappingProfiles) in Assembly of Application Layer, 
// scans classes that inherit from AutoMapper.Profile to instantiate IMapper.
builder.Services.AddAutoMapper(typeof(MappingProfiles).Assembly);

// Registers Validators with DI container and it will scan all validators in Assembly of Application Layer,
// to provide information for validating commands.
builder.Services.AddValidatorsFromAssemblyContaining<CreateActivityValidator>();

// AddTransient means DI container instantiates this service per HTTP request and disposes after using.
builder.Services.AddTransient<ExceptionMiddleware>();

// AddIdentityApiEndpoints<User> get a set of API endpoints for user authentication, for .Net to manage users based on Domain.User entity.
builder.Services.AddIdentityApiEndpoints<User>(opt =>
{
    // unique email is required because username has to be email address to login in asp.net identity.
    opt.User.RequireUniqueEmail = true;
})
// registers the services necessary for working with roles like creating roles, assigning roles...
.AddRoles<IdentityRole>()
// tells the Identity system to use EF Core for storing user and role information.
.AddEntityFrameworkStores<AppDbContext>();

// Adds Authorization services to the DI container.
builder.Services.AddAuthorization(opt =>
{
    // Adds a new policy named IsActivityHost, for reference in ActivitiesController.cs.
    opt.AddPolicy("IsActivityHost", policy =>
    {
        // In order to satisfy IsActivityHost policy, the authorization middleware will look for a handler to satisfy IsHostRequirement.
        policy.Requirements.Add(new IsHostRequirement());
    });
});

// Registers the handler with the DI container. When the system processes an IAuthorizationRequirement and finds that it is an IsHostRequirement,
// the class that should be used is IsHostRequirementHandler. So the authorisation middleware will use IsHostRequirementHandler to satisfy IsHostRequirement.
builder.Services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();

// Registers CloudinarySettings configuration settings with DI container to use in other files.
// GetSection("CloudinarySettings") gets the CloudName, ApiKey and ApiSecret from settings file, 
// then bind that configured section to a class CloudinarySettings in the Infrastructure project, make it strongly type.
// so any file can inject <CloudinarySettings> and it will be type safe as the type is created in CloudinarySettings of the Infrastructure project.
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

// creates the web application object with services to define HTTP request pipeline.
var app = builder.Build();

// *******************************************************************************************************
// Middleware: Configure the HTTP request pipeline.

// Exception must be placed before any middleware to use it to catch and handle exceptions that occur in subsequent middleware or in controllers/ services.
// Make it global error handler. The DI container creates an instance of ExceptionMiddleware and insert it into the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

// allows client of the address to access to the API.
app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod()
    // allows Browser to send credentials such as cookies to the API.
    .AllowCredentials()
    // Origin Header of the HTTP request must be:
    .WithOrigins("http://localhost:3000", "https://localhost:3000"));

// Activate Authentication and Authorisation middleware before using Controller to handle HTTP requests.
// It inspects the HTTP request for the authentication cookie, decodes it to create a User object to know
// if User is logged in and the identity of the User. The User object becomes HttpContext.User.
app.UseAuthentication();
// It takes authenticated User and check if the User is allowed to access the requested endpoint.
// Because there are some endpoints where authorisation policy is enforced.
// like opt.Filters.Add(new AuthorizeFilter(policy)); and policy.Requirements.Add(new IsHostRequirement());
app.UseAuthorization();

// Both app.MapControllers() and app.MapGroup("api").MapIdentityApi<User>() run at the same time
// to provide all endpoints for the application.
// The server performs a single search against endpoints to find the endpoint that matches
// HTTP method (GET, POST...) and the request path (/api/something).
// When the server receives a HTTP Request, MapControllers finds matching controller to handle it 
// by dropping "Controller" from controllers to match route to controller name.
// The route must match api/[controller] defined in BaseApiController so the server can find the right controller for the request.
app.MapControllers();

// The route of /api will be added to any route of identity endpoints such as login, register, logout, manage/info etc.
// For Identity endpoints provided by .Net Identity, the default routes dont come with /api,
// so need to add api to make it work.
app.MapGroup("api").MapIdentityApi<User>();

// Tell the Server to send the HTTP request from /comments endpoint to CommentHub .
// For example https://localhost:5001/comments?activityId=...
app.MapHub<CommentHub>("/comments");

// *******************************************************************************************************************************************************
// Create temporary DI scope for startup tasks to resolve and dispose services (AppDbContext instance) after try block finishes and before the application starts running (app.Run).
using var scope = app.Services.CreateScope();

// assign service provider specific to this newly created scope to get instance of registered services later.
var services = scope.ServiceProvider;

try
{
    // AppDbContext is the service to interact with database.
    // get an instance of AppDbContext service from the service provider.
    var context = services.GetRequiredService<AppDbContext>();

    // get an instance of UserManager service that only accepts User Entity from the service provider.
    var userManager = services.GetRequiredService<UserManager<User>>();

    // Once the application starts running, it will create database and ensure database schema is updated.
    await context.Database.MigrateAsync();

    // Seed initial data using DbInitializer from Persistence layer.
    await DbInitializer.SeedData(context, userManager);
}

catch (Exception ex)

{
    // <ILogger<Program>> requests an instance of ILogger that is specifically configured for the Program class.
    var logger = services.GetRequiredService<ILogger<Program>>();

    logger.LogError(ex, "An error occured during migration.");
}

// *******************************************************************************************************
// start Kestrel web server.
app.Run();
