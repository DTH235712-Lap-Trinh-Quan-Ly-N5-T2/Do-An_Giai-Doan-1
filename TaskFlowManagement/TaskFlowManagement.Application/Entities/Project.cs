namespace TaskFlowManagement.Core.Entities
{
    public class Project
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int OwnerId { get; set; }
        public User Owner { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /* ================= RELATIONSHIPS ================= */

        // 1 Project -> Many Tasks
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();

        // 1 Project -> Many Users (Owner or Members sẽ làm sau nếu cần)
    }
}