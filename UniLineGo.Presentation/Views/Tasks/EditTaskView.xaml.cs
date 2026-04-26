using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UniLineGo.Application.Services;
using UniLineGo.Presentation.ViewModels;

namespace UniLineGo.Presentation.Views;

public partial class EditTaskView : UserControl
{
    private readonly TaskViewModel _vm;
    private readonly MainView _mainView;
    private readonly int _taskId;
    private DateTime? _selectedDate = null;

    public EditTaskView(TaskService taskService, MainView mainView, int taskId)
    {
        InitializeComponent();
        _vm = new TaskViewModel(taskService);
        _mainView = mainView;
        _taskId = taskId;

        _ = LoadTaskAsync(taskService, taskId);
    }

    private async System.Threading.Tasks.Task LoadTaskAsync(TaskService taskService, int taskId)
    {
        var task = await taskService.GetTaskByIdAsync(taskId);
        if (task == null)
        {
            MessageBox.Show("Завдання не знайдено.", "Помилка",
                MessageBoxButton.OK, MessageBoxImage.Error);
            _mainView.ShowTasksList();
            return;
        }

        TitleBox.Text = task.Title;
        DescriptionBox.Text = task.Description ?? string.Empty;

        if (task.Deadline.HasValue)
        {
            var local = task.Deadline.Value.ToLocalTime();
            _selectedDate = local.Date;
            DeadlineCalendar.SelectedDate = local.Date;
            HourBox.Text = local.Hour.ToString("D2");
            MinuteBox.Text = local.Minute.ToString("D2");
            // Template may not be applied yet — defer update
            _ = Dispatcher.InvokeAsync(UpdateDateLabel,
                System.Windows.Threading.DispatcherPriority.Loaded);
        }
    }

    private void DatePickerButton_Click(object sender, RoutedEventArgs e)
    {
        CalendarPopup.IsOpen = !CalendarPopup.IsOpen;
    }

    private void DeadlineCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DeadlineCalendar.SelectedDate.HasValue)
        {
            _selectedDate = DeadlineCalendar.SelectedDate.Value;
            UpdateDateLabel();
            CalendarPopup.IsOpen = false;
        }
    }

    private void UpdateDateLabel()
    {
        if (DatePickerButton.Template.FindName("DateLabel", DatePickerButton) is TextBlock lbl)
        {
            if (_selectedDate.HasValue)
            {
                lbl.Text = _selectedDate.Value.ToString("dd.MM.yyyy");
                lbl.Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#3D3047"));
            }
            else
            {
                lbl.Text = "Оберіть дату";
                lbl.Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#B0A4BA"));
            }
        }
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        ErrorText.Visibility = Visibility.Collapsed;

        var title = TitleBox.Text.Trim();
        if (string.IsNullOrEmpty(title))
        {
            ShowError("Введіть назву завдання.");
            return;
        }

        var description = string.IsNullOrWhiteSpace(DescriptionBox.Text)
            ? null : DescriptionBox.Text.Trim();

        DateTime? deadline = null;
        if (_selectedDate.HasValue)
        {
            int hour = ParseTimePart(HourBox.Text, 0, 23);
            int minute = ParseTimePart(MinuteBox.Text, 0, 59);
            deadline = new DateTime(
                _selectedDate.Value.Year, _selectedDate.Value.Month, _selectedDate.Value.Day,
                hour, minute, 0);
        }

        const int priority = 2;

        var (success, message) = await _vm.UpdateTaskAsync(_taskId, title, description, deadline, priority);

        if (success)
            _mainView.ShowTasksList();
        else
            ShowError(message);
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
        => _mainView.ShowTasksList();

    private void ShowError(string msg)
    {
        ErrorText.Text = msg;
        ErrorText.Visibility = Visibility.Visible;
    }

    private void TimeBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !int.TryParse(e.Text, out _);
    }

    private void HourBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(HourBox.Text, out int h))
            HourBox.Text = Math.Clamp(h, 0, 23).ToString("D2");
        else
            HourBox.Text = "00";
    }

    private void MinuteBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(MinuteBox.Text, out int m))
            MinuteBox.Text = Math.Clamp(m, 0, 59).ToString("D2");
        else
            MinuteBox.Text = "00";
    }

    private static int ParseTimePart(string text, int min, int max)
    {
        if (int.TryParse(text, out int val))
            return Math.Clamp(val, min, max);
        return min;
    }
}