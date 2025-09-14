namespace TaskNest.Web.DTOs
{
    public class BoardColumnDto
    {
        public Guid Id { get; set; }
        public Guid BoardId { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
    }
}
