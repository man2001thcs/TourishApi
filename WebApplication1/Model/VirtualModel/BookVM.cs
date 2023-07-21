namespace WebApplication1.Model.VirtualModel
{
    public class BookVM
    {
        public Guid id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int PageNumber { get; set; }
        public Guid PublisherId { get; set; }
        public Guid AuthorId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
