namespace TaskManager.Services.Dto;

public class TaskListItemDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
    public string DueDate { get; init; } = string.Empty;
    public DateTime DueDateRaw { get; init; }
    public string StatusDisplay { get; init; } = string.Empty;
    public bool IsCompleted { get; init; }
    public bool IsOverdue { get; init; }
}
