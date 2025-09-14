namespace TaskNest.Domain.Entities
{
    public class Board
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
        public ICollection<BoardColumn> BoardColumns { get; set; } = new List<BoardColumn>();

        //New : Foreign Key to ApplicationUser
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? User { get; set; }

    }
}
