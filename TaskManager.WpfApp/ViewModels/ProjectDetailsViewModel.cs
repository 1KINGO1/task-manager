using System.Collections.ObjectModel;
using System.Windows.Input;
using TaskManager.Services;
using TaskManager.Services.Dto;
using TaskManager.WpfApp.Services;

namespace TaskManager.WpfApp.ViewModels;

public class ProjectDetailsViewModel : ViewModelBase, INavigationAware
{
    private readonly IProjectService _projectService;
    private readonly INavigationService _navigationService;
    private readonly IBusyService _busyService;
    private readonly IDialogService _dialogService;

    private int _projectId;
    private string _projectName = string.Empty;
    private string _projectType = string.Empty;
    private string _projectDescription = string.Empty;
    private string _progressText = string.Empty;
    private bool _hasTasks;
    private bool _projectExists;

    private readonly List<TaskListItemDto> _allTasks = new();
    private string _taskSearchText = string.Empty;
    private string _selectedTaskSortOption = TaskSortOptions.DueDateAsc;
    private string _selectedTaskStatusFilter = StatusFilters.AllLabel;

    public string ProjectName { get => _projectName; set => SetProperty(ref _projectName, value); }
    public string ProjectType { get => _projectType; set => SetProperty(ref _projectType, value); }
    public string ProjectDescription { get => _projectDescription; set => SetProperty(ref _projectDescription, value); }
    public string ProgressText { get => _progressText; set => SetProperty(ref _progressText, value); }
    public bool HasTasks { get => _hasTasks; set => SetProperty(ref _hasTasks, value); }
    public bool ProjectExists { get => _projectExists; set => SetProperty(ref _projectExists, value); }

    public ObservableCollection<TaskListItemDto> Tasks { get; } = [];

    public IReadOnlyList<string> TaskSortOptionsList { get; } = TaskSortOptions.All;
    public IReadOnlyList<string> TaskStatusFilters { get; } = StatusFilters.All;


    public string TaskSearchText
    {
        get => _taskSearchText;
        set { if (SetProperty(ref _taskSearchText, value)) ApplyTaskFilterAndSort(); }
    }

    public string SelectedTaskSortOption
    {
        get => _selectedTaskSortOption;
        set { if (SetProperty(ref _selectedTaskSortOption, value)) ApplyTaskFilterAndSort(); }
    }

    public string SelectedTaskStatusFilter
    {
        get => _selectedTaskStatusFilter;
        set { if (SetProperty(ref _selectedTaskStatusFilter, value)) ApplyTaskFilterAndSort(); }
    }

    public ICommand EditProjectCommand { get; }
    public ICommand DeleteProjectCommand { get; }
    public ICommand AddTaskCommand { get; }
    public ICommand OpenTaskCommand { get; }
    public ICommand EditTaskCommand { get; }
    public ICommand DeleteTaskCommand { get; }
    public ICommand ToggleTaskCompletedCommand { get; }

    public ProjectDetailsViewModel(
        IProjectService projectService,
        INavigationService navigationService,
        IBusyService busyService,
        IDialogService dialogService,
        ITaskItemService taskItemService)
    {
        _projectService = projectService;
        _navigationService = navigationService;
        _busyService = busyService;
        _dialogService = dialogService;
        _taskItemService = taskItemService;

        EditProjectCommand = new RelayCommand(_ => _navigationService.NavigateTo<ProjectEditViewModel>(_projectId),
            _ => ProjectExists);
        DeleteProjectCommand = new AsyncRelayCommand(DeleteProjectAsync, () => ProjectExists);
        AddTaskCommand = new RelayCommand(_ => _navigationService.NavigateTo<TaskEditViewModel>(new TaskEditArgs(null, _projectId)),
            _ => ProjectExists);

        OpenTaskCommand = new RelayCommand(OnOpenTask);
        EditTaskCommand = new RelayCommand(OnEditTask);
        DeleteTaskCommand = new AsyncRelayCommand(DeleteTaskAsync);
        ToggleTaskCompletedCommand = new AsyncRelayCommand(ToggleTaskCompletedAsync);
    }

    private readonly ITaskItemService _taskItemService;

    public void OnNavigatedTo(object? parameter)
    {
        if (parameter is int projectId)
        {
            _projectId = projectId;
            _ = LoadProjectDetailsAsync();
        }
    }

    private async Task LoadProjectDetailsAsync()
    {
        using (_busyService.BeginWork("Завантаження проєкту..."))
        {
            try
            {
                var project = await _projectService.GetProjectDetailAsync(_projectId);
                if (project is null)
                {
                    ProjectName = "Проєкт не знайдено";
                    ProjectExists = false;
                    _allTasks.Clear();
                    Tasks.Clear();
                    HasTasks = false;
                    return;
                }

                ProjectExists = true;
                ProjectName = project.Name;
                ProjectType = $"Тип: {project.Type}";
                ProjectDescription = project.Description;
                ProgressText =
                    $"Прогрес: {project.ProgressPercent:F0}% ({project.CompletedTaskCount}/{project.TotalTaskCount} завдань виконано)";

                _allTasks.Clear();
                _allTasks.AddRange(project.Tasks);
                ApplyTaskFilterAndSort();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Не вдалося завантажити проєкт:\n{ex.Message}");
            }
        }
    }

    private void ApplyTaskFilterAndSort()
    {
        IEnumerable<TaskListItemDto> query = _allTasks;

        if (!string.IsNullOrWhiteSpace(_taskSearchText))
        {
            var term = _taskSearchText.Trim();
            query = query.Where(t =>
                t.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                t.Priority.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        query = _selectedTaskStatusFilter switch
        {
            StatusFilters.Pending => query.Where(t => !t.IsCompleted && !t.IsOverdue),
            StatusFilters.Completed => query.Where(t => t.IsCompleted),
            StatusFilters.Overdue => query.Where(t => t.IsOverdue),
            _ => query,
        };

        query = _selectedTaskSortOption switch
        {
            TaskSortOptions.TitleAsc => query.OrderBy(t => t.Title, StringComparer.CurrentCultureIgnoreCase),
            TaskSortOptions.TitleDesc => query.OrderByDescending(t => t.Title, StringComparer.CurrentCultureIgnoreCase),
            TaskSortOptions.DueDateAsc => query.OrderBy(t => t.DueDateRaw),
            TaskSortOptions.DueDateDesc => query.OrderByDescending(t => t.DueDateRaw),
            TaskSortOptions.PriorityAsc => query.OrderBy(t => PriorityOrder(t.Priority)),
            _ => query.OrderBy(t => t.Id)
        };

        Tasks.Clear();
        foreach (var task in query)
            Tasks.Add(task);

        HasTasks = Tasks.Count > 0;
    }

    private async Task DeleteProjectAsync()
    {
        if (!ProjectExists)
            return;
        var message = _allTasks.Count > 0
            ? $"Видалити проєкт \"{ProjectName}\" разом із {_allTasks.Count} завданнями?"
            : $"Видалити проєкт \"{ProjectName}\"?";
        if (!_dialogService.Confirm(message, "Видалення проєкту"))
            return;

        using (_busyService.BeginWork("Видалення проєкту..."))
        {
            try
            {
                var ok = await _projectService.DeleteProjectAsync(_projectId);
                if (!ok)
                {
                    _dialogService.ShowError("Проєкт не знайдено.");
                    return;
                }
                _navigationService.GoBack();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Не вдалося видалити проєкт:\n{ex.Message}");
            }
        }
    }

    private void OnOpenTask(object? parameter)
    {
        if (parameter is TaskListItemDto task)
            _navigationService.NavigateTo<TaskDetailsViewModel>(task.Id);
    }

    private void OnEditTask(object? parameter)
    {
        if (parameter is TaskListItemDto task)
            _navigationService.NavigateTo<TaskEditViewModel>(new TaskEditArgs(task.Id, _projectId));
    }

    private async Task DeleteTaskAsync(object? parameter)
    {
        if (parameter is not TaskListItemDto task)
            return;

        if (!_dialogService.Confirm($"Видалити завдання \"{task.Title}\"?", "Видалення завдання"))
            return;

        using (_busyService.BeginWork("Видалення завдання..."))
        {
            try
            {
                var ok = await _taskItemService.DeleteTaskAsync(task.Id);
                if (!ok)
                {
                    _dialogService.ShowError("Завдання не знайдено.");
                    return;
                }
                await LoadProjectDetailsAsync();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Не вдалося видалити завдання:\n{ex.Message}");
            }
        }
    }

    private async Task ToggleTaskCompletedAsync(object? parameter)
    {
        if (parameter is not TaskListItemDto task)
            return;

        using (_busyService.BeginWork("Оновлення завдання..."))
        {
            try
            {
                var form = new TaskFormDto
                {
                    Id = task.Id,
                    ProjectId = _projectId,
                    Title = task.Title,
                    Description = string.Empty,
                    Priority = task.Priority,
                    DueDate = task.DueDateRaw,
                    IsCompleted = !task.IsCompleted
                };

                var fresh = await _taskItemService.GetTaskDetailAsync(task.Id);
                if (fresh is null)
                {
                    await LoadProjectDetailsAsync();
                    return;
                }
                form.Description = fresh.Description;
                form.Priority = fresh.Priority;
                form.DueDate = fresh.DueDateRaw;

                await _taskItemService.UpdateTaskAsync(form);
                await LoadProjectDetailsAsync();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Не вдалося оновити завдання:\n{ex.Message}");
            }
        }
    }

    private static int PriorityOrder(string priority) => priority switch
    {
        "Critical" => 0,
        "High" => 1,
        "Medium" => 2,
        "Low" => 3,
        _ => 99
    };

    private static class TaskSortOptions
    {
        public const string DueDateAsc = "Дедлайн (ранні першими)";
        public const string DueDateDesc = "Дедлайн (пізні першими)";
        public const string TitleAsc = "Назва (А-Я)";
        public const string TitleDesc = "Назва (Я-А)";
        public const string PriorityAsc = "Пріоритет";

        public static IReadOnlyList<string> All { get; } =
        [
            DueDateAsc, DueDateDesc, TitleAsc, TitleDesc, PriorityAsc
        ];
    }

    private static class StatusFilters
    {
        public const string AllLabel = "Усі";
        public const string Pending = "У процесі";
        public const string Completed = "Виконані";
        public const string Overdue = "Прострочені";

        public static IReadOnlyList<string> All { get; } = [AllLabel, Pending, Completed, Overdue];
    }
}
