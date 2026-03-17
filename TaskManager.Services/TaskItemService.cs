using TaskManager.Repositories;
using TaskManager.Services.Dto;

namespace TaskManager.Services;

public class TaskItemService : ITaskItemService
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IProjectRepository _projectRepository;

    public TaskItemService(ITaskItemRepository taskItemRepository, IProjectRepository projectRepository)
    {
        _taskItemRepository = taskItemRepository;
        _projectRepository = projectRepository;
    }

    public TaskDetailDto? GetTaskDetail(int taskId)
    {
        var task = _taskItemRepository.GetTaskById(taskId);
        if (task is null)
            return null;

        var project = _projectRepository.GetProjectById(task.ProjectId);
        bool isOverdue = !task.IsCompleted && task.DueDate < DateTime.Today;
        string status = task.IsCompleted ? "Done" : (isOverdue ? "OVERDUE" : "Pending");

        return new TaskDetailDto
        {
            Id = task.Id,
            ProjectId = task.ProjectId,
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority.ToString(),
            DueDate = task.DueDate.ToString("yyyy-MM-dd"),
            StatusDisplay = status,
            IsCompleted = task.IsCompleted,
            IsOverdue = isOverdue,
            ProjectName = project?.Name ?? "Unknown"
        };
    }
}
