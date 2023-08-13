using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data.RelationData
{
    [Table("BookAuthor")]
    public class BookAuthor
    {
        public Guid BookId { get; set; }
        public Book Book { get; set; }
        public Guid AuthorId { get; set; }
        public Author Author { get; set; }
    }
}
