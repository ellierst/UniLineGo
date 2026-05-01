using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UniLineGo.Application.Services;

namespace UniLineGo.Presentation.Views;

public class DeadlineItem
{
    public int    Id           { get; set; }
    public string Title        { get; set; } = string.Empty;
    public string DeadlineText { get; set; } = string.Empty;
    public string UrgencyTag   { get; set; } = string.Empty;
}

public partial class HomePageView : UserControl
{
    private readonly TaskService _taskService;
    private readonly MainView    _mainView;

    public HomePageView(TaskService taskService, MainView mainView)
    {
        InitializeComponent();
        _taskService = taskService;
        _mainView    = mainView;
        _ = LoadAsync();
    }

    private async System.Threading.Tasks.Task LoadAsync()
    {
        var tasks = (await _taskService.GetAllTasksAsync()).ToList();

        TotalCount.Text      = tasks.Count.ToString();
        DoneCount.Text       = tasks.Count(t => t.IsCompleted).ToString();
        PendingCount.Text    = tasks.Count(t => !t.IsCompleted).ToString();
        NoDeadlineCount.Text = tasks.Count(t => !t.Deadline.HasValue).ToString();

        var today = DateTime.Today;
        var upcoming = tasks
            .Where(t => !t.IsCompleted && t.Deadline.HasValue &&
                        t.Deadline.Value.ToLocalTime().Date >= today)
            .OrderBy(t => t.Deadline!.Value)
            .Take(6)
            .Select(t =>
            {
                var local    = t.Deadline!.Value.ToLocalTime();
                var daysLeft = (local.Date - today).Days;
                string urgency = daysLeft switch
                {
                    0 => "Сьогодні",
                    1 => "Завтра",
                    _ => $"Через {daysLeft} дн."
                };
                return new DeadlineItem
                {
                    Id           = t.Id,
                    Title        = t.Title,
                    DeadlineText = local.ToString("dd.MM.yyyy HH:mm"),
                    UrgencyTag   = urgency
                };
            })
            .ToList();

        if (upcoming.Count == 0)
        {
            DeadlineScroll.Visibility = Visibility.Collapsed;
            EmptyState.Visibility     = Visibility.Visible;
        }
        else
        {
            DeadlineList.ItemsSource  = upcoming;
            DeadlineScroll.Visibility = Visibility.Visible;
            EmptyState.Visibility     = Visibility.Collapsed;
        }
    }

    // ── Click on a deadline row ──────────────────────────────────────────────
    private void DeadlineRow_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border border && border.Tag is int id)
            _mainView.ShowTaskDetail(id, "home");
    }

    private void DeadlineRow_MouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is Border b)
            b.Background = new SolidColorBrush(Color.FromRgb(245, 242, 248));
    }

    private void DeadlineRow_MouseLeave(object sender, MouseEventArgs e)
    {
        if (sender is Border b)
            b.Background = Brushes.Transparent;
    }

    private void AddTask_Click(object sender, RoutedEventArgs e)
        => _mainView.ShowAddTask();
}