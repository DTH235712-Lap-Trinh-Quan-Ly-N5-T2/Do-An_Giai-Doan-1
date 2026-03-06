using Microsoft.EntityFrameworkCore;
using TaskFlowManagement.Core.Entities;
using TaskFlowManagement.Core.Interfaces;
using TaskFlowManagement.Infrastructure.Data;

namespace TaskFlowManagement.Infrastructure.Repositories
{
    /// <summary>
    /// Triển khai IProjectRepository – thao tác dữ liệu cho Project.
    /// Project là entity trung tâm: liên kết User, Customer, Task, Expense.
    /// </summary>
    public class ProjectRepository : IProjectRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public ProjectRepository(IDbContextFactory<AppDbContext> contextFactory)
            => _contextFactory = contextFactory;

        // -------------------------------------------------------
        // Lấy project kèm Owner và Customer – đủ để hiển thị trên grid
        // -------------------------------------------------------
        public async Task<Project?> GetByIdAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Projects
                .AsNoTracking()
                .Include(p => p.Owner)    // Eager load: tránh lazy load N+1
                .Include(p => p.Customer)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // -------------------------------------------------------
        // Lấy chi tiết đầy đủ 1 dự án (dùng cho form chi tiết)
        // Eager load tất cả navigation properties cần thiết trong 1 query (JOIN nhiều bảng)
        // -------------------------------------------------------
        public async Task<Project?> GetWithDetailsAsync(int projectId)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Projects
                .AsNoTracking()
                .Include(p => p.Owner)
                .Include(p => p.Customer)
                .Include(p => p.Members).ThenInclude(m => m.User)    // Danh sách thành viên + thông tin
                .Include(p => p.Tasks).ThenInclude(t => t.Status)    // Task kèm trạng thái
                .Include(p => p.Tasks).ThenInclude(t => t.AssignedTo)// Task kèm người phụ trách
                .Include(p => p.Expenses)                             // Chi phí
                .FirstOrDefaultAsync(p => p.Id == projectId);
        }

        // -------------------------------------------------------
        // Lấy tất cả project sắp xếp mới nhất lên đầu
        // -------------------------------------------------------
        public async Task<List<Project>> GetAllAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Projects
                .AsNoTracking()
                .Include(p => p.Owner)
                .Include(p => p.Customer)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        // -------------------------------------------------------
        // Lấy project theo Owner – dùng cho Manager xem dự án mình quản lý
        // -------------------------------------------------------
        public async Task<List<Project>> GetByOwnerAsync(int ownerId)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Projects
                .AsNoTracking()
                .Include(p => p.Customer)
                .Where(p => p.OwnerId == ownerId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        // -------------------------------------------------------
        // Lấy project theo thành viên – Developer xem dự án mình tham gia
        // Join qua bảng ProjectMembers, lọc LeftAt == null (chưa rời nhóm)
        // -------------------------------------------------------
        public async Task<List<Project>> GetByMemberAsync(int userId)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.ProjectMembers
                .AsNoTracking()
                .Include(pm => pm.Project).ThenInclude(p => p.Customer)
                .Include(pm => pm.Project).ThenInclude(p => p.Owner)
                .Where(pm => pm.UserId == userId && pm.LeftAt == null) // Chỉ thành viên đang active
                .Select(pm => pm.Project)
                .Distinct() // Phòng trường hợp data trùng
                .ToListAsync();
        }

        // -------------------------------------------------------
        // Lấy dự án đang active: InProgress hoặc NotStarted
        // Sắp xếp theo deadline gần nhất lên đầu – ưu tiên việc cấp bách
        // Dùng cho Dashboard tổng quan
        // -------------------------------------------------------
        public async Task<List<Project>> GetActiveProjectsAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Projects
                .AsNoTracking()
                .Include(p => p.Owner)
                .Include(p => p.Customer)
                .Where(p => p.Status == "InProgress" || p.Status == "NotStarted")
                .OrderBy(p => p.PlannedEndDate) // Deadline gần nhất lên trên
                .ToListAsync();
        }

        // -------------------------------------------------------
        // Tính tổng chi phí dự án từ bảng Expenses
        // SumAsync tính trực tiếp trên DB: SELECT SUM(Amount) WHERE ProjectId=?
        // Không load toàn bộ Expense records về memory
        // -------------------------------------------------------
        public async Task<decimal> GetTotalExpenseAsync(int projectId)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Expenses
                .Where(e => e.ProjectId == projectId)
                .SumAsync(e => e.Amount);
        }

        // -------------------------------------------------------
        // Cập nhật trạng thái dự án (NotStarted/InProgress/OnHold/Completed/Cancelled)
        // ExecuteUpdateAsync: chỉ UPDATE 2 cột, không cần SELECT toàn bộ entity trước
        // -------------------------------------------------------
        public async Task UpdateStatusAsync(int projectId, string status)
        {
            using var ctx = _contextFactory.CreateDbContext();
            await ctx.Projects
                .Where(p => p.Id == projectId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.Status, status)
                    .SetProperty(p => p.UpdatedAt, DateTime.UtcNow));
        }

        // -------------------------------------------------------
        // Thêm dự án mới, tự set timestamp
        // -------------------------------------------------------
        public async Task AddAsync(Project project)
        {
            using var ctx = _contextFactory.CreateDbContext();
            project.CreatedAt = DateTime.UtcNow;
            project.UpdatedAt = DateTime.UtcNow;
            await ctx.Projects.AddAsync(project);
            await ctx.SaveChangesAsync();
        }

        // -------------------------------------------------------
        // Cập nhật dự án – tự update UpdatedAt, bảo vệ CreatedAt
        // -------------------------------------------------------
        public async Task UpdateAsync(Project project)
        {
            using var ctx = _contextFactory.CreateDbContext();
            project.UpdatedAt = DateTime.UtcNow;
            ctx.Projects.Attach(project);
            ctx.Entry(project).State = EntityState.Modified;
            ctx.Entry(project).Property(p => p.CreatedAt).IsModified = false; // Không ghi đè ngày tạo
            await ctx.SaveChangesAsync();
        }

        // -------------------------------------------------------
        // Hard delete project – cascade sẽ xóa Members, Tasks, Expenses liên quan
        // ExecuteDeleteAsync: DELETE ... WHERE Id=? trực tiếp trên DB
        // -------------------------------------------------------
        public async Task DeleteAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            await ctx.Projects.Where(p => p.Id == id).ExecuteDeleteAsync();
        }
    }
}
