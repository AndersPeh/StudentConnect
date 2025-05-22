using API.Middleware;
using Application.Activities.Queries;
using Application.Activities.Validators;
using Application.Core;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Register controllers as services automatically with DI container and for service provider to request an instance later.
builder.Services.AddControllers();

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

// Registers AddAutoMapper with DI container.
// AutoMapper will look for typeof(MappingProfiles) in Assembly of Application Layer, 
// scans classes that inherit from AutoMapper.Profile to instantiate IMapper.
builder.Services.AddAutoMapper(typeof(MappingProfiles).Assembly);

// Registers Validators with DI container and it will scan all validators in Assembly of Application Layer,
// to provide information for validating commands.
builder.Services.AddValidatorsFromAssemblyContaining<CreateActivityValidator>();

// AddTransient means DI container instantiates this service per HTTP request and disposes after using.
builder.Services.AddTransient<ExceptionMiddleware>();

var app = builder.Build();

// *******************************************************************************************************
// Middleware: Configure the HTTP request pipeline.

// Exception must be placed before any middleware to use it to catch and ahndle exceptions that occur in subsequent middleware or in controllers/ services.
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod()
    .WithOrigins("http://localhost:3000", "https://localhost:3000"));

// When the server receives a HTTP Request, MapControllers finds matching controller to handle it 
// by dropping "Controller" from controllers to match route to controller name.
app.MapControllers();

// *******************************************************************************************************
// Create temporary DI scope for startup tasks to resolve and dispose services (AppDbContext instance) after try block finishes and before the application starts running (app.Run).
using var scope = app.Services.CreateScope();

// assign service provider specific to this newly created scope to get instance of registered services later.
var services = scope.ServiceProvider;

try
{
    // AppDbContext is the service to interact with database.
    // get an instance of <AppDbContext> service from the service provider.
    var context = services.GetRequiredService<AppDbContext>();

    // Once the application starts running, it will create database and ensure database schema is updated.
    await context.Database.MigrateAsync();

    // Seed initial data using DbInitializer from Persistence layer.
    await DbInitializer.SeedData(context);
}

catch (Exception ex)

{
    var logger = services.GetRequiredService<ILogger<Program>>();

    logger.LogError(ex, "An error occured during migration.");
}

// *******************************************************************************************************
// start Kestrel web server.
app.Run();
