using System;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

// DbContext allows us to interact with the database.
// It connects domain models and the database.
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public required DbSet<Activity> Activities { get; set; }
}
