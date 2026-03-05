using TaskManager.Models;

namespace TaskManager.Services;

public interface IStorageService
{
    IReadOnlyList<ProjectModel> GetAllProjects();
    IReadOnlyList<TaskItemModel> GetTasksByProjectId(int projectId);
}
