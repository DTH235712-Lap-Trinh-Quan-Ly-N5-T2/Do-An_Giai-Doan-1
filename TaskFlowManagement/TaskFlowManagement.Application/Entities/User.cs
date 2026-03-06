using System.ComponentModel.DataAnnotations;

namespace TaskFlowManagement.Core.Entities
{
    public class User
    {
        public int Id { get; set; }

        /// <summary>Tên đăng nhập (unique)</summary>
        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        /// <summary>Mật khẩu lưu dạng BCrypt hash</summary>
        [Required, MaxLength(256)]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(255)]
        public string? AvatarPath { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLogin { get; set; }

        /* ================= RELATIONSHIPS ================= */
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<Project> OwnedProjects { get; set; } = new List<Project>();
        public ICollection<ProjectMember> ProjectMemberships { get; set; } = new List<ProjectMember>();
        public ICollection<TaskItem> CreatedTasks { get; set; } = new List<TaskItem>();
        public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Expense> CreatedExpenses { get; set; } = new List<Expense>();
    }
}
