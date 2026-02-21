using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.ConsoleApp;

public static class Program
{
    private static readonly StorageService _storageService = new();

    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== Task Manager ===\n");

        bool running = true;

        while (running)
        {
            List<ProjectModel> projects = LoadProjects();
            ShowProjectList(projects);

            Console.WriteLine("\nEnter project number to view details, or 'q' to quit:");
            string? input = Console.ReadLine()?.Trim();

            if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
            {
                running = false;
                continue;
            }

            if (!int.TryParse(input, out int projectId))
            {
                Console.WriteLine("Invalid input. Please enter a valid project ID.\n");
                continue;
            }

            ProjectModel? selected = projects.Find(p => p.Id == projectId);
            if (selected is null)
            {
                Console.WriteLine($"Project with ID {projectId} not found.\n");
                continue;
            }

            ShowProjectDetails(selected);
        }

        Console.WriteLine("\nGoodbye!");
    }

    private static List<ProjectModel> LoadProjects()
    {
        List<ProjectModel> projects = _storageService.GetAllProjects();

        foreach (ProjectModel project in projects)
        {
            List<TaskItemModel> tasks = _storageService.GetTasksByProjectId(project.Id);
            project.Tasks = tasks;
        }

        return projects;
    }

    private static void ShowProjectList(List<ProjectModel> projects)
    {
        Console.WriteLine("--- Projects ---");

        foreach (ProjectModel project in projects)
        {
            Console.WriteLine(project.ToShortString());
        }
    }

    private static void ShowProjectDetails(ProjectModel project)
    {
        Console.WriteLine();
        Console.WriteLine(project.ToDetailedString());

        if (project.Tasks.Count == 0)
        {
            Console.WriteLine("  No tasks in this project.\n");
            return;
        }

        Console.WriteLine("\n  --- Tasks ---");
        foreach (TaskItemModel task in project.Tasks)
        {
            Console.WriteLine(task.ToShortString());
        }

        bool viewingTasks = true;

        while (viewingTasks)
        {
            Console.WriteLine("\nEnter task number for full details, or 'b' to go back:");
            string? taskInput = Console.ReadLine()?.Trim();

            if (string.Equals(taskInput, "b", StringComparison.OrdinalIgnoreCase))
            {
                viewingTasks = false;
                Console.WriteLine();
                continue;
            }

            if (!int.TryParse(taskInput, out int taskId))
            {
                Console.WriteLine("Invalid input. Please enter a valid task ID.");
                continue;
            }

            TaskItemModel? selectedTask = project.Tasks.Find(t => t.Id == taskId);
            if (selectedTask is null)
            {
                Console.WriteLine($"Task with ID {taskId} not found in this project.");
                continue;
            }

            Console.WriteLine();
            Console.WriteLine(selectedTask.ToDetailedString());
        }
    }
}
