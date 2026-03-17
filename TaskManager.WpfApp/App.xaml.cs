using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Services;
using TaskManager.WpfApp.Services;
using TaskManager.WpfApp.ViewModels;

namespace TaskManager.WpfApp;

public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var services = new ServiceCollection();

        services.AddTaskManagerServices();

        services.AddSingleton<MainViewModel>();
        services.AddTransient<ProjectsListViewModel>();
        services.AddTransient<ProjectDetailsViewModel>();
        services.AddTransient<TaskDetailsViewModel>();

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

        var serviceProvider = services.BuildServiceProvider();

        var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
        var navigationService = serviceProvider.GetRequiredService<INavigationService>();
        navigationService.NavigateTo<ProjectsListViewModel>();

        mainWindow.Show();
    }
}
