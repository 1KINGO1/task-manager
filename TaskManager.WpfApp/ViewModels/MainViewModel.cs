using System.Windows.Input;
using TaskManager.WpfApp.Services;

namespace TaskManager.WpfApp.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ViewModelBase? _currentViewModel;
    private string _title = "Task Manager";
    private bool _canGoBack;
    private INavigationService? _navigationService;

    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            SetProperty(ref _currentViewModel, value);
            UpdateTitle();
        }
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public bool CanGoBack
    {
        get => _canGoBack;
        set => SetProperty(ref _canGoBack, value);
    }

    public ICommand GoBackCommand { get; }

    public MainViewModel()
    {
        GoBackCommand = new RelayCommand(OnGoBack, () => CanGoBack);
    }

    public void SetNavigationService(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public void UpdateNavigationState()
    {
        CanGoBack = _navigationService?.CanGoBack ?? false;
    }

    private void OnGoBack()
    {
        _navigationService?.GoBack();
    }

    private void UpdateTitle()
    {
        Title = CurrentViewModel switch
        {
            ProjectsListViewModel => "Task Manager",
            ProjectDetailsViewModel => "Деталі проєкту",
            TaskDetailsViewModel => "Деталі завдання",
            _ => "Task Manager"
        };
    }
}