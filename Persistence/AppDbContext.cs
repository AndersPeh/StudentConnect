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
    // It interacts with Activities table in the database and the table columns follow Activity class defined in the Domain.
    public required DbSet<Activity> Activities { get; set; }
}
