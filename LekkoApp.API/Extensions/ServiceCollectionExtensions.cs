using LekkoApp.Core.Services;
using LekkoApp.Repositories;

namespace LekkoApp.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
