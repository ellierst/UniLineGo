namespace UniLineGo.Application.Services;

using UniLineGo.Domain.Interfaces;

/// <summary>
/// Фоновий сервіс, що щохвилини перевіряє завдання
/// і викликає колбек для відображення сповіщення.
/// </summary>
public class ReminderService : IDisposable
{
    private readonly ITaskRepository _taskRepository;
    private readonly TaskService     _taskService;

    private Timer?                         _timer;
    private Action<string, string, string>? _showNotification;

    public ReminderService(ITaskRepository taskRepository, TaskService taskService)
    {
        _taskRepository = taskRepository;
        _taskService    = taskService;
    }

    /// <summary>
    /// Реєструємо колбек для показу UI-сповіщення.
    /// Параметри: title, body, emoji.
    /// </summary>
    public void SetNotificationCallback(Action<string, string, string> callback)
        => _showNotification = callback;

    /// <summary>
    /// Запускаємо таймер. Перша перевірка — через 30 секунд,
    /// потім кожну хвилину.
    /// </summary>
    public void Start()
    {
        _timer = new Timer(
            async _ => await CheckAsync(),
            null,
            TimeSpan.FromSeconds(0),
            TimeSpan.FromSeconds(15));
    }

    private async Task CheckAsync()
    {
        try
        {
            var userId = AuthService.CurrentUser?.Id;
            if (userId == null) return;

            var tasks = await _taskRepository.GetAllByUserAsync(userId.Value);
            var now   = DateTime.UtcNow;

            foreach (var task in tasks)
            {
                if (task.IsCompleted)           continue;
                if (task.ReminderSent)          continue;
                if (!task.Deadline.HasValue)    continue;
                if (!task.ReminderMinutes.HasValue) continue;

                var triggerAt = task.Deadline.Value.AddMinutes(-task.ReminderMinutes.Value);

                // Спрацьовуємо, якщо зараз у вікні [triggerAt, triggerAt + 2 хв)
                if (now >= triggerAt && now < triggerAt.AddMinutes(2))
                {
                    await _taskService.MarkReminderSentAsync(task.Id);

                    var label = ReminderLabel(task.ReminderMinutes.Value);
                    var localDeadline = task.Deadline.Value.ToLocalTime()
                                             .ToString("dd.MM.yyyy HH:mm");

                    _showNotification?.Invoke(
                        task.Title,
                        $"Дедлайн {label} — {localDeadline}",
                        "⏰");
                }
            }
        }
        catch
        {
            // Ігноруємо помилки у фоновому потоці, щоб таймер не зупинявся
        }
    }

    private static string ReminderLabel(int minutes) => minutes switch
    {
        15    => "за 15 хвилин",
        30    => "за 30 хвилин",
        60    => "за годину",
        1440  => "за день",
        2880  => "за 2 дні",
        10080 => "за тиждень",
        _     => $"за {minutes} хвилин"
    };

    public void Dispose() => _timer?.Dispose();
}