using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskFlowManagement.Core.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public bool IsCompleted { get; set; }

        /// <summary>% hoàn thành: 0–100</summary>
        [Range(0, 100)]
        public byte ProgressPercent { get; set; } = 0;

        /// <summary>Giờ ước tính</summary>
        [Column(TypeName = "decimal(6,1)")]
        public decimal? EstimatedHours { get; set; }

        /// <summary>Giờ thực tế</summary>
        [Column(TypeName = "decimal(6,1)")]
        public decimal? ActualHours { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }

        /* ================= RELATIONSHIPS ================= */

        // Project
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        // Sub-task support (self-referencing)
        public int? ParentTaskId { get; set; }
        public TaskItem? ParentTask { get; set; }
        public ICollection<TaskItem> SubTasks { get; set; } = new List<TaskItem>();

        // Created By
        public int CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

        // Assigned To (1 người chính)
        public int? AssignedToId { get; set; }
        public User? AssignedTo { get; set; }

        // Lookup
        public int PriorityId { get; set; }
        public Priority Priority { get; set; } = null!;

        public int StatusId { get; set; }
        public Status Status { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // 1 Task -> Many Comments, Attachments, Tags
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
    }
}
