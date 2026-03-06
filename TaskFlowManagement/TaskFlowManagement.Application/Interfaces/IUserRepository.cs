using TaskFlowManagement.Core.Entities;

namespace TaskFlowManagement.Core.Interfaces
{
    /// <summary>
    /// Interface định nghĩa các thao tác dữ liệu liên quan đến User.
    /// Kế thừa IRepository&lt;User&gt; để có sẵn CRUD cơ bản.
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Tìm user theo tên đăng nhập (dùng cho Login).
        /// Chỉ trả về user đang active (IsActive = true).
        /// </summary>
        Task<User?> GetByUsernameAsync(string username);

        /// <summary>
        /// Tìm user theo email (dùng cho reset mật khẩu, kiểm tra trùng lặp).
        /// </summary>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Lấy user kèm danh sách Role (dùng ngay sau Login để load quyền vào AppSession).
        /// Dùng Include + ThenInclude để eager load UserRoles → Role.
        /// </summary>
        Task<User?> GetWithRolesAsync(int userId);

        /// <summary>
        /// Kiểm tra username đã tồn tại chưa (dùng khi tạo tài khoản mới).
        /// Dùng AnyAsync thay vì FirstOrDefault để tối ưu – chỉ cần biết có hay không.
        /// </summary>
        Task<bool> IsUsernameExistsAsync(string username);

        /// <summary>
        /// Cập nhật thời gian đăng nhập cuối (gọi fire-and-forget sau login thành công).
        /// Dùng ExecuteUpdateAsync để tránh load toàn bộ entity chỉ để update 1 cột.
        /// </summary>
        Task UpdateLastLoginAsync(int userId);
    }
}
