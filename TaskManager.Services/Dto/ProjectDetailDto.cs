namespace TaskManager.Services.Dto;

public class ProjectDetailDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public double ProgressPercent { get; init; }
    public int CompletedTaskCount { get; init; }
    public int TotalTaskCount { get; init; }
    public List<TaskListItemDto> Tasks { get; init; } = [];
}
