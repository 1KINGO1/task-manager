using TaskManager.Repositories.Models;
using TaskManager.Repositories.Storage;

namespace TaskManager.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly InMemoryStorage _storage;

    public ProjectRepository(InMemoryStorage storage)
    {
        _storage = storage;
    }

    public IReadOnlyList<ProjectData> GetAllProjects()
    {
        return _storage.Projects;
    }

    public ProjectData? GetProjectById(int id)
    {
        return _storage.Projects.FirstOrDefault(p => p.Id == id);
    }
}
