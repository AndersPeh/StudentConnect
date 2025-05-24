using System;
using Domain;

namespace Persistence;

public class DbInitializer
{
    // Task means void for async method.
    // static as it doesn't rely on data from DbInitializer.
    // SeedData uses method dependency injection. It needs AppDbContext to run.
    // In API -> Program.cs, it passes context to this method. It is a static method with dependency. 
    public static async Task SeedData(AppDbContext context)
    {
        // Only seed data if there is no activity
        if (context.Activities.Any()) return;

        var activities = new List<Activity>
        {
            new() {
                Title = "SecTalks Gold Coast",
                Date = DateTime.Now.AddMonths(-2),
                Description = "Building a Resilient Data Security Program: Practical Lessons and the Path to Maturity",
                Category = "SecTalks",
                City = "Gold Coast",
                Venue = "Cohort Innovation Space, 16 Nexus Way, Southport QLD 4215",
                Latitude = -27.962450,
                Longitude = 153.386110,
            },
            new() {
                Title = "She Connects",
                Date = DateTime.Now.AddMonths(-1),
                Description = "Everyone passionate about supporting women in tech is welcome to join!",
                Category = "ALL_THINGS_BLOCKCHAIN",
                City = "Brisbane",
                Venue = "The Precinct, Level 2/315 Brunswick St, Fortitude Valley QLD 4006",
                Latitude = -27.645809,
                Longitude = 153.170837
            },
            new() {
                Title = "AI & Society | Run Club",
                Date = DateTime.Now.AddMonths(1),
                Description = "Run, jog, or walk — at your pace",
                Category = "AI_Society",
                City = "Brisbane",
                Venue = "Felons Brewing Co, 5 Boundary St, Brisbane City QLD 4000",
                Latitude = -27.462231,
                Longitude = 153.035141
            },
            new() {
                Title = "PayPal Developer Meetup: Brisbane",
                Date = DateTime.Now.AddMonths(2),
                Description = "Scaling Omnichannel Experiences in the AI Era",
                Category = "paypal",
                City = "Brisbane",
                Venue = "The Precinct, Level 2/315 Brunswick St, Fortitude Valley QLD 4006",
                Latitude = -27.645809,
                Longitude = 153.170837
            },
            new()
            {
                Title = "Bitcoin Brisbane Meetup",
                Date = DateTime.Now.AddMonths(3),
                Description = "This event is Bitcoin-only",
                Category = "drinks",
                City = "Brisbane",
                Venue = "Brisbane One Apartments, 1 Cordelia St, South Brisbane QLD 4101",
                Latitude = -27.473640,
                Longitude = 153.013794
            },
            new()
            {
                Title = "Responsible AI",
                Date = DateTime.Now.AddMonths(4),
                Description = "From bias and transparency to accountability and regulation, this session unpacks the ethical tensions that come with deploying AI in real-world settings.",
                Category = "Responsible_AI",
                City = "Brisbane",
                Venue = "The Precinct, Level 2/315 Brunswick St, Fortitude Valley QLD 4006",
                Latitude = -27.645809,
                Longitude = 153.170837
            },
            new()
            {
                Title = "GDG Brisbane",
                Date = DateTime.Now.AddMonths(5),
                Description = "All brought to you by the local tech community, right here in Brisbane.",
                Category = "google",
                City = "Brisbane",
                Venue = "QUT S Block, Brisbane City QLD 4000",
                Latitude = -27.488043,
                Longitude = 153.018769
            },
            new()
            {
                Title = "Europe trip",
                Date = DateTime.Now.AddMonths(6),
                Description = "Enjoy a trip with university students",
                Category = "culture",
                City = "Paris",
                Venue = "Eiffel Tower, Av. Gustave Eiffel, 75007 Paris, France",
                Latitude = 48.8582599,
                Longitude = 2.2945006
            },
            new()
            {
                Title = "Crafted Beer & Cider Festival",
                Date = DateTime.Now.AddMonths(7),
                Description = "Party with university students",
                Category = "music",
                City = "Gold Coast",
                Venue = "32 Hooker Blvd, Broadbeach Waters QLD 4218",
                Latitude = -28.0359059,
                Longitude = 153.4124276
            },
            new()
            {
                Title = "Lunch at Q1",
                Date = DateTime.Now.AddMonths(8),
                Description = "Anyone interested in joining me?",
                Category = "food",
                City = "Gold Coast",
                Venue = "Q1, 9 Hamilton Ave, Surfers Paradise QLD 4217",
                Latitude = -28.0063488,
                Longitude = 153.4296317
            }
        };

        // uses Entity Framework to prepare Activity objects to be added to Activities table.
        context.Activities.AddRange(activities);

        // add Activity ojects to the table.
        await context.SaveChangesAsync();

    }
}
