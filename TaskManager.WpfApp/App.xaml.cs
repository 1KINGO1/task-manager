using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Services;
using TaskManager.WpfApp.Services;
using TaskManager.WpfApp.ViewModels;

namespace TaskManager.WpfApp;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    private async void Application_Startup(object sender, StartupEventArgs e)
    {
        var services = new ServiceCollection();

        var storageFilePath = GetStorageFilePath();
        services.AddTaskManagerServices(storageFilePath);

        services.AddSingleton<IBusyService, BusyService>();
        services.AddSingleton<IDialogService, DialogService>();

        services.AddSingleton<MainViewModel>();
        services.AddTransient<ProjectsListViewModel>();
        services.AddTransient<ProjectDetailsViewModel>();
        services.AddTransient<ProjectEditViewModel>();
        services.AddTransient<TaskDetailsViewModel>();
        services.AddTransient<TaskEditViewModel>();

        services.AddSingleton<Func<Type, ViewModelBase>>(provider =>
            viewModelType => (ViewModelBase)provider.GetRequiredService(viewModelType));

        services.AddSingleton<INavigationService>(provider =>
        {
            var mainViewModel = provider.GetRequiredService<MainViewModel>();
            var factory = provider.GetRequiredService<Func<Type, ViewModelBase>>();
            var navigationService = new NavigationService(factory, mainViewModel);
            mainViewModel.SetNavigationService(navigationService);
            return navigationService;
        });

        services.AddSingleton<MainWindow>();

        _serviceProvider = services.BuildServiceProvider();

        var initializer = _serviceProvider.GetRequiredService<IStorageInitializer>();
        try
        {
            await initializer.InitializeAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не вдалося ініціалізувати сховище:\n{ex.Message}",
                "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
            return;
        }

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        var navigationService = _serviceProvider.GetRequiredService<INavigationService>();
        navigationService.NavigateTo<ProjectsListViewModel>();

        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }

    private static string GetStorageFilePath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var directory = Path.Combine(appData, "TaskManager");
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, "task-manager.json");
    }
}
