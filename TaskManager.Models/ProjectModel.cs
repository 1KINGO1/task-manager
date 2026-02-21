using TaskManager.Data;

namespace TaskManager.Models;

public class ProjectModel
{
    public int Id { get; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ProjectType Type { get; set; }

    public List<TaskItemModel> Tasks { get; set; } = new();

    public double ProgressPercent =>
        Tasks.Count == 0 ? 0 : (double)Tasks.Count(t => t.IsCompleted) / Tasks.Count * 100;

    public ProjectModel(int id, string name, string description, ProjectType type)
    {
        Id = id;
        Name = name;
        Description = description;
        Type = type;
    }

    public static ProjectModel FromData(ProjectData data)
    {
        return new ProjectModel(data.Id, data.Name, data.Description, data.Type);
    }

    public string ToShortString()
    {
        return $"[{Id}] {Name} | {Type} | Progress: {ProgressPercent:F0}%";
    }

    public string ToDetailedString()
    {
        return $"""
            Project #{Id}: {Name}
              Type:        {Type}
              Description: {Description}
              Progress:    {ProgressPercent:F0}% ({Tasks.Count(t => t.IsCompleted)}/{Tasks.Count} tasks completed)
            """;
    }
}
