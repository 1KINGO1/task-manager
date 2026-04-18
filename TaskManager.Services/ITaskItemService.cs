using TaskManager.Services.Dto;

namespace TaskManager.Services;

public interface ITaskItemService
{
    Task<TaskDetailDto?> GetTaskDetailAsync(int taskId);
    Task<TaskListItemDto> CreateTaskAsync(TaskFormDto form);
    Task<bool> UpdateTaskAsync(TaskFormDto form);
    Task<bool> DeleteTaskAsync(int taskId);
}
