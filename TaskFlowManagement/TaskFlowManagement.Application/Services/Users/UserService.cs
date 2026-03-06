using TaskFlowManagement.Core.Entities;
using TaskFlowManagement.Core.Helpers;
using TaskFlowManagement.Core.Interfaces;
using TaskFlowManagement.Core.Interfaces.Services;

// Namespace số nhiều "Users" (không phải "User") vì:
//   "Services.User" trùng tên với Entity class User
//   → C# nhầm "User" là namespace thay vì class → lỗi biên dịch.
namespace TaskFlowManagement.Core.Services.Users
{
    /// <summary>
    /// Triển khai IUserService – nghiệp vụ quản lý người dùng.
    ///
    /// Phân biệt với IAuthService:
    ///   IAuthService = xác thực (login, hash, verify password)
    ///   IUserService = quản lý (CRUD user, đổi mật khẩu, phân quyền)
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IAuthService    _authService; // Dùng để hash/verify password

        public UserService(IUserRepository userRepo, IAuthService authService)
        {
            _userRepo    = userRepo;
            _authService = authService;
        }

        // Lấy tất cả user active (IsActive = true), sắp xếp A-Z
        public Task<List<User>> GetAllActiveUsersAsync()
            => _userRepo.GetAllAsync();

        public Task<User?> GetByIdAsync(int id)
            => _userRepo.GetByIdAsync(id);

        // -------------------------------------------------------
        // Tạo user mới: validate → hash BCrypt → lưu DB
        // -------------------------------------------------------
        public async Task<(bool Success, string Message)> CreateUserAsync(
            string username, string fullName, string email,
            string password, string roleName)
        {
            // Validate lần lượt – dừng sớm khi gặp lỗi đầu tiên
            if (!ValidationHelper.IsValidUsername(username))
                return (false, "Username chỉ gồm chữ, số, dấu gạch dưới (3–50 ký tự).");

            if (await _userRepo.IsUsernameExistsAsync(username))
                return (false, $"Username '{username}' đã tồn tại.");

            if (!ValidationHelper.IsValidEmail(email))
                return (false, "Email không hợp lệ.");

            if (!ValidationHelper.IsPasswordStrong(password))
                return (false, "Mật khẩu phải có ít nhất 6 ký tự.");

            // Tạo entity – hash BCrypt qua AuthService (không duplicate logic)
            var user = new User
            {
                Username     = username.Trim(),
                FullName     = fullName.Trim(),
                Email        = email.Trim(),
                PasswordHash = _authService.HashPassword(password),
                IsActive     = true
            };

            await _userRepo.AddAsync(user);
            return (true, "Tạo tài khoản thành công.");
        }

        public async Task<(bool Success, string Message)> UpdateUserAsync(User user)
        {
            await _userRepo.UpdateAsync(user);
            return (true, "Cập nhật thông tin thành công.");
        }

        // Soft delete: set IsActive = false, không xóa khỏi DB
        // Giữ lịch sử task, comment, expense liên quan đến user
        public async Task<(bool Success, string Message)> DeactivateUserAsync(int userId)
        {
            await _userRepo.DeleteAsync(userId);
            return (true, "Tài khoản đã bị vô hiệu hóa.");
        }

        // -------------------------------------------------------
        // Đổi mật khẩu: verify cũ → validate mới → hash BCrypt → lưu
        // -------------------------------------------------------
        public async Task<(bool Success, string Message)> ChangePasswordAsync(
            int userId, string oldPassword, string newPassword)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                return (false, "Không tìm thấy tài khoản.");

            // Bắt buộc verify mật khẩu cũ trước khi cho đổi
            if (!_authService.VerifyPassword(oldPassword, user.PasswordHash))
                return (false, "Mật khẩu cũ không đúng.");

            if (!ValidationHelper.IsPasswordStrong(newPassword))
                return (false, "Mật khẩu mới phải có ít nhất 6 ký tự.");

            // Cập nhật chỉ PasswordHash – UpdateAsync sẽ bảo vệ các cột khác
            user.PasswordHash = _authService.HashPassword(newPassword);
            await _userRepo.UpdateAsync(user);
            return (true, "Đổi mật khẩu thành công.");
        }
    }
}
