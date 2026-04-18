using TaskManager.Repositories;
using TaskManager.Repositories.Models;
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

    public async Task<TaskDetailDto?> GetTaskDetailAsync(int taskId)
    {
        var task = await _taskItemRepository.GetByIdAsync(taskId);
        if (task is null)
            return null;

        var project = await _projectRepository.GetByIdAsync(task.ProjectId);
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
            DueDateRaw = task.DueDate,
            StatusDisplay = status,
            IsCompleted = task.IsCompleted,
            IsOverdue = isOverdue,
            ProjectName = project?.Name ?? "Unknown"
        };
    }

    public async Task<TaskListItemDto> CreateTaskAsync(TaskFormDto form)
    {
        ArgumentNullException.ThrowIfNull(form);
        ValidateTask(form);

        var data = new TaskItemData(
            id: 0,
            projectId: form.ProjectId,
            title: form.Title.Trim(),
            description: form.Description?.Trim() ?? string.Empty,
            priority: ParsePriority(form.Priority),
            dueDate: form.DueDate.Date,
            isCompleted: form.IsCompleted);

        var created = await _taskItemRepository.AddAsync(data);
        return MapToListItem(created);
    }

    public async Task<bool> UpdateTaskAsync(TaskFormDto form)
    {
        ArgumentNullException.ThrowIfNull(form);
        if (form.Id is null)
            throw new ArgumentException("Task Id is required for update.", nameof(form));
        ValidateTask(form);

        var data = new TaskItemData(
            id: form.Id.Value,
            projectId: form.ProjectId,
            title: form.Title.Trim(),
            description: form.Description?.Trim() ?? string.Empty,
            priority: ParsePriority(form.Priority),
            dueDate: form.DueDate.Date,
            isCompleted: form.IsCompleted);

        return await _taskItemRepository.UpdateAsync(data);
    }

    public Task<bool> DeleteTaskAsync(int taskId) =>
        _taskItemRepository.DeleteAsync(taskId);

    private static TaskListItemDto MapToListItem(TaskItemData task)
    {
        bool isOverdue = !task.IsCompleted && task.DueDate < DateTime.Today;
        string status = task.IsCompleted ? "Done" : (isOverdue ? "OVERDUE" : "Pending");

        return new TaskListItemDto
        {
            Id = task.Id,
            Title = task.Title,
            Priority = task.Priority.ToString(),
            DueDate = task.DueDate.ToString("yyyy-MM-dd"),
            DueDateRaw = task.DueDate,
            StatusDisplay = status,
            IsCompleted = task.IsCompleted,
            IsOverdue = isOverdue
        };
    }

    private static void ValidateTask(TaskFormDto form)
    {
        if (string.IsNullOrWhiteSpace(form.Title))
            throw new ArgumentException("Назва завдання не може бути порожньою.", nameof(form));
    }

    private static TaskPriority ParsePriority(string value)
    {
        if (Enum.TryParse<TaskPriority>(value, ignoreCase: true, out var p))
            return p;
        throw new ArgumentException($"Невідомий пріоритет: '{value}'.");
    }
}
