using System.Windows.Input;
using TaskManager.Services;
using TaskManager.Services.Dto;
using TaskManager.WpfApp.Services;

namespace TaskManager.WpfApp.ViewModels;

public class TaskEditViewModel : ViewModelBase, INavigationAware
{
    private readonly ITaskItemService _taskItemService;
    private readonly IProjectService _projectService;
    private readonly INavigationService _navigationService;
    private readonly IBusyService _busyService;
    private readonly IDialogService _dialogService;

    private int? _taskId;
    private int _projectId;
    private string _title = string.Empty;
    private string _description = string.Empty;
    private string _selectedPriority = EnumOptions.TaskPriorities.FirstOrDefault() ?? string.Empty;
    private DateTime _dueDate = DateTime.Today;
    private bool _isCompleted;
    private string _projectName = string.Empty;
    private bool _isNew = true;

    public bool IsNew { get => _isNew; private set => SetProperty(ref _isNew, value); }

    public string FormTitle => IsNew ? "Створення нового завдання" : "Редагування завдання";

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public string SelectedPriority
    {
        get => _selectedPriority;
        set => SetProperty(ref _selectedPriority, value);
    }

    public DateTime DueDate
    {
        get => _dueDate;
        set => SetProperty(ref _dueDate, value);
    }

    public bool IsCompleted
    {
        get => _isCompleted;
        set => SetProperty(ref _isCompleted, value);
    }

    public string ProjectName
    {
        get => _projectName;
        set => SetProperty(ref _projectName, value);
    }

    public IReadOnlyList<string> Priorities => EnumOptions.TaskPriorities;

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public TaskEditViewModel(
        ITaskItemService taskItemService,
        IProjectService projectService,
        INavigationService navigationService,
        IBusyService busyService,
        IDialogService dialogService)
    {
        _taskItemService = taskItemService;
        _projectService = projectService;
        _navigationService = navigationService;
        _busyService = busyService;
        _dialogService = dialogService;

        SaveCommand = new AsyncRelayCommand(SaveAsync);
        CancelCommand = new RelayCommand(() => _navigationService.GoBack());
    }

    public void OnNavigatedTo(object? parameter)
    {
        if (parameter is TaskEditArgs args)
        {
            _projectId = args.ProjectId;
            _taskId = args.TaskId;
            IsNew = args.TaskId is null;
            _ = LoadAsync();
        }
        OnPropertyChanged(nameof(FormTitle));
    }

    private async Task LoadAsync()
    {
        using (_busyService.BeginWork("Завантаження..."))
        {
            try
            {
                var project = await _projectService.GetProjectDetailAsync(_projectId);
                ProjectName = project?.Name ?? string.Empty;

                if (_taskId is not null)
                {
                    var task = await _taskItemService.GetTaskDetailAsync(_taskId.Value);
                    if (task is null)
                    {
                        _dialogService.ShowError("Завдання не знайдено.");
                        _navigationService.GoBack();
                        return;
                    }

                    Title = task.Title;
                    Description = task.Description;
                    SelectedPriority = task.Priority;
                    DueDate = task.DueDateRaw.Date;
                    IsCompleted = task.IsCompleted;
                }
                else
                {
                    DueDate = DateTime.Today.AddDays(7);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Не вдалося завантажити дані:\n{ex.Message}");
            }
        }
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            _dialogService.ShowError("Назва завдання не може бути порожньою.");
            return;
        }

        var form = new TaskFormDto
        {
            Id = _taskId,
            ProjectId = _projectId,
            Title = Title,
            Description = Description,
            Priority = SelectedPriority,
            DueDate = DueDate,
            IsCompleted = IsCompleted
        };

        using (_busyService.BeginWork(IsNew ? "Створення завдання..." : "Збереження завдання..."))
        {
            try
            {
                if (IsNew)
                    await _taskItemService.CreateTaskAsync(form);
                else
                    await _taskItemService.UpdateTaskAsync(form);

                _navigationService.GoBack();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Не вдалося зберегти завдання:\n{ex.Message}");
            }
        }
    }
}
