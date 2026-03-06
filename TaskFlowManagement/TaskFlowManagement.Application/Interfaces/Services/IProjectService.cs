using TaskFlowManagement.Core.Entities;

namespace TaskFlowManagement.Core.Interfaces.Services
{
    /// <summary>
    /// Interface cho Project Service – nghiệp vụ quản lý dự án.
    /// </summary>
    public interface IProjectService
    {
        Task<List<Project>> GetProjectsForUserAsync(int userId, bool isManager);
        Task<Project?> GetProjectDetailsAsync(int projectId);
        Task<(bool Success, string Message)> CreateProjectAsync(Project project);
        Task<(bool Success, string Message)> UpdateProjectAsync(Project project);
        Task<(bool Success, string Message)> DeleteProjectAsync(int projectId);
        Task<(bool Success, string Message)> AddMemberAsync(int projectId, int userId, string projectRole);
        Task<(bool Success, string Message)> RemoveMemberAsync(int projectId, int userId);
    }
}
