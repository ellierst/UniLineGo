using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Media;

namespace UniLineGo.Presentation.Views;

public partial class ReminderNotificationWindow : Window
{
    private readonly DispatcherTimer _autoClose;

    public ReminderNotificationWindow(string title, string body, string emoji = "⏰")
    {
        InitializeComponent();

        TitleText.Text = title;
        BodyText.Text  = body;
        EmojiText.Text = emoji;

        // Position: bottom-right corner, above taskbar
        PositionWindow();

        // Auto-dismiss after 6 seconds
        _autoClose = new DispatcherTimer { Interval = TimeSpan.FromSeconds(6) };
        _autoClose.Tick += (_, _) =>
        {
            _autoClose.Stop();
            BeginSlideOut();
        };

        Loaded += (_, _) =>
        {
            (FindResource("SlideIn") as Storyboard)?.Begin(this);
            _autoClose.Start();
            SystemSounds.Exclamation.Play(); 
        };
    }

    private void PositionWindow()
    {
        var screen = SystemParameters.WorkArea;
        Left = screen.Right  - Width  - 16;
        Top  = screen.Bottom - Height - 16;

        // Recalculate after layout pass (SizeToContent)
        ContentRendered += (_, _) =>
        {
            Top  = screen.Bottom - ActualHeight - 16;
        };
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        _autoClose.Stop();
        BeginSlideOut();
    }

    private void BeginSlideOut()
    {
        (FindResource("SlideOut") as Storyboard)?.Begin(this);
    }

    private void SlideOut_Completed(object sender, EventArgs e)
    {
        Close();
    }

    /// <summary>
    /// Фабричний метод — завжди викликати з UI-потоку.
    /// </summary>
    public static void Show(string title, string body, string emoji = "⏰")
    {
        var win = new ReminderNotificationWindow(title, body, emoji);
        win.Show();
    }
}