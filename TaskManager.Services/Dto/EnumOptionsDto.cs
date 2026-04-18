namespace TaskManager.Services.Dto;

public static class EnumOptions
{
    public static IReadOnlyList<string> ProjectTypes { get; } =
        Enum.GetNames(typeof(Repositories.Models.ProjectType));

    public static IReadOnlyList<string> TaskPriorities { get; } =
        Enum.GetNames(typeof(Repositories.Models.TaskPriority));
}
