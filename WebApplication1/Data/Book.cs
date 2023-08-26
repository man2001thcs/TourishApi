using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Data.RelationData;

namespace WebApplication1.Data
{
    [Table("Book")]
    public class Book
    {
        [Key]
        public Guid id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }
        public int PageNumber { get; set; }
        public Guid? PublisherId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        // Relationship
        public BookStatus? BookStatus { get; set; }
        public ICollection<BookCategory> BookCategories { get; set; }
        public Publisher? Publisher { get; set; }
        public ICollection<BookAuthor> BookAuthors { get; set; }
        public ICollection<FullReceipt> FullReceiptList { get; set; }
        public ICollection<BookVoucher> BookVouchers { get; set; }

        public Book()
        {
            this.FullReceiptList = new List<FullReceipt>();
        }


    }
}
