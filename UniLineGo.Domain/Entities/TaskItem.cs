namespace UniLineGo.Domain.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
    public bool IsCompleted { get; set; } = false;
    public int Priority { get; set; } = 1;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Навігаційна властивість
    public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
}
