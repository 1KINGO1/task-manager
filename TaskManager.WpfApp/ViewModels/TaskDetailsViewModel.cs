using TaskManager.Services;
using TaskManager.WpfApp.Services;

namespace TaskManager.WpfApp.ViewModels;

public class TaskDetailsViewModel : ViewModelBase, INavigationAware
{
    private readonly ITaskItemService _taskItemService;

    private string _taskTitle = string.Empty;
    private string _projectName = string.Empty;
    private string _priority = string.Empty;
    private string _dueDate = string.Empty;
    private string _statusDisplay = string.Empty;
    private string _description = string.Empty;
    private bool _isCompleted;
    private bool _isOverdue;

    public string TaskTitle
    {
        get => _taskTitle;
        set => SetProperty(ref _taskTitle, value);
    }

    public string ProjectName
    {
        get => _projectName;
        set => SetProperty(ref _projectName, value);
    }

    public string Priority
    {
        get => _priority;
        set => SetProperty(ref _priority, value);
    }

    public string DueDate
    {
        get => _dueDate;
        set => SetProperty(ref _dueDate, value);
    }

    public string StatusDisplay
    {
        get => _statusDisplay;
        set => SetProperty(ref _statusDisplay, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public bool IsCompleted
    {
        get => _isCompleted;
        set => SetProperty(ref _isCompleted, value);
    }

    public bool IsOverdue
    {
        get => _isOverdue;
        set => SetProperty(ref _isOverdue, value);
    }

    public TaskDetailsViewModel(ITaskItemService taskItemService)
    {
        _taskItemService = taskItemService;
    }

    public void OnNavigatedTo(object? parameter)
    {
        if (parameter is int taskId)
            LoadTaskDetails(taskId);
    }

    private void LoadTaskDetails(int taskId)
    {
        var task = _taskItemService.GetTaskDetail(taskId);
        if (task is null)
        {
            TaskTitle = "Завдання не знайдено";
            return;
        }

        TaskTitle = task.Title;
        ProjectName = task.ProjectName;
        Priority = task.Priority;
        DueDate = task.DueDate;
        StatusDisplay = task.StatusDisplay;
        Description = task.Description;
        IsCompleted = task.IsCompleted;
        IsOverdue = task.IsOverdue;
    }
}

