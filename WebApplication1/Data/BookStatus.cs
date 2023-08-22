using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data
{
    [Table("BookStatus")]
    public class BookStatus
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int SoldNumberInMonth { get; set; }
        public int TotalSoldNumber { get; set; }
        public int RemainNumber { get; set; }
        public double CurrentPrice { get; set; }
        public DateTime UpdateDate { get; set; }
        public Book Book { get; set; }

        public BookStatus()
        {
            this.SoldNumberInMonth = 0;
            this.Book = new Book();
        }


    }
}
