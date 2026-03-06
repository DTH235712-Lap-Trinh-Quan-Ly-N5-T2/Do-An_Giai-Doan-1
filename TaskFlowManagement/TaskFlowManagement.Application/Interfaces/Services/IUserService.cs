using TaskFlowManagement.Core.Entities;

namespace TaskFlowManagement.Core.Interfaces.Services
{
    /// <summary>
    /// Interface cho User Service – nghiệp vụ liên quan đến quản lý người dùng.
    /// Tách biệt với IAuthService (Auth = xác thực, User = quản lý).
    /// </summary>
    public interface IUserService
    {
        Task<List<User>> GetAllActiveUsersAsync();
        Task<User?> GetByIdAsync(int id);

        /// <summary>
        /// Tạo user mới kèm hash password và gán role.
        /// Service xử lý toàn bộ: validate, hash, add UserRole.
        /// </summary>
        Task<(bool Success, string Message)> CreateUserAsync(
            string username, string fullName, string email,
            string password, string roleName);

        Task<(bool Success, string Message)> UpdateUserAsync(User user);

        /// <summary>Soft delete – set IsActive = false.</summary>
        Task<(bool Success, string Message)> DeactivateUserAsync(int userId);

        /// <summary>Đổi mật khẩu – verify old pass trước khi hash new pass.</summary>
        Task<(bool Success, string Message)> ChangePasswordAsync(
            int userId, string oldPassword, string newPassword);
    }
}
