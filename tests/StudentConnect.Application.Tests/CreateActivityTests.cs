
using Application.Activities.Commands;
using Application.Activities.DTOs;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Persistence;

namespace StudentConnect.Application.Tests;

public class CreateActivityTests
{
    // _context, _mapper and _userAccessorMock hold the instances of CreateActivity Handler's dependencies.
    // They are intialised once only.

    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    private readonly AppDbContext failingContext;
    private readonly Mock<IUserAccessor> _userAccessorMock;

    // For each Fact in CreateActivityTests class, xUnit creates a new instance of the CreateActivityTests() constructor. 
    public CreateActivityTests()
    {
        // Setup IMapper to tell it to use the exact same mapping rules from MappingProfiles as main application.
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

        // Use a special context that overrides SaveChangesAsync to always fail.
        failingContext = new FailingAppDbContext(dbConfig);

        // Setup a mock for IUserAccessor because there is no HTTP context to get a user.
        _userAccessorMock = new Mock<IUserAccessor>();

    }

    // A helper class for testing database save failures.
    // It inherits from the real AppDbContext but overrides the save method.
    public class FailingAppDbContext : AppDbContext
    {
        public FailingAppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }
    }

    // xUnit: Fact marks below should be run by the test runner.
    [Fact]
    public async Task Handle_ShouldCreateActivityAndAttendee_AndReturnSuccessResult()
    {
        // Arrange: Set up conditions and data needed for the test.

        // User entities.
        var user = new User { Id = Guid.NewGuid().ToString(), UserName = "andy", DisplayName = "Andy" };

        // Add data to the in-memory database.
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Configure the mock to simulate the process of getting a user.
        // When GetUserAsync is called, return the test user.
        _userAccessorMock.Setup(x => x.GetUserAsync()).ReturnsAsync(user);

        // Create a DTO that represents the incoming request from the client.
        var createActivityDto = new CreateActivityDto
        {
            Title = "DotNet Gathering",
            Date = DateTime.UtcNow.AddDays(10),
            Description = "How to use xUnit.",
            Category = "Tech",
            City = "Brisbane",
            Venue = "The Precinct",
        };

        // Act: Execute the code to be tested.
        // Create an instance of the handler that we are testing, then pass pre-configured in-memory context, real mapper and 
        // mocked user accessor to it. 
        // .Object is crucial for passing the mocked instance to the handler.
        var handler = new CreateActivity.Handler(_context, _mapper, _userAccessorMock.Object);

        // Create the command object which takes createActivityDto as its ActivityDto.
        var command = new CreateActivity.Command { ActivityDto = createActivityDto };

        // Execute Handle method which will run the logic against the in-memory database.
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert: Check the result using Fluent Assertions for readable checks.

        result.IsSuccess.Should().BeTrue();

        // The new activity ID should be returned.
        result.Value.Should().NotBeNullOrEmpty();

        // Check the activity was saved to the database correctly.
        var activityInDatabase = await _context.Activities
            .Include(activity => activity.Attendees)
            .FirstOrDefaultAsync(activity => activity.Id == result.Value);

        activityInDatabase.Should().NotBeNull();

        // Perform deep comparison between the DTO and the created entity. 
        activityInDatabase.Should().BeEquivalentTo(createActivityDto);

        // Check if the host is the user who created the activity.
        activityInDatabase.Attendees.Should().HaveCount(1);
        activityInDatabase.Attendees.First().UserId.Should().Be(user.Id);
        activityInDatabase.Attendees.First().IsHost.Should().BeTrue();

    }

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenDatabaseSaveFails()
    {
        // Arrange: Set up conditions and data needed for the test.

        // User entities.
        var user = new User { Id = Guid.NewGuid().ToString(), UserName = "andy", DisplayName = "Andy" };

        // Add data to the in-memory database.
        failingContext.Users.Add(user);

        // Configure the mock to simulate the process of getting a user.
        // When GetUserAsync is called, return the test user.
        _userAccessorMock.Setup(x => x.GetUserAsync()).ReturnsAsync(user);

        // Create a DTO that represents the incoming request from the client.
        var createActivityDto = new CreateActivityDto
        {
            Title = "DotNet Gathering",
            Date = DateTime.UtcNow.AddDays(10),
            Description = "How to use xUnit.",
            Category = "Tech",
            City = "Brisbane",
            Venue = "The Precinct",
        };

        // Act: Execute the code to be tested.
        // Create an instance of the handler that we are testing, then pass pre-configured in-memory context, real mapper and 
        // mocked user accessor to it. 
        // .Object is crucial for passing the mocked instance to the handler.
        var handler = new CreateActivity.Handler(failingContext, _mapper, _userAccessorMock.Object);

        // Create the command object which takes createActivityDto as its ActivityDto.
        var command = new CreateActivity.Command { ActivityDto = createActivityDto };

        // Execute Handle method which will run the logic against the in-memory database.
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert: Check the result using Fluent Assertions for readable checks.
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(400);
        result.Error.Should().Be("Failed to create the activity");


    }
}

