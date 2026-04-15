using System.Windows;
using UniLineGo.Application.Services;

namespace UniLineGo.Presentation.Views;

public partial class LoginWindow : Window
{
    private readonly AuthService _authService;

    public LoginWindow(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        var email = EmailBox.Text.Trim();
        var password = PasswordBox.Password;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowError("Заповніть всі поля.");
            return;
        }

        var (success, message, user) = await _authService.LoginAsync(email, password);

        if (success)
        {
            new MainWindow().Show();
            Close();
        }
        else
        {
            ShowError(message);
        }
    }

    private void GoToRegister_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        new RegisterWindow(_authService).Show();
        Close();
    }

    private void ShowError(string message)
    {
        ErrorText.Text = message;
        ErrorText.Visibility = Visibility.Visible;
    }
}