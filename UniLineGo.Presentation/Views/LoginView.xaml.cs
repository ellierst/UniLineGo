using System.Windows;
using System.Windows.Controls;
using UniLineGo.Application.Services;

namespace UniLineGo.Presentation.Views;

public partial class LoginView : UserControl
{
    private readonly AuthService _authService;
    private readonly ShellWindow _shell;

    public LoginView(AuthService authService, ShellWindow shell)
    {
        InitializeComponent();
        _authService = authService;
        _shell = shell;
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
            _shell.NavigateTo(new MainView(_authService, _shell));
        else
            ShowError(message);
    }

    private void GoToRegister_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        => _shell.NavigateTo(new RegisterView(_authService, _shell));

    private void ShowError(string message)
    {
        ErrorText.Text = message;
        ErrorText.Visibility = Visibility.Visible;
    }
}