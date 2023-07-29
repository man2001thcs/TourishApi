using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data
{
    [Table("Voucher")]
    public class Voucher
    {
        [Key]
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public float discountFloat { get; set; }
        public double discountAmount { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        // Relationship
        public ICollection<BookVoucher> BookVouchers { get; set; }

    }
}
