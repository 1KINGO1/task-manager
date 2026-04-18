using TaskManager.Repositories.Models;

namespace TaskManager.Repositories.Storage;

internal sealed class StorageSnapshot
{
    public List<ProjectData> Projects { get; set; } = new();
    public List<TaskItemData> Tasks { get; set; } = new();
}
