using TaskManager.WpfApp.ViewModels;

namespace TaskManager.WpfApp.Services;

public class NavigationService : INavigationService
{
    private readonly Func<Type, ViewModelBase> _viewModelFactory;
    private readonly MainViewModel _mainViewModel;
    private readonly Stack<ViewModelBase> _navigationStack = new();

    public NavigationService(Func<Type, ViewModelBase> viewModelFactory, MainViewModel mainViewModel)
    {
        _viewModelFactory = viewModelFactory;
        _mainViewModel = mainViewModel;
    }

    public bool CanGoBack => _navigationStack.Count > 0;

    public void NavigateTo<TViewModel>(object? parameter = null) where TViewModel : class
    {
        if (_mainViewModel.CurrentViewModel is not null)
            _navigationStack.Push(_mainViewModel.CurrentViewModel);

        var viewModel = _viewModelFactory(typeof(TViewModel));

        if (viewModel is INavigationAware navigationAware)
            navigationAware.OnNavigatedTo(parameter);

        _mainViewModel.CurrentViewModel = viewModel;
        _mainViewModel.UpdateNavigationState();
    }

    public void GoBack()
    {
        if (!CanGoBack)
            return;

        _mainViewModel.CurrentViewModel = _navigationStack.Pop();
        _mainViewModel.UpdateNavigationState();
    }
}
