using Microsoft.Extensions.DependencyInjection;
using TaskManager.Repositories;

namespace TaskManager.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTaskManagerServices(this IServiceCollection services)
    {
        services.AddRepositories();
        services.AddSingleton<IProjectService, ProjectService>();
        services.AddSingleton<ITaskItemService, TaskItemService>();
        return services;
    }
}
