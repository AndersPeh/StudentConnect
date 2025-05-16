using System;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

// DbContext connects domain models and the database.
// (DbContextOptions options) is the constructor for AppDbContext.
// options must be provided to create the class, it is configured by Program.cs in API layer.
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    // It interacts with Activities table in the database and the table columns follow Activity class defined in the Domain.
    public required DbSet<Activity> Activities { get; set; }
}
