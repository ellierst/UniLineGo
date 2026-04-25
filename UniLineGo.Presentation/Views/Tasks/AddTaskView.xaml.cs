using System.Windows;
using System.Windows.Controls;
using UniLineGo.Application.Services;
using UniLineGo.Presentation.ViewModels;

namespace UniLineGo.Presentation.Views;

public partial class AddTaskView : UserControl
{
    private readonly TaskViewModel _vm;
    private readonly MainView _mainView;

    public AddTaskView(TaskService taskService, MainView mainView)
    {
        InitializeComponent();
        _vm = new TaskViewModel(taskService);
        _mainView = mainView;
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
        if (DeadlinePicker.SelectedDate.HasValue)
            deadline = DeadlinePicker.SelectedDate.Value;

        int priority = 2;
        if (PriorityCombo.SelectedItem is ComboBoxItem pi && pi.Tag is string pt)
            priority = int.TryParse(pt, out var p) ? p : 2;

        var (success, message) = await _vm.AddTaskAsync(title, description, deadline, priority);

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
}