namespace TaskManager.Services.Dto;

public class TaskDetailDto
{
    public int Id { get; init; }
    public int ProjectId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
    public string DueDate { get; init; } = string.Empty;
    public string StatusDisplay { get; init; } = string.Empty;
    public bool IsCompleted { get; init; }
    public bool IsOverdue { get; init; }
    public string ProjectName { get; init; } = string.Empty;
}
