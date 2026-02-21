using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Services;

public class StorageService
{
    public List<ProjectModel> GetAllProjects()
    {
        return FakeStorage.Projects
            .Select(ProjectModel.FromData)
            .ToList();
    }

    public List<TaskItemModel> GetTasksByProjectId(int projectId)
    {
        return FakeStorage.Tasks
            .Where(t => t.ProjectId == projectId)
            .Select(TaskItemModel.FromData)
            .ToList();
    }
}
