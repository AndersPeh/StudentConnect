using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence;

// (DbContextOptions options) is Depedency Injection from Program.cs to tell AppDbContext how to connect to the database.
// options must be provided to create the class, it is configured by Program.cs in API layer.
// This class inherits database interaction capabilities from IdentityDbContext.
// IdentityDbContext<User> is required for the authentication system to work because it knows how to create and manage tables needed for the Identity system.
//  IdentityDbContext<User> already configures User for EF Core to query or save it. We just to need configure other entities here.
public class AppDbContext(DbContextOptions options) : IdentityDbContext<User>(options)
{
    // DbSet<Activity> provides methods (like Add, Remove) and LINQ extension methods (like Where) to manage Activity entities.
    // Entity Framework core maps DbSet<Activity> to Activities table in the database, so Activity represents Activities table.
    // When any part uses AppDbContext.Activities, EF will translate C# queries into SQL commands.
    public DbSet<Activity> Activities { get; set; } = null!;

    // This allows EF Core to query / save ActivityAttendee data.
    public DbSet<ActivityAttendee> ActivityAttendees { get; set; } = null!;

    public DbSet<Photo> Photos { get; set; } = null!;

    public DbSet<Comment> Comments { get; set; } = null!;

    public DbSet<UserFollowing> UserFollowings { get; set; } = null!;

    // OnModelCreating is for configuring C# classes map to the database schema.
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Because EF Core usually looks for an ID from entity to use as a primary key.
        // For entities that use composite primary key, need to specify it in OnModelCreating.
        base.OnModelCreating(builder);

        // define relationships, keys, constraints that cannot be expressed by C# property types and attributes.
        // add ActivityId and UserId as composite primary key to ActivityAttendee.
        builder.Entity<ActivityAttendee>(activityAttendeeEntity => activityAttendeeEntity.HasKey(compositeKey => new { compositeKey.ActivityId, compositeKey.UserId }));

        // establish relationship: ActivityAttendee belongs to a User, so UserId is a foreign key.
        builder.Entity<ActivityAttendee>()
            .HasOne(activityAttendeeInstance => activityAttendeeInstance.User)
            // .WithMany tells EF Core that the navigation property on the User entity is called Activities.
            // userInstance.Activities represents ActivityAttendee records for that User.
            .WithMany(userInstance => userInstance.Activities)
            .HasForeignKey(activityAttendeeInstance => activityAttendeeInstance.UserId);

        // establish relationship: ActivityAttendee belongs to an Activity, so ActivityId is a foreign key.
        builder.Entity<ActivityAttendee>()
            .HasOne(activityAttendeeInstance => activityAttendeeInstance.Activity)
            // .WithMany(x => x.Attendees) tells EF Core that the navigation property on the Activity entity is called Attendees.
            // activityInstance.Attendees represents ActivityAttendee records for that Activty.
            .WithMany(activityInstance => activityInstance.Attendees)
            .HasForeignKey(activityAttendeeInstance => activityAttendeeInstance.ActivityId);

        builder.Entity<UserFollowing>(userFollowingEntity =>
        {
            // UserFollowing table has composite primary key of ObserverId and TargetId.
            userFollowingEntity.HasKey(compositeKey => new { compositeKey.ObserverId, compositeKey.TargetId });

            // UserFollowing entity belongs to 1 Observer from User entity (Foreign Key Observer Id).
            userFollowingEntity.HasOne(userFollowingInstance => userFollowingInstance.Observer)
            // A User entity has many Followings navigation property.
                .WithMany(userInstance => userInstance.Followings)
                .HasForeignKey(userFollowingInstance => userFollowingInstance.ObserverId)
                // When parent entity (User entity) is deleted, the UserFollowing records where the User is Observer will be deleted.
                .OnDelete(DeleteBehavior.Cascade);

            // UserFollowing entity belongs to 1 Target from User entity (Foreign Key Target Id).
            userFollowingEntity.HasOne(userFollowingInstance => userFollowingInstance.Target)
                // A User entity has many Followers navigation property.
                .WithMany(userInstance => userInstance.Followers)
                .HasForeignKey(userFollowingInstance => userFollowingInstance.TargetId)
                // When parent entity (User entity) is deleted, the UserFollowing records where the User is Target will be deleted.
                .OnDelete(DeleteBehavior.Cascade);
        });

        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            // When saving to the database, converts any DateTime value to UTC before saving.
            value => value.ToUniversalTime(),
            // When reading from the database, treat the value as UTC.
            value => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        );

        // Go through every entity type in models (Activity, Comment, Photo, ActivityAttendee),
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            // Go through every property of each entity, if the property is
            // DateTime, use the dateTimeConverter to read or save it as UTC.
            foreach (var property in entityType.GetProperties())
            {

                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
            }
        }
    }
}
