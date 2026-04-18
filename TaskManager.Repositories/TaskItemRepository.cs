using TaskManager.Repositories.Models;
using TaskManager.Repositories.Storage;

namespace TaskManager.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly IDataStore _dataStore;

    public TaskItemRepository(IDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<IReadOnlyList<TaskItemData>> GetByProjectIdAsync(int projectId) =>
        _dataStore.GetTasksByProjectIdAsync(projectId);

    public Task<TaskItemData?> GetByIdAsync(int id) =>
        _dataStore.GetTaskByIdAsync(id);

    public Task<TaskItemData> AddAsync(TaskItemData task) =>
        _dataStore.AddTaskAsync(task);

    public Task<bool> UpdateAsync(TaskItemData task) =>
        _dataStore.UpdateTaskAsync(task);

    public Task<bool> DeleteAsync(int id) =>
        _dataStore.DeleteTaskAsync(id);
}
