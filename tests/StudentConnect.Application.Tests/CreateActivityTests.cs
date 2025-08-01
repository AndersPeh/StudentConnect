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

// This class contains ALL tests for the CreateActivity feature.
public class CreateActivityTests
{
    // They are intialised once only. They can only be assigned a value in the constructor, preventing accidental changes later.
    private readonly IMapper _mapper;
    private readonly Mock<IUserAccessor> _userAccessorMock;

    // This is the constructor. xUnit creates a new instance of this entire class for every single test ([Fact]) it runs.
    // This ensures that each test starts with a fresh, clean state and cannot interfere with other tests.
    public CreateActivityTests()
    {
        // Setup IMapper to use the real mapping profiles so we are testing against the exact same mapping logic as our main application.
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfiles>());
        _mapper = mapperConfig.CreateMapper();

        // Setup a mock for IUserAccessor. The real IUserAccessor depends on an HTTP context to find the logged-in user, 
        // which doesn't exist in a test environment. Moq creates a fake version that we can control completely.
        _userAccessorMock = new Mock<IUserAccessor>();
    }

    // The [Fact] attribute from xUnit marks this method as an automated test.
    [Fact]
    public async Task Handle_ShouldCreateActivityAndReturnSuccess_WhenRequestIsValid()
    {
        // Arrange: Set up all conditions and data for the test.

        // Setup AppDbContext to use a unique in-memory database for this specific test.
        var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
            // UseInMemoryDatabase tells EF Core to create a temporary database in memory, so it will be discarded after each use.
            .UseInMemoryDatabase(databaseName: "CreateActivity_SuccessDb")
            .Options;

        // Create a real instance of the database context using these options.
        var context = new AppDbContext(dbOptions);

        // 1. Create a test 'User' object that will act as the person creating the activity.
        var user = new User { Id = "user-host-a", UserName = "andy mike", DisplayName = "Andy Mike" };
        // Add this user to the in-memory database.
        context.Users.Add(user);
        // Save the changes so the user exists in the database before the handler runs.
        await context.SaveChangesAsync();

        // 2. Configure the mock's behavior. .Setup() tells Moq: "When the GetUserAsync() method is called on this mock object,
        // .ReturnsAsync(user) tells it to return the user object created.   
        _userAccessorMock.Setup(x => x.GetUserAsync()).ReturnsAsync(user);

        // 3. Create the input DTO. This simulates the JSON data coming from the client's web browser.
        var createActivityDto = new CreateActivityDto
        {
            Title = "Dotnet Gathering",
            Date = DateTime.UtcNow.AddDays(10),
            Description = "Backend activity.",
            Category = "Tech",
            City = "Brisbane",
            Venue = "The Precinct"
        };

        // 4. Create the command object by wrapping the DTO inside and it will be sent to the handler.
        var command = new CreateActivity.Command { ActivityDto = createActivityDto };

        // 5. Instantiate the handler by injecting its dependencies: the real context, real mapper, and the MOCKED user accessor.
        // .Object is how to get the actual controllable mock instance from the Moq wrapper.
        var handler = new CreateActivity.Handler(context, _mapper, _userAccessorMock.Object);

        // Act: Execute the single method being tested.
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert: Verify the outcome.

        // Check that the handler returned a success result.
        result.IsSuccess.Should().BeTrue();
        // Check that the success result contains a value. The new activity ID should be returned.
        result.Value.Should().NotBeNullOrEmpty();

        // Verify the activity was actually saved to the database correctly.
        // query the database again to see if the handler correctly saved the new activity.
        var activityInDb = await context.Activities
        // eagerly load Activityattendees.
            .Include(a => a.Attendees)
            .FirstOrDefaultAsync(a => a.Id == result.Value);

        // Verify that an activity with the new ID was actually found in the database.
        activityInDb.Should().NotBeNull();
        // BeEquivalentTo performs a deep, property-by-property comparison to ensure the saved entity matches the input DTO.
        activityInDb.Should().BeEquivalentTo(createActivityDto);

        // Verify that the creator was added as the host attendee.
        activityInDb.Attendees.Should().HaveCount(1);
        activityInDb.Attendees.First().UserId.Should().Be(user.Id);
        activityInDb.Attendees.First().IsHost.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenDatabaseSaveFails()
    {
        // ARRANGE: Set up the failure scenario.

        // 1. Configure the database options, again with a unique name for isolation.
        var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "CreateActivity_DbFailDb")
            .Options;
        // Use a special context that overrides SaveChangesAsync to always fail.
        var failingContext = new FailingAppDbContext(dbOptions);

        // 2. Define a user and add it to the failing context.
        var user = new User { Id = "user-host-b", UserName = "failuser", DisplayName = "Fail User" };
        failingContext.Users.Add(user);
        // We don't need to SaveChanges here because the whole point is that saving will fail.

        // Configure the mock to return the mock user, just like in the success test.
        _userAccessorMock.Setup(x => x.GetUserAsync()).ReturnsAsync(user);

        // 3. Create the DTO and command.
        var createActivityDto = new CreateActivityDto { Title = "A Failing Activity" };
        var command = new CreateActivity.Command { ActivityDto = createActivityDto };

        // 4. Instantiate the handler by injecting the FAILING context. 
        var handler = new CreateActivity.Handler(failingContext, _mapper, _userAccessorMock.Object);

        // Act: Execute the method being tested which wil fail during the SaveChangesAsync call.
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert: Verify the failure outcome.
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Failed to create the activity");
        result.Code.Should().Be(400);
    }

    // This is a private helper class, nested inside the test class because it's only used here.
    // It inherits from the real AppDbContext, so it has all the same properties and methods.
    private class FailingAppDbContext(DbContextOptions<AppDbContext> options) : AppDbContext(options)
    {
        // Override SaveChangesAsync to always return 0, simulating a failed database operation.
        // The return type must be Task<int> to match the base method's signature.
        // Task.FromResult() is a helper that creates an already completed task with the given result.
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }
    }
}