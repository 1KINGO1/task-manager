using Microsoft.Extensions.DependencyInjection;
using TaskManager.Repositories.Storage;

namespace TaskManager.Repositories;

public static class RepositoryServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, string storageFilePath)
    {
        services.AddSingleton<IDataStore>(_ => new JsonFileDataStore(storageFilePath));
        services.AddSingleton<IProjectRepository, ProjectRepository>();
        services.AddSingleton<ITaskItemRepository, TaskItemRepository>();
        return services;
    }
}
