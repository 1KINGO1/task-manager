using TaskManager.Repositories.Models;

namespace TaskManager.Repositories;

public interface IProjectRepository
{
    IReadOnlyList<ProjectData> GetAllProjects();
    ProjectData? GetProjectById(int id);
}
