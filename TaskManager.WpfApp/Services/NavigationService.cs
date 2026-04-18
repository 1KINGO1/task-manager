using TaskManager.WpfApp.ViewModels;

namespace TaskManager.WpfApp.Services;

public class NavigationService : INavigationService
{
    private readonly Func<Type, ViewModelBase> _viewModelFactory;
    private readonly MainViewModel _mainViewModel;
    private readonly Stack<NavigationEntry> _navigationStack = new();
    private NavigationEntry? _current;

    public NavigationService(Func<Type, ViewModelBase> viewModelFactory, MainViewModel mainViewModel)
    {
        _viewModelFactory = viewModelFactory;
        _mainViewModel = mainViewModel;
    }

    public bool CanGoBack => _navigationStack.Count > 0;

    public void NavigateTo<TViewModel>(object? parameter = null) where TViewModel : class
    {
        if (_current is not null)
            _navigationStack.Push(_current);

        var viewModel = _viewModelFactory(typeof(TViewModel));
        var entry = new NavigationEntry(viewModel, parameter);

        _current = entry;
        _mainViewModel.CurrentViewModel = viewModel;
        _mainViewModel.UpdateNavigationState();

        if (viewModel is INavigationAware navigationAware)
            navigationAware.OnNavigatedTo(parameter);
    }

    public void GoBack()
    {
        if (!CanGoBack)
            return;

        var previous = _navigationStack.Pop();
        _current = previous;
        _mainViewModel.CurrentViewModel = previous.ViewModel;
        _mainViewModel.UpdateNavigationState();

        if (previous.ViewModel is INavigationAware navigationAware)
            navigationAware.OnNavigatedTo(previous.Parameter);
    }

    private sealed record NavigationEntry(ViewModelBase ViewModel, object? Parameter);
}
