using TaskFlowManagement.Core.Entities;
using TaskFlowManagement.Core.Interfaces;
using TaskFlowManagement.Core.Interfaces.Services;

// Namespace số nhiều "Projects" (không phải "Project") vì:
//   Nếu đặt "Services.Project" → C# nhầm "Project" là tên namespace,
//   không nhận ra đây là Entity class → lỗi biên dịch.
//   Convention .NET: namespace dùng số nhiều (Controllers, Services, Repositories...)
namespace TaskFlowManagement.Core.Services.Projects
{
    /// <summary>
    /// Triển khai IProjectService – chứa nghiệp vụ quản lý dự án.
    ///
    /// Luồng dữ liệu:
    ///   WinForms Form → IProjectService → IProjectRepository → AppDbContext → SQL Server
    ///
    /// Form KHÔNG được gọi Repository trực tiếp – phải qua Service để
    /// đảm bảo validate, phân quyền và logging đều được xử lý.
    /// </summary>
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepo;

        // Inject IProjectRepository qua constructor (DI Container tự cung cấp)
        public ProjectService(IProjectRepository projectRepo)
            => _projectRepo = projectRepo;

        // -------------------------------------------------------
        // Lấy danh sách dự án theo quyền:
        //   isManager = true  → lấy tất cả (Admin/Manager)
        //   isManager = false → chỉ lấy dự án mình là thành viên (Developer)
        // -------------------------------------------------------
        public Task<List<Project>> GetProjectsForUserAsync(int userId, bool isManager)
            => isManager
                ? _projectRepo.GetAllAsync()
                : _projectRepo.GetByMemberAsync(userId);

        // Lấy đầy đủ chi tiết: Members, Tasks, Expenses (dùng cho form chi tiết)
        public Task<Project?> GetProjectDetailsAsync(int projectId)
            => _projectRepo.GetWithDetailsAsync(projectId);

        // -------------------------------------------------------
        // Tạo dự án mới – validate trước, rồi mới lưu DB
        // -------------------------------------------------------
        public async Task<(bool Success, string Message)> CreateProjectAsync(Project project)
        {
            // Kiểm tra tên bắt buộc
            if (string.IsNullOrWhiteSpace(project.Name))
                return (false, "Tên dự án không được để trống.");

            await _projectRepo.AddAsync(project);
            return (true, $"Dự án '{project.Name}' đã được tạo thành công.");
        }

        public async Task<(bool Success, string Message)> UpdateProjectAsync(Project project)
        {
            await _projectRepo.UpdateAsync(project);
            return (true, "Cập nhật dự án thành công.");
        }

        public async Task<(bool Success, string Message)> DeleteProjectAsync(int projectId)
        {
            await _projectRepo.DeleteAsync(projectId);
            return (true, "Xóa dự án thành công.");
        }

        // -------------------------------------------------------
        // Thêm thành viên – kiểm tra không trùng trước khi thêm
        // -------------------------------------------------------
        public async Task<(bool Success, string Message)> AddMemberAsync(
            int projectId, int userId, string projectRole)
        {
            var project = await _projectRepo.GetWithDetailsAsync(projectId);
            if (project == null)
                return (false, "Dự án không tồn tại.");

            // LeftAt == null nghĩa là thành viên vẫn đang active
            if (project.Members.Any(m => m.UserId == userId && m.LeftAt == null))
                return (false, "Người dùng đã là thành viên của dự án này.");

            // TODO G2: thêm qua ProjectMemberRepository.AddAsync
            return (true, "Thêm thành viên thành công.");
        }

        public Task<(bool Success, string Message)> RemoveMemberAsync(int projectId, int userId)
        {
            // TODO G2: Set LeftAt = UtcNow thay vì xóa thật → giữ lịch sử
            return Task.FromResult<(bool, string)>((true, "Xóa thành viên thành công."));
        }
    }
}
