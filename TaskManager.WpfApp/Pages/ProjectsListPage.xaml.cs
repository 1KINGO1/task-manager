using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.WpfApp.Pages;

public partial class ProjectsListPage : Page
{
    private readonly MainWindow _mainWindow;
    private readonly IStorageService _storageService;

    public ProjectsListPage(MainWindow mainWindow)
    {
        InitializeComponent();
        _mainWindow = mainWindow;
        _storageService = App.ServiceProvider.GetRequiredService<IStorageService>();
        LoadProjects();
    }

    private void LoadProjects()
    {
        var projects = _storageService.GetAllProjects().ToList();
        foreach (var project in projects)
        {
            project.Tasks = _storageService.GetTasksByProjectId(project.Id).ToList();
        }
        ProjectsListBox.ItemsSource = projects;
    }

    private void ProjectsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ProjectsListBox.SelectedItem is ProjectModel project)
        {
            _mainWindow.NavigateToProjectDetails(project.Id);
        }
    }
}
