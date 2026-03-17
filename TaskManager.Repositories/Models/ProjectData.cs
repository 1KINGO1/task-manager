namespace TaskManager.Repositories.Models;

public class ProjectData
{
    public int Id { get; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ProjectType Type { get; set; }

    public ProjectData(int id, string name, string description, ProjectType type)
    {
        Id = id;
        Name = name;
        Description = description;
        Type = type;
    }
}
