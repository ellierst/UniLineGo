namespace UniLineGo.Application.Services;

using UniLineGo.Domain.Entities;
using UniLineGo.Domain.Interfaces;

public class TaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    private static int CurrentUserId =>
        AuthService.CurrentUser?.Id
        ?? throw new InvalidOperationException("Користувач не авторизований.");

    public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        => await _taskRepository.GetAllByUserAsync(CurrentUserId);

    public async Task<TaskItem?> GetTaskByIdAsync(int id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        // повертаємо тільки якщо завдання належить поточному юзеру
        return task?.UserId == CurrentUserId ? task : null;
    }

    public async Task<(bool Success, string Message, int Id)> AddTaskAsync(
        string title, string? description, DateTime? deadline, int priority)
    {
        if (string.IsNullOrWhiteSpace(title))
            return (false, "Назва завдання не може бути порожньою.", 0);

        var task = new TaskItem
        {
            UserId      = CurrentUserId,
            Title       = title.Trim(),
            Description = description?.Trim(),
            Deadline    = deadline?.ToUniversalTime(),
            Priority    = priority,
            IsCompleted = false,
            CreatedAt   = DateTime.UtcNow,
            UpdatedAt   = DateTime.UtcNow
        };

        var id = await _taskRepository.AddAsync(task);
        return (true, "Завдання додано!", id);
    }

    public async Task<(bool Success, string Message)> UpdateTaskAsync(
        int id, string title, string? description, DateTime? deadline, int priority)
    {
        var task = await GetTaskByIdAsync(id);
        if (task == null)
            return (false, "Завдання не знайдено.");

        if (string.IsNullOrWhiteSpace(title))
            return (false, "Назва завдання не може бути порожньою.");

        task.Title       = title.Trim();
        task.Description = description?.Trim();
        task.Deadline    = deadline?.ToUniversalTime();
        task.Priority    = priority;
        task.UpdatedAt   = DateTime.UtcNow;

        await _taskRepository.UpdateAsync(task);
        return (true, "Завдання оновлено!");
    }

    public async Task<(bool Success, string Message)> DeleteTaskAsync(int id)
    {
        var task = await GetTaskByIdAsync(id);
        if (task == null)
            return (false, "Завдання не знайдено.");

        await _taskRepository.DeleteAsync(id);
        return (true, "Завдання видалено.");
    }

    public async Task<(bool Success, string Message)> ToggleCompletionAsync(int id)
    {
        var task = await GetTaskByIdAsync(id);
        if (task == null)
            return (false, "Завдання не знайдено.");

        task.IsCompleted = !task.IsCompleted;
        task.UpdatedAt   = DateTime.UtcNow;

        await _taskRepository.UpdateAsync(task);
        return (true, task.IsCompleted ? "Виконано!" : "Позначено як невиконане.");
    }

    public static string GetTaskStatus(TaskItem task)
    {
        if (task.IsCompleted)
            return "Виконано";
        if (task.Deadline.HasValue && task.Deadline.Value < DateTime.UtcNow)
            return "Прострочено";
        return "На виконання";
    }
}