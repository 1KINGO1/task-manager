namespace TaskManager.Repositories.Models;

public class ProjectData
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ProjectType Type { get; set; }

    public ProjectData()
    {
    }

    public ProjectData(int id, string name, string description, ProjectType type)
    {
        Id = id;
        Name = name;
        Description = description;
        Type = type;
    }
}
