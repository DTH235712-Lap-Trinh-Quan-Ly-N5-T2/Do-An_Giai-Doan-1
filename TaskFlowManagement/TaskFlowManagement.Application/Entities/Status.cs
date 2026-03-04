namespace TaskFlowManagement.Core.Entities
{
    public class Status
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
    }
}