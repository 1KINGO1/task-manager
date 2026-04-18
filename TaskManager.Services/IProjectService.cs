using TaskManager.Services.Dto;

namespace TaskManager.Services;

public interface IProjectService
{
    Task<IReadOnlyList<ProjectListItemDto>> GetAllProjectsAsync();
    Task<ProjectDetailDto?> GetProjectDetailAsync(int projectId);
    Task<ProjectListItemDto> CreateProjectAsync(ProjectFormDto form);
    Task<bool> UpdateProjectAsync(ProjectFormDto form);
    Task<bool> DeleteProjectAsync(int projectId);
}
