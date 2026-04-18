using System.Windows.Input;
using TaskManager.Services;
using TaskManager.Services.Dto;
using TaskManager.WpfApp.Services;

namespace TaskManager.WpfApp.ViewModels;

public class ProjectEditViewModel : ViewModelBase, INavigationAware
{
    private readonly IProjectService _projectService;
    private readonly INavigationService _navigationService;
    private readonly IBusyService _busyService;
    private readonly IDialogService _dialogService;

    private int? _projectId;
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _selectedType = EnumOptions.ProjectTypes[0];
    private bool _isNew = true;

    public bool IsNew { get => _isNew; private set => SetProperty(ref _isNew, value); }

    public string FormTitle => IsNew ? "Створення нового проєкту" : "Редагування проєкту";

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public string SelectedType
    {
        get => _selectedType;
        set => SetProperty(ref _selectedType, value);
    }

    public IReadOnlyList<string> ProjectTypes => EnumOptions.ProjectTypes;

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public ProjectEditViewModel(
        IProjectService projectService,
        INavigationService navigationService,
        IBusyService busyService,
        IDialogService dialogService)
    {
        _projectService = projectService;
        _navigationService = navigationService;
        _busyService = busyService;
        _dialogService = dialogService;

        SaveCommand = new AsyncRelayCommand(SaveAsync);
        CancelCommand = new RelayCommand(() => _navigationService.GoBack());
    }

    public void OnNavigatedTo(object? parameter)
    {
        if (parameter is int projectId)
        {
            _projectId = projectId;
            IsNew = false;
            _ = LoadAsync(projectId);
        }
        else
        {
            _projectId = null;
            IsNew = true;
        }
        OnPropertyChanged(nameof(FormTitle));
    }

    private async Task LoadAsync(int projectId)
    {
        using (_busyService.BeginWork("Завантаження проєкту..."))
        {
            try
            {
                var project = await _projectService.GetProjectDetailAsync(projectId);
                if (project is null)
                {
                    _dialogService.ShowError("Проєкт не знайдено.");
                    _navigationService.GoBack();
                    return;
                }

                Name = project.Name;
                Description = project.Description;
                SelectedType = project.Type;
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Не вдалося завантажити проєкт:\n{ex.Message}");
            }
        }
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            _dialogService.ShowError("Назва проєкту не може бути порожньою.");
            return;
        }

        var form = new ProjectFormDto
        {
            Id = _projectId,
            Name = Name,
            Description = Description,
            Type = SelectedType
        };

        using (_busyService.BeginWork(IsNew ? "Створення проєкту..." : "Збереження проєкту..."))
        {
            try
            {
                if (IsNew)
                    await _projectService.CreateProjectAsync(form);
                else
                    await _projectService.UpdateProjectAsync(form);

                _navigationService.GoBack();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Не вдалося зберегти проєкт:\n{ex.Message}");
            }
        }
    }
}
