using TaskFlowManagement.Core.Entities;

namespace TaskFlowManagement.Core.Interfaces
{
    /// <summary>
    /// Interface cho Task repository – module trung tâm của hệ thống.
    /// Ngoài CRUD cơ bản, cung cấp nhiều query chuyên biệt cho Dashboard và Kanban.
    /// </summary>
    public interface ITaskRepository : IRepository<TaskItem>
    {
        /// <summary>
        /// Phân trang + lọc đa tiêu chí (project, người phụ trách, status, priority, từ khóa).
        /// Trả về tuple (Items, TotalCount) để tính số trang ở UI.
        /// Tham số nullable = không bắt buộc, chỉ WHERE khi có giá trị.
        /// </summary>
        Task<(List<TaskItem> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize,
            int? projectId      = null,
            int? assignedToId   = null,
            int? statusId       = null,
            int? priorityId     = null,
            string? searchKeyword = null);

        /// <summary>
        /// Lấy tất cả task của 1 dự án, có thể lọc chỉ lấy task gốc (không có sub-task).
        /// Sắp xếp: Priority.Level DESC → DueDate ASC (task quan trọng + gần deadline lên đầu).
        /// </summary>
        Task<List<TaskItem>> GetByProjectAsync(int projectId, bool includeSubTasks = false);

        /// <summary>
        /// Lấy task chưa hoàn thành được giao cho 1 developer cụ thể.
        /// Dùng cho màn hình "Công việc của tôi" – Developer chỉ thấy task của mình.
        /// </summary>
        Task<List<TaskItem>> GetAssignedToUserAsync(int userId);

        /// <summary>
        /// Lấy các task đã quá hạn (DueDate &lt; Now và chưa hoàn thành).
        /// Dùng cho widget cảnh báo trên Dashboard.
        /// </summary>
        Task<List<TaskItem>> GetOverdueAsync();

        /// <summary>
        /// Lấy task sắp đến hạn trong vòng N ngày tới (mặc định 7 ngày).
        /// Dùng cho widget nhắc nhở trên Dashboard.
        /// </summary>
        Task<List<TaskItem>> GetDueSoonAsync(int days = 7);

        /// <summary>
        /// Cập nhật tiến độ % của task – tự động set IsCompleted và CompletedAt khi = 100.
        /// Dùng ExecuteUpdateAsync chỉ update đúng 4 cột, không cần load toàn bộ entity.
        /// Hàm này được gọi nhiều nên tối ưu là ưu tiên.
        /// </summary>
        Task UpdateProgressAsync(int taskId, byte progressPercent);

        /// <summary>
        /// Đếm số task theo từng Status trong 1 dự án.
        /// Trả về Dictionary (Status.Name → Count) dùng cho biểu đồ pie chart Dashboard.
        /// GroupBy trực tiếp trên DB bằng LINQ, không load data thừa về memory.
        /// </summary>
        Task<Dictionary<string, int>> GetStatusSummaryByProjectAsync(int projectId);
    }
}
