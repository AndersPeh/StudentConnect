namespace Domain;

public class Comment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public required string Body { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign Keys (One User to Many Comments and One Activity to Many Comments).
    public required string UserId { get; set; }

    public required string ActivityId { get; set; }

    // Navigation properties, establish Many to One relationships here.
    public User User { get; set; } = null!;

    public Activity Activity { get; set; } = null!;

}
