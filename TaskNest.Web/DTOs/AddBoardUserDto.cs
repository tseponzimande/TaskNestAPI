namespace TaskNest.Web.DTOs
{
    public class AddBoardUserDto
    {
        public string UserEmail { get; set; } = null!;
        public BoardUserRole Role { get; set; }
    }
}
