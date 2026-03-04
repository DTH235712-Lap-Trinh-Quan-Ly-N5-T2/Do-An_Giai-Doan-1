namespace TaskFlowManagement.Core.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        /* ================= RELATIONSHIPS ================= */

        // Many-to-many Roles
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        // 1 User -> Many Projects (nếu là owner)
        public ICollection<Project> Projects { get; set; } = new List<Project>();

        // 1 User -> Many Tasks created
        public ICollection<TaskItem> CreatedTasks { get; set; } = new List<TaskItem>();

        // 1 User -> Many Tasks assigned
        public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();

        // 1 User -> Many Comments
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}