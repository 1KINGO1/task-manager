namespace TaskManager.Services.Dto;

public class ProjectListItemDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public double ProgressPercent { get; init; }
}
