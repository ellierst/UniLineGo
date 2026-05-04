namespace UniLineGo.Presentation.ViewModels;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UniLineGo.Application.Services;
using UniLineGo.Domain.Entities;

public class TaskViewModel : INotifyPropertyChanged
{
    private readonly TaskService _taskService;

    public ObservableCollection<TaskItemDisplay> Tasks { get; } = new();

    private string _filterStatus = "Всі";
    public string FilterStatus
    {
        get => _filterStatus;
        set { _filterStatus = value; OnPropertyChanged(); _ = LoadTasksAsync(); }
    }

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(); _ = LoadTasksAsync(); }
    }

    public TaskViewModel(TaskService taskService)
    {
        _taskService = taskService;
    }

    public async System.Threading.Tasks.Task LoadTasksAsync()
    {
        var all = await _taskService.GetAllTasksAsync();
        var filtered = all.Where(t =>
        {
            var status = TaskService.GetTaskStatus(t);
            bool matchFilter = FilterStatus switch
            {
                "На виконання" => status == "На виконання",
                "Виконано"     => status == "Виконано",
                "Прострочено"  => status == "Прострочено",
                _              => true
            };
            bool matchSearch = string.IsNullOrEmpty(SearchText) ||
                t.Title.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase);
            return matchFilter && matchSearch;
        });

        Tasks.Clear();
        foreach (var t in filtered.OrderByDescending(t => t.CreatedAt))
            Tasks.Add(new TaskItemDisplay(t));
    }

    public async System.Threading.Tasks.Task<(bool, string)> AddTaskAsync(
        string title, string? description, DateTime? deadline, int priority,
        int? reminderMinutes = null)
    {
        var result = await _taskService.AddTaskAsync(
            title, description, deadline, priority, reminderMinutes);
        if (result.Success) await LoadTasksAsync();
        return (result.Success, result.Message);
    }

    public async System.Threading.Tasks.Task<(bool, string)> UpdateTaskAsync(
        int id, string title, string? description, DateTime? deadline, int priority,
        int? reminderMinutes = null)
    {
        var result = await _taskService.UpdateTaskAsync(
            id, title, description, deadline, priority, reminderMinutes);
        if (result.Success) await LoadTasksAsync();
        return result;
    }

    public async System.Threading.Tasks.Task<(bool, string)> DeleteTaskAsync(int id)
    {
        var result = await _taskService.DeleteTaskAsync(id);
        if (result.Success) await LoadTasksAsync();
        return result;
    }

    public async System.Threading.Tasks.Task<(bool, string)> ToggleCompletionAsync(int id)
    {
        var result = await _taskService.ToggleCompletionAsync(id);
        if (result.Success) await LoadTasksAsync();
        return result;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class TaskItemDisplay
{
    public int     Id          { get; }
    public string  Title       { get; }
    public string? Description { get; }
    public DateTime? Deadline  { get; }
    public bool    IsCompleted { get; }
    public int     Priority    { get; }
    public int?    ReminderMinutes { get; }

    public string Status => TaskService.GetTaskStatus(new TaskItem
    {
        IsCompleted = IsCompleted,
        Deadline    = Deadline
    });

    public string DeadlineText => Deadline.HasValue
        ? Deadline.Value.ToLocalTime().ToString("dd.MM.yyyy HH:mm")
        : "—";

    public string ReminderText => ReminderMinutes switch
    {
        15    => "За 15 хвилин",
        30    => "За 30 хвилин",
        60    => "За годину",
        1440  => "За день",
        2880  => "За 2 дні",
        10080 => "За тиждень",
        null  => "Без нагадування",
        _     => $"За {ReminderMinutes} хв"
    };

    public TaskItemDisplay(TaskItem task)
    {
        Id              = task.Id;
        Title           = task.Title;
        Description     = task.Description;
        Deadline        = task.Deadline;
        IsCompleted     = task.IsCompleted;
        Priority        = task.Priority;
        ReminderMinutes = task.ReminderMinutes;
    }
}