using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using UniLineGo.Application.Services;
using UniLineGo.Presentation.ViewModels;

namespace UniLineGo.Presentation.Views;

public partial class TasksView : UserControl
{
    private readonly TaskViewModel _vm;
    private readonly MainView _mainView;
    private bool _isUpdating = false;

    public TasksView(TaskService taskService, MainView mainView)
    {
        _vm = new TaskViewModel(taskService);
        _mainView = mainView;

        InitializeComponent();

        Loaded += (_, _) => MoveIndicator(TabAll);

        _ = LoadAsync();
    }

    private async System.Threading.Tasks.Task LoadAsync()
    {
        _isUpdating = true;
        await _vm.LoadTasksAsync();
        RefreshList();
        _isUpdating = false;
    }

    private void RefreshList()
    {
        _isUpdating = true;
        TasksList.ItemsSource = null;
        TasksList.ItemsSource = _vm.Tasks;
        _isUpdating = false;

        var count = _vm.Tasks.Count;
        FooterCount.Text = $"Показано {count} із {count} завдань";
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_vm == null) return;
        SearchPlaceholder.Visibility = string.IsNullOrEmpty(SearchBox.Text)
            ? Visibility.Visible : Visibility.Collapsed;
        _vm.SearchText = SearchBox.Text;
    }

    private void TabAll_Click(object sender, RoutedEventArgs e)     => ApplyTab("Всі",          TabAll);
    private void TabPending_Click(object sender, RoutedEventArgs e) => ApplyTab("На виконання", TabPending);
    private void TabDone_Click(object sender, RoutedEventArgs e)    => ApplyTab("Виконано",     TabDone);
    private void TabOverdue_Click(object sender, RoutedEventArgs e) => ApplyTab("Прострочено",  TabOverdue);

    private void ApplyTab(string filter, Button btn)
    {
        if (_vm == null) return;
        _vm.FilterStatus = filter;

        foreach (var b in new[] { TabAll, TabPending, TabDone, TabOverdue })
        {
            b.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9A8A9E"));
            b.FontWeight = FontWeights.Normal;
        }
        btn.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5A4E6D"));
        btn.FontWeight = FontWeights.SemiBold;

        MoveIndicator(btn);
    }

    private void AddTask_Click(object sender, RoutedEventArgs e)
        => _mainView.ShowAddTask();

    // ── Open task detail on row click ────────────────────────────────────────
    private void TaskRow_Click(object sender, MouseButtonEventArgs e)
    {
        // Ignore clicks that originated from a Button or CheckBox (edit/delete/toggle)
        if (e.OriginalSource is DependencyObject src)
        {
            var parent = src;
            while (parent != null)
            {
                if (parent is ButtonBase) return;   // Button or CheckBox — skip
                if (parent is Border b && b.Tag is int) break; // reached our row
                parent = VisualTreeHelper.GetParent(parent);
            }
        }

        if (sender is Border border && border.Tag is int id)
            _mainView.ShowTaskDetail(id, "tasks");
    }

    private void EditTask_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is int id)
            _mainView.ShowEditTask(id);
    }

    private async void DeleteTask_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn || btn.Tag is not int id) return;

        var result = MessageBox.Show(
            "Видалити це завдання?", "Підтвердження",
            MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        var (success, message) = await _vm.DeleteTaskAsync(id);
        if (!success)
            MessageBox.Show(message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        else
            RefreshList();
    }

    private async void TaskCheck_Changed(object sender, RoutedEventArgs e)
    {
        if (_isUpdating) return;
        if (sender is not CheckBox cb || cb.Tag is not int id) return;

        _isUpdating = true;
        await _vm.ToggleCompletionAsync(id);
        RefreshList();
        _isUpdating = false;
    }

    private void MoveIndicator(Button btn)
    {
        btn.Dispatcher.InvokeAsync(() =>
        {
            btn.UpdateLayout();
            var pos = btn.TransformToVisual(TabsContainer).Transform(new System.Windows.Point(0, 0));
            TabIndicator.Width = btn.ActualWidth;
            TabIndicator.Margin = new Thickness(pos.X, 0, 0, 0);
        }, System.Windows.Threading.DispatcherPriority.Loaded);
    }

    private void TaskRow_MouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is Border b)
            b.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0EBF4"));
    }

    private void TaskRow_MouseLeave(object sender, MouseEventArgs e)
    {
        if (sender is Border b)
            b.Background = Brushes.Transparent;
    }
}