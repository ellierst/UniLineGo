using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UniLineGo.Application.Services;
using UniLineGo.Presentation.ViewModels;

namespace UniLineGo.Presentation.Views;

public partial class AddTaskView : UserControl
{
    private readonly TaskViewModel _vm;
    private readonly MainView _mainView;
    private DateTime? _selectedDate = null;

    public AddTaskView(TaskService taskService, MainView mainView)
    {
        InitializeComponent();
        _vm = new TaskViewModel(taskService);
        _mainView = mainView;
    }

    // ── Date picker ────────────────────────────────────────────────────────

    private void DatePickerButton_Click(object sender, RoutedEventArgs e)
        => CalendarPopup.IsOpen = !CalendarPopup.IsOpen;

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
                lbl.Text       = _selectedDate.Value.ToString("dd.MM.yyyy");
                lbl.Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#3D3047"));
            }
            else
            {
                lbl.Text       = "Оберіть дату";
                lbl.Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#B0A4BA"));
            }
        }
    }

    // ── Save ───────────────────────────────────────────────────────────────

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
            int hour   = ParseTimePart(HourBox.Text,   0, 23);
            int minute = ParseTimePart(MinuteBox.Text, 0, 59);
            deadline = new DateTime(
                _selectedDate.Value.Year, _selectedDate.Value.Month, _selectedDate.Value.Day,
                hour, minute, 0);
        }

        const int priority     = 2;
        int? reminderMinutes   = GetReminderMinutes();

        // Якщо дедлайн не встановлено, нагадування не має сенсу
        if (reminderMinutes.HasValue && deadline == null)
        {
            ShowError("Для нагадування потрібно вказати дедлайн.");
            return;
        }

        var (success, message) = await _vm.AddTaskAsync(
            title, description, deadline, priority, reminderMinutes);

        if (success)
            _mainView.ShowTasksList();
        else
            ShowError(message);
    }

    // ── Reminder combo helper ──────────────────────────────────────────────

    /// <summary>
    /// Перетворює вибраний пункт нагадування на кількість хвилин.
    /// null — нагадування вимкнено.
    /// </summary>
    private int? GetReminderMinutes()
    {
        if (ReminderCombo.SelectedItem is not ComboBoxItem item) return null;

        return (item.Content as string) switch
        {
            "За 15 хвилин"   => 15,
            "За 30 хвилин"   => 30,
            "За годину"      => 60,
            "За день"        => 1440,
            "За 2 дні"       => 2880,
            "За тиждень"     => 10080,
            _                => null  // "Без нагадування"
        };
    }

    // ── Navigation ─────────────────────────────────────────────────────────

    private void BackButton_Click(object sender, RoutedEventArgs e)
        => _mainView.ShowTasksList();

    // ── Helpers ────────────────────────────────────────────────────────────

    private void ShowError(string msg)
    {
        ErrorText.Text       = msg;
        ErrorText.Visibility = Visibility.Visible;
    }

    private void TimeBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        => e.Handled = !int.TryParse(e.Text, out _);

    private void HourBox_LostFocus(object sender, RoutedEventArgs e)
    {
        HourBox.Text = int.TryParse(HourBox.Text, out int h)
            ? Math.Clamp(h, 0, 23).ToString("D2")
            : "00";
    }

    private void MinuteBox_LostFocus(object sender, RoutedEventArgs e)
    {
        MinuteBox.Text = int.TryParse(MinuteBox.Text, out int m)
            ? Math.Clamp(m, 0, 59).ToString("D2")
            : "00";
    }

    private static int ParseTimePart(string text, int min, int max)
        => int.TryParse(text, out int val) ? Math.Clamp(val, min, max) : min;
}