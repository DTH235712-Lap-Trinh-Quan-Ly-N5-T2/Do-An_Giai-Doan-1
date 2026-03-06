using System.ComponentModel.DataAnnotations;

namespace TaskFlowManagement.Core.Entities
{
    public class Status
    {
        public int Id { get; set; }

        /// <summary>Todo | InProgress | InReview | Done</summary>
        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        /// <summary>Thứ tự hiển thị trên Kanban</summary>
        public byte DisplayOrder { get; set; } = 0;

        [MaxLength(7)]
        public string? ColorHex { get; set; } // e.g. "#4CAF50"

        public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
    }
}
