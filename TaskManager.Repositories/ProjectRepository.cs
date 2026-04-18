using TaskManager.Repositories.Models;
using TaskManager.Repositories.Storage;

namespace TaskManager.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly IDataStore _dataStore;

    public ProjectRepository(IDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<IReadOnlyList<ProjectData>> GetAllAsync() =>
        _dataStore.GetProjectsAsync();

    public Task<ProjectData?> GetByIdAsync(int id) =>
        _dataStore.GetProjectByIdAsync(id);

    public Task<ProjectData> AddAsync(ProjectData project) =>
        _dataStore.AddProjectAsync(project);

    public Task<bool> UpdateAsync(ProjectData project) =>
        _dataStore.UpdateProjectAsync(project);

    public Task<bool> DeleteAsync(int id) =>
        _dataStore.DeleteProjectAsync(id);
}
