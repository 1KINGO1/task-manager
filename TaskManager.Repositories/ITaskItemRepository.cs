using TaskManager.Repositories.Models;

namespace TaskManager.Repositories;

public interface ITaskItemRepository
{
    IReadOnlyList<TaskItemData> GetTasksByProjectId(int projectId);
    TaskItemData? GetTaskById(int id);
}
