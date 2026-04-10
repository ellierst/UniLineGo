using UniLineGo.Domain.Entities;

public class Reminder
{
    public int Id { get; set; }
    public int? TaskId { get; set; }
    public int? ScheduleEntryId { get; set; }
    public DateTime RemindAt { get; set; }
    public bool IsSent { get; set; } = false;
    public DateTime CreatedAt { get; set; }

    // Навігаційні властивості
    public TaskItem? Task { get; set; }
    public ScheduleEntry? ScheduleEntry { get; set; }
}