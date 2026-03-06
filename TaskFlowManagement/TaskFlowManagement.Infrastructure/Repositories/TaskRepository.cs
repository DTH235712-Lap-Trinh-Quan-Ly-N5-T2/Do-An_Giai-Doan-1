using Microsoft.EntityFrameworkCore;
using TaskFlowManagement.Core.Entities;
using TaskFlowManagement.Core.Interfaces;
using TaskFlowManagement.Infrastructure.Data;

namespace TaskFlowManagement.Infrastructure.Repositories
{
    /// <summary>
    /// Triển khai ITaskRepository – module phức tạp nhất do TaskItem có nhiều quan hệ.
    /// Đặc biệt chú ý:
    ///   - GetPagedAsync: phân trang + multi-filter để tránh load toàn bộ record
    ///   - UpdateProgressAsync: tối ưu chỉ update 4 cột thay vì toàn bộ entity
    ///   - GetStatusSummaryByProjectAsync: GroupBy trực tiếp trên DB cho Dashboard
    /// </summary>
    public class TaskRepository : ITaskRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public TaskRepository(IDbContextFactory<AppDbContext> contextFactory)
            => _contextFactory = contextFactory;

        // -------------------------------------------------------
        // Lấy 1 task theo ID, kèm đầy đủ navigation properties để hiển thị chi tiết
        // -------------------------------------------------------
        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.TaskItems
                .AsNoTracking()
                .Include(t => t.Priority)   // Mức ưu tiên (Low/Medium/High/Critical)
                .Include(t => t.Status)     // Trạng thái (Todo/InProgress/InReview/Done)
                .Include(t => t.Category)   // Loại task (Bug/Feature/...)
                .Include(t => t.AssignedTo) // Người được giao
                .Include(t => t.CreatedBy)  // Người tạo
                .Include(t => t.SubTasks)   // Danh sách task con
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        // -------------------------------------------------------
        // Lấy tất cả task – chỉ dùng cho dataset nhỏ
        // Với data lớn, dùng GetPagedAsync để tránh load toàn bộ
        // -------------------------------------------------------
        public async Task<List<TaskItem>> GetAllAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.TaskItems
                .AsNoTracking()
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.AssignedTo)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        // -------------------------------------------------------
        // Phân trang + lọc đa tiêu chí (core query của TaskList form)
        //
        // Kỹ thuật: Xây dựng IQueryable trước (chưa gọi DB),
        // rồi thêm WHERE từng bước nếu có tham số,
        // cuối cùng mới Execute 1 lần duy nhất.
        // → SQL chỉ sinh ra đúng phần WHERE cần thiết
        // -------------------------------------------------------
        public async Task<(List<TaskItem> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize,
            int? projectId      = null,
            int? assignedToId   = null,
            int? statusId       = null,
            int? priorityId     = null,
            string? searchKeyword = null)
        {
            using var ctx = _contextFactory.CreateDbContext();

            // Bắt đầu với query base – chưa execute
            var query = ctx.TaskItems
                .AsNoTracking()
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.Category)
                .Include(t => t.AssignedTo)
                .AsQueryable();

            // Thêm điều kiện lọc chỉ khi có giá trị (optional filter)
            if (projectId.HasValue)
                query = query.Where(t => t.ProjectId == projectId);

            if (assignedToId.HasValue)
                query = query.Where(t => t.AssignedToId == assignedToId);

            if (statusId.HasValue)
                query = query.Where(t => t.StatusId == statusId);

            if (priorityId.HasValue)
                query = query.Where(t => t.PriorityId == priorityId);

            if (!string.IsNullOrWhiteSpace(searchKeyword))
                query = query.Where(t =>
                    t.Title.Contains(searchKeyword) ||
                    (t.Description != null && t.Description.Contains(searchKeyword)));

            // Đếm tổng trước khi phân trang (dùng cho tính số trang ở UI)
            var totalCount = await query.CountAsync();

            // Phân trang: bỏ qua (page-1)*pageSize record, lấy pageSize record tiếp theo
            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        // -------------------------------------------------------
        // Lấy task theo dự án
        // includeSubTasks=false: chỉ lấy task gốc (không có cha) → hiển thị cây task
        // Sắp xếp: Priority cao nhất → Deadline gần nhất
        // -------------------------------------------------------
        public async Task<List<TaskItem>> GetByProjectAsync(int projectId, bool includeSubTasks = false)
        {
            using var ctx = _contextFactory.CreateDbContext();

            var query = ctx.TaskItems
                .AsNoTracking()
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.AssignedTo)
                .Where(t => t.ProjectId == projectId);

            if (!includeSubTasks)
                query = query.Where(t => t.ParentTaskId == null); // Chỉ task gốc

            return await query
                .OrderByDescending(t => t.Priority.Level) // Cao → Thấp
                .ThenBy(t => t.DueDate)                    // Gần deadline lên trên
                .ToListAsync();
        }

        // -------------------------------------------------------
        // Lấy task của 1 developer (màn hình "Công việc của tôi")
        // Chỉ lấy task chưa hoàn thành, sắp xếp deadline gần nhất lên đầu
        // -------------------------------------------------------
        public async Task<List<TaskItem>> GetAssignedToUserAsync(int userId)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.TaskItems
                .AsNoTracking()
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.Project) // Thêm Project để hiển thị thuộc dự án nào
                .Where(t => t.AssignedToId == userId && !t.IsCompleted)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        // -------------------------------------------------------
        // Lấy task đã quá hạn (DueDate < Now và chưa xong)
        // Dùng cho widget cảnh báo đỏ trên Dashboard
        // -------------------------------------------------------
        public async Task<List<TaskItem>> GetOverdueAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            var now = DateTime.UtcNow;
            return await ctx.TaskItems
                .AsNoTracking()
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .Where(t => t.DueDate < now && !t.IsCompleted)
                .OrderBy(t => t.DueDate) // Task quá hạn lâu nhất lên đầu
                .ToListAsync();
        }

        // -------------------------------------------------------
        // Lấy task sắp đến hạn trong N ngày tới (mặc định 7 ngày)
        // Dùng cho widget nhắc nhở màu vàng trên Dashboard
        // -------------------------------------------------------
        public async Task<List<TaskItem>> GetDueSoonAsync(int days = 7)
        {
            using var ctx = _contextFactory.CreateDbContext();
            var now       = DateTime.UtcNow;
            var threshold = now.AddDays(days); // Ngưỡng thời gian
            return await ctx.TaskItems
                .AsNoTracking()
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .Where(t => t.DueDate >= now && t.DueDate <= threshold && !t.IsCompleted)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        // -------------------------------------------------------
        // Thêm task mới
        // -------------------------------------------------------
        public async Task AddAsync(TaskItem task)
        {
            using var ctx = _contextFactory.CreateDbContext();
            task.CreatedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;
            await ctx.TaskItems.AddAsync(task);
            await ctx.SaveChangesAsync();
        }

        // -------------------------------------------------------
        // Cập nhật task, bảo vệ CreatedAt không bị ghi đè
        // -------------------------------------------------------
        public async Task UpdateAsync(TaskItem task)
        {
            using var ctx = _contextFactory.CreateDbContext();
            task.UpdatedAt = DateTime.UtcNow;
            ctx.TaskItems.Attach(task);
            ctx.Entry(task).State = EntityState.Modified;
            ctx.Entry(task).Property(t => t.CreatedAt).IsModified = false; // Không đổi ngày tạo
            await ctx.SaveChangesAsync();
        }

        // -------------------------------------------------------
        // Cập nhật tiến độ % (được gọi nhiều → tối ưu tốc độ)
        //
        // ExecuteUpdateAsync sinh ra SQL:
        //   UPDATE TaskItems SET ProgressPercent=?, IsCompleted=?, UpdatedAt=?, CompletedAt=?
        //   WHERE Id=?
        // Không cần SELECT toàn bộ entity, tiết kiệm 1 round-trip đến DB
        //
        // Logic tự động:
        //   - progress=100 → IsCompleted=true, CompletedAt=Now
        //   - progress<100 → IsCompleted=false, CompletedAt=null (mở lại nếu nhầm)
        // -------------------------------------------------------
        public async Task UpdateProgressAsync(int taskId, byte progressPercent)
        {
            using var ctx = _contextFactory.CreateDbContext();
            await ctx.TaskItems
                .Where(t => t.Id == taskId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(t => t.ProgressPercent, progressPercent)
                    .SetProperty(t => t.IsCompleted,     progressPercent == 100)
                    .SetProperty(t => t.UpdatedAt,       DateTime.UtcNow)
                    .SetProperty(t => t.CompletedAt,
                        progressPercent == 100 ? DateTime.UtcNow : (DateTime?)null));
        }

        // -------------------------------------------------------
        // Xóa task (hard delete)
        // -------------------------------------------------------
        public async Task DeleteAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            await ctx.TaskItems.Where(t => t.Id == id).ExecuteDeleteAsync();
        }

        // -------------------------------------------------------
        // Thống kê số task theo Status cho 1 dự án (dùng cho pie chart Dashboard)
        //
        // GroupBy trực tiếp trên DB:
        //   SELECT Status.Name, COUNT(*) FROM TaskItems
        //   JOIN Statuses ON ... WHERE ProjectId=? GROUP BY Status.Name
        //
        // Trả về Dictionary để dễ bind vào chart: { "Todo": 5, "InProgress": 3, "Done": 10 }
        // -------------------------------------------------------
        public async Task<Dictionary<string, int>> GetStatusSummaryByProjectAsync(int projectId)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.TaskItems
                .AsNoTracking()
                .Where(t => t.ProjectId == projectId)
                .GroupBy(t => t.Status.Name)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }
    }
}
