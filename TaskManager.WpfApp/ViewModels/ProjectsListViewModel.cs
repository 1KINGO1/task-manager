using System.Collections.ObjectModel;
using System.Windows.Input;
using TaskManager.Services;
using TaskManager.Services.Dto;
using TaskManager.WpfApp.Services;

namespace TaskManager.WpfApp.ViewModels;

public class ProjectsListViewModel : ViewModelBase, INavigationAware
{
    private readonly IProjectService _projectService;
    private readonly INavigationService _navigationService;
    private ProjectListItemDto? _selectedProject;

    public ObservableCollection<ProjectListItemDto> Projects { get; } = [];

    public ProjectListItemDto? SelectedProject
    {
        get => _selectedProject;
        set
        {
            if (SetProperty(ref _selectedProject, value) && value is not null)
                NavigateToProjectDetails(value);
        }
    }

    public ICommand SelectProjectCommand { get; }

    public ProjectsListViewModel(IProjectService projectService, INavigationService navigationService)
    {
        _projectService = projectService;
        _navigationService = navigationService;
        SelectProjectCommand = new RelayCommand(OnSelectProject);
    }

    public void OnNavigatedTo(object? parameter)
    {
        LoadProjects();
    }

    private void LoadProjects()
    {
        Projects.Clear();
        var projects = _projectService.GetAllProjects();
        foreach (var project in projects)
            Projects.Add(project);
    }

    private void NavigateToProjectDetails(ProjectListItemDto project)
    {
        _navigationService.NavigateTo<ProjectDetailsViewModel>(project.Id);
        _selectedProject = null;
        OnPropertyChanged(nameof(SelectedProject));
    }

    private void OnSelectProject(object? parameter)
    {
        if (parameter is ProjectListItemDto project)
            NavigateToProjectDetails(project);
    }
}
