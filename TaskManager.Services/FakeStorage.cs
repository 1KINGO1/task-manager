using TaskManager.Data;

namespace TaskManager.Services;

internal static class FakeStorage
{
    private static List<ProjectData>? _projects;
    private static List<TaskItemData>? _tasks;

    internal static List<ProjectData> Projects => _projects ??= CreateProjects();
    internal static List<TaskItemData> Tasks => _tasks ??= CreateTasks();

    private static List<ProjectData> CreateProjects()
    {
        return new List<ProjectData>
        {
            new(1, "University Course Platform",
                "Web application for managing university courses, assignments, and grades.",
                ProjectType.Educational),

            new(2, "Inventory Management System",
                "Internal tool for tracking warehouse stock, shipments, and supplier orders.",
                ProjectType.Work),

            new(3, "Personal Finance Tracker",
                "Mobile-friendly app for tracking daily expenses, income, and savings goals.",
                ProjectType.Personal)
        };
    }

    private static List<TaskItemData> CreateTasks()
    {
        return new List<TaskItemData>
        {
            new(1, 1, "Design database schema",
                "Create ER diagram and define tables for courses, students, and enrollments.",
                TaskPriority.High, new DateTime(2026, 2, 10), true),

            new(2, 1, "Implement user authentication",
                "Set up JWT-based authentication with role support (student, teacher, admin).",
                TaskPriority.Critical, new DateTime(2026, 2, 15), true),

            new(3, 1, "Build course listing page",
                "Display available courses with filters by department and semester.",
                TaskPriority.Medium, new DateTime(2026, 2, 20), true),

            new(4, 1, "Create assignment submission module",
                "Allow students to upload files and submit assignments before the deadline.",
                TaskPriority.High, new DateTime(2026, 3, 1)),

            new(5, 1, "Develop grading interface",
                "Teachers can view submissions, leave feedback, and assign grades.",
                TaskPriority.High, new DateTime(2026, 3, 5)),

            new(6, 1, "Add email notifications",
                "Send email alerts for new assignments, grade updates, and deadlines.",
                TaskPriority.Medium, new DateTime(2026, 3, 10)),

            new(7, 1, "Write unit tests for API",
                "Cover authentication, course CRUD, and enrollment endpoints with tests.",
                TaskPriority.Low, new DateTime(2026, 3, 15)),

            new(8, 1, "Set up CI/CD pipeline",
                "Configure GitHub Actions for automated build, test, and deployment.",
                TaskPriority.Medium, new DateTime(2026, 3, 20)),

            new(9, 1, "Create student dashboard",
                "Show enrolled courses, upcoming deadlines, and recent grades.",
                TaskPriority.Medium, new DateTime(2026, 3, 25)),

            new(10, 1, "Prepare project documentation",
                "Write README, API docs, and deployment guide for the project.",
                TaskPriority.Low, new DateTime(2026, 4, 1)),

            new(11, 2, "Define product data model",
                "Create classes for products, categories, suppliers, and stock levels.",
                TaskPriority.High, new DateTime(2026, 2, 18), true),

            new(12, 2, "Build stock level report",
                "Generate a summary report showing current stock, low-stock alerts, and reorder suggestions.",
                TaskPriority.Medium, new DateTime(2026, 3, 1)),
        };
    }
}
