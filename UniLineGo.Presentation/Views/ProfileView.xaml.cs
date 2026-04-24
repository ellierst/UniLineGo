using System.Windows;
using System.Windows.Controls;
using UniLineGo.Application.Services;

namespace UniLineGo.Presentation.Views;

public partial class ProfileView : UserControl
{
    private readonly AuthService _authService;
    private readonly ShellWindow _shell;

    public ProfileView(AuthService authService, ShellWindow shell)
    {
        InitializeComponent();
        _authService = authService;
        _shell = shell;

        // Заповни поточні дані
        var user = AuthService.CurrentUser;
        if (user != null)
        {
            UsernameBox.Text = user.Username;
            EmailBox.Text = user.Email;
        }
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var username = UsernameBox.Text.Trim();
            var email = EmailBox.Text.Trim();
            var newPassword = NewPasswordBox.Password;
            var confirm = ConfirmPasswordBox.Password;

            if (string.IsNullOrEmpty(username))
            {
                ShowMessage("Ім'я користувача не може бути порожнім.", isError: true);
                return;
            }

            if (!email.Contains('@') || !email.Contains('.') || string.IsNullOrEmpty(email))
            {
                ShowMessage("Введіть коректний email.", isError: true);
                return;
            }

            if (newPassword != confirm)
            {
                ShowMessage("Паролі не співпадають.", isError: true);
                return;
            }

            var userId = AuthService.CurrentUser!.Id;
            var (success, message) = await _authService.UpdateProfileAsync(
                userId, username, email, string.IsNullOrEmpty(newPassword) ? null : newPassword);

            ShowMessage(message, isError: !success);

            if (success)
            {
                NewPasswordBox.Clear();
                ConfirmPasswordBox.Clear();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка: {ex.Message}\n\n{ex.InnerException?.Message}",
                "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _shell.NavigateTo(new MainView(_authService, _shell));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка: {ex.Message}", "Помилка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "Ви впевнені що хочете вийти?", "Вихід",
            MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            _authService.Logout();
            _shell.NavigateTo(new LoginView(_authService, _shell));
        }
    }

    private void ShowMessage(string message, bool isError)
    {
        MessageText.Text = message;
        MessageText.Foreground = isError
            ? System.Windows.Media.Brushes.Red
            : System.Windows.Media.Brushes.Green;
        MessageText.Visibility = Visibility.Visible;
    }
}