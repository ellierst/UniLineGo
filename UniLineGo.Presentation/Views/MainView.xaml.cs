using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using UniLineGo.Application.Services;

namespace UniLineGo.Presentation.Views;

public partial class MainView : UserControl
{
    private readonly AuthService      _authService;
    private readonly TaskService      _taskService;
    private readonly ShellWindow      _shell;
    private readonly IServiceProvider _serviceProvider;

    private Button? _activeNav;

    public MainView(AuthService authService, TaskService taskService,
                    ShellWindow shell, IServiceProvider serviceProvider)
    {
        try { InitializeComponent(); }
        catch (Exception ex)
        {
            MessageBox.Show($"InitializeComponent failed:\n{ex.Message}\n{ex.InnerException?.Message}",
                "MainView XAML Error", MessageBoxButton.OK, MessageBoxImage.Error);
            throw;
        }

        _authService     = authService;
        _taskService     = taskService;
        _shell           = shell;
        _serviceProvider = serviceProvider;

        try
        {
            SetActiveNav(NavTasks);
            ContentArea.Content = new TasksView(_taskService, this);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Помилка завантаження TasksView:\n{ex.Message}\n\n{ex.InnerException?.Message}\n\n{ex.StackTrace}",
                "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  Навігація
    // ═══════════════════════════════════════════════════════════════════════

    private void NavHome_Click(object sender, RoutedEventArgs e)
    {
        SetActiveNav(NavHome);
        ContentArea.Content = new HomePageView();
    }

    private void NavTasks_Click(object sender, RoutedEventArgs e)
    {
        SetActiveNav(NavTasks);
        ContentArea.Content = new TasksView(_taskService, this);
    }

    private void NavCalendar_Click(object sender, RoutedEventArgs e)
    {
        SetActiveNav(NavCalendar);
        ContentArea.Content = new CalendarView(_taskService, this);
    }

    private void NavSettings_Click(object sender, RoutedEventArgs e)
    {
        SetActiveNav(NavSettings);
        ContentArea.Content = new ProfileView(_authService, _shell, this, _serviceProvider);
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "Ви впевнені що хочете вийти?", "Вихід",
            MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        _authService.Logout();
        _shell.NavigateTo(new LoginView(_authService, _shell, _serviceProvider));
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  Допоміжні
    // ═══════════════════════════════════════════════════════════════════════

    private void SetActiveNav(Button btn)
    {
        if (_activeNav != null)
            _activeNav.Background = System.Windows.Media.Brushes.Transparent;

        _activeNav = btn;
        btn.Background = new System.Windows.Media.SolidColorBrush(
            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#7D6B8E"));
    }

    private static UserControl CreatePlaceholder(string title)
    {
        var uc = new UserControl();
        uc.Content = new TextBlock
        {
            Text       = title,
            FontSize   = 28,
            FontWeight = FontWeights.SemiBold,
            Foreground = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#C3B3D4")),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center
        };
        return uc;
    }

    public void ShowAddTask()  => ContentArea.Content = new AddTaskView(_taskService, this);
    public void ShowEditTask(int taskId) => ContentArea.Content = new EditTaskView(_taskService, this, taskId);
    public void ShowTasksList()
    {
        SetActiveNav(NavTasks);
        ContentArea.Content = new TasksView(_taskService, this);
    }
}