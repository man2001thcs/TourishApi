using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Data.RelationData;

namespace WebApplication1.Data
{
    [Table("Voucher")]
    public class Voucher
    {
        [Key]
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public float DiscountFloat { get; set; }
        public double DiscountAmount { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        // Relationship
        public ICollection<BookVoucher> BookVouchers { get; set; }

    }
}
