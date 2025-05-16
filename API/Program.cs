// Flow: Client Request -> Web Server -> HTTP Request Pipeline -> MapControllers 
// -> Find Matching Controller from Registered Controllers (builder.Services.)
// -> Execute Controller Method in Application Layer.

using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add internal and external services to the container.
// Register controllers that execute different HTTP requests.
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
// It processes request by running MapControllers and sends back the response afterwards.

// MapControllers finds matching controller to handle HTTP request.
app.MapControllers();

app.Run();
