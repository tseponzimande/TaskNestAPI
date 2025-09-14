namespace TaskNest.Web.DTOs
{
    public class TaskItemDto
    {
        public Guid Id { get; set; }
        public Guid ColumnId { get; set; }
        public string Title { get; set; } = null!;
        public int Position { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
