using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UniLineGo.Domain.Interfaces;
using UniLineGo.Infrastructure.Repositories;
using UniLineGo.Infrastructure.Data;  

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Реєстрація репозиторіїв
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<IReminderRepository, ReminderRepository>();

        return services;
    }
}