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

    public async Task<IReadOnlyList<ProjectListItemDto>> GetAllProjectsAsync()
    {
        var projects = await _projectRepository.GetAllAsync();
        var result = new List<ProjectListItemDto>(projects.Count);

        foreach (var project in projects)
        {
            var tasks = await _taskItemRepository.GetByProjectIdAsync(project.Id);
            result.Add(new ProjectListItemDto
            {
                Id = project.Id,
                Name = project.Name,
                Type = project.Type.ToString(),
                ProgressPercent = CalculateProgress(tasks),
                TotalTaskCount = tasks.Count
            });
        }

        return result;
    }

    public async Task<ProjectDetailDto?> GetProjectDetailAsync(int projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project is null)
            return null;

        var tasks = await _taskItemRepository.GetByProjectIdAsync(projectId);
        int completed = tasks.Count(t => t.IsCompleted);

        return new ProjectDetailDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Type = project.Type.ToString(),
            TotalTaskCount = tasks.Count,
            CompletedTaskCount = completed,
            ProgressPercent = CalculateProgress(tasks),
            Tasks = tasks.Select(MapToTaskListItem).ToList()
        };
    }

    public async Task<ProjectListItemDto> CreateProjectAsync(ProjectFormDto form)
    {
        ArgumentNullException.ThrowIfNull(form);
        ValidateProject(form);

        var data = new ProjectData(
            id: 0,
            name: form.Name.Trim(),
            description: form.Description?.Trim() ?? string.Empty,
            type: ParseProjectType(form.Type));

        var created = await _projectRepository.AddAsync(data);
        return new ProjectListItemDto
        {
            Id = created.Id,
            Name = created.Name,
            Type = created.Type.ToString(),
            ProgressPercent = 0,
            TotalTaskCount = 0
        };
    }

    public async Task<bool> UpdateProjectAsync(ProjectFormDto form)
    {
        ArgumentNullException.ThrowIfNull(form);
        if (form.Id is null)
            throw new ArgumentException("Project Id is required for update.", nameof(form));
        ValidateProject(form);

        var data = new ProjectData(
            id: form.Id.Value,
            name: form.Name.Trim(),
            description: form.Description?.Trim() ?? string.Empty,
            type: ParseProjectType(form.Type));

        return await _projectRepository.UpdateAsync(data);
    }

    public Task<bool> DeleteProjectAsync(int projectId) =>
        _projectRepository.DeleteAsync(projectId);

    private static double CalculateProgress(IReadOnlyList<TaskItemData> tasks)
    {
        if (tasks.Count == 0)
            return 0;
        int completed = tasks.Count(t => t.IsCompleted);
        return (double)completed / tasks.Count * 100;
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
            DueDateRaw = task.DueDate,
            StatusDisplay = status,
            IsCompleted = task.IsCompleted,
            IsOverdue = isOverdue
        };
    }

    private static void ValidateProject(ProjectFormDto form)
    {
        if (string.IsNullOrWhiteSpace(form.Name))
            throw new ArgumentException("Назва проєкту не може бути порожньою.", nameof(form));
    }

    private static ProjectType ParseProjectType(string value)
    {
        if (Enum.TryParse<ProjectType>(value, ignoreCase: true, out var type))
            return type;
        throw new ArgumentException($"Невідомий тип проєкту: '{value}'.");
    }
}
