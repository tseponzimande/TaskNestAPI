namespace TaskNest.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Board>? Boards { get; set; } = new List<Board>();

        // Navigation property for BoardUser
        public ICollection<BoardUser> BoardUsers { get; set; } = new List<BoardUser>();

    }
}
