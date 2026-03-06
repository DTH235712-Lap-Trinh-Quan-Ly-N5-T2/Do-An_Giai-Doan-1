using TaskFlowManagement.Core.Entities;
using TaskFlowManagement.Core.Interfaces;
using TaskFlowManagement.Core.Interfaces.Services;

// Namespace số nhiều "Tasks" (không phải "Task") vì:
//   "Services.Task" trùng tên với C# System.Threading.Tasks.Task
//   → mọi Task<T> trong file bị nhầm là namespace thay vì kiểu trả về
//   → lỗi biên dịch hàng loạt.
//   GlobalUsings.cs đã import System.Threading.Tasks nên Task<T> luôn resolve đúng.
namespace TaskFlowManagement.Core.Services.Tasks
{
    /// <summary>
    /// Triển khai ITaskService – chứa nghiệp vụ quản lý công việc (Task).
    ///
    /// Luồng dữ liệu:
    ///   WinForms Form → ITaskService → ITaskRepository → AppDbContext → SQL Server
    /// </summary>
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepo;

        // Inject qua constructor – DI Container cung cấp TaskRepository
        public TaskService(ITaskRepository taskRepo) => _taskRepo = taskRepo;

        // -------------------------------------------------------
        // Phân trang + lọc đa tiêu chí – delegate thẳng xuống Repository
        // Tất cả tham số nullable: chỉ WHERE khi có giá trị (optional filter)
        // -------------------------------------------------------
        public Task<(List<TaskItem> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize,
            int? projectId    = null,
            int? assignedToId = null,
            int? statusId     = null,
            int? priorityId   = null,
            string? keyword   = null)
            => _taskRepo.GetPagedAsync(page, pageSize,
                projectId, assignedToId, statusId, priorityId, keyword);

        // Màn hình "Công việc của tôi" – Developer chỉ thấy task được giao cho mình
        public Task<List<TaskItem>> GetMyTasksAsync(int userId)
            => _taskRepo.GetAssignedToUserAsync(userId);

        // Widget đỏ Dashboard – task đã quá hạn chưa hoàn thành
        public Task<List<TaskItem>> GetOverdueTasksAsync()
            => _taskRepo.GetOverdueAsync();

        // Widget vàng Dashboard – task sắp đến hạn trong N ngày tới
        public Task<List<TaskItem>> GetDueSoonTasksAsync(int days = 7)
            => _taskRepo.GetDueSoonAsync(days);

        // Pie chart Dashboard – đếm task theo từng Status
        public Task<Dictionary<string, int>> GetStatusSummaryAsync(int projectId)
            => _taskRepo.GetStatusSummaryByProjectAsync(projectId);

        // -------------------------------------------------------
        // Tạo task mới – validate title trước
        // -------------------------------------------------------
        public async Task<(bool Success, string Message)> CreateTaskAsync(TaskItem task)
        {
            if (string.IsNullOrWhiteSpace(task.Title))
                return (false, "Tiêu đề task không được để trống.");

            await _taskRepo.AddAsync(task);
            return (true, "Tạo task thành công.");
        }

        public async Task<(bool Success, string Message)> UpdateTaskAsync(TaskItem task)
        {
            await _taskRepo.UpdateAsync(task);
            return (true, "Cập nhật task thành công.");
        }

        // -------------------------------------------------------
        // Cập nhật tiến độ (0-100%)
        // progress = 100 → tự động đánh dấu hoàn thành
        // -------------------------------------------------------
        public async Task<(bool Success, string Message)> UpdateProgressAsync(
            int taskId, byte progress)
        {
            // Validate: tiến độ chỉ từ 0 đến 100
            if (progress > 100)
                return (false, "Tiến độ phải từ 0 đến 100.");

            await _taskRepo.UpdateProgressAsync(taskId, progress);
            return progress == 100
                ? (true, "🎉 Task đã hoàn thành!")
                : (true, $"Cập nhật tiến độ {progress}% thành công.");
        }

        public async Task<(bool Success, string Message)> DeleteTaskAsync(int taskId)
        {
            await _taskRepo.DeleteAsync(taskId);
            return (true, "Xóa task thành công.");
        }
    }
}
