using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UniLineGo.Application.Services;
using UniLineGo.Presentation.ViewModels;
using UniLineGo.Presentation.Views;

public partial class App : System.Windows.Application
{
    private ServiceProvider _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();

        // Рядок підключення
        const string connectionString =
            "Host=localhost;Port=5432;Database=unilinego;Username=postgres;Password=yourpassword";

        // Підключення шарів
        services.AddInfrastructure(connectionString);

        // Реєстрація сервісів Application
        services.AddScoped<TaskService>();
        services.AddScoped<ScheduleService>();
        services.AddScoped<ReminderService>();

        // Реєстрація ViewModels
        services.AddTransient<TaskViewModel>();
        services.AddTransient<ScheduleViewModel>();

        // Реєстрація головного вікна
        services.AddTransient<MainWindow>();

        _serviceProvider = services.BuildServiceProvider();

        // Автоматичне застосування міграцій при запуску
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }
}