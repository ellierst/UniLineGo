using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UniLineGo.Application.Services;

namespace UniLineGo.Presentation.Views;

public partial class TaskDetailView : UserControl
{
    private readonly TaskService _taskService;
    private readonly MainView _mainView;
    private readonly int _taskId;
    // Remember where to go back: "home" or "tasks" or "calendar"
    private readonly string _returnTo;

    public TaskDetailView(TaskService taskService, MainView mainView, int taskId, string returnTo = "tasks")
    {
        InitializeComponent();
        _taskService = taskService;
        _mainView    = mainView;
        _taskId      = taskId;
        _returnTo    = returnTo;
        _ = LoadAsync();
    }

    private async System.Threading.Tasks.Task LoadAsync()
    {
        var task = await _taskService.GetTaskByIdAsync(_taskId);
        if (task == null)
        {
            MessageBox.Show("Завдання не знайдено.", "Помилка",
                MessageBoxButton.OK, MessageBoxImage.Error);
            GoBack();
            return;
        }

        TitleText.Text = task.Title;
        DescriptionText.Text = string.IsNullOrWhiteSpace(task.Description)
            ? "Опис відсутній"
            : task.Description;

        DeadlineText.Text = task.Deadline.HasValue
            ? task.Deadline.Value.ToLocalTime().ToString("dd.MM.yyyy HH:mm")
            : "Без дедлайну";

        CompletedCheck.IsChecked = task.IsCompleted;
        CompletedText.Text = task.IsCompleted ? "Виконано" : "Не виконано";

        var status = TaskService.GetTaskStatus(task);
        StatusText.Text = status;

        (Color bg, Color fg) = status switch
        {
            "Виконано"    => (Color.FromRgb(232, 245, 233), Color.FromRgb(56, 142, 60)),
            "Прострочено" => (Color.FromRgb(253, 236, 234), Color.FromRgb(198, 40, 40)),
            _             => (Color.FromRgb(255, 243, 224), Color.FromRgb(230, 81, 0))
        };
        StatusBadge.Background = new SolidColorBrush(bg);
        StatusText.Foreground  = new SolidColorBrush(fg);
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
        => _mainView.ShowEditTask(_taskId);

    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "Видалити це завдання?", "Підтвердження",
            MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        var taskService = _taskService;
        var vm = new ViewModels.TaskViewModel(taskService);
        var (success, message) = await vm.DeleteTaskAsync(_taskId);

        if (!success)
            MessageBox.Show(message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        else
            GoBack();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e) => GoBack();

    private void GoBack()
    {
        switch (_returnTo)
        {
            case "home":     _mainView.ShowHome();      break;
            case "calendar": _mainView.ShowCalendar();  break;
            default:         _mainView.ShowTasksList(); break;
        }
    }
}