using TaskManager.Repositories.Models;

namespace TaskManager.Repositories.Storage;

public interface IDataStore
{
    Task InitializeAsync();

    Task<IReadOnlyList<ProjectData>> GetProjectsAsync();
    Task<ProjectData?> GetProjectByIdAsync(int id);
    Task<ProjectData> AddProjectAsync(ProjectData project);
    Task<bool> UpdateProjectAsync(ProjectData project);
    Task<bool> DeleteProjectAsync(int id);

    Task<IReadOnlyList<TaskItemData>> GetTasksByProjectIdAsync(int projectId);
    Task<TaskItemData?> GetTaskByIdAsync(int id);
    Task<TaskItemData> AddTaskAsync(TaskItemData task);
    Task<bool> UpdateTaskAsync(TaskItemData task);
    Task<bool> DeleteTaskAsync(int id);
}
