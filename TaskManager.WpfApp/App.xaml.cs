using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Services;

namespace TaskManager.WpfApp;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var services = new ServiceCollection();
        services.AddTaskManagerServices();
        ServiceProvider = services.BuildServiceProvider();

        var mainWindow = new MainWindow();
        mainWindow.Show();
    }
}
