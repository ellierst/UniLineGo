namespace UniLineGo.Domain.Entities;

public class TaskItem
{
    public int      Id             { get; set; }
    public int      UserId         { get; set; }
    public string   Title          { get; set; } = string.Empty;
    public string?  Description    { get; set; }
    public DateTime? Deadline      { get; set; }
    public int      Priority       { get; set; }
    public bool     IsCompleted    { get; set; }
    public DateTime CreatedAt      { get; set; }
    public DateTime UpdatedAt      { get; set; }

    /// <summary>
    /// Скільки хвилин до дедлайну надіслати нагадування.
    /// null = нагадування відключено.
    /// </summary>
    public int? ReminderMinutes { get; set; }

    /// <summary>
    /// Чи вже було надіслано нагадування для цього завдання.
    /// Скидається при зміні дедлайну або нагадування.
    /// </summary>
    public bool ReminderSent { get; set; }

    // Навігаційні властивості (потрібні AppDbContext)
    public User User { get; set; } = null!;
    public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
}