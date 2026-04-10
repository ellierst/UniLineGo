namespace UniLineGo.Domain.Entities;

public class ScheduleEntry
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Room { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int DayOfWeek { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
}
