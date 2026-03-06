using TaskFlowManagement.Core.Entities;

namespace TaskFlowManagement.Core.Interfaces.Services
{
    /// <summary>
    /// Interface cho Task Service – nghiệp vụ quản lý công việc.
    /// </summary>
    public interface ITaskService
    {
        Task<(List<TaskItem> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize,
            int? projectId = null, int? assignedToId = null,
            int? statusId = null, int? priorityId = null,
            string? keyword = null);

        Task<List<TaskItem>> GetMyTasksAsync(int userId);
        Task<List<TaskItem>> GetOverdueTasksAsync();
        Task<List<TaskItem>> GetDueSoonTasksAsync(int days = 7);
        Task<Dictionary<string, int>> GetStatusSummaryAsync(int projectId);

        Task<(bool Success, string Message)> CreateTaskAsync(TaskItem task);
        Task<(bool Success, string Message)> UpdateTaskAsync(TaskItem task);
        Task<(bool Success, string Message)> UpdateProgressAsync(int taskId, byte progress);
        Task<(bool Success, string Message)> DeleteTaskAsync(int taskId);
    }
}
