using System;
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
    // Entity Framework core maps DbSet<Activity> to Activities table in the database, so Activities property represents Activities table.
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
        base.OnModelCreating(builder);

        // define relationships, keys, constraints that cannot be expressed by C# property types and attributes.
        // add ActivityId and UserId Primary Keys to ActivityAttendee.
        builder.Entity<ActivityAttendee>(x => x.HasKey(a => new { a.ActivityId, a.UserId }));

        // establish relationship: ActivityAttendee belongs to a User, so UserId is a foreign key.
        // .WithMany(x => x.Activities) tells EF Core that the navigation property on the User entity is called Activities.
        // This Activities represents ActivityAttendee records for that User.
        builder.Entity<ActivityAttendee>()
            .HasOne(x => x.User)
            .WithMany(x => x.Activities)
            .HasForeignKey(x => x.UserId);

        // establish relationship: ActivityAttendee belongs to an Activity, so ActivityId is a foreign key.
        // .WithMany(x => x.Attendees) tells EF Core that the navigation property on the Activity entity is called Attendees.
        // This Attendees represents ActivityAttendee records for that Activty.
        builder.Entity<ActivityAttendee>()
            .HasOne(x => x.Activity)
            .WithMany(x => x.Attendees)
            .HasForeignKey(x => x.ActivityId);

        builder.Entity<UserFollowing>(x =>
        {
            x.HasKey(k => new { k.ObserverId, k.TargetId });

            x.HasOne(o => o.Observer)
                .WithMany(f => f.Followings)
                .HasForeignKey(o => o.ObserverId)
                .OnDelete(DeleteBehavior.Cascade);

            x.HasOne(o => o.Target)
                .WithMany(f => f.Followers)
                .HasForeignKey(o => o.TargetId)
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
