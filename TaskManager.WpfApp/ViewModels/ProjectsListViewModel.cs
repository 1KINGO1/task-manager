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
    private readonly IBusyService _busyService;
    private readonly IDialogService _dialogService;

    private readonly List<ProjectListItemDto> _allProjects = new();
    private string _searchText = string.Empty;
    private string _selectedSortOption = SortOptions.NameAsc;
    private bool _isEmpty;

    public ObservableCollection<ProjectListItemDto> Projects { get; } = [];
    public IReadOnlyList<string> SortOptionsList { get; } = SortOptions.All;

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
                ApplyFilterAndSort();
        }
    }

    public string SelectedSortOption
    {
        get => _selectedSortOption;
        set
        {
            if (SetProperty(ref _selectedSortOption, value))
                ApplyFilterAndSort();
        }
    }

    public bool IsEmpty
    {
        get => _isEmpty;
        private set => SetProperty(ref _isEmpty, value);
    }

    public ICommand OpenProjectCommand { get; }
    public ICommand EditProjectCommand { get; }
    public ICommand DeleteProjectCommand { get; }
    public ICommand AddProjectCommand { get; }
    public ICommand RefreshCommand { get; }

    public ProjectsListViewModel(
        IProjectService projectService,
        INavigationService navigationService,
        IBusyService busyService,
        IDialogService dialogService)
    {
        _projectService = projectService;
        _navigationService = navigationService;
        _busyService = busyService;
        _dialogService = dialogService;

        OpenProjectCommand = new RelayCommand(OnOpenProject);
        EditProjectCommand = new RelayCommand(OnEditProject);
        DeleteProjectCommand = new AsyncRelayCommand(OnDeleteProjectAsync);
        AddProjectCommand = new RelayCommand(OnAddProject);
        RefreshCommand = new AsyncRelayCommand(LoadProjectsAsync);
    }

    public void OnNavigatedTo(object? parameter)
    {
        _ = LoadProjectsAsync();
    }

    private async Task LoadProjectsAsync()
    {
        using (_busyService.BeginWork("Завантаження проєктів..."))
        {
            try
            {
                var projects = await _projectService.GetAllProjectsAsync();
                _allProjects.Clear();
                _allProjects.AddRange(projects);
                ApplyFilterAndSort();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Не вдалося завантажити проєкти:\n{ex.Message}");
            }
        }
    }

    private void ApplyFilterAndSort()
    {
        IEnumerable<ProjectListItemDto> query = _allProjects;

        if (!string.IsNullOrWhiteSpace(_searchText))
        {
            var term = _searchText.Trim();
            query = query.Where(p =>
                p.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                p.Type.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        query = _selectedSortOption switch
        {
            SortOptions.NameAsc => query.OrderBy(p => p.Name, StringComparer.CurrentCultureIgnoreCase),
            SortOptions.NameDesc => query.OrderByDescending(p => p.Name, StringComparer.CurrentCultureIgnoreCase),
            SortOptions.TypeAsc => query.OrderBy(p => p.Type).ThenBy(p => p.Name),
            SortOptions.ProgressAsc => query.OrderBy(p => p.ProgressPercent).ThenBy(p => p.Name),
            SortOptions.ProgressDesc => query.OrderByDescending(p => p.ProgressPercent).ThenBy(p => p.Name),
            _ => query.OrderBy(p => p.Id)
        };

        Projects.Clear();
        foreach (var project in query)
            Projects.Add(project);

        IsEmpty = Projects.Count == 0;
    }

    private void OnOpenProject(object? parameter)
    {
        if (parameter is ProjectListItemDto project)
            _navigationService.NavigateTo<ProjectDetailsViewModel>(project.Id);
    }

    private void OnEditProject(object? parameter)
    {
        if (parameter is ProjectListItemDto project)
            _navigationService.NavigateTo<ProjectEditViewModel>(project.Id);
    }

    private void OnAddProject()
    {
        _navigationService.NavigateTo<ProjectEditViewModel>(null);
    }

    private async Task OnDeleteProjectAsync(object? parameter)
    {
        if (parameter is not ProjectListItemDto project)
            return;

        var confirmMessage = project.TotalTaskCount > 0
            ? $"Видалити проєкт \"{project.Name}\" разом із {project.TotalTaskCount} пов'язаними завданнями?"
            : $"Видалити проєкт \"{project.Name}\"?";

        if (!_dialogService.Confirm(confirmMessage, "Видалення проєкту"))
            return;

        using (_busyService.BeginWork("Видалення проєкту..."))
        {
            try
            {
                var deleted = await _projectService.DeleteProjectAsync(project.Id);
                if (!deleted)
                {
                    _dialogService.ShowError("Проєкт не знайдено.");
                    return;
                }
                _allProjects.RemoveAll(p => p.Id == project.Id);
                ApplyFilterAndSort();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Не вдалося видалити проєкт:\n{ex.Message}");
            }
        }
    }

    private static class SortOptions
    {
        public const string NameAsc = "Назва (А-Я)";
        public const string NameDesc = "Назва (Я-А)";
        public const string TypeAsc = "Тип";
        public const string ProgressAsc = "Прогрес (зростання)";
        public const string ProgressDesc = "Прогрес (спадання)";

        public static IReadOnlyList<string> All { get; } =
        [
            NameAsc, NameDesc, TypeAsc, ProgressAsc, ProgressDesc
        ];
    }
}
