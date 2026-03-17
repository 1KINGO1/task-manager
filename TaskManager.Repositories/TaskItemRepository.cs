using TaskManager.Repositories.Models;
using TaskManager.Repositories.Storage;

namespace TaskManager.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly InMemoryStorage _storage;

    public TaskItemRepository(InMemoryStorage storage)
    {
        _storage = storage;
    }

    public IReadOnlyList<TaskItemData> GetTasksByProjectId(int projectId)
    {
        return _storage.Tasks.Where(t => t.ProjectId == projectId).ToList();
    }

    public TaskItemData? GetTaskById(int id)
    {
        return _storage.Tasks.FirstOrDefault(t => t.Id == id);
    }
}
