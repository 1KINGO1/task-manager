using TaskManager.Repositories;
using TaskManager.Repositories.Models;
using TaskManager.Services.Dto;

namespace TaskManager.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskItemRepository _taskItemRepository;

    public ProjectService(IProjectRepository projectRepository, ITaskItemRepository taskItemRepository)
    {
        _projectRepository = projectRepository;
        _taskItemRepository = taskItemRepository;
    }

    public IReadOnlyList<ProjectListItemDto> GetAllProjects()
    {
        var projects = _projectRepository.GetAllProjects();

        return projects.Select(p =>
        {
            var tasks = _taskItemRepository.GetTasksByProjectId(p.Id);
            int total = tasks.Count;
            int completed = tasks.Count(t => t.IsCompleted);
            double progress = total == 0 ? 0 : (double)completed / total * 100;

            return new ProjectListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                Type = p.Type.ToString(),
                ProgressPercent = progress
            };
        }).ToList();
    }

    public ProjectDetailDto? GetProjectDetail(int projectId)
    {
        var project = _projectRepository.GetProjectById(projectId);
        if (project is null)
            return null;

        var tasks = _taskItemRepository.GetTasksByProjectId(projectId);
        int completed = tasks.Count(t => t.IsCompleted);

        return new ProjectDetailDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Type = project.Type.ToString(),
            TotalTaskCount = tasks.Count,
            CompletedTaskCount = completed,
            ProgressPercent = tasks.Count == 0 ? 0 : (double)completed / tasks.Count * 100,
            Tasks = tasks.Select(MapToTaskListItem).ToList()
        };
    }

    private static TaskListItemDto MapToTaskListItem(TaskItemData task)
    {
        bool isOverdue = !task.IsCompleted && task.DueDate < DateTime.Today;
        string status = task.IsCompleted ? "Done" : (isOverdue ? "OVERDUE" : "Pending");

        return new TaskListItemDto
        {
            Id = task.Id,
            Title = task.Title,
            Priority = task.Priority.ToString(),
            DueDate = task.DueDate.ToString("yyyy-MM-dd"),
            StatusDisplay = status,
            IsCompleted = task.IsCompleted,
            IsOverdue = isOverdue
        };
    }
}
