namespace TaskNest.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Board>? Boards { get; set; }
    }
}
