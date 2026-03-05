using Microsoft.Extensions.DependencyInjection;

namespace TaskManager.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTaskManagerServices(this IServiceCollection services)
    {
        services.AddSingleton<IProjectRepository, FakeProjectRepository>();
        services.AddSingleton<IStorageService, StorageService>();
        return services;
    }
}
