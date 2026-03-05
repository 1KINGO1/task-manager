using TaskManager.Data;

namespace TaskManager.Services;

public interface IProjectRepository
{
    IReadOnlyList<ProjectData> GetAllProjects();
    IReadOnlyList<TaskItemData> GetTasksByProjectId(int projectId);
}
