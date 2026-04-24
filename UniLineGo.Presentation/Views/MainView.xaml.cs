using System.Windows.Controls;

namespace UniLineGo.Presentation.Views;

public partial class MainView : UserControl
{
    private readonly UniLineGo.Application.Services.AuthService _authService;
    private readonly ShellWindow _shell;

    public MainView(UniLineGo.Application.Services.AuthService authService, ShellWindow shell)
    {
        InitializeComponent();
        _authService = authService;
        _shell = shell;
    }

    private void ProfileButton_Click(object sender, System.Windows.RoutedEventArgs e)
        => _shell.NavigateTo(new ProfileView(_authService, _shell));
}