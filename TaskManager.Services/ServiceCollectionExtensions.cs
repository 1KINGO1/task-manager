using Microsoft.Extensions.DependencyInjection;
using TaskManager.Repositories;

namespace TaskManager.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTaskManagerServices(this IServiceCollection services, string storageFilePath)
    {
        services.AddRepositories(storageFilePath);
        services.AddSingleton<IProjectService, ProjectService>();
        services.AddSingleton<ITaskItemService, TaskItemService>();
        services.AddSingleton<IStorageInitializer, StorageInitializer>();
        return services;
    }
}
