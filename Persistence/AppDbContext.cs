using System;
using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

// (DbContextOptions options) is Depedency Injection from Program.cs to tell AppDbContext how to connect to the database.
// options must be provided to create the class, it is configured by Program.cs in API layer.
// This class inherits database interaction capabilities from IdentityDbContext, all identity operations are strongly typed to User class.
public class AppDbContext(DbContextOptions options) : IdentityDbContext<User>(options)
{
    // DbSet<Activity> provides methods (like Add, Remove) and LINQ extension methods (like Where) to manage Activity entities.
    // Entity Framework core maps DbSet<Activity> to Activities table in the database, so Activities property represents Activities table.
    // When any part uses context.Activities, EF will translate C# queries into SQL commands.
    public required DbSet<Activity> Activities { get; set; }
}
