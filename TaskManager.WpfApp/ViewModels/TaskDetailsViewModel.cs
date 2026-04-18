using System.Windows.Input;
using TaskManager.Services;
using TaskManager.WpfApp.Services;

namespace TaskManager.WpfApp.ViewModels;

public class TaskDetailsViewModel : ViewModelBase, INavigationAware
{
    private readonly ITaskItemService _taskItemService;
    private readonly INavigationService _navigationService;
    private readonly IBusyService _busyService;
    private readonly IDialogService _dialogService;

    private int _taskId;
    private int _projectId;
    private string _taskTitle = string.Empty;
    private string _projectName = string.Empty;
    private string _priority = string.Empty;
    private string _dueDate = string.Empty;
    private string _statusDisplay = string.Empty;
    private string _description = string.Empty;
    private bool _isCompleted;
    private bool _isOverdue;
    private bool _taskExists;

    public string TaskTitle { get => _taskTitle; set => SetProperty(ref _taskTitle, value); }
    public string ProjectName { get => _projectName; set => SetProperty(ref _projectName, value); }
    public string Priority { get => _priority; set => SetProperty(ref _priority, value); }
    public string DueDate { get => _dueDate; set => SetProperty(ref _dueDate, value); }
    public string StatusDisplay { get => _statusDisplay; set => SetProperty(ref _statusDisplay, value); }
    public string Description { get => _description; set => SetProperty(ref _description, value); }
    public bool IsCompleted { get => _isCompleted; set => SetProperty(ref _isCompleted, value); }
    public bool IsOverdue { get => _isOverdue; set => SetProperty(ref _isOverdue, value); }
    public bool TaskExists { get => _taskExists; set => SetProperty(ref _taskExists, value); }

    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }

    public TaskDetailsViewModel(
        ITaskItemService taskItemService,
        INavigationService navigationService,
        IBusyService busyService,
        IDialogService dialogService)
    {
        _taskItemService = taskItemService;
        _navigationService = navigationService;
        _busyService = busyService;
        _dialogService = dialogService;

        EditCommand = new RelayCommand(
            _ => _navigationService.NavigateTo<TaskEditViewModel>(new TaskEditArgs(_taskId, _projectId)),
            _ => TaskExists);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => TaskExists);
    }

    public void OnNavigatedTo(object? parameter)
    {
        if (parameter is int taskId)
        {
            _taskId = taskId;
            _ = LoadAsync();
        }
    }

    private async Task LoadAsync()
    {
        using (_busyService.BeginWork("Завантаження завдання..."))
        {
            try
            {
                var task = await _taskItemService.GetTaskDetailAsync(_taskId);
                if (task is null)
                {
                    TaskExists = false;
                    TaskTitle = "Завдання не знайдено";
                    return;
                }

                TaskExists = true;
                _projectId = task.ProjectId;
                TaskTitle = task.Title;
                ProjectName = task.ProjectName;
                Priority = task.Priority;
                DueDate = task.DueDate;
                StatusDisplay = task.StatusDisplay;
                Description = task.Description;
                IsCompleted = task.IsCompleted;
                IsOverdue = task.IsOverdue;
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Не вдалося завантажити завдання:\n{ex.Message}");
            }
        }
    }

    private async Task DeleteAsync()
    {
        if (!TaskExists)
            return;
        if (!_dialogService.Confirm($"Видалити завдання \"{TaskTitle}\"?", "Видалення завдання"))
            return;

        using (_busyService.BeginWork("Видалення завдання..."))
        {
            try
            {
                var ok = await _taskItemService.DeleteTaskAsync(_taskId);
                if (!ok)
                {
                    _dialogService.ShowError("Завдання не знайдено.");
                    return;
                }
                _navigationService.GoBack();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Не вдалося видалити завдання:\n{ex.Message}");
            }
        }
    }
}
