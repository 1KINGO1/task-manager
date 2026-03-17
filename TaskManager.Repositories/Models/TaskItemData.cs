namespace TaskManager.Repositories.Models;

public class TaskItemData
{
    public int Id { get; }
    public int ProjectId { get; }
    public string Title { get; set; }
    public string Description { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsCompleted { get; set; }

    public TaskItemData(int id, int projectId, string title, string description,
        TaskPriority priority, DateTime dueDate, bool isCompleted)
    {
        Id = id;
        ProjectId = projectId;
        Title = title;
        Description = description;
        Priority = priority;
        DueDate = dueDate;
        IsCompleted = isCompleted;
    }

    public TaskItemData(int id, int projectId, string title, string description,
        TaskPriority priority, DateTime dueDate)
        : this(id, projectId, title, description, priority, dueDate, false)
    {
    }
}
