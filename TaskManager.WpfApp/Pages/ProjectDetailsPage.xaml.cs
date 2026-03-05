using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.WpfApp.Pages;

public partial class ProjectDetailsPage : Page
{
    private readonly MainWindow _mainWindow;
    private readonly IStorageService _storageService;
    private readonly int _projectId;

    public ProjectDetailsPage(MainWindow mainWindow, int projectId)
    {
        InitializeComponent();
        _mainWindow = mainWindow;
        _projectId = projectId;
        _storageService = App.ServiceProvider.GetRequiredService<IStorageService>();
        LoadProjectDetails();
    }

    private void LoadProjectDetails()
    {
        var projects = _storageService.GetAllProjects().ToList();
        var project = projects.FirstOrDefault(p => p.Id == _projectId);
        if (project is null)
        {
            ProjectNameText.Text = "Проєкт не знайдено";
            return;
        }

        project.Tasks = _storageService.GetTasksByProjectId(_projectId).ToList();

        ProjectNameText.Text = project.Name;
        ProjectTypeText.Text = $"Тип: {project.Type}";
        ProjectDescriptionText.Text = project.Description;
        ProjectProgressText.Text = $"Прогрес: {project.ProgressPercent:F0}% ({project.Tasks.Count(t => t.IsCompleted)}/{project.Tasks.Count} завдань виконано)";

        if (project.Tasks.Count > 0)
        {
            TasksListBox.ItemsSource = project.Tasks;
            NoTasksText.Visibility = Visibility.Collapsed;
            TasksListBox.Visibility = Visibility.Visible;
        }
        else
        {
            NoTasksText.Visibility = Visibility.Visible;
            TasksListBox.Visibility = Visibility.Collapsed;
        }
    }

    private void TasksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TasksListBox.SelectedItem is TaskItemModel task)
        {
            _mainWindow.NavigateToTaskDetails(_projectId, task.Id);
        }
    }
}
