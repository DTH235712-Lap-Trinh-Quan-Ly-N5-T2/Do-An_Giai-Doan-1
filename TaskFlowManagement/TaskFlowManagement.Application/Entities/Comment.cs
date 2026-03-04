namespace TaskFlowManagement.Core.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /* ================= RELATIONSHIPS ================= */

        // Many Comments -> 1 Task
        public int TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; } = null!;

        // Many Comments -> 1 User
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}