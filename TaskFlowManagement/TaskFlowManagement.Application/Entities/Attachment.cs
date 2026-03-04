namespace TaskFlowManagement.Core.Entities
{
    public class Attachment
    {
        public int Id { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.Now;

        /* ================= RELATIONSHIPS ================= */

        // Many Attachments -> 1 Task
        public int TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; } = null!;
    }
}