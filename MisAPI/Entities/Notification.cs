namespace MisAPI.Entities;

public class Notification
{
    public Notification(string email, DateTime scheduledDate)
    {
        Email = email;
        ScheduledDate = scheduledDate;
    }

    public Guid Id { get; set; }
    public string Email { get; set; }
    public DateTime ScheduledDate { get; set; }
    public bool IsSent { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
