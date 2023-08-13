using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data.RelationData
{
    [Table("BookPublisher")]
    public class BookPublisher
    {
        public Guid BookId { get; set; }
        public Book Book { get; set; }
        public Guid PublisherId { get; set; }
        public Publisher Publisher { get; set; }

    }
}
