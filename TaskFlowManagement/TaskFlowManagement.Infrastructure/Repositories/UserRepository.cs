using Microsoft.EntityFrameworkCore;
using TaskFlowManagement.Core.Entities;
using TaskFlowManagement.Core.Interfaces;
using TaskFlowManagement.Infrastructure.Data;

namespace TaskFlowManagement.Infrastructure.Repositories
{
    /// <summary>
    /// Triển khai IUserRepository – xử lý toàn bộ thao tác dữ liệu liên quan đến User.
    ///
    /// Dùng IDbContextFactory để tạo DbContext mới cho mỗi operation.
    /// Lý do: WinForms không có request scope như web app, mỗi thao tác nên có context riêng.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public UserRepository(IDbContextFactory<AppDbContext> contextFactory)
            => _contextFactory = contextFactory;

        // -------------------------------------------------------
        // Lấy user theo ID – dùng cho load chi tiết, không kèm Roles
        // -------------------------------------------------------
        public async Task<User?> GetByIdAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            // AsNoTracking: chỉ đọc, không cần EF theo dõi thay đổi → nhẹ hơn
            return await ctx.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        }

        // -------------------------------------------------------
        // Lấy tất cả user đang active, sắp xếp A-Z theo tên
        // IsActive = false là "soft deleted" – vẫn trong DB nhưng ẩn đi
        // -------------------------------------------------------
        public async Task<List<User>> GetAllAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Users
                .AsNoTracking()
                .Where(u => u.IsActive)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        // -------------------------------------------------------
        // Tìm user theo username (dùng khi đăng nhập)
        // Chỉ WHERE IsActive = true để tài khoản bị vô hiệu hóa không đăng nhập được
        // -------------------------------------------------------
        public async Task<User?> GetByUsernameAsync(string username)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
        }

        // -------------------------------------------------------
        // Tìm user theo email (kiểm tra trùng, reset mật khẩu sau này)
        // -------------------------------------------------------
        public async Task<User?> GetByEmailAsync(string email)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        // -------------------------------------------------------
        // Lấy user kèm danh sách Roles (dùng ngay sau Login)
        // Include(UserRoles).ThenInclude(Role) = JOIN bảng UserRoles và Roles
        // Tránh N+1 query: không dùng lazy loading cho collection
        // -------------------------------------------------------
        public async Task<User?> GetWithRolesAsync(int userId)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Users
                .AsNoTracking()
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        // -------------------------------------------------------
        // Kiểm tra username đã tồn tại chưa (validate khi tạo user mới)
        // AnyAsync tốt hơn FirstOrDefaultAsync vì SQL: SELECT 1 WHERE EXISTS(...)
        // -------------------------------------------------------
        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Users.AnyAsync(u => u.Username == username);
        }

        // -------------------------------------------------------
        // Cập nhật LastLogin sau khi đăng nhập thành công
        // ExecuteUpdateAsync: UPDATE ... SET LastLogin=NOW WHERE Id=? 
        // Không cần load entity về – tiết kiệm 1 SELECT round-trip
        // -------------------------------------------------------
        public async Task UpdateLastLoginAsync(int userId)
        {
            using var ctx = _contextFactory.CreateDbContext();
            await ctx.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.LastLogin, DateTime.UtcNow));
        }

        // -------------------------------------------------------
        // Thêm user mới vào DB
        // Tự động set CreatedAt = UtcNow tại đây để nhất quán
        // -------------------------------------------------------
        public async Task AddAsync(User user)
        {
            using var ctx = _contextFactory.CreateDbContext();
            user.CreatedAt = DateTime.UtcNow;
            await ctx.Users.AddAsync(user);
            await ctx.SaveChangesAsync();
        }

        // -------------------------------------------------------
        // Cập nhật thông tin user (không cho update CreatedAt và PasswordHash)
        // Attach → Modified State để EF biết đây là Update, không phải Insert
        // IsModified = false cho các cột không muốn thay đổi
        // -------------------------------------------------------
        public async Task UpdateAsync(User user)
        {
            using var ctx = _contextFactory.CreateDbContext();
            ctx.Users.Attach(user);
            ctx.Entry(user).State = EntityState.Modified;
            ctx.Entry(user).Property(u => u.CreatedAt).IsModified    = false; // Không đổi ngày tạo
            ctx.Entry(user).Property(u => u.PasswordHash).IsModified = false; // Dùng hàm riêng đổi mật khẩu
            await ctx.SaveChangesAsync();
        }

        // -------------------------------------------------------
        // Soft delete: set IsActive = false thay vì xóa thật
        // Lý do: giữ lịch sử, tránh lỗi foreign key (task/comment vẫn tham chiếu user)
        // -------------------------------------------------------
        public async Task DeleteAsync(int id)
        {
            using var ctx = _contextFactory.CreateDbContext();
            await ctx.Users
                .Where(u => u.Id == id)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.IsActive, false));
        }
    }
}
