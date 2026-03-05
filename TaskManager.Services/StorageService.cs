using TaskManager.Models;

namespace TaskManager.Services;

public class StorageService : IStorageService
{
    private readonly IProjectRepository _repository;

    public StorageService(IProjectRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public IReadOnlyList<ProjectModel> GetAllProjects()
    {
        return _repository.GetAllProjects()
            .Select(ProjectModel.FromData)
            .ToList();
    }

    public IReadOnlyList<TaskItemModel> GetTasksByProjectId(int projectId)
    {
        return _repository.GetTasksByProjectId(projectId)
            .Select(TaskItemModel.FromData)
            .ToList();
    }
}
