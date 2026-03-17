namespace TaskManager.WpfApp.Services;

public interface INavigationService
{
    void NavigateTo<TViewModel>(object? parameter = null) where TViewModel : class;
    void GoBack();
    bool CanGoBack { get; }
}
