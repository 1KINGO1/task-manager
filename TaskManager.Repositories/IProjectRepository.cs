using TaskManager.Repositories.Models;

namespace TaskManager.Repositories;

public interface IProjectRepository
{
    Task<IReadOnlyList<ProjectData>> GetAllAsync();
    Task<ProjectData?> GetByIdAsync(int id);
    Task<ProjectData> AddAsync(ProjectData project);
    Task<bool> UpdateAsync(ProjectData project);
    Task<bool> DeleteAsync(int id);
}
