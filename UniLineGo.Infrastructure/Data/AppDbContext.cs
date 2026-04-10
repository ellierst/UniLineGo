using Microsoft.EntityFrameworkCore;
using UniLineGo.Domain.Entities;

public class AppDbContext : DbContext
{
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<ScheduleEntry> ScheduleEntries { get; set; }
    public DbSet<Reminder> Reminders { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── TaskItem ──────────────────────────────
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("tasks");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id).HasColumnName("id");
            entity.Property(t => t.Title)
                  .HasColumnName("title")
                  .IsRequired()
                  .HasMaxLength(255);
            entity.Property(t => t.Description).HasColumnName("description");
            entity.Property(t => t.Deadline).HasColumnName("deadline");
            entity.Property(t => t.IsCompleted).HasColumnName("is_completed");
            entity.Property(t => t.Priority).HasColumnName("priority");
            entity.Property(t => t.CreatedAt).HasColumnName("created_at");
            entity.Property(t => t.UpdatedAt).HasColumnName("updated_at");
        });

        // ── ScheduleEntry ─────────────────────────
        modelBuilder.Entity<ScheduleEntry>(entity =>
        {
            entity.ToTable("schedule_entries");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).HasColumnName("id");
            entity.Property(s => s.Title)
                  .HasColumnName("title")
                  .IsRequired()
                  .HasMaxLength(255);
            entity.Property(s => s.Room).HasColumnName("room");
            entity.Property(s => s.StartTime).HasColumnName("start_time");
            entity.Property(s => s.EndTime).HasColumnName("end_time");
            entity.Property(s => s.DayOfWeek).HasColumnName("day_of_week");
            entity.Property(s => s.CreatedAt).HasColumnName("created_at");
            entity.Property(s => s.UpdatedAt).HasColumnName("updated_at");
        });

        // ── Reminder ──────────────────────────────
        modelBuilder.Entity<Reminder>(entity =>
        {
            entity.ToTable("reminders");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Id).HasColumnName("id");
            entity.Property(r => r.TaskId).HasColumnName("task_id");
            entity.Property(r => r.ScheduleEntryId).HasColumnName("schedule_entry_id");
            entity.Property(r => r.RemindAt).HasColumnName("remind_at");
            entity.Property(r => r.IsSent).HasColumnName("is_sent");
            entity.Property(r => r.CreatedAt).HasColumnName("created_at");

            // Зв'язки
            entity.HasOne(r => r.Task)
                  .WithMany(t => t.Reminders)
                  .HasForeignKey(r => r.TaskId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.ScheduleEntry)
                  .WithMany(s => s.Reminders)
                  .HasForeignKey(r => r.ScheduleEntryId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}