using TaskManager.Services.Dto;

namespace TaskManager.Services;

public interface ITaskItemService
{
    TaskDetailDto? GetTaskDetail(int taskId);
}
