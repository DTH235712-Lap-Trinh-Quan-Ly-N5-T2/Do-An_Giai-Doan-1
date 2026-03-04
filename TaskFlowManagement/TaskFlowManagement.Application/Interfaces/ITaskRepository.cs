using TaskFlowManagement.Core.Entities;

namespace TaskFlowManagement.Core.Interfaces
{
    public interface ITaskRepository
    {
        // Tạo Form bằng DI
        Task<List<TaskItem>> GetAllAsync();

        // Lấy toàn bộ
        Task<TaskItem?> GetByIdAsync(int id);

        // Lấy theo Id
        Task AddAsync(TaskItem task);

        // Cập nhật
        Task UpdateAsync(TaskItem task);

        // Xoá
        Task DeleteAsync(int id);

        // 
        Task<List<TaskItem>> GetTasksPagedAsync(int page, int pageSize);
    }
}