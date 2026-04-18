namespace TaskManager.Repositories.Models;

public class TaskItemData
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsCompleted { get; set; }

    public TaskItemData()
    {
    }

    public TaskItemData(int id, int projectId, string title, string description,
        TaskPriority priority, DateTime dueDate, bool isCompleted = false)
    {
        Id = id;
        ProjectId = projectId;
        Title = title;
        Description = description;
        Priority = priority;
        DueDate = dueDate;
        IsCompleted = isCompleted;
    }
}
