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

    private string _projectName = string.Empty;
    private string _projectType = string.Empty;
    private string _projectDescription = string.Empty;
    private string _progressText = string.Empty;
    private bool _hasTasks;
    private TaskListItemDto? _selectedTask;

    public string ProjectName
    {
        get => _projectName;
        set => SetProperty(ref _projectName, value);
    }

    public string ProjectType
    {
        get => _projectType;
        set => SetProperty(ref _projectType, value);
    }

    public string ProjectDescription
    {
        get => _projectDescription;
        set => SetProperty(ref _projectDescription, value);
    }

    public string ProgressText
    {
        get => _progressText;
        set => SetProperty(ref _progressText, value);
    }

    public bool HasTasks
    {
        get => _hasTasks;
        set => SetProperty(ref _hasTasks, value);
    }

    public ObservableCollection<TaskListItemDto> Tasks { get; } = [];

    public TaskListItemDto? SelectedTask
    {
        get => _selectedTask;
        set
        {
            if (SetProperty(ref _selectedTask, value) && value is not null)
                NavigateToTaskDetails(value);
        }
    }

    public ICommand SelectTaskCommand { get; }

    public ProjectDetailsViewModel(IProjectService projectService, INavigationService navigationService)
    {
        _projectService = projectService;
        _navigationService = navigationService;
        SelectTaskCommand = new RelayCommand(OnSelectTask);
    }

    public void OnNavigatedTo(object? parameter)
    {
        if (parameter is int projectId)
            LoadProjectDetails(projectId);
    }

    private void LoadProjectDetails(int projectId)
    {
        var project = _projectService.GetProjectDetail(projectId);
        if (project is null)
        {
            ProjectName = "Проєкт не знайдено";
            return;
        }

        ProjectName = project.Name;
        ProjectType = $"Тип: {project.Type}";
        ProjectDescription = project.Description;
        ProgressText = $"Прогрес: {project.ProgressPercent:F0}% ({project.CompletedTaskCount}/{project.TotalTaskCount} завдань виконано)";

        Tasks.Clear();
        foreach (var task in project.Tasks)
            Tasks.Add(task);

        HasTasks = Tasks.Count > 0;
    }

    private void NavigateToTaskDetails(TaskListItemDto task)
    {
        _navigationService.NavigateTo<TaskDetailsViewModel>(task.Id);
        _selectedTask = null;
        OnPropertyChanged(nameof(SelectedTask));
    }

    private void OnSelectTask(object? parameter)
    {
        if (parameter is TaskListItemDto task)
            NavigateToTaskDetails(task);
    }
}
