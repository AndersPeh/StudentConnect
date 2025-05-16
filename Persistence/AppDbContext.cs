using System;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

// DbContext allows us to interact with the database.
// It connects domain models and the database.
// (DbContextOptions options) is the constructor for AppDbContext.
// options must be provided to create the class, it is an object that holds things like which database to use, database connection.
// options will then be passed to DbContext to configure Entity Framework Core.
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    // Activities interacts with Activities table in the database and the table content follows Activity class defined in the Domain.
    public required DbSet<Activity> Activities { get; set; }
}
