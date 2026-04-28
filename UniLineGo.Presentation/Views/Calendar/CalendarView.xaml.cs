using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UniLineGo.Application.Services;
using UniLineGo.Domain.Entities;

namespace UniLineGo.Presentation.Views;

public partial class CalendarView : UserControl
{
    private readonly TaskService _taskService;
    private readonly MainView _mainView;
    private DateTime _currentMonth;
    private IEnumerable<TaskItem> _allTasks = [];

    private static readonly string[] MonthNames =
    [
        "Січень", "Лютий", "Березень", "Квітень", "Травень", "Червень",
        "Липень", "Серпень", "Вересень", "Жовтень", "Листопад", "Грудень"
    ];

    public CalendarView(TaskService taskService, MainView mainView)
    {
        InitializeComponent();
        _taskService = taskService;
        _mainView = mainView;
        _currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        _ = LoadAndRenderAsync();
    }

    private async System.Threading.Tasks.Task LoadAndRenderAsync()
    {
        _allTasks = await _taskService.GetAllTasksAsync();
        RenderCalendar();
    }

    private void RenderCalendar()
    {
        MonthLabel.Text = $"{MonthNames[_currentMonth.Month - 1]} {_currentMonth.Year} р.";

        CalendarGrid.Children.Clear();
        CalendarGrid.RowDefinitions.Clear();
        CalendarGrid.ColumnDefinitions.Clear();

        for (int c = 0; c < 7; c++)
            CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        int firstDow = ((int)_currentMonth.DayOfWeek + 6) % 7; 
        int daysInMonth = DateTime.DaysInMonth(_currentMonth.Year, _currentMonth.Month);
        int totalCells = firstDow + daysInMonth;
        int rows = (int)Math.Ceiling(totalCells / 7.0);

        for (int r = 0; r < rows; r++)
            CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        for (int i = 0; i < rows * 7; i++)
        {
            int dayNum = i - firstDow + 1;
            bool inMonth = dayNum >= 1 && dayNum <= daysInMonth;

            var cell = BuildCell(dayNum, inMonth);
            Grid.SetRow(cell, i / 7);
            Grid.SetColumn(cell, i % 7);
            CalendarGrid.Children.Add(cell);
        }
    }

    private Border BuildCell(int dayNum, bool inMonth)
    {
        bool isToday = inMonth &&
            dayNum == DateTime.Today.Day &&
            _currentMonth.Month == DateTime.Today.Month &&
            _currentMonth.Year == DateTime.Today.Year;

        var cell = new Border
        {
            BorderBrush = new SolidColorBrush(Color.FromRgb(237, 232, 237)),
            BorderThickness = new Thickness(0.5),
            Background = Brushes.Transparent,
            Cursor = inMonth ? Cursors.Hand : Cursors.Arrow
        };

        var stack = new StackPanel { Margin = new Thickness(6, 4, 6, 4) };

        var dayLabel = new Border
        {
            Width = 26, Height = 26,
            CornerRadius = new CornerRadius(13),
            HorizontalAlignment = HorizontalAlignment.Left,
            Background = isToday
                ? new SolidColorBrush(Color.FromRgb(107, 91, 123))
                : Brushes.Transparent
        };
        dayLabel.Child = new TextBlock
        {
            Text = inMonth ? dayNum.ToString() : string.Empty,
            FontSize = 13,
            FontWeight = isToday ? FontWeights.Bold : FontWeights.Normal,
            Foreground = isToday
                ? Brushes.White
                : inMonth
                    ? new SolidColorBrush(Color.FromRgb(61, 48, 71))
                    : new SolidColorBrush(Color.FromRgb(196, 188, 204)),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        stack.Children.Add(dayLabel);

        if (inMonth)
        {
            var date = new DateTime(_currentMonth.Year, _currentMonth.Month, dayNum);
            var dayTasks = _allTasks
                .Where(t => t.Deadline.HasValue &&
                            t.Deadline.Value.ToLocalTime().Date == date)
                .Take(3)
                .ToList();

            foreach (var t in dayTasks)
            {
                var chip = BuildTaskChip(t);
                stack.Children.Add(chip);
            }

            cell.MouseEnter += (_, _) =>
            {
                if (!isToday)
                    cell.Background = new SolidColorBrush(Color.FromRgb(245, 242, 248));
            };
            cell.MouseLeave += (_, _) =>
            {
                if (!isToday)
                    cell.Background = Brushes.Transparent;
            };
        }

        cell.Child = stack;
        return cell;
    }

    private static Border BuildTaskChip(TaskItem task)
    {
        var status = TaskService.GetTaskStatus(task);
        Color bg; Color fg;
        if (status == "Виконано")
        {
            bg = Color.FromRgb(232, 245, 233);
            fg = Color.FromRgb(56, 142, 60);
        }
        else if (status == "Прострочено")
        {
            bg = Color.FromRgb(253, 236, 234);
            fg = Color.FromRgb(198, 40, 40);
        }
        else
        {
            bg = Color.FromRgb(255, 243, 224);
            fg = Color.FromRgb(230, 81, 0);
        }

        string timeStr = task.Deadline.HasValue
            ? task.Deadline.Value.ToLocalTime().ToString("HH:mm")
            : string.Empty;

        var chip = new Border
        {
            Background = new SolidColorBrush(bg),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(5, 2, 5, 2),
            Margin = new Thickness(0, 2, 0, 0),
            Cursor = Cursors.Hand
        };

        var inner = new StackPanel();

        inner.Children.Add(new TextBlock
        {
            Text = task.Title,
            FontSize = 11,
            FontWeight = FontWeights.SemiBold,
            Foreground = new SolidColorBrush(fg),
            TextTrimming = TextTrimming.CharacterEllipsis,
            TextWrapping = TextWrapping.NoWrap
        });

        if (!string.IsNullOrEmpty(timeStr) && timeStr != "00:00")
        {
            inner.Children.Add(new TextBlock
            {
                Text = timeStr,
                FontSize = 10,
                Foreground = new SolidColorBrush(fg) { Opacity = 0.75 }
            });
        }

        chip.Child = inner;
        return chip;
    }

    private void PrevMonth_Click(object sender, RoutedEventArgs e)
    {
        _currentMonth = _currentMonth.AddMonths(-1);
        RenderCalendar();
    }

    private void NextMonth_Click(object sender, RoutedEventArgs e)
    {
        _currentMonth = _currentMonth.AddMonths(1);
        RenderCalendar();
    }

    private void AddEventButton_Click(object sender, RoutedEventArgs e)
    {
        _mainView.ShowAddTask();
    }
}