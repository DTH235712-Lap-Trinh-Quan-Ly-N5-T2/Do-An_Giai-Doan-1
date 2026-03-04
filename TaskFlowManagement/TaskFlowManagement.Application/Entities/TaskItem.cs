namespace TaskFlowManagement.Core.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? DueDate { get; set; }

        /* ================== RELATIONSHIPS ================== */

        // Project (Many Task -> 1 Project)
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        // Created By (Many Task -> 1 User)
        public int CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

        // Assigned To (Many Task -> 1 User)
        public int? AssignedToId { get; set; }
        public User? AssignedTo { get; set; }

        // 
        public int PriorityId { get; set; }
        public Priority Priority { get; set; } = null!;

        public int StatusId { get; set; }
        public Status Status { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // 1 Task -> Many Comments
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        // 1 Task -> Many Attachments
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

        // Many-to-many with Tag
        public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
    }
}