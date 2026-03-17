using TaskManager.Services.Dto;

namespace TaskManager.Services;

public interface IProjectService
{
    IReadOnlyList<ProjectListItemDto> GetAllProjects();
    ProjectDetailDto? GetProjectDetail(int projectId);
}
