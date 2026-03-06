using TaskFlowManagement.Core.Entities;

namespace TaskFlowManagement.Core.Interfaces
{
    /// <summary>
    /// Interface cho Project repository.
    /// Cung cấp các query chuyên biệt ngoài CRUD cơ bản.
    /// </summary>
    public interface IProjectRepository : IRepository<Project>
    {
        /// <summary>
        /// Lấy danh sách dự án mà user là Owner/PM.
        /// Dùng cho Manager xem dự án của mình quản lý.
        /// </summary>
        Task<List<Project>> GetByOwnerAsync(int ownerId);

        /// <summary>
        /// Lấy danh sách dự án mà user là thành viên (ProjectMember).
        /// Dùng cho Developer xem dự án mình tham gia.
        /// Lọc LeftAt == null để bỏ qua thành viên đã rời dự án.
        /// </summary>
        Task<List<Project>> GetByMemberAsync(int userId);

        /// <summary>
        /// Lấy chi tiết đầy đủ 1 dự án:
        /// Owner, Customer, Members (kèm User), Tasks (kèm Status + AssignedTo), Expenses.
        /// Dùng cho form chi tiết dự án – tránh lazy loading bằng cách eager load tất cả.
        /// </summary>
        Task<Project?> GetWithDetailsAsync(int projectId);

        /// <summary>
        /// Lấy các dự án đang hoạt động (Status = InProgress hoặc NotStarted).
        /// Sắp xếp theo PlannedEndDate để ưu tiên dự án gần deadline.
        /// Dùng cho Dashboard tổng quan.
        /// </summary>
        Task<List<Project>> GetActiveProjectsAsync();

        /// <summary>
        /// Tính tổng chi phí thực tế của dự án từ bảng Expenses.
        /// Dùng SumAsync trực tiếp trên DB – không load toàn bộ records về memory.
        /// </summary>
        Task<decimal> GetTotalExpenseAsync(int projectId);

        /// <summary>
        /// Cập nhật trạng thái dự án (NotStarted/InProgress/OnHold/Completed/Cancelled).
        /// Dùng ExecuteUpdateAsync để chỉ update đúng 2 cột Status + UpdatedAt, không load entity.
        /// </summary>
        Task UpdateStatusAsync(int projectId, string status);
    }
}
