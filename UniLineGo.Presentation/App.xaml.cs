using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UniLineGo.Application.Services;
using UniLineGo.Infrastructure.Data;
using UniLineGo.Presentation.Views;

namespace UniLineGo.Presentation;

public partial class App : System.Windows.Application
{
    private ServiceProvider _serviceProvider = null!;
    private ReminderService? _reminderService;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found.");

            var services = new ServiceCollection();
            services.AddInfrastructure(connectionString);
            services.AddSingleton<AuthService>();
            services.AddSingleton<TaskService>();
            services.AddSingleton<ReminderService>();

            _serviceProvider = services.BuildServiceProvider();

            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            }

            // Wire up the reminder notification callback
            _reminderService = _serviceProvider.GetRequiredService<ReminderService>();
            _reminderService.SetNotificationCallback((title, body, emoji) =>
            {
                // Must invoke on UI thread
                Dispatcher.InvokeAsync(() =>
                    ReminderNotificationWindow.Show(title, body, emoji));
            });

            _reminderService.Start();

            var shell       = new ShellWindow();
            var authService = _serviceProvider.GetRequiredService<AuthService>();
            shell.NavigateTo(new LoginView(authService, shell, _serviceProvider));
            shell.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Помилка запуску:\n\n{ex.Message}\n\n{ex.InnerException?.Message}",
                "UniLineGo — помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown();
        }
    }
}