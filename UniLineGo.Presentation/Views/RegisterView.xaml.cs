using System.Windows;
using System.Windows.Controls;
using UniLineGo.Application.Services;

namespace UniLineGo.Presentation.Views;

public partial class RegisterView : UserControl
{
    private readonly AuthService _authService;
    private readonly ShellWindow _shell;

    public RegisterView(AuthService authService, ShellWindow shell)
    {
        InitializeComponent();
        _authService = authService;
        _shell = shell;
    }

    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        var username = UsernameBox.Text.Trim();
        var email = EmailBox.Text.Trim();
        var password = PasswordBox.Password;
        var confirm = ConfirmPasswordBox.Password;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) ||
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirm))
        { ShowError("Заповніть всі поля."); return; }

        if (password != confirm)
        { ShowError("Паролі не співпадають."); return; }

        if (password.Length < 6)
        { ShowError("Пароль має бути не менше 6 символів."); return; }

        var (success, message) = await _authService.RegisterAsync(username, email, password);

        if (success)
        {
            MessageBox.Show("Реєстрація успішна! Тепер увійдіть.", "Успіх",
                MessageBoxButton.OK, MessageBoxImage.Information);
            _shell.NavigateTo(new LoginView(_authService, _shell));
        }
        else
            ShowError(message);
    }

    private void GoToLogin_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        => _shell.NavigateTo(new LoginView(_authService, _shell));

    private void ShowError(string message)
    {
        ErrorText.Text = message;
        ErrorText.Visibility = Visibility.Visible;
    }
}