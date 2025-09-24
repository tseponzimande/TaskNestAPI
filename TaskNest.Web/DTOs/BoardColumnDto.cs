namespace TaskNest.Web.DTOs
{
    public class BoardColumnDto
    {
        public Guid Id { get; set; }
        public Guid BoardId { get; set; }
        public string Name { get; set; } = string.Empty;

        // renamed to match entity property 'Order'
        public int Order { get; set; }
    }
}

