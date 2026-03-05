using System.Windows;
using System.Windows.Navigation;
using TaskManager.WpfApp.Pages;

namespace TaskManager.WpfApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainFrame.Navigated += OnFrameNavigated;
        NavigateToProjectsList();
    }

    public void NavigateToProjectsList()
    {
        MainFrame.Navigate(new ProjectsListPage(this));
    }

    public void NavigateToProjectDetails(int projectId)
    {
        MainFrame.Navigate(new ProjectDetailsPage(this, projectId));
    }

    public void NavigateToTaskDetails(int projectId, int taskId)
    {
        MainFrame.Navigate(new TaskDetailsPage(this, projectId, taskId));
    }

    private void OnFrameNavigated(object sender, NavigationEventArgs e)
    {
        BackButton.Visibility = MainFrame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
        TitleText.Text = e.Content switch
        {
            ProjectsListPage => "Task Manager",
            ProjectDetailsPage => "Деталі проєкту",
            TaskDetailsPage => "Деталі завдання",
            _ => "Task Manager"
        };
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (MainFrame.CanGoBack)
            MainFrame.GoBack();
    }
}
