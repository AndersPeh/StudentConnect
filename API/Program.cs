using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Register controllers as services for service provider to request an instance later.
builder.Services.AddControllers();

// provides database configuration to AppDbContext (which is options object).
// It uses AddDbContext, so it is dependent on Persistence layer.
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
// It processes request by running MapControllers and sends back the response afterwards.

// MapControllers finds matching controller to handle HTTP request.
app.MapControllers();

// Create scope for startup tasks to resolve and dispose services after try catch,
// before the application starts running (app.Run).
using var scope = app.Services.CreateScope();

// assign service provider to services to get instance of registered services later.
var services = scope.ServiceProvider;

try
{
    // get an instance of AppDbContext service from the service provider, to perform database work later.
    var context = services.GetRequiredService<AppDbContext>();

    // Once the application starts running, it will create database and apply migration.
    await context.Database.MigrateAsync();

    // Seed data using DbInitializer from Persistence layer.
    await DbInitializer.SeedData(context);
}

catch (Exception ex)

{
    var logger = services.GetRequiredService<ILogger<Program>>();

    logger.LogError(ex, "An error occured during migration.");
}

app.Run();
