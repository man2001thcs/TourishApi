using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Data
{
    [Table("BookVoucher")]
    public class BookVoucher
    {
        public Guid BookId { get; set; }
        public Book Book { get; set; }
        public Guid VoucherId { get; set; }
        public Voucher Voucher { get; set; }

    }
}
