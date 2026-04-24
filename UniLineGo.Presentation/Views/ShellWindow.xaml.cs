using System.Windows;
using System.Windows.Controls;

namespace UniLineGo.Presentation.Views;

public partial class ShellWindow : Window
{
    public ShellWindow()
    {
        InitializeComponent();
    }

    public void NavigateTo(UserControl view)
    {
        MainContent.Content = view;
    }
}