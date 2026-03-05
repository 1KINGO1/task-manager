using TaskManager.Data;

namespace TaskManager.Models;

public class TaskItemModel
{
    public int Id { get; }
    public int ProjectId { get; }
    public string Title { get; set; }
    public string Description { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsCompleted { get; set; }

    public bool IsOverdue => !IsCompleted && DueDate < DateTime.Today;

    public string StatusDisplay => IsCompleted ? "Done" : (IsOverdue ? "OVERDUE" : "Pending");

    public TaskItemModel(int id, int projectId, string title, string description,
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

    public static TaskItemModel FromData(TaskItemData data)
    {
        return new TaskItemModel(
            data.Id, data.ProjectId, data.Title, data.Description,
            data.Priority, data.DueDate, data.IsCompleted);
    }

    public string ToShortString()
    {
        string status = IsCompleted ? "Done" : (IsOverdue ? "OVERDUE" : "Pending");
        return $"  [{Id}] {Title} | {Priority} | Due: {DueDate:yyyy-MM-dd} | {status}";
    }

    public string ToDetailedString()
    {
        string status = IsCompleted ? "Completed" : (IsOverdue ? "OVERDUE" : "Pending");
        return $"""
            Task #{Id}: {Title}
              Project ID:  {ProjectId}
              Priority:    {Priority}
              Due date:    {DueDate:yyyy-MM-dd}
              Status:      {status}
              Description: {Description}
            """;
    }
}
