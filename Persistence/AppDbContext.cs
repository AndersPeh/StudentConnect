using System;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

// (DbContextOptions options) is the primary constructor for AppDbContext.
// options must be provided to create the class, it is configured by Program.cs in API layer.
// This class inherits from DbContext. When AppDbContext object is created, it will call
// DbContext constructor, to pass options for AppDbContext to interact with the database.
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    // DbSet<Activity> provides methods (like Add, Remove) and LINQ extension methods (like Where) to manage Activity entities.
    // Entity Framework core maps DbSet<Activity> to Activities table in the database, so Activities property represents Activities table.
    // When any part uses context.Activities, EF will translate C# queries into SQL commands.
    public required DbSet<Activity> Activities { get; set; }
}
