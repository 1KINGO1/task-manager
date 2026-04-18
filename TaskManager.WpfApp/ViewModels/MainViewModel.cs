using System.ComponentModel;
using System.Windows.Input;
using TaskManager.WpfApp.Services;

namespace TaskManager.WpfApp.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IBusyService _busyService;
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

    public bool IsBusy => _busyService.IsBusy;
    public bool IsNotBusy => !_busyService.IsBusy;
    public string? BusyMessage => _busyService.Message;

    public ICommand GoBackCommand { get; }

    public MainViewModel(IBusyService busyService)
    {
        _busyService = busyService;
        _busyService.PropertyChanged += OnBusyServiceChanged;
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

    private void OnBusyServiceChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IBusyService.IsBusy))
        {
            OnPropertyChanged(nameof(IsBusy));
            OnPropertyChanged(nameof(IsNotBusy));
        }
        else if (e.PropertyName == nameof(IBusyService.Message))
        {
            OnPropertyChanged(nameof(BusyMessage));
        }
    }

    private void UpdateTitle()
    {
        Title = CurrentViewModel switch
        {
            ProjectsListViewModel => "Менеджер проєктів",
            ProjectEditViewModel editVm => editVm.IsNew ? "Новий проєкт" : "Редагування проєкту",
            ProjectDetailsViewModel => "Деталі проєкту",
            TaskEditViewModel taskEditVm => taskEditVm.IsNew ? "Нове завдання" : "Редагування завдання",
            TaskDetailsViewModel => "Деталі завдання",
            _ => "Task Manager"
        };
    }
}
