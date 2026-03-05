using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Services;

namespace TaskManager.WpfApp.Pages;

public partial class TaskDetailsPage : Page
{
    private readonly MainWindow _mainWindow;
    private readonly IStorageService _storageService;
    private readonly int _projectId;
    private readonly int _taskId;

    public TaskDetailsPage(MainWindow mainWindow, int projectId, int taskId)
    {
        InitializeComponent();
        _mainWindow = mainWindow;
        _projectId = projectId;
        _taskId = taskId;
        _storageService = App.ServiceProvider.GetRequiredService<IStorageService>();
        LoadTaskDetails();
    }

    private void LoadTaskDetails()
    {
        var tasks = _storageService.GetTasksByProjectId(_projectId);
        var task = tasks.FirstOrDefault(t => t.Id == _taskId);
        if (task is null)
        {
            TaskTitleText.Text = "Завдання не знайдено";
            return;
        }

        TaskTitleText.Text = task.Title;
        ProjectIdText.Text = task.ProjectId.ToString();
        PriorityText.Text = task.Priority.ToString();
        DueDateText.Text = task.DueDate.ToString("yyyy-MM-dd");
        StatusText.Text = task.StatusDisplay;
        DescriptionText.Text = task.Description;
    }
}
