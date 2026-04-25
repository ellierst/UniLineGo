using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using UniLineGo.Application.Services;

namespace UniLineGo.Presentation.Views;

public partial class LoginView : UserControl
{
    private readonly AuthService _authService;
    private readonly ShellWindow _shell;
    private readonly IServiceProvider _serviceProvider;

    public LoginView(AuthService authService, ShellWindow shell, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _authService = authService;
        _shell = shell;
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var email = EmailBox.Text.Trim();
            var password = PasswordBox.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ShowError("Заповніть всі поля.");
                return;
            }

            var (success, message, _) = await _authService.LoginAsync(email, password);

            if (success)
            {
                var taskService = _serviceProvider.GetRequiredService<TaskService>();
                _shell.NavigateTo(new MainView(_authService, taskService, _shell, _serviceProvider));
            }
            else
                ShowError(message);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Помилка після входу:\n\n{ex.Message}\n\n{ex.InnerException?.Message}\n\n{ex.StackTrace}",
                "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void GoToRegister_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        => _shell.NavigateTo(new RegisterView(_authService, _shell, _serviceProvider));

    private void ShowError(string message)
    {
        ErrorText.Text = message;
        ErrorText.Visibility = Visibility.Visible;
    }
}