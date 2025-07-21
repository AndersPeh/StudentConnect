using Application.Activities.DTOs;
using Application.Activities.Queries;
using Application.Core;
using AutoMapper;
using Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace StudentConnect.Application.Tests;

public class GetActivityListTests
{
    // _context and _mapper hold the instances of GetActivityList Handler's dependencies.
    // They are intialised once only.

    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    // For each Fact in GetActivityListTests class, xUnit creates a new instance of the GetActivityListTests() constructor. 
    public GetActivityListTests()
    {
        // Setup IMapper to tell it to use the exact same mapping rules from MappingProfifles as main application.
        // So the real mapping logic is tested.
        var mapperConfig = new MapperConfiguration(configurator =>
        {
            configurator.AddProfile<MappingProfiles>();
        });
        // _mapper holds the configured instance.
        _mapper = mapperConfig.CreateMapper();

        // Setup AppDbContext to use in-memory database. Configures database connection and the database is discarded after each test.
        var dbConfig = new DbContextOptionsBuilder<AppDbContext>()
            // The in-memory database has a unique name (Guid) to ensure this test has an isolated database, separated
            // from other tests. It also ensures every single test runs on a new, completely empty and isolated database.
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            // produces the final DbContextOptionsBuilder<AppDbContext> object after taking rule added (UseInMemoryDatabase).
            .Options;

        // _context holds a new instance of configured AppDbContext using in-memory database.
        _context = new AppDbContext(dbConfig);
    }

    // xUnit: Fact marks below should be run by the test runner.
    [Fact]
    public async Task Handle_ShouldReturnListOfActivityDtos_WhenActivitiesExist()
    {
        // Arrange: Set up conditions and data needed for the test.

        // User entities.
        var users = new List<User>
        {
            new() { Id = Guid.NewGuid().ToString(), UserName = "andy", DisplayName = "Andy" },
            new() { Id = Guid.NewGuid().ToString(), UserName = "bobby", DisplayName = "Bobby" },
            new() { Id = Guid.NewGuid().ToString(), UserName = "cindy", DisplayName = "Cindy" },
        };

        // Activity entities.
        var activities = new List<Activity>
        {
            new Activity
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Dotnet Gathering",
                Date = DateTime.UtcNow,
                Description = "Introduction to C# Unit Testing.",
                Category = "Backend",
                IsCancelled = false,
                City = "Brisbane",
                Venue = "The Precinct",
                Attendees =
                [
                    // Andy is the host
                    new() { UserId = users[0].Id, User = users[0], IsHost = true },
                    // Bobby is an attendee
                    new() { UserId = users[1].Id, User = users[1] }
                ]
            },

            new Activity
            {
                Id = Guid.NewGuid().ToString(),
                Title = "BrisJS Gathering",
                Date = DateTime.UtcNow,
                Description = "Introduction to TypeScript Unit Testing.",
                Category = "Frontend",
                IsCancelled = false,
                City = "Brisbane",
                Venue = "Auto & General Office",
                Attendees =
                [
                    // Cindy is the host
                    new() { UserId = users[2].Id, User = users[2], IsHost = true }
                ]
            },
        };

        // Add data to the in-memory database.
        _context.Users.AddRange(users);
        _context.Activities.AddRange(activities);
        await _context.SaveChangesAsync();

        // Act: Execute the code to be tested.
        // Create an instance of the handler that we are testing, then pass pre-configured in-memory context and real mapper to it.
        var handler = new GetActivityList.Handler(_context, _mapper);

        // Create the query object which is empty (no need request details to get all activityattendees with ProjectTo).
        var query = new GetActivityList.Query();

        // Execute Handle method which will run the logic against the in-memory database.
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert: Check the result using Fluent Assertions for readable checks.

        // Create the expected result after mapping for comparing with the result later.
        var expectedActivities = _mapper.Map<List<ActivityDto>>(activities);

        // This performs a deep comparison to ensure the result from the handler is equivalent to expectedActivities
        // after mapping our test data.
        result.Should().BeEquivalentTo(expectedActivities);

    }
}

// result.Should().NotBeNull();

// // the result shold contain a list with 2 activities in this test case.
// result.Should().HaveCount(2);

// var databaseFirstActivity = activities.First(x => x.Title == "Dotnet Gathering");
// var resultDtoFirstActivity = result.First(x => x.Title == "Dotnet Gathering");

// resultDtoFirstActivity.Should().BeOfType<ActivityDto>();
// resultDtoFirstActivity.Category.Should().Be(databaseFirstActivity.Category);
// resultDtoFirstActivity.Venue.Should().Be(databaseFirstActivity.Venue);

// var databaseSecondActivity = activities.First(x => x.Title == "BrisJS Gathering");
// var resultDtoSecondActivity = result.First(x => x.Title == "BrisJS Gathering");

// resultDtoSecondActivity.Should().BeOfType<ActivityDto>();
// resultDtoSecondActivity.Category.Should().Be(databaseSecondActivity.Category);
// resultDtoSecondActivity.Venue.Should().Be(databaseSecondActivity.Venue);