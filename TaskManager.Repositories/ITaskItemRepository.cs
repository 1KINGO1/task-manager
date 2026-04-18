using TaskManager.Repositories.Models;

namespace TaskManager.Repositories;

public interface ITaskItemRepository
{
    Task<IReadOnlyList<TaskItemData>> GetByProjectIdAsync(int projectId);
    Task<TaskItemData?> GetByIdAsync(int id);
    Task<TaskItemData> AddAsync(TaskItemData task);
    Task<bool> UpdateAsync(TaskItemData task);
    Task<bool> DeleteAsync(int id);
}
