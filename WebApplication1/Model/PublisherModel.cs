namespace WebApplication1.Model
{
    public class PublisherModel
    {
        public Guid Id { get; set; }
        public string PublisherName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
