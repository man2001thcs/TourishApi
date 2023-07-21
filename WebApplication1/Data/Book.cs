using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required]
        [MaxLength(200)]
        public string? Description { get; set; }

        [Required]
        [ForeignKey("Category")]
        public Guid AuthorId { get; set; }
        public Category? Category { get; set; }
        public int PageNumber { get; set; }
        public Guid PublisherId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public ICollection<FullReceipt> FullReceiptList { get; set; }

        public Book()
        {
            this.FullReceiptList = new List<FullReceipt>();
        }


    }
}
